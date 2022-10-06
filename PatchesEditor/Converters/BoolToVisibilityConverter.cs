using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace PatchesEditor.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Bool to visibility (if true, visible, else collapsed)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        /// <summary>
        /// Visibility to bool
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = (Visibility)value;

            if (visible == Visibility.Visible) return true;
            else return false;
        }
    }
}
