using Minesweeper.Views;
using MvvmEssentials.Core.Dialog;
using System.Windows;

namespace Minesweeper.Services
{
    internal class ApplicationHostService
    {
        public ApplicationHostService(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        private IDialogService dialogService;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            await Task.CompletedTask;

            if (!Application.Current.Windows.OfType<GameWindow>().Any())
            {
                dialogService.Show(typeof(GameWindow));
            }

            await Task.CompletedTask;
        }
    }
}