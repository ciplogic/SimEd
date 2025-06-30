using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace SimEd.Converters;

public class StringToFontFamilyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => value is string fontName ? new FontFamily(fontName) : string.Empty;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}