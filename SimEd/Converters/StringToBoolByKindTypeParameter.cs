using System.Globalization;
using Avalonia.Data.Converters;

namespace SimEd.Converters;

public class StringToBoolByKindTypeParameter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is string str && value is string target)
        {

            return str.Split(',').Any(x => Equals(x, target));
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}