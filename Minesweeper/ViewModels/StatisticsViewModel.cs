using Minesweeper.Core;
using Minesweeper.Core.DataModels;
using MvvmEssentials.Core;
using MvvmEssentials.Core.Commands;
using MvvmEssentials.Core.Dialog;

namespace Minesweeper.ViewModels
{
    public class StatisticsViewModel : ObservableObject
    {
        private Statistics _statistics;
        private StatsForDifficultyHost _selectedDifficultyStats;

        public object? Title => "Statistics";

        public DialogResult DialogResult { get; set; } = DialogResult.OK;

        public StatsForDifficultyHost SelectedDifficultyStats
        {
            get => _selectedDifficultyStats;
            set => SetProperty(ref _selectedDifficultyStats, value);
        }

        public Action Close { get; set; }

        public RelayCommand<GameDifficulty> SelectedDifficultyCommand => new(UpdateScoredForSelectedDifficulty);
        public RelayCommand ResetStatsCommand => new(_statistics.ResetStatistics);

        public StatisticsViewModel(Statistics statistics)
        {
            _statistics = statistics;
            UpdateScoredForSelectedDifficulty(GameDifficulty.Easy);
        }

        private void UpdateScoredForSelectedDifficulty(GameDifficulty difficulty)
        {
            SelectedDifficultyStats = difficulty switch
            {
                GameDifficulty.Easy => _statistics.EasyDifficulty,
                GameDifficulty.Medium => _statistics.MediumDifficulty,
                GameDifficulty.Hard => _statistics.HardDifficulty,
                _ => throw new Exception("Cannot show stats for this difficulty")
            };
        }

        public IDialogParameters? ResultParameters()
        {
            return null;
        }
    }
}