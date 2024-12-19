using Minesweeper.Views;
using MvvmEssentials.Core;
using MvvmEssentials.Core.Dialog;
using MvvmEssentials.Core.Navigation;

namespace Minesweeper
{
    /// <summary>
    /// Defines which view refers to which value for consistency in all application.
    /// </summary>
    [IsDialogContentEnum, IsNavigationContentEnum]
    public enum ViewType
    {
        [NavigateTo(DestinationType = typeof(GameEndWindow))]
        GameEnd,

        [NavigateTo(DestinationType = typeof(StatisticsWindow))]
        Statistics,

        [NavigateTo(DestinationType = typeof(ChangeDifficultyPage))]
        ChangeDifficulty
    }
}