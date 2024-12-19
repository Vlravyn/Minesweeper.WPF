using System.Globalization;
using System.Windows.Data;

namespace Minesweeper.Converters
{
    /// <summary>
    /// Converts an <see cref="Enum"/> to <see cref="string"/>
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToStringConverter : BaseValueConverter<EnumToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return string.Empty;

            if (value is Enum @enum)
                return @enum.ToString();

            throw new ArgumentException("the passed in value must be a type of enum");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}