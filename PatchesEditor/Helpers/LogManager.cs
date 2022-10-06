using PatchesEditor.Data;
using System;
using System.IO;
using System.Windows;

namespace PatchesEditor.Helpers
{
    /// <summary>
    /// Static class that contains methods to write to the log file
    /// </summary>
    public static class LogManager
    {
        private static readonly string logFilePath = Path.Combine(@"C:\Patches Editor\", "PatchesEditorLog.txt");

        /// <summary>
        /// Writes an entry at the end of the log file
        /// </summary>
        /// <param name="level">Log level for this log entry</param>
        /// <param name="message">Log entry</param>
        public static void WriteToLog(LogLevel level, string message)
        {
            // Don't write 'everything' entries if the param is set to 'basic'
            if (level == LogLevel.Everything && Globals.AppParameters.LogLevel == LogLevel.Basic) return;

            try
            {
                CreateLogFile();
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    string line = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss} - {message}";
                    streamWriter.WriteLine(line);
                    streamWriter.Close();
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not access log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the log file if it doesn't exist
        /// </summary>
        private static void CreateLogFile()
        {
            if (File.Exists(logFilePath) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.OpenOrCreate)) 
                { 
                    fileStream.Close();
                }
            }
        }
    }
}
