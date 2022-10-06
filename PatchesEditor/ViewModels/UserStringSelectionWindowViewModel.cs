using PatchesEditor.Data;
using PatchesEditor.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PatchesEditor.ViewModels
{
    /// <summary>
    /// View Model for UserStringSelectionWindow
    /// </summary>
    public class UserStringSelectionWindowViewModel : INotifyPropertyChanged
    {
        #region Private Variables

        private string windowTitle;
        private string selectedString;
        private string groupBoxTitle;

        #endregion

        #region Properties

        /// <summary>
        /// Window title
        /// </summary>
        public string WindowTitle
        {
            get => windowTitle;
            set
            {
                if (value != windowTitle)
                {
                    windowTitle = value;
                    OnPropertyChanged("WindowTitle");
                }
            }
        }

        /// <summary>
        /// Group box title
        /// </summary>
        public string GroupBoxTitle
        {
            get => groupBoxTitle;
            set
            {
                if (value != groupBoxTitle)
                {
                    groupBoxTitle = value;
                    OnPropertyChanged("GroupBoxTitle");
                }
            }
        }

        /// <summary>
        /// All strings to choose from
        /// </summary>
        public ObservableCollection<string> AllStrings { get; set; }

        /// <summary>
        /// Selected string
        /// </summary>
        public string SelectedString
        {
            get => selectedString;
            set
            {
                if (value != selectedString)
                {
                    selectedString = value;
                    OnPropertyChanged("SelectedString");
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Contructor for the User String Selection Window View Model
        /// </summary>
        /// <param name="programName">Name of the program for the window title.</param>
        /// <param name="section">What kind of value is the user selecting. For the window title.</param>
        /// <param name="strings">List of strings to choose from</param>
        public UserStringSelectionWindowViewModel(string programName, Section section, List<string> strings)
        {
            LogManager.WriteToLog(LogLevel.Basic, $"Opening User String Selection window with {strings.Count} options.");
            SelectedString = string.Empty;
            if (strings.Count == 0) return;

            if (section == Section.Impact)
                WindowTitle = $"Select which impact to use for {programName} for the current patch notes. Or click the + to add a custom value.";
            else if (section == Section.Dependencies)
                WindowTitle = $"Select which dependencies to use for {programName} for the current patch notes. Or click the + to add a custom value. " +
                    $"This will be used to generate the starting dependencies for the program.";

            GroupBoxTitle = $"Select {section.ToString().ToLower()} for {programName}:";

            AllStrings = new ObservableCollection<string>();
            strings.ForEach(x => AllStrings.Add(x));
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
