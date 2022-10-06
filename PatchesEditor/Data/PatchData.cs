using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace PatchesEditor.Data
{
    /// <summary>
    /// Class that contains the data for a patch notes
    /// </summary>
    public class PatchData : INotifyPropertyChanged
    {
        #region Private Variables

        private DateTime _patchDate;
        private string _ticket;
        private ReferenceType _referenceType;
        private string _referenceNumber;
        private string _background;
        private ImpactData _impact;
        private string _allDependencies;
        private string _descriptionOfChanges;
        private string _instructions;
        private string _shortDescription;
        private string _filePath;
        private bool _needsCommonFiles;
        private bool _needsSqlScripts;
        private bool _modified;

        #endregion

        #region Public Properties

        /// <summary>
        /// Patch Date
        /// </summary>
        public DateTime PatchDate
        {
            get => _patchDate;
            set
            {
                if (value != _patchDate)
                {
                    _patchDate = value;
                    _modified = true;
                    OnPropertyChanged("PatchDate");
                }
            }
        }

        /// <summary>
        /// Programmers
        /// </summary>
        public ObservableCollection<string> Programmers { get; set; }

        /// <summary>
        /// Testers
        /// </summary>
        public ObservableCollection<string> Testers { get; set; }

        /// <summary>
        /// Ticket Number
        /// </summary>
        public string Ticket
        {
            get => _ticket;
            set
            {
                if (value != _ticket)
                {
                    _modified = true;
                    _ticket = value;
                    OnPropertyChanged("Ticket");
                }
            }
        }

        /// <summary>
        /// Reference Type
        /// </summary>
        public ReferenceType ReferenceType
        {
            get => _referenceType;
            set
            {
                if (value != _referenceType)
                {
                    _modified = true;
                    _referenceType = value;
                    OnPropertyChanged("ReferenceType");
                }
            }
        }

        /// <summary>
        /// Reference Number
        /// </summary>
        public string ReferenceNumber
        {
            get => _referenceNumber;
            set
            {
                if (value != _referenceNumber)
                {
                    _modified = true;
                    _referenceNumber = value;
                    OnPropertyChanged("ReferenceNumber");
                }
            }
        }

        /// <summary>
        /// Background
        /// </summary>
        public string Background
        {
            get => _background;
            set
            {
                if (value != _background)
                {
                    _modified = true;
                    _background = value;
                    OnPropertyChanged("Background");
                }
            }
        }

        /// <summary>
        /// Impact
        /// </summary>
        public ImpactData Impact
        {
            get => _impact;
            set
            {
                if (value != _impact)
                {
                    _modified = true;
                    _impact = value;
                    OnPropertyChanged("Impact");
                }
            }
        }

        /// <summary>
        /// All dependencies string
        /// </summary>
        public string AllDependencies
        {
            get => _allDependencies;
            set
            {
                if (value != _allDependencies)
                {
                    _allDependencies = value;
                    _modified = true;
                    OnPropertyChanged("AllDependencies");
                }
            }
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        public ObservableCollection<DependenciesData> Dependencies { get; set; }

        /// <summary>
        /// Description of Changes
        /// </summary>
        public string DescriptionOfChanges
        {
            get => _descriptionOfChanges;
            set
            {
                if (value != _descriptionOfChanges)
                {
                    _modified = true;
                    _descriptionOfChanges = value;
                    OnPropertyChanged("DescriptionOfChanges");
                }
            }
        }

        /// <summary>
        /// Instructions
        /// </summary>
        public string Instructions
        {
            get => _instructions;
            set
            {
                if (value != _instructions)
                {
                    _modified = true;
                    _instructions = value;
                    OnPropertyChanged("Instructions");
                }
            }
        }

        /// <summary>
        /// Programs Used
        /// </summary>
        public ObservableCollection<ProgramData> ProgramsUsed { get; set; }

        /// <summary>
        /// Short description
        /// </summary>
        public string ShortDescription
        {
            get => _shortDescription;
            set
            {
                if (value != _shortDescription)
                {
                    _modified = true;
                    _shortDescription = value;
                    OnPropertyChanged("ShortDescription");
                }
            }
        }

        /// <summary>
        /// File path
        /// </summary>
        [JsonIgnore]
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (value != _filePath)
                {
                    _modified = true;
                    _filePath = value;
                    OnPropertyChanged("FilePath");
                }
            }
        }

        /// <summary>
        /// Needs Common Files
        /// </summary>
        public bool NeedsCommonFiles
        {
            get => _needsCommonFiles;
            set
            {
                if (value != _needsCommonFiles)
                {
                    _modified = true;
                    _needsCommonFiles = value;
                    OnPropertyChanged("NeedsCommonFiles");
                }
            }
        }

        /// <summary>
        /// Needs SQL Scripts
        /// </summary>
        public bool NeedsSqlScripts
        {
            get => _needsSqlScripts;
            set
            {
                if (value != _needsSqlScripts)
                {
                    _modified = true;
                    _needsSqlScripts = value;
                    OnPropertyChanged("NeedsSqlScripts");
                }
            }
        }

        /// <summary>
        /// Returns the Modified flag
        /// </summary>
        [JsonIgnore]
        public bool Modified
        {
            get
            {
                return _modified || Impact.Modified || Dependencies.Any(x => x.Modified) || ProgramsUsed.Any(x => x.Modified);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of PatchData with default values
        /// </summary>
        public PatchData()
        {
            Programmers = new ObservableCollection<string>();
            Testers = new ObservableCollection<string>();
            ProgramsUsed = new ObservableCollection<ProgramData>();
            Dependencies = new ObservableCollection<DependenciesData>();
            ClearAllFields();
        }

        /// <summary>
        /// Create a new instance of PatchData with the values from the PatchData passed
        /// </summary>
        /// <param name="patch">PatchData to copy</param>
        public PatchData(PatchData patch)
        {
            FilePath = patch.FilePath;
            PatchDate = patch.PatchDate;
            Programmers = new ObservableCollection<string>();
            foreach (string programmer in patch.Programmers)
            {
                Programmers.Add(programmer);
            }
            Testers = new ObservableCollection<string>();
            foreach (string tester in patch.Testers)
            {
                Testers.Add(tester);
            }
            Ticket = patch.Ticket;
            if (string.IsNullOrEmpty(patch.ReferenceNumber) == false)
            {
                ReferenceType = patch.ReferenceType;
                ReferenceNumber = patch.ReferenceNumber;
            }
            Background = patch.Background;
            Impact = new ImpactData(patch.Impact);
            AllDependencies = patch.AllDependencies;
            Dependencies = new ObservableCollection<DependenciesData>();
            foreach (DependenciesData dependencies in patch.Dependencies)
            {
                Dependencies.Add(dependencies);
            }
            DescriptionOfChanges = patch.DescriptionOfChanges;
            Instructions = patch.Instructions;
            ProgramsUsed = new ObservableCollection<ProgramData>();
            foreach (ProgramData program in patch.ProgramsUsed)
            {
                ProgramsUsed.Add(program);
            }
            NeedsCommonFiles = patch.NeedsCommonFiles;
            NeedsSqlScripts = patch.NeedsSqlScripts;
            ShortDescription = patch.ShortDescription;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the Modified property
        /// </summary>
        public void ResetModified()
        {
            _modified = false;
            Impact.Reset();

            foreach (DependenciesData item in Dependencies)
            {
                item.Reset();
            }

            foreach (ProgramData item in ProgramsUsed)
            {
                item.Reset();
            }
        }

        /// <summary>
        /// Sets the Modified property to true
        /// </summary>
        public void SetModified()
        {
            _modified = true;
        }

        /// <summary>
        /// Clears all fields
        /// </summary>
        public void ClearAllFields()
        {
            PatchDate = DateTime.Today;
            Programmers.Clear();
            Testers.Clear();
            Ticket = string.Empty;
            ReferenceType = ReferenceType.Reference;
            ReferenceNumber = string.Empty;
            Background = string.Empty;
            Impact = new ImpactData(string.Empty);
            AllDependencies = string.Empty;
            Dependencies.Clear();
            DescriptionOfChanges = string.Empty;
            Instructions = string.Empty;
            ProgramsUsed.Clear();
            ShortDescription = string.Empty;
            FilePath = string.Empty;
            NeedsCommonFiles = false;
            NeedsSqlScripts = false;
            ResetModified();
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
