using Newtonsoft.Json;
using System.ComponentModel;

namespace PatchesEditor.Data
{
    /// <summary>
    /// This class contains the information for a dependency program
    /// </summary>
    public class DependencyProgram : INotifyPropertyChanged
    {
        #region Private Variables

        private string _programName;
        private DependencyStatus _status;
        private string _version;
        private bool _modified;

        #endregion

        #region Properties

        /// <summary>
        /// Available program name
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
        /// Dependency status for the program
        /// </summary>
        public DependencyStatus Status
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    _modified = true;
                    OnPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Program version
        /// </summary>
        public string Version
        {
            get => _version;
            set
            {
                if (value != _version)
                {
                    _version = value;
                    _modified = true;
                    OnPropertyChanged("Version");
                }
            }
        }

        /// <summary>
        /// Modified status
        /// </summary>
        [JsonIgnore]
        public bool Modified { get => _modified; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DependencyProgram()
        {
            ProgramName = string.Empty;
            Status = DependencyStatus.NotRequired;
            Version = string.Empty;
        }

        /// <summary>
        /// Parameter constructor
        /// </summary>
        /// <param name="program">Program name</param>
        /// <param name="status">Dependency status</param>
        /// <param name="version">Program version</param>
        public DependencyProgram(string program, DependencyStatus status, string version)
        {
            ProgramName = program;
            Status = status;
            Version = version;
        }

        /// <summary>
        /// Creates a new instance of a DependencyProgram class
        /// </summary>
        /// <param name="origDependencyProgram">Original object</param>
        public DependencyProgram(DependencyProgram origDependencyProgram)
        {
            ProgramName = origDependencyProgram.ProgramName;
            Status = origDependencyProgram.Status;
            Version = origDependencyProgram.Version;
        }

        #endregion

        #region Public Methods

        public void Reset()
        {
            _modified = false;
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
