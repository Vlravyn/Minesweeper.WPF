using Minesweeper.Core.DataModels;
using MvvmEssentials.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Minesweeper.Core
{
    /// <summary>
    /// Contains all the game logic for minesweeper game.
    /// </summary>
    public class Game : ObservableObject
    {
        private uint remainingTilesCount;
        private readonly Statistics _statistics;

        private int _rows;
        private int _columns;
        private int _mines;
        private GameState _gameState;
        private int _assumedRemainingMines;

        /// <summary>
        /// Raised when the game ends.
        /// </summary>
        public event EventHandler<GameEndEventArgs> GameEnd;

        /// <summary>
        /// Stopwatch that tracks how much time has passed since the game started.
        /// </summary>
        public Stopwatch Stopwatch { get; private set; }

        /// <summary>
        /// The difficulty set for this game.
        /// </summary>
        public GameDifficultyHost Difficulty { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public int AssumedRemainingMines
        {
            get => _assumedRemainingMines;
            set => SetProperty(ref _assumedRemainingMines, value);
        }

        /// <summary>
        /// The Current state of the game
        /// </summary>
        public GameState GameState
        {
            get => _gameState;
            set => SetProperty(ref _gameState, value);
        }

        /// <summary>
        /// The total number of rows in this game
        /// </summary>
        public int Rows
        {
            get => _rows;
            private set => SetProperty(ref _rows, value);
        }

        /// <summary>
        /// The total number of columns in this game
        /// </summary>
        public int Columns
        {
            get => _columns;
            private set => SetProperty(ref _columns, value);
        }

        /// <summary>
        /// The total mines in this game.
        /// </summary>
        public int TotalMines
        {
            get => _mines;
            private set => SetProperty(ref _mines, value);
        }

        /// <summary>
        /// All the tiles that are being used in this game.
        /// </summary>
        public ObservableCollection<Tile> AllTiles { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="Game"/>
        /// </summary>
        /// <param name="difficulty">The difficulty for this game.</param>
        public Game(GameDifficultyHost difficulty, Statistics statistics)
        {
            Difficulty = difficulty;
            _statistics = statistics;
            Rows = difficulty.Rows;
            Columns = difficulty.Columns;
            TotalMines = difficulty.Mines;

            _assumedRemainingMines = TotalMines;
            remainingTilesCount = (uint)(Rows * Columns);

            AllTiles = [];
            for (ushort row = 0; row < Rows; row++)
            {
                for (ushort col = 0; col < Columns; col++)
                    AllTiles.Add(new Tile(TileState.Covered, row, col));
            }

            Stopwatch = new Stopwatch();
            GameEnd += OnGameEnd;
        }

        /// <summary>
        /// Update the statistics after game ends.
        /// </summary>
        private static void OnGameEnd(object? sender, GameEndEventArgs e)
        {
            if (sender is Game game)
            {
                StatsForDifficultyHost? currentDifficulty = game.Difficulty.DifficultyType switch
                {
                    GameDifficulty.Easy => game._statistics.EasyDifficulty,
                    GameDifficulty.Medium => game._statistics.MediumDifficulty,
                    GameDifficulty.Hard => game._statistics.HardDifficulty,
                    _ => null
                };

                //do not do update any statistics if game was using custom difficulty.
                if (currentDifficulty == null)
                    return;

                //updating the statistics for the given difficulty.
                currentDifficulty.GamesPlayed++;

                if (e.GameWon)
                {
                    currentDifficulty.GamesWon++;
                    currentDifficulty.CurrentWinningStreak++;
                    currentDifficulty.LongestLosingStreak = 0;

                    if (currentDifficulty.CurrentWinningStreak > currentDifficulty.LongestWinningStreak)
                        currentDifficulty.LongestWinningStreak = currentDifficulty.CurrentWinningStreak;

                    if (currentDifficulty.BestTime is null || (ulong)game.Stopwatch.Elapsed.TotalSeconds < currentDifficulty.BestTime)
                        currentDifficulty.BestTime = (ulong)game.Stopwatch.Elapsed.TotalSeconds;

                }
                else
                {
                    currentDifficulty.LongestLosingStreak++;
                    currentDifficulty.CurrentWinningStreak = 0;
                }

                game._statistics.SaveStatistics();
            }
        }

        /// <summary>
        /// Opens a tile.
        /// </summary>
        /// <param name="tile">the tile to open</param>
        /// <exception cref="ArgumentNullException">thrown when the tile is null</exception>
        public void OpenTile(Tile tile)
        {
            if (GameState == GameState.GameEnd)
                return;

            if (tile == null)
                throw new ArgumentNullException(nameof(tile), "The tile being opened cannot be null");

            //do not attempt to open the tile if tile is flagged as bomb or has already been opened.
            if (tile.TileState == TileState.Flagged || tile.TileState == TileState.Unconvered)
                return;

            if (GameState == GameState.NewGame)
            {
                PlantBombs(tile);
                GameState = GameState.InProgress;
                Stopwatch.Start();
            }

            if (tile.ContainsBomb)
            {
                GameState = GameState.GameEnd;
                GameEnd?.Invoke(this, new GameEndEventArgs(false));
                Stopwatch.Stop();
            }
            else
            {
                tile.AdjacentMinesCount = 0;

                //Getting all the adjacent tiles.
                //The rows and columns of the adjacent tiles can only be + or - 1 of this tile. So quering for it.
                var adjacentTiles = AllTiles.Where(t => t.Row <= tile.Row + 1 && t.Row >= tile.Row - 1 && t.Column <= tile.Column + 1 && t.Column >= tile.Column - 1);

                foreach (var adjacentTile in adjacentTiles)
                {
                    if (adjacentTile.ContainsBomb)
                        tile.AdjacentMinesCount++;
                }

                tile.TileState = TileState.Unconvered;

                --remainingTilesCount;
                if (remainingTilesCount == TotalMines)
                    CheckIfGameWon();
                //Attempt to open the adjacent tiles if this tile has no adjacent bombs.
                if (tile.AdjacentMinesCount == 0)
                {
                    foreach (var adjacentTile in adjacentTiles)
                        OpenTile(adjacentTile);
                }
            }
        }

        /// <summary>
        /// Plants the bomb for a new game.
        /// </summary>
        /// <param name="tile">
        /// the tile which was clicked first after the new game started.
        /// Used to make sure bomb is not in the tile that was clicked first.
        /// </param>
        private void PlantBombs(Tile tile)
        {
            if (GameState == GameState.GameEnd)
                return;

            Random random = new();
            var placedMines = 0;

            while (placedMines < TotalMines)
            {
                var xRow = random.Next(0, Rows - 1);
                var xColumn = random.Next(0, Columns - 1);

                var xTile = AllTiles.First(t => t.Column == xColumn && t.Row == xRow);

                if (xTile != tile && !xTile.ContainsBomb)
                {
                    xTile.ContainsBomb = true;
                    ++placedMines;
                }
            }
        }

        /// <summary>
        /// Cycles the <see cref="Tile.TileState"/>
        /// </summary>
        /// <param name="tile">the <see cref="Tile"/> to cycle the <see cref="TileState"/> of</param>
        public void CycleUncoveredStates(Tile? tile)
        {
            if (tile == null)
                return;

            if (tile.TileState == TileState.Unconvered)
                return;

            switch (tile.TileState)
            {
                case TileState.Covered:
                    tile.TileState = TileState.Flagged;
                    --AssumedRemainingMines;
                    break;

                case TileState.Flagged:
                    tile.TileState = TileState.QuestionMarked;
                    ++AssumedRemainingMines;
                    break;

                case TileState.QuestionMarked:
                    tile.TileState = TileState.Covered;
                    break;
            }

            if (AssumedRemainingMines == 0)
                CheckIfGameWon();
        }

        private void CheckIfGameWon()
        {
            var coveredTiles = AllTiles.Where(t => t.TileState != TileState.Unconvered);

            if (coveredTiles.All(t => t.ContainsBomb) && coveredTiles.Count() == TotalMines)
            {
                GameState = GameState.GameEnd;
                GameEnd?.Invoke(this, new GameEndEventArgs(true));
            }
        }
    }
}