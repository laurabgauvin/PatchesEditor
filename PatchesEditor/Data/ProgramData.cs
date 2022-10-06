using Newtonsoft.Json;
using PatchesEditor.Helpers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace PatchesEditor.Data
{
    /// <summary>
    /// Class that contains the data for a program being patched out
    /// </summary>
    public class ProgramData : INotifyPropertyChanged
    {
        #region Private Variables

        private string _fullPath;
        private string _programName;
        private string _versionString;
        private DateTime _createdDate;
        private long _size;
        private bool _modified;

        #endregion

        #region Properties

        /// <summary>
        /// Full path of the program (including program name)
        /// </summary>
        [JsonIgnore]
        public string FullPath
        {
            get => _fullPath;
            set
            {
                if (value != _fullPath)
                {
                    _fullPath = value;
                    _modified = true;
                    OnPropertyChanged("FullPath");
                }
            }
        }

        /// <summary>
        /// Program name (including extension)
        /// </summary>
        public string ProgramName
        {
            get => _programName;
            set
            {
                if (value != _programName)
                {
                    _programName = value;
                    _modified = true;
                    OnPropertyChanged("ProgramName");
                }
            }
        }

        /// <summary>
        /// File version (formatted string)
        /// </summary>
        public string VersionString
        {
            get => _versionString;
            set
            {
                if (value != _versionString)
                {
                    _versionString = value;
                    _modified = true;
                    OnPropertyChanged("VersionString");
                }
            }
        }

        /// <summary>
        /// Created date
        /// </summary>
        public DateTime CreatedDate
        {
            get => _createdDate;
            set
            {
                if (value != _createdDate)
                {
                    _createdDate = value;
                    _modified = true;
                    OnPropertyChanged("CreatedDate");
                }
            }
        }

        /// <summary>
        /// Size
        /// </summary>
        public long Size
        {
            get => _size;
            set
            {
                if (value != _size)
                {
                    _size = value;
                    _modified = true;
                    OnPropertyChanged("Size");
                }
            }
        }

        /// <summary>
        /// Modified flag
        /// </summary>
        [JsonIgnore]
        public bool Modified { get => _modified; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProgramData()
        {
            FullPath = string.Empty;
            ProgramName = string.Empty;
            VersionString = string.Empty;
            CreatedDate = DateTime.MinValue;
            Size = 0;
        }

        /// <summary>
        /// Constructor from a file path
        /// </summary>
        /// <param name="fullPath">Program file path</param>
        public ProgramData(string fullPath)
        {
            FullPath = fullPath;
            GenerateProperties();
        }

        /// <summary>
        /// Constructor to create a copy of an existing object
        /// </summary>
        /// <param name="origProgramData">Original object to copy</param>
        public ProgramData(ProgramData origProgramData)
        {
            FullPath = origProgramData.FullPath;
            ProgramName = origProgramData.ProgramName;
            VersionString = origProgramData.VersionString;
            CreatedDate = origProgramData.CreatedDate;
            Size = origProgramData.Size;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Grabs all properties from the file from FullPath
        /// </summary>
        private void GenerateProperties()
        {
            if (string.IsNullOrEmpty(FullPath))
            {
                FullPath = string.Empty;
                ProgramName = string.Empty;
                VersionString = string.Empty;
                CreatedDate = DateTime.MinValue;
                Size = 0;
                return;
            }

            try
            {
                ProgramName = Path.GetFileName(FullPath);
                GenerateVersionString();
                CreatedDate = File.GetLastWriteTime(FullPath);

                // Get the size of the file so that it matches what is displayed in Windows Explorer
                int tmp = Math.DivRem((int)new FileInfo(FullPath).Length, 1024, out int remaining);
                if (remaining > 0) tmp++;
                Size = tmp;
            }
            catch (Exception e)
            {
                MessageBox.Show($"GenerateProperties: Error reading file properties for {FullPath}: {e.Message}");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the modified flag
        /// </summary>
        public void Reset()
        {
            _modified = false;
        }

        /// <summary>
        /// Generates the string version number
        /// </summary>
        public void GenerateVersionString()
        {
            if (Globals.IsAndroid(ProgramName))
            {
                // For Android apk files, the version is in the file name
                string apkVersion = string.Empty;
                string[] tmp = ProgramName.Split('.');
                if (tmp.Length > 1)
                {
                    foreach (string item in tmp[1].Split('_'))
                    {
                        if (int.TryParse(item, out int number))
                            apkVersion += number + ".";
                    }

                    apkVersion = apkVersion.TrimEnd('.');
                }
                VersionString = apkVersion;
            }
            else
            {
                if (string.IsNullOrEmpty(FullPath) == false)
                {
                    VersionString = FileVersionInfo.GetVersionInfo(FullPath).FileVersion;
                }
            }
        }

        /// <summary>
        /// Imports a text line of Programs Used
        /// </summary>
        /// <param name="line">Text line of Programs Used</param>
        /// <returns></returns>
        public void ImportProgramsUsed(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;

            string[] tmp = line.Split(',');
            if (tmp.Length < 3) return;

            ProgramName = tmp[0];
            if (DateTime.TryParse(tmp[1].Substring(9), out DateTime date))
                CreatedDate = date;
            if (long.TryParse(tmp[2].Substring(6).Replace("KB", "").Trim(), out long size))
                Size = size;

            if (tmp.Length >= 4 && tmp[3].Length > 10) VersionString = tmp[3].Substring(9);
        }

        #endregion

        #region INotify interface

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
