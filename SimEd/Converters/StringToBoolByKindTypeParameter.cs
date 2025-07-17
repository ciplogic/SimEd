using System.Globalization;
using Avalonia.Data.Converters;

namespace SimEd.Converters;

public class StringToBoolByKindTypeParameter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Equals(value, parameter);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}