using Minesweeper.Core;
using Minesweeper.Core.DataModels;
using Minesweeper.Views;
using MvvmEssentials.Core;
using MvvmEssentials.Core.Commands;
using MvvmEssentials.Core.Common;
using MvvmEssentials.Core.Dialog;
using MvvmEssentials.Navigation.WPF.Dialog;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Threading;

namespace Minesweeper.ViewModels
{
    public class GameWindowViewModel : ObservableObject, IViewAware
    {
        private readonly IDialogService dialogService;
        private readonly Statistics statistics;
        private Game _game;
        private string _currentTime;
        private bool _showAllBombs;

        /// <summary>
        /// The timer to invoke an event periodically to update the time according to <see cref="Game.Stopwatch"/> time.
        /// </summary>
        private DispatcherTimer dispatcherTimer;

        public Game Game
        {
            get => _game;
            set => SetProperty(ref _game, value);
        }


        /// <summary>
        /// Sets whether all the locations of the bombs should be shown.
        /// </summary>
        public bool ShowAllBombs
        {
            get => _showAllBombs;
            set => SetProperty(ref _showAllBombs, value);
        }

        public string CurrentTime
        {
            get => _currentTime;
            set => SetProperty(ref _currentTime, value);
        }

        public RelayCommand RestartCommand => new(() => RestartGame());
        public RelayCommand ChangeDifficultyCommand => new(OpenChangeDifficultyView);
        public RelayCommand<Tile> OpenTileCommand => new(Game.OpenTile);
        public RelayCommand<Tile> CycleCoveredStatesCommand => new(Game.CycleUncoveredStates);
        public RelayCommand OpenStatisticsCommand => new(OpenStatistics);

        public Action Close { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="GameWindowViewModel"/>
        /// </summary>
        /// <param name="dialogService">the dialog service being used in this application</param>
        public GameWindowViewModel(IDialogService dialogService, Statistics statistics)
        {
            this.dialogService = dialogService;
            this.statistics = statistics;
        }
        /// <summary>
        /// Initializes a new game.
        /// </summary>
        /// <param name="difficulty">the difficulty for this game.</param>
        private void InitializeGame(GameDifficultyHost difficulty)
        {
            Game = new Game(difficulty, statistics);
            Game.GameEnd += OnGameEnd;

            //Setting up dispatcher timer to update time according to the stopwatch inside the game.
            dispatcherTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };
            dispatcherTimer.Tick += UpdateTime;
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Restarts the game
        /// </summary>
        private void RestartGame(GameDifficultyHost? difficulty = null)
        {
            ShowAllBombs = false;
            Game.GameEnd -= OnGameEnd;
            dispatcherTimer.Tick -= UpdateTime;
            InitializeGame(difficulty ?? Game.Difficulty);
        }

        /// <summary>
        /// Updates the <see cref="CurrentTime"/> to show the time of <see cref="Game.Stopwatch"/>
        /// </summary>
        private void UpdateTime(object? sender, EventArgs e)
        {
            CurrentTime = Math.Floor(Game.Stopwatch.Elapsed.TotalSeconds).ToString();
        }

        /// <summary>
        /// Opens the statistics view.
        /// </summary>
        private void OpenStatistics()
        {
            Game.Stopwatch.Stop();
            dialogService.ShowDialog(typeof(StatisticsWindow), new DialogParameters(), (callbackParameters) => { });

            if (Game.Stopwatch.ElapsedMilliseconds > 0 && !Game.Stopwatch.IsRunning)
                Game.Stopwatch.Start();
        }
        /// <summary>
        /// Opens the view which allows the difficulty to be changed.
        /// </summary>
        private void OpenChangeDifficultyView()
        {
            Game.Stopwatch.Stop();
            dialogService.ShowDialog(ViewType.ChangeDifficulty, new DialogParameters()
            {
                {"currentDifficulty", Game.Difficulty}
            }, ChangeDifficultyCallback);

            if (Game.Stopwatch.ElapsedMilliseconds > 0 && !Game.Stopwatch.IsRunning)
                Game.Stopwatch.Start();
        }

        /// <summary>
        /// Callback method after the change difficulty view is closed.
        /// </summary>
        /// <param name="parameters">the parameters passed back from the view.</param>
        private void ChangeDifficultyCallback(IDialogParameters? parameters)
        {
            var chosenDifficulty = parameters?.FirstOrDefault(t => t.Key == "newDifficulty").Value;

            if (chosenDifficulty is GameDifficultyHost cd)
                RestartGame(cd);
        }

        /// <summary>
        /// Run when the game ends.
        /// </summary>
        /// <param name="sender">the object that invoked this method.</param>
        /// <param name="e">the event args for this method</param>
        private void OnGameEnd(object? sender, GameEndEventArgs e)
        {
            if (!e.GameWon)
            {
                new SoundPlayer(new MemoryStream(Properties.Resources.GameLoseAudio)).Play();
                ShowAllBombs = true;
            }

            dialogService.ShowDialog(typeof(GameEndWindow), new DialogParameters()
            {
                { "GameWon", e.GameWon },
                { "TimeTaken", Math.Floor(Game.Stopwatch.Elapsed.TotalSeconds) },
                { nameof(GameDifficultyHost), Game.Difficulty }
            }, GameEndDialogCallback);
        }

        /// <summary>
        /// Run when the view showing the dialog telling the user that the game has ended closes.
        /// </summary>
        /// <param name="parameters">the parameters that the dialog passed back to this class.</param>
        private void GameEndDialogCallback(IDialogParameters? parameters)
        {
            if (parameters is not null)
            {
                bool? playAgain = (bool?)parameters.FirstOrDefault(t => t.Key == "playAgain").Value;

                if (playAgain == true)
                    RestartGame(Game.Difficulty);
                else if (playAgain is null)
                {
                    bool? exitGame = (bool?)parameters.FirstOrDefault(t => t.Key == "exitGame").Value;

                    if (exitGame == true)
                        Application.Current.Shutdown();
                }
            }
        }

        public void OnOpened(IParameters? parameters)
        {
            InitializeGame(GameDifficultyHost.Easy);
            new SoundPlayer(new MemoryStream(Properties.Resources.GameStartAudio)).Play();
        }

        public void OnClosing()
        {
        }

        public bool CanClose() => true;
    }
}