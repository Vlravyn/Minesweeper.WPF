using Minesweeper.ViewModels;
using System.Windows;

namespace Minesweeper.Views
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow(GameWindowViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}