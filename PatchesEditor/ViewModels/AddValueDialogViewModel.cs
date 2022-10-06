using System.ComponentModel;

namespace PatchesEditor.ViewModels
{
    /// <summary>
    /// View model for the AddValueDialog
    /// </summary>
    public class AddValueDialogViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Type of value that is added
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Dialog title
        /// </summary>
        public string DialogTitle { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddValueDialogViewModel()
        {
            Initialize();
        }

        /// <summary>
        /// Constructor for AddValueDialogViewModel
        /// </summary>
        /// <param name="valueType">Value type</param>
        public AddValueDialogViewModel(string valueType)
        {
            Initialize();

            if (string.IsNullOrEmpty(valueType) == false)
            {
                ValueType = valueType;
                DialogTitle = $"Add New {valueType}";
            }
        }

        /// <summary>
        /// Initialize properties
        /// </summary>
        private void Initialize()
        {
            ValueType = "Value";
            DialogTitle = "Add new value";
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
