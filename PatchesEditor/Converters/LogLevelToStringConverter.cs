using PatchesEditor.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace PatchesEditor.Converters
{
    public class LogLevelToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert LogLevel enum to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logLevel = (LogLevel)value;

            return GetValue(logLevel);
        }

        /// <summary>
        /// Convert string to LogLevel enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var logLevel = value.ToString();

            if (logLevel == "Basic")
                return LogLevel.Basic;
            else if (logLevel == "Everything")
                return LogLevel.Everything;

            return LogLevel.Basic;
        }

        public string[] Values => GetValues();

        public static string GetValue(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Basic)
                return "Basic";
            else if (logLevel == LogLevel.Everything)
                return "Everything";

            return "";
        }

        public static string[] GetValues()
        {
            List<string> list = new List<string>();
            foreach (LogLevel logLevel in Enum.GetValues(typeof(LogLevel)))
            {
                list.Add(GetValue(logLevel));
            }

            return list.ToArray();
        }
    }
}
