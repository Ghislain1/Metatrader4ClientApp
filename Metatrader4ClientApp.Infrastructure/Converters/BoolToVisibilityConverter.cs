// <copyright company="Ghislain One Inc.">
//  Copyright (c) GhislainOne
//  This computer program includes confidential, proprietary
//  information and is a trade secret of GhislainOne. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of Ghis. All Rights Reserved.
// </copyright>

using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Data;
using System;

namespace Metatrader4ClientApp.Infrastructure.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    public class BoolToVisibilityConverter : IValueConverter
    {
        public static BoolToVisibilityConverter Instance { get; } = new();
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return (value is true) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
