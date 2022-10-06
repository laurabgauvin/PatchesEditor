using PatchesEditor.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PatchesEditor.Helpers
{
    /// <summary>
    /// Class that contains global variables and methods used by the entire application
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Application parameters
        /// </summary>
        public static AppParameters AppParameters { get; set; }

        /// <summary>
        /// Returns whether the file name ends with extension .txt
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        public static bool IsTxtFile(string fileName)
        {
            return string.Compare(Path.GetExtension(fileName), ".txt", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns whether the file name ends with extension .patch
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        public static bool IsPatchFile(string fileName)
        {
            return string.Compare(Path.GetExtension(fileName), ".patch", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns whether the file is an Android with extension .apk
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        public static bool IsAndroid(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            return string.Compare(Path.GetExtension(fileName), ".apk", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Gets the Android file name without the version
        /// </summary>
        /// <param name="fileName">Original file name</param>
        /// <param name="checkIsAndroid">Check if the file is an Android apk first?</param>
        /// <returns></returns>
        public static string GetAndroidName(string fileName, bool checkIsAndroid = false)
        {
            if (checkIsAndroid && IsAndroid(fileName) == false) return fileName;

            if (fileName.Contains(".") == false) return fileName;
            return fileName.Split('.')[0];
        }

        /// <summary>
        /// Returns whether the file name is "Service.exe"
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        public static bool IsService(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            return string.Compare(fileName, "Service.exe", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns whether the file name is "CommonFiles.exe"
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        public static bool IsCommonFiles(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            return string.Compare(fileName, "CommonFiles.exe", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns whether the file name is "SqlScripts.sql"
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns></returns>
        public static bool IsSqlScripts(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            return string.Compare(fileName, "SqlScripts.sql", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Formats the passed strings list in a comma-delimited string with the last string having an 'and'.
        /// </summary>
        /// <param name="itemsList">List of strings to join</param>
        /// <returns></returns>
        public static string BuildFormattedList(List<string> itemsList)
        {
            string formattedList;
            if (itemsList.Count == 1) formattedList = itemsList[0];
            else formattedList = string.Join(", ", itemsList.ToArray(), 0, itemsList.Count - 1) + " and " + itemsList.LastOrDefault();

            return formattedList;
        }
    }
}
