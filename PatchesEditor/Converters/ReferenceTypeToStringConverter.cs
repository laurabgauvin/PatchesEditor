using PatchesEditor.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace PatchesEditor.Converters
{
    public class ReferenceTypeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert ReferenceType enum to string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var refType = (ReferenceType)value;

            return GetValue(refType);
        }

        /// <summary>
        /// Convert string to ReferenceType enum
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var refType = value.ToString();

            if (refType == "Quote #")
                return ReferenceType.Quote;
            else if (refType == "Reference #")
                return ReferenceType.Reference;
            else if (refType == "Defect #")
                return ReferenceType.Defect;

            return ReferenceType.Reference;
        }

        /// <summary>
        /// All values to display on the dropdown
        /// </summary>
        public string[] Values => GetValues();

        /// <summary>
        /// Get formatted string for passed ReferenceType
        /// </summary>
        /// <param name="refType"></param>
        /// <returns></returns>
        public static string GetValue(ReferenceType refType)
        {
            if (refType == ReferenceType.Quote)
                return "Quote #";
            else if (refType == ReferenceType.Reference)
                return "Reference #";
            else if (refType == ReferenceType.Defect)
                return "Defect #";

            return "";
        }

        /// <summary>
        /// Get all string values
        /// </summary>
        /// <returns></returns>
        public static string[] GetValues()
        {
            List<string> list = new List<string>();
            foreach (ReferenceType refType in Enum.GetValues(typeof(ReferenceType)))
            {
                list.Add(GetValue(refType));
            }

            return list.ToArray();
        }
    }
}
