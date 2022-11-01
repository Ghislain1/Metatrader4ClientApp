﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Metatrader4ClientApp.Infrastructure.Converters
{
    public class PercentConverter : IValueConverter
    {
        public static PercentConverter Instance { get; } = new();

        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var result = value as decimal?;
            if (result == null)
                result = 0;

            return System.String.Format(CultureInfo.CurrentUICulture, "{0:F1}%", result.Value);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

       
    }
}
