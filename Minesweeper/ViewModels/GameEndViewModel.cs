using Minesweeper.Core;
using Minesweeper.Core.DataModels;
using MvvmEssentials.Core;
using MvvmEssentials.Core.Commands;
using MvvmEssentials.Core.Dialog;
using MvvmEssentials.Navigation.WPF.Dialog;

namespace Minesweeper.ViewModels
{
    public class GameEndViewModel : ObservableObject, IDialogAware
    {
        private string _winOrLoseText;
        private string _timeTaken;
        private ushort _winPercentage;
        private ulong _gamesPlayed;
        private ulong _gamesWon;
        private object? _title;
        private ulong? _bestTime;

        //specifies that the user tries to play the game again if true and exit the game if false.
        private bool playAgainOrExit = true;
        private readonly Statistics _statistics;

        /// <summary>
        /// The number of games played.
        /// </summary>
        public ulong GamesPlayed
        {
            get => _gamesPlayed;
            internal set => SetProperty(ref _gamesPlayed, value);
        }

        /// <summary>
        /// The number of games won.
        /// </summary>
        public ulong GamesWon
        {
            get => _gamesWon;
            internal set => SetProperty(ref _gamesWon, value);
        }

        /// <summary>
        /// The best time that has be achieved yet.
        /// </summary>
        public ulong? BestTime
        {
            get => _bestTime;
            internal set => SetProperty(ref _bestTime, value);
        }

        public string WinOrLoseText
        {
            get => _winOrLoseText;
            set => SetProperty(ref _winOrLoseText, value);
        }

        public string TimeTaken
        {
            get => _timeTaken;
            set => SetProperty(ref _timeTaken, value);
        }

        public DateTime Date
        {
            get => DateTime.Now;
        }

        public ushort WinPercentage
        {
            get => _winPercentage;
            set => SetProperty(ref _winPercentage, value);
        }

        public RelayCommand ExitGameCommand => new(ExitGame);
        public RelayCommand StartNewGameCommand => new(StartNewGame);

        private void StartNewGame()
        {
            playAgainOrExit = true;
            Close.Invoke();
        }

        public object? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public DialogResult DialogResult { get; set; } = DialogResult.OK;
        public Action Close { get; set; }

        private void ExitGame()
        {
            playAgainOrExit = false;
            Close.Invoke();
        }

        public GameEndViewModel(Statistics statistics)
        {
            WinOrLoseText = "";
            _statistics = statistics;
        }

        public void OnClosing()
        {
        }

        public void OnOpened(IDialogParameters? parameters)
        {
            bool gameWon = (bool)parameters?.First(t => t.Key == "GameWon").Value;

            if (gameWon)
            {
                WinOrLoseText = "Congratulations, you have won the game.";
                Title = "Game won";
            }
            else
            {
                WinOrLoseText = "Unfortunately, you have lost the game.";
                Title = "Game lost.";
            }

            TimeTaken = parameters?.First(t => t.Key == "TimeTaken").Value.ToString() ?? "0";

            if(parameters?.First(t => t.Key == nameof(GameDifficultyHost)).Value is GameDifficultyHost difficulty)
            {
                StatsForDifficultyHost? currentDifficulty = difficulty.DifficultyType switch
                {
                    GameDifficulty.Easy => _statistics.EasyDifficulty,
                    GameDifficulty.Medium => _statistics.MediumDifficulty,
                    GameDifficulty.Hard => _statistics.HardDifficulty,
                    _ => null
                };

                if (currentDifficulty != null)
                {
                    GamesPlayed = currentDifficulty.GamesPlayed;
                    GamesWon = currentDifficulty.GamesWon;
                    BestTime = currentDifficulty.BestTime;

                    if (GamesPlayed > 0)
                        WinPercentage = Convert.ToUInt16((double)(GamesWon * 100) / GamesPlayed);
                }
            }
            else
                throw new ArgumentNullException(nameof(difficulty), $"The type of difficulty used must be passed to the {nameof(GameEndViewModel)}");
        }

        public IDialogParameters? ResultParameters()
        {
            var parameters = new DialogParameters();
            if (playAgainOrExit)
                parameters.Add("playAgain", true);
            else
                parameters.Add("exitGame", true);

            return parameters;
        }

        public bool CanClose()
        {
            return true;
        }
    }
}