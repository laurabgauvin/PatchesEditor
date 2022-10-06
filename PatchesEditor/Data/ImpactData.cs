using Newtonsoft.Json;
using System.ComponentModel;

namespace PatchesEditor.Data
{
    /// <summary>
    /// This class contains the data for the patch Impact
    /// </summary>
    public class ImpactData : INotifyPropertyChanged
    {
        #region Private Variables

        private bool _hasService;
        private bool _serviceDoesCheckpoint;
        private string _serviceImpact;
        private string _allImpact;
        private bool _modified;

        #endregion

        #region Properties

        /// <summary>
        /// Does this patch contain the service exe
        /// </summary>
        public bool HasService
        {
            get => _hasService;
            set
            {
                if (value != _hasService)
                {
                    _modified = true;
                    _hasService = value;
                    OnPropertyChanged("HasService");
                }
            }
        }

        /// <summary>
        /// Does the service for this patch create a checkpoint
        /// </summary>
        public bool ServiceDoesCheckpoint
        {
            get => _serviceDoesCheckpoint;
            set
            {
                if (value != _serviceDoesCheckpoint)
                {
                    _modified = true;
                    _serviceDoesCheckpoint = value;
                    OnPropertyChanged("ServiceDoesCheckpoint");
                }
            }
        }

        /// <summary>
        /// Text for the impact of the service
        /// </summary>
        public string ServiceImpact
        {
            get => _serviceImpact;
            set
            {
                if (value != _serviceImpact)
                {
                    _modified = true;
                    _serviceImpact = value;
                    OnPropertyChanged("ServiceImpact");
                }
            }
        }

        /// <summary>
        /// Text for the impact of all programs
        /// </summary>
        public string AllImpact
        {
            get => _allImpact;
            set
            {
                if (value != _allImpact)
                {
                    _modified = true;
                    _allImpact = value;
                    OnPropertyChanged("AllImpact");
                }
            }
        }

        /// <summary>
        /// Gets the Modified flag
        /// </summary>
        [JsonIgnore]
        public bool Modified { get => _modified; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ImpactData(string defaultImpact)
        {
            HasService = false;
            ServiceDoesCheckpoint = false;
            ServiceImpact = string.Empty;
            AllImpact = defaultImpact;
        }

        /// <summary>
        /// Constructor with fields
        /// </summary>
        /// <param name="hasService">Has service</param>
        /// <param name="serviceDoesCp">Service does Cp</param>
        /// <param name="serviceImpact">Service impact</param>
        /// <param name="allImpact">All impact</param>
        public ImpactData(bool hasService, bool serviceDoesCp, string serviceImpact, string allImpact)
        {
            HasService = hasService;
            ServiceDoesCheckpoint = serviceDoesCp;
            ServiceImpact = serviceImpact;
            AllImpact = allImpact;
            Reset();
        }

        /// <summary>
        /// Create a new instance of ImpactData from an original object
        /// </summary>
        /// <param name="origImpact">Original object to copy</param>
        public ImpactData(ImpactData origImpact)
        {
            HasService = origImpact.HasService;
            ServiceDoesCheckpoint = origImpact.ServiceDoesCheckpoint;
            ServiceImpact = origImpact.ServiceImpact;
            AllImpact = origImpact.AllImpact;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reset the Modified flag
        /// </summary>
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
