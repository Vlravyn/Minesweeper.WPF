using Minesweeper.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace Minesweeper.Views
{
    /// <summary>
    /// Interaction logic for ChangeDifficultyPage.xaml
    /// </summary>
    public partial class ChangeDifficultyPage : Page
    {
        public ChangeDifficultyPage(ChangeDifficultyViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private Regex PositiveInteger = new("^[0-9]*$");

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

                if (!PositiveInteger.IsMatch(newText))
                    e.Handled = true;
            }
        }
    }
}