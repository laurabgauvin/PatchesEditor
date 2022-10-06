using PatchesEditor.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace PatchesEditor.Converters
{
    public class DependencyStatusToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert DependencyStatus enum to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var depStatus = (DependencyStatus)value;

            return GetValue(depStatus);
        }

        /// <summary>
        /// Convert string to DependencyStatus enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var depStatus = value.ToString();

            if (depStatus == "Not Required")
                return DependencyStatus.NotRequired;
            else if (depStatus == "Required")
                return DependencyStatus.Required;
            else if (depStatus == "Previous")
                return DependencyStatus.Previous;

            return DependencyStatus.NotRequired;
        }

        /// <summary>
        /// All available DependencyStatus values
        /// </summary>
        public string[] Values => GetValues();

        public static string GetValue(DependencyStatus depStatus)
        {
            if (depStatus == DependencyStatus.NotRequired)
                return "Not Required";
            else if (depStatus == DependencyStatus.Required)
                return "Required";
            else if (depStatus == DependencyStatus.Previous)
                return "Previous";

            return "";
        }

        public static string[] GetValues()
        {
            List<string> list = new List<string>();
            foreach (DependencyStatus depStatus in Enum.GetValues(typeof(DependencyStatus)))
            {
                list.Add(GetValue(depStatus));
            }

            return list.ToArray();
        }
    }
}
