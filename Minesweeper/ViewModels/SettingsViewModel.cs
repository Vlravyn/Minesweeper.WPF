using MvvmEssentials.Core;
using MvvmEssentials.Core.Common;
using MvvmEssentials.Core.Dialog;

namespace Minesweeper.ViewModels
{
    internal class SettingsViewModel : ObservableObject, IViewAware
    {
        public Action Close { get; set; }



        public SettingsViewModel()
        {
            
        }
        public bool CanClose()
        {
            return true;
        }

        public void OnClosing()
        {
        }

        public void OnOpened(IParameters? parameters)
        {
        }
    }
}
