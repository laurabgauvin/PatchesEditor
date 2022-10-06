using PatchesEditor.Data;
using PatchesEditor.Helpers;
using System.Windows.Input;

namespace PatchesEditor.ViewModels
{
    /// <summary>
    /// View Model for StartingPage
    /// </summary>
    public class StartingPageViewModel
    {
        /// <summary>
        /// Go to the main page and start a new patch
        /// </summary>
        public ICommand New => new CommandImplementation(x => { Mediator.Notify("GoToMainPage", StartupType.New); });

        /// <summary>
        /// Go to the main page and open an existing patch
        /// </summary>
        public ICommand Open => new CommandImplementation(x => { Mediator.Notify("GoToMainPage", StartupType.Open); });

        /// <summary>
        /// Go to the main page and import from an existing patch
        /// </summary>
        public ICommand Import => new CommandImplementation(x => { Mediator.Notify("GoToMainPage", StartupType.Import); });

        /// <summary>
        /// Go to the main page and start a merge
        /// </summary>
        public ICommand Merge => new CommandImplementation(x => { Mediator.Notify("GoToMainPage", StartupType.Merge); });

        /// <summary>
        /// Go to the main page
        /// </summary>
        public ICommand Nothing => new CommandImplementation(x => { Mediator.Notify("GoToMainPage", StartupType.Nothing); });

        /// <summary>
        /// Go to the parameters page
        /// </summary>
        public ICommand Parameters => new CommandImplementation(x => { Mediator.Notify("GoToParametersPage"); });
    }
}
