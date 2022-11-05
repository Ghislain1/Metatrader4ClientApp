
namespace Metatrader4ClientApp.Infrastructure.Converters
{
    using MaterialDesignThemes.Wpf;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;


    public class GlyphToPackIconKindConverter : IValueConverter
    {
        public static GlyphToPackIconKindConverter Instance { get; } = new();
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
             
            if (value is string glyphValue)
            {
                return Enum.Parse<PackIconKind>(glyphValue);
            }

            return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
