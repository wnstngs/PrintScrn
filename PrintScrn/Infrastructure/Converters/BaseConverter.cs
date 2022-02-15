using System;
using System.Globalization;
using System.Windows.Data;

namespace PrintScrn.Infrastructure.Converters;

public abstract class BaseConverter : IValueConverter
{
    public abstract object? Convert(object value, Type type, object parameter, CultureInfo cultureInfo);

    public abstract object? ConvertBack(object value, Type type, object parameter, CultureInfo cultureInfo);
}
