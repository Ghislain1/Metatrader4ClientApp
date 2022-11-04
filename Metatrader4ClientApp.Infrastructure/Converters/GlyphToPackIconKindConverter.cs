// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.Converters
{
  
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
 
 
public class GlyphToPackIconKindConverter : IValueConverter
{
    public static CurrencyConverter Instance { get; } = new();
    public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        var result = value as decimal?;

        if (result == null)
            result = 0;

        return System.String.Format(CultureInfo.CurrentUICulture, "{0:C}", result.Value);
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
    {
        throw new System.NotImplementedException();
    }
}
}
