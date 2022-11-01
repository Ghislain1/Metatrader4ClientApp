﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Metatrader4ClientApp.Infrastructure.Converters
{
    public class DecimalToColorConverter : IValueConverter
    {        public static DecimalToColorConverter Instance { get; } = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is decimal))
            {
                return null;
            }

            decimal decimalValue = (decimal)value;
            string color;
            if (decimalValue < 0m)
            {
                color = "#ffff0000";
            }
            else
            {
                color = "#ff00cc00";
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

    
    }
}