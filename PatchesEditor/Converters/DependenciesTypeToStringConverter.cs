using PatchesEditor.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PatchesEditor.Converters
{
    public class DependenciesTypeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert DependenciesType enum to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var depType = (DependenciesType)value;

            return GetValue(depType);
        }

        /// <summary>
        /// Convert string to DependenciesType enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var depType = value.ToString();

            if (depType == "All")
                return DependenciesType.All;
            else if (depType == "None")
                return DependenciesType.None;
            else if (depType == "Some")
                return DependenciesType.Some;

            return DependenciesType.Some;
        }

        /// <summary>
        /// All available DependenciesType values
        /// </summary>
        public string[] Values => GetValues();

        public static string GetValue(DependenciesType depType)
        {
            if (depType == DependenciesType.All)
                return "All";
            else if (depType == DependenciesType.None)
                return "None";
            else if (depType == DependenciesType.Some)
                return "Some";

            return "";
        }

        public static string[] GetValues()
        {
            List<string> list = new List<string>();
            foreach (DependenciesType depType in Enum.GetValues(typeof(DependenciesType)))
            {
                list.Add(GetValue(depType));
            }

            return list.ToArray();
        }
    }
}
