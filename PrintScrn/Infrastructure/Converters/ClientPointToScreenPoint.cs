using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PrintScrn.Infrastructure.Converters;

public class ClientPointToScreenPoint : BaseConverter
{
    public override object? Convert(object value, Type type, object parameter, CultureInfo cultureInfo)
    {
        if (value is not Point point)
        {
            return null;
        }
        if (parameter is not Control control)
        {
            return null;
        }

        return control.PointToScreen(point);
    }
    
    public override object? ConvertBack(object value, Type type, object parameter, CultureInfo cultureInfo)
    {
        if (value is not Point point)
        {
            return null;
        }
        if (parameter is not Control control)
        {
            return null;
        }

        return control.PointFromScreen(point);
    }
}
