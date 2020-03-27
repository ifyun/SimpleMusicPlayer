using System;
using System.Globalization;
using System.Windows.Data;

namespace SimpleMusicPlayer
{
    /// <summary>
    /// <see cref="Music.IsSelected"/>
    /// Music.IsSelected convert to Foreground.
    /// </summary>
    class SelectToForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value == true ? "Black" : "#666666";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
