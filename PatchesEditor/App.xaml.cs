using PatchesEditor.Data;
using PatchesEditor.Helpers;
using PatchesEditor.ViewModels;
using System.Text;
using System.Windows;

namespace PatchesEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// OnStartup application event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load parameters
            using (ParametersManager paramsManager = new ParametersManager())
            {
                Globals.AppParameters = paramsManager.ReadParameters();
            }

            LogManager.WriteToLog(LogLevel.Basic, "**********");
            LogManager.WriteToLog(LogLevel.Everything, "Application started.");
            LogManager.WriteToLog(LogLevel.Basic, $"Log Level: {Globals.AppParameters.LogLevel}");

            // Check startup arguments for filename
            StringBuilder str = new StringBuilder();
            foreach (string arg in e.Args)
            {
                str.Append(arg);
                str.Append(" ");
            }
            string fileName = str.ToString().Trim();

            // Start main window
            MainWindow app = new MainWindow();
            if (string.IsNullOrEmpty(fileName) == false)
            {
                LogManager.WriteToLog(LogLevel.Basic, $"Starting arguments: {fileName}.");
                app.DataContext = new ApplicationViewModel(fileName);
            }
            else
                app.DataContext = new ApplicationViewModel();

            app.Show();
        }

        /// <summary>
        /// OnExit application event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            LogManager.WriteToLog(LogLevel.Everything, "Application closed.");
            base.OnExit(e);
        }
    }
}
