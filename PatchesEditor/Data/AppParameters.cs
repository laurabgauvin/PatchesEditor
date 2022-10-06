using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PatchesEditor.Data
{
    /// <summary>
    /// Class that contains the parameters for the application
    /// </summary>
    public class AppParameters : INotifyPropertyChanged
    {
        #region Private variables

        private LogLevel _logLevel;
        private string _mergeSaveFileName;
        private string _patchesDirectory;
        private string _noneImpact;
        private string _checkpointImpact;
        private string _allDependencies;
        private string _noneDependencies;
        private bool _spellcheck;
        private bool _modified;

        #endregion

        #region Properties

        /// <summary>
        /// List of available programmers
        /// </summary>
        public ObservableCollection<string> Programmers { get; set; }

        /// <summary>
        /// List of available testers
        /// </summary>
        public ObservableCollection<string> Testers { get; set; }

        /// <summary>
        /// List of programs in the Install\Resources folder
        /// </summary>
        public ObservableCollection<string> ProgramsResources { get; set; }

        /// <summary>
        /// Listed of extensions ignored when generating programs list
        /// </summary>
        public ObservableCollection<string> IgnoredExtentions { get; set; }

        /// <summary>
        /// Version of the Params file
        /// </summary>
        public int ParametersVersion { get; set; }

        /// <summary>
        /// Patches directory
        /// </summary>
        public string PatchesDirectory
        {
            get => _patchesDirectory;
            set
            {
                if (value != _patchesDirectory)
                {
                    _modified = true;
                    _patchesDirectory = value;
                    OnPropertyChanged("PatchesDirectory");
                }
            }
        }

        /// <summary>
        /// List of programs that should have a resource file
        /// </summary>
        public ObservableCollection<string> ProgramsWithResourceFiles { get; set; }

        /// <summary>
        /// Programs that should always have all dependencies
        /// </summary>
        public ObservableCollection<string> AllDependenciesPrograms { get; set; }

        /// <summary>
        /// Programs that should always have no dependencies
        /// </summary>
        public ObservableCollection<string> NoneDependenciesPrograms { get; set; }

        /// <summary>
        /// Name of the merge save file
        /// </summary>
        public string MergeSaveFileName
        {
            get => _mergeSaveFileName;
            set
            {
                if (value != _mergeSaveFileName)
                {
                    _modified = true;
                    _mergeSaveFileName = value;
                    OnPropertyChanged("MergeSaveFileName");
                }
            }
        }

        /// <summary>
        /// Programs for which the digital signature and copyright checks should not be performed
        /// </summary>
        public ObservableCollection<string> ProgramsIgnoreSignatureCopyright { get; set; }

        /// <summary>
        /// Default 'none' impact text
        /// </summary>
        public string NoneImpact
        {
            get => _noneImpact;
            set
            {
                if (value != _noneImpact)
                {
                    _modified = true;
                    _noneImpact = value;
                    OnPropertyChanged("NoneImpact");
                }
            }
        }

        /// <summary>
        /// Default checkpoint impact text
        /// </summary>
        public string CheckpointImpact
        {
            get => _checkpointImpact;
            set
            {
                if (value != _checkpointImpact)
                {
                    _modified = true;
                    _checkpointImpact = value;
                    OnPropertyChanged("CheckpointImpact");
                }
            }
        }

        /// <summary>
        /// Default 'all' dependencies text
        /// </summary>
        public string AllDependencies
        {
            get => _allDependencies;
            set
            {
                if (value != _allDependencies)
                {
                    _modified = true;
                    _allDependencies = value;
                    OnPropertyChanged("AllDependencies");
                }
            }
        }

        /// <summary>
        /// Default 'none' dependencies text
        /// </summary>
        public string NoneDependencies
        {
            get => _noneDependencies;
            set
            {
                if (value != _noneDependencies)
                {
                    _modified = true;
                    _noneDependencies = value;
                    OnPropertyChanged("NoneDependencies");
                }
            }
        }

        /// <summary>
        /// Log level
        /// </summary>
        public LogLevel LogLevel
        {
            get => _logLevel;
            set
            {
                if (value != _logLevel)
                {
                    _modified = true;
                    _logLevel = value;
                    OnPropertyChanged("LogLevel");
                }
            }
        }

        /// <summary>
        /// Spellcheck
        /// </summary>
        public bool Spellcheck
        {
            get => _spellcheck;
            set
            {
                if (value != _spellcheck)
                {
                    _modified = true;
                    _spellcheck = value;
                    OnPropertyChanged("Spellcheck");
                }
            }
        }

        /// <summary>
        /// Modified flag
        /// </summary>
        [JsonIgnore]
        public bool Modified { get => _modified; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="setDefaults">Set default values</param>
        public AppParameters(bool setDefaults = false)
        {
            if (setDefaults)
                SetDefaults();

            Reset();
        }

        /// <summary>
        /// Set default values
        /// </summary>
        private void SetDefaults()
        {
            ParametersVersion = 1;
            LogLevel = LogLevel.Basic;
            PatchesDirectory = @"C:\Project\patches\{0}\";
            MergeSaveFileName = "!MERGE_SAVE.patch";
            Spellcheck = false;

            NoneImpact = "This change can be applied with no impact to the system.";
            CheckpointImpact = "If you are currently running a service older than version {0}, applying this service will generate a checkpoint.";
            AllDependencies = "Ensure that all current patches are applied.";
            NoneDependencies = "None.";

            Programmers = new ObservableCollection<string>()
            {
                "Katherine Johnson",
                "Mary Jackson",
                "Dorothy Vaughn",
                "Christine Darden",
                "Patricia Cowings"
            };

            Testers = new ObservableCollection<string>()
            {
                "Marie Curie",
                "Ada Lovelace",
                "Alice Ball"
            };

            ProgramsResources = new ObservableCollection<string>()
            {
                "Localization.resources.dll"
            };

            IgnoredExtentions = new ObservableCollection<string>()
            {
                ".txt",
                ".doc",
                ".docx",
                ".pdf",
                ".patch",
                ".zip"
            };

            ProgramsWithResourceFiles = new ObservableCollection<string>()
            {
                "Localization.dll"
            };

            AllDependenciesPrograms = new ObservableCollection<string>()
            {
                "Service.exe"
            };

            NoneDependenciesPrograms = new ObservableCollection<string>()
            {
                "CommonFiles.exe"
            };

            ProgramsIgnoreSignatureCopyright = new ObservableCollection<string>()
            {
                "CommonFiles.exe"
            };
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
        /// Set the modified flag to true
        /// </summary>
        public void SetModified()
        {
            _modified = true;
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
