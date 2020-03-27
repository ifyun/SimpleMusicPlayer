using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleMusicPlayer
{
    /// <summary>
    /// <see cref="Music.IsSelected"/>
    /// Music.IsSelected convert to FontWeight.
    /// </summary>
    class SelectToFontWeight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value == true ? "Bold" : "Regular";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
