﻿

namespace Metatrader4ClientApp.Infrastructure.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        public static InverseBoolConverter Instance { get; } = new();

        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
            value is false;

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) =>
            value is false;
    }
}
