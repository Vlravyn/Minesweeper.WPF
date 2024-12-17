using Minesweeper.ViewModels;
using System.Windows;

namespace Minesweeper.Views
{
    /// <summary>
    /// Interaction logic for GameEndWindow.xaml
    /// </summary>
    public partial class GameEndWindow : Window
    {
        public GameEndWindow(GameEndViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}