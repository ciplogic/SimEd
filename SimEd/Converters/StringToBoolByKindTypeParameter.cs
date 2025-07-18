using System.Globalization;
using Avalonia.Data.Converters;
using ZLinq;

namespace SimEd.Converters;

public class StringToBoolByKindTypeParameter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is not string str || value is not string target)
        {
            return false;
        }

        if (!str.Contains(','))
        {
            return str == target;
        }

        string[] commaSeparatedStrings = str.Split(',');
        return commaSeparatedStrings.AsValueEnumerable().Any(x => Equals(x, target));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}