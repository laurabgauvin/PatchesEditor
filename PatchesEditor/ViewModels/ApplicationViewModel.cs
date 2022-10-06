using PatchesEditor.Data;
using PatchesEditor.Helpers;
using PatchesEditor.Views;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace PatchesEditor.ViewModels
{
    /// <summary>
    /// Class in charge of the current active view model
    /// </summary>
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        #region Private Variables

        private Page currentPage;
        private StartingPage startingPage;
        private MainPage mainPage;
        private ParametersPage parametersPage;

        #endregion

        #region Public Properties

        /// <summary>
        /// Current page displayed in the window
        /// </summary>
        public Page CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    LogManager.WriteToLog(LogLevel.Everything, $"Switch to page {currentPage.GetType().Name}.");
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for ApplicationViewModel
        /// </summary>
        public ApplicationViewModel()
        {
            Startup();
            mainPage = new MainPage(StartupType.Nothing);
            CurrentPage = startingPage;
        }

        /// <summary>
        /// Constructor with filename
        /// </summary>
        /// <param name="fileName">File name to load on startup</param>
        public ApplicationViewModel(string fileName)
        {
            Startup();
            mainPage = new MainPage(fileName);
            CurrentPage = mainPage;
        }

        /// <summary>
        /// Starting tasks
        /// </summary>
        private void Startup()
        {
            startingPage = new StartingPage();
            parametersPage = new ParametersPage();

            Mediator.Subscribe("GoToStartingPage", GoToStartingPage);
            Mediator.Subscribe("GoToMainPage", GoToMainPage);
            Mediator.Subscribe("GoToParametersPage", GoToParametersPage);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Changes the current page
        /// </summary>
        /// <param name="page">New page</param>
        private void ChangePage(Page page)
        {
            CurrentPage = page;
        }

        /// <summary>
        /// Go to the StartingPage
        /// </summary>
        /// <param name="o"></param>
        private void GoToStartingPage(object o)
        {
            ChangePage(startingPage);
        }

        /// <summary>
        /// Go to the ParametersPage
        /// </summary>
        /// <param name="o"></param>
        private void GoToParametersPage(object o)
        {
            ChangePage(parametersPage);
        }

        /// <summary>
        /// Go to the MainPage
        /// </summary>
        /// <param name="o"></param>
        private void GoToMainPage(object o)
        {
            if (o != null)
            {
                if (Enum.TryParse(o.ToString(), out StartupType start))
                    mainPage = new MainPage(start);
                else
                    mainPage = new MainPage(StartupType.Nothing);
            }

            ChangePage(mainPage);
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
