using Minesweeper.Core.DataModels;
using MvvmEssentials.Core;
using MvvmEssentials.Core.Commands;
using MvvmEssentials.Core.Dialog;
using MvvmEssentials.Navigation.WPF.Dialog;

namespace Minesweeper.ViewModels
{
    public class ChangeDifficultyViewModel : ObservableObject, IDialogAware
    {
        private ushort? _rows;
        private ushort? _columns;
        private ushort? _mines;
        private GameDifficulty _selectedDifficulty;

        public object? Title => "Change Difficulty";
        public DialogResult DialogResult { get; set; } = DialogResult.Cancel;
        public Action Close { get; set; }

        /// <summary>
        /// The rows set by the user for custom difficulty.
        /// </summary>
        public ushort? Rows
        {
            get => _rows;
            set => SetProperty(ref _rows, value);
        }

        /// <summary>
        /// the columns set by the user for custom difficulty
        /// </summary>
        public ushort? Columns
        {
            get => _columns;
            set => SetProperty(ref _columns, value);
        }

        /// <summary>
        /// the mines set by the user for custom difficulty
        /// </summary>
        public ushort? Mines
        {
            get => _mines;
            set => SetProperty(ref _mines, value);
        }

        /// <summary>
        /// The current selected difficulty
        /// </summary>
        public GameDifficulty SelectedDifficulty
        {
            get => _selectedDifficulty;
            set => SetProperty(ref _selectedDifficulty, value);
        }

        public RelayCommand<GameDifficulty> UpdateSelectedDifficultyCommand => new(UpdateSelectedDifficulty);
        public RelayCommand OKCommand => new(ConfirmNewDifficulty);
        public RelayCommand CancelCommand => new(CloseThisView);

        /// <summary>
        /// Creates an instance of <see cref="ChangeDifficultyViewModel"/>
        /// </summary>
        public ChangeDifficultyViewModel()
        {
        }

        private void CloseThisView()
        {
            CloseWithoutSave = true;
            Close?.Invoke();
        }

        /// <summary>
        /// Updates the <see cref="SelectedDifficulty"/>
        /// </summary>
        /// <param name="difficulty">the new selected difficulty</param>
        private void UpdateSelectedDifficulty(GameDifficulty difficulty)
        {
            SelectedDifficulty = difficulty;
        }

        /// <summary>
        ///
        /// </summary>
        private void ConfirmNewDifficulty()
        {
            DialogResult = DialogResult.OK;
            Close?.Invoke();
        }

        private bool CloseWithoutSave = false;
        public bool CanClose()
        {
            if (CloseWithoutSave)
                return true;
            if (SelectedDifficulty != GameDifficulty.Custom)
                return true;
            else
            {
                if (Rows != null && Columns != null && Mines != null)
                    return true;
                return false;
            }
        }

        public void OnClosing()
        {
        }

        public void OnOpened(IDialogParameters? parameters)
        {
            var currentDifficulty = parameters?.FirstOrDefault(t => t.Key == "currentDifficulty").Value as GameDifficultyHost;

            if (currentDifficulty is null)
                return;

            SelectedDifficulty = currentDifficulty.DifficultyType;
            CloseWithoutSave = false;
        }

        public IDialogParameters? ResultParameters()
        {           
            if(CloseWithoutSave)
                return null;
            else
            {
                GameDifficultyHost chosenDifficulty = SelectedDifficulty switch
                {
                    GameDifficulty.Easy => GameDifficultyHost.Easy,
                    GameDifficulty.Medium => GameDifficultyHost.Medium,
                    GameDifficulty.Hard => GameDifficultyHost.Hard,
                    GameDifficulty.Custom => new GameDifficultyHost()
                    {
                        //This possible null reference is addressed at CanClose() method, which does not allow view to close when values are null
                        //so this method is never run until the values are explicitely set.
                        Rows = (ushort)Rows,
                        Columns = (ushort)Columns,
                        Mines = (ushort)Mines,
                    }
                };
                return new DialogParameters()
                {
                    {"newDifficulty",  chosenDifficulty}
                };
            }
        }
    }
}