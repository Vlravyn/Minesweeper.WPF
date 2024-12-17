using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minesweeper.Core;
using Minesweeper.Services;
using Minesweeper.ViewModels;
using Minesweeper.Views;
using MvvmEssentials.Core.Dialog;
using MvvmEssentials.Navigation.WPF.Dialog;
using System.IO;
using System.Reflection;
using System.Windows;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly IHost _host =
           Host
           .CreateDefaultBuilder()
           .ConfigureAppConfiguration(c =>
           {
               var entryAssembly = Assembly.GetEntryAssembly() ?? throw new Exception("Entry assembly is null. Occured while cofigurating the application host.");

               var assemblyPath = Path.GetDirectoryName(entryAssembly.Location);
               if (string.IsNullOrEmpty(assemblyPath))
                   throw new Exception("Directory path of EntryAssembly is null");

               c.SetBasePath(assemblyPath);
           })
           .ConfigureAppConfiguration(d1 =>
           {
           })
           .ConfigureServices((context, services) =>
           {
               services.AddSingleton<IDialogService, DialogService>();

               services.AddSingleton<Statistics>();

               services.AddSingleton<ApplicationHostService>();

               services.AddSingleton<GameWindowViewModel>();
               services.AddSingleton<GameEndViewModel>();
               services.AddSingleton<StatisticsViewModel>();

               services.AddSingleton<GameWindow>();
               services.AddSingleton<GameEndWindow>();
               services.AddTransient<StatisticsWindow>();
           })
           .Build();

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();
            await GetService<ApplicationHostService>().StartAsync(new CancellationToken());
        }

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>() where T : class
        {
            var result = _host.Services.GetService(typeof(T));
            if (result is T converted)
                return converted;

            throw new Exception($"Could not find a registered service of type {typeof(T).Name}");
        }
    }
}