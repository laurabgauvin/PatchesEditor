using Newtonsoft.Json;
using PatchesEditor.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PatchesEditor.Data
{
    /// <summary>
    /// This class contains the data for the dependencies for a single program
    /// </summary>
    public class DependenciesData : INotifyPropertyChanged
    {
        #region Private Variables

        private string _programName;
        private DependenciesType _type;
        private string _text;
        private bool _modified;

        #endregion

        #region Properties

        /// <summary>
        /// Program name
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
        /// Type of dependencies for the program
        /// </summary>
        public DependenciesType Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    _modified = true;
                    OnPropertyChanged("Type");
                    GenerateText();
                }
            }
        }

        /// <summary>
        /// Dependencies text for the program
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    _modified = true;
                    OnPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Program dependencies
        /// </summary>
        public ObservableCollection<DependencyProgram> ProgramDependencies { get; set; }

        /// <summary>
        /// Modified flag
        /// </summary>
        [JsonIgnore]
        public bool Modified
        {
            get
            {
                return _modified || ProgramDependencies.Any(x => x.Modified);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DependenciesData()
        {
            ProgramName = string.Empty;
            Type = DependenciesType.None;
            Text = string.Empty;
            ProgramDependencies = new ObservableCollection<DependencyProgram>();
            AttachEventHandler();
        }

        /// <summary>
        /// Constructor with fields
        /// </summary>
        /// <param name="programName">Program name</param>
        /// <param name="type">Type</param>
        /// <param name="text">Dependencies text</param>
        /// <param name="programDependencies">List of program dependencies</param>
        public DependenciesData(string programName, DependenciesType type, string text,
            ObservableCollection<DependencyProgram> programDependencies)
        {
            ProgramName = programName;
            Type = type;
            Text = text;

            ProgramDependencies = new ObservableCollection<DependencyProgram>();
            foreach (DependencyProgram item in programDependencies)
            {
                ProgramDependencies.Add(new DependencyProgram(item));
            }
            Reset();
            AttachEventHandler();
        }

        /// <summary>
        /// Create a new instance of the object by copying an existing object
        /// </summary>
        /// <param name="origDependenciesData">Original object to copy</param>
        public DependenciesData(DependenciesData origDependenciesData)
        {
            ProgramName = origDependenciesData.ProgramName;
            Type = origDependenciesData.Type;
            Text = origDependenciesData.Text;

            ProgramDependencies = new ObservableCollection<DependencyProgram>();
            foreach (DependencyProgram item in origDependenciesData.ProgramDependencies)
            {
                ProgramDependencies.Add(new DependencyProgram(item));
            }
            Reset();
            AttachEventHandler();
        }

        #endregion

        #region DependencyProgram Event Handler

        private PropertyChangedEventHandler _propertyChangedEventHandler;

        /// <summary>
        /// Attach the event handler for the ProgramDependencies collection
        /// </summary>
        private void AttachEventHandler()
        {
            _propertyChangedEventHandler = ProgramDependencies_PropertyChanged;
            ProgramDependencies.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
            {
                // Subscribe event
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        // Subscribe
                        foreach (INotifyPropertyChanged propertyChanged in e.NewItems)
                        {
                            propertyChanged.PropertyChanged += _propertyChangedEventHandler;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        // Unsubscribe
                        foreach (INotifyPropertyChanged propertyChanged in e.OldItems)
                        {
                            propertyChanged.PropertyChanged -= _propertyChangedEventHandler;
                        }
                        break;
                }
            };
        }

        /// <summary>
        /// Event firing when fields on ProgramDependencies are changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgramDependencies_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Compare(e.PropertyName, "Status", StringComparison.OrdinalIgnoreCase) == 0 &&
                ProgramDependencies.Any(x => x.Status != DependencyStatus.NotRequired))
            {
                Type = DependenciesType.Some;
            }
            else if (string.Compare(e.PropertyName, "Status", StringComparison.OrdinalIgnoreCase) == 0 &&
                ProgramDependencies.All(x => x.Status == DependencyStatus.NotRequired) &&
                Type == DependenciesType.Some)
            {
                Type = DependenciesType.None;
            }

            GenerateText();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the modified flag
        /// </summary>
        public void Reset()
        {
            _modified = false;

            foreach (DependencyProgram item in ProgramDependencies)
            {
                item.Reset();
            }
        }

        /// <summary>
        /// Generates the dependencies text property
        /// </summary>
        public void GenerateText()
        {
            if (Type == DependenciesType.All) Text = Globals.AppParameters.AllDependencies;
            else if (Type == DependenciesType.None) Text = Globals.AppParameters.NoneDependencies;
            else
            {
                Text = string.Empty;
                List<DependencyProgram> all = ProgramDependencies.Distinct().OrderBy(x => x.ProgramName).ToList();
                List<DependencyProgram> currentProg = all.Where(x => x.Status == DependencyStatus.Required).ToList();
                List<DependencyProgram> previousProg = all.Where(x => x.Status == DependencyStatus.Previous).ToList();

                if (currentProg.Count == 0 && previousProg.Count == 0) Text = Globals.AppParameters.NoneDependencies;
                else
                {
                    if (currentProg.Count > 0)
                    {
                        StringBuilder currentDep = new StringBuilder();
                        currentDep.Append("Requires ");

                        List<string> tmpProgList = new List<string>();
                        foreach (DependencyProgram prog in currentProg)
                        {
                            if (string.IsNullOrEmpty(prog.Version)) tmpProgList.Add($"{prog.ProgramName}");
                            else tmpProgList.Add($"{prog.ProgramName} version {prog.Version} or later");
                        }

                        currentDep.Append(Globals.BuildFormattedList(tmpProgList));
                        currentDep.Append(".");
                        Text = currentDep.ToString();
                    }

                    if (previousProg.Count > 0)
                    {
                        StringBuilder previousDep = new StringBuilder();
                        previousDep.Append("Requires ");

                        List<string> tmpProgList = new List<string>();
                        foreach (DependencyProgram prog in previousProg)
                        {
                            if (string.IsNullOrEmpty(prog.Version)) tmpProgList.Add($"{prog.ProgramName}");
                            else tmpProgList.Add($"{prog.ProgramName} version {prog.Version} or later");
                        }

                        previousDep.Append(Globals.BuildFormattedList(tmpProgList));
                        previousDep.Append(" as the result of a previous patch.");
                        if (string.IsNullOrEmpty(Text))
                            Text = previousDep.ToString();
                        else
                            Text += Environment.NewLine + Environment.NewLine + previousDep.ToString();
                    }
                }
            }
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
