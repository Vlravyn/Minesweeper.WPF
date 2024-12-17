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
using System.Windows.Automation;
using System.Windows.Threading;

namespace Minesweeper.ViewModels
{
    public class GameWindowViewModel : ObservableObject
    {
        private readonly IDialogService dialogService;
        private readonly Statistics statistics;
        private Game _game;
        private string _currentTime;

        private DispatcherTimer dispatcherTimer;

        public Game Game
        {
            get => _game;
            set => SetProperty(ref _game, value);
        }

        private bool _showAllBombs;

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

        public RelayCommand RestartCommand => new(RestartGame);
        public RelayCommand<Tile> OpenTileCommand => new(Game.OpenTile);
        public RelayCommand<Tile> CycleCoveredStatesCommand => new(Game.CycleUncoveredStates);
        public RelayCommand OpenStatisticsCommand => new(OpenStatistics);

        /// <summary>
        /// Creates an instance of <see cref="GameWindowViewModel"/>
        /// </summary>
        /// <param name="dialogService">the dialog service being used in this application</param>
        public GameWindowViewModel(IDialogService dialogService, Statistics statistics)
        {
            this.dialogService = dialogService;
            this.statistics = statistics;

            InitializeGame();
            new SoundPlayer(new MemoryStream(Properties.Resources.GameStartAudio)).Play();
        }

        private void InitializeGame()
        {
            Game = new Game(GameDifficultyHost.Easy, statistics);
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
        private void RestartGame()
        {
            ShowAllBombs = false;
            Game.GameEnd -= OnGameEnd;
            dispatcherTimer.Tick -= UpdateTime;
            InitializeGame();
        }

        /// <summary>
        /// Updates the <see cref="CurrentTime"/> to show the time of <see cref="Game.Stopwatch"/>
        /// </summary>
        private void UpdateTime(object? sender, EventArgs e)
        {
            CurrentTime = Math.Floor(Game.Stopwatch.Elapsed.TotalSeconds).ToString();
        }
        private void OpenStatistics()
        {
            dialogService.ShowDialog(typeof(StatisticsWindow), new DialogParameters(), (callbackParameters) => { });
        }

        private void OnGameEnd(object? sender, GameEndEventArgs e)
        {
            if(!e.GameWon)
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

        private void GameEndDialogCallback(IDialogParameters? parameters)
        {
            if(parameters is not null)
            {
                bool? playAgain = (bool?)parameters.FirstOrDefault(t => t.Key =="playAgain").Value;

                if (playAgain == true)
                    RestartGame();
                else if(playAgain is null)
                {
                    bool? exitGame = (bool?)parameters.FirstOrDefault(t => t.Key == "exitGame").Value;

                    if (exitGame == true)
                        Application.Current.Shutdown();
                }
            }
        }
    }
}