using MaterialDesignThemes.Wpf;
using PatchesEditor.Data;
using PatchesEditor.Helpers;
using PatchesEditor.Views;
using System.ComponentModel;
using System.Windows.Input;

namespace PatchesEditor.ViewModels
{
    /// <summary>
    /// View Model for ParametersPage
    /// </summary>
    public class ParametersPageViewModel : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Current parameters displayed
        /// </summary>
        public AppParameters CurrentParameters { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ParametersPageViewModel()
        {
            CurrentParameters = Globals.AppParameters;
        }

        #endregion

        #region Add Values to Lists

        public ICommand AddValue => new CommandImplementation(AddValueCommand);

        /// <summary>
        /// Open Add Value Dialog to input a new value in the list
        /// </summary>
        private async void AddValueCommand(object o)
        {
            AddValueDialog view = new AddValueDialog()
            {
                DataContext = new AddValueDialogViewModel(o.ToString())
            };

            var dialogResult = await DialogHost.Show(view, "RootDialog");
            if (dialogResult != null && dialogResult is string result && string.IsNullOrEmpty(result) == false)
            {
                AddParameter(o.ToString(), result);
            }
        }

        /// <summary>
        /// Adds value to the selected parameter
        /// </summary>
        /// <param name="param">Parameter name/code</param>
        /// <param name="value">Value to add</param>
        private void AddParameter(string param, string value)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Adding value '{value}' to param '{param}'.");

            switch (param)
            {
                case "Programmer":
                    if (CurrentParameters.Programmers.Contains(value) == false)
                    {
                        CurrentParameters.Programmers.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "Tester":
                    if (CurrentParameters.Testers.Contains(value) == false)
                    {
                        CurrentParameters.Testers.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "Extension":
                    if (CurrentParameters.IgnoredExtentions.Contains(value) == false)
                    {
                        CurrentParameters.IgnoredExtentions.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "Ignore Copyright":
                    if (CurrentParameters.ProgramsIgnoreSignatureCopyright.Contains(value) == false)
                    {
                        CurrentParameters.ProgramsIgnoreSignatureCopyright.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "Resources Directory":
                    if (CurrentParameters.ProgramsResources.Contains(value) == false)
                    {
                        CurrentParameters.ProgramsResources.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "Prog w/ Resources":
                    if (CurrentParameters.ProgramsWithResourceFiles.Contains(value) == false)
                    {
                        CurrentParameters.ProgramsWithResourceFiles.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "All Dependencies":
                    if (CurrentParameters.AllDependenciesPrograms.Contains(value) == false)
                    {
                        CurrentParameters.AllDependenciesPrograms.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                case "None Dependencies":
                    if (CurrentParameters.NoneDependenciesPrograms.Contains(value) == false)
                    {
                        CurrentParameters.NoneDependenciesPrograms.Add(value);
                        CurrentParameters.SetModified();
                    }
                    break;
                
                default:
                    break;
            }
        }

        #endregion

        #region Delete Values from Lists

        public ICommand DeleteProgrammer => new CommandImplementation(DeleteProgrammerCommand);
        public ICommand DeleteTester => new CommandImplementation(DeleteTesterCommand);
        public ICommand DeleteResourcesDir => new CommandImplementation(DeleteResourcesDirCommand);
        public ICommand DeleteIgnoredExtensions => new CommandImplementation(DeleteIgnoredExtensionsCommand);
        public ICommand DeleteProgramWithResources => new CommandImplementation(DeleteProgramWithResourcesCommand);
        public ICommand DeleteAllDependencies => new CommandImplementation(DeleteAllDependenciesCommand);
        public ICommand DeleteNoneDependencies => new CommandImplementation(DeleteNoneDependenciesCommand);
        public ICommand DeleteIgnoreCopyright => new CommandImplementation(DeleteIgnoreCopyrightCommand);

        /// <summary>
        /// Delete selected programmer from the list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteProgrammerCommand(object o)
        {
            DeleteParameter(ParameterType.Programmer, o.ToString());
        }

        /// <summary>
        /// Delete selected tester from the list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteTesterCommand(object o)
        {
            DeleteParameter(ParameterType.Tester, o.ToString());
        }

        /// <summary>
        /// Delete selected program from the resources directory list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteResourcesDirCommand(object o)
        {
            DeleteParameter(ParameterType.ResourceDir, o.ToString());
        }

        /// <summary>
        /// Delete selected extension
        /// </summary>
        /// <param name="o"></param>
        private void DeleteIgnoredExtensionsCommand(object o)
        {
            DeleteParameter(ParameterType.IgnoredExtensions, o.ToString());
        }

        /// <summary>
        /// Delete program from the programs with resources list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteProgramWithResourcesCommand(object o)
        {
            DeleteParameter(ParameterType.ProgramsWithResources, o.ToString());
        }

        /// <summary>
        /// Delete program from the none dependencies list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteNoneDependenciesCommand(object o)
        {
            DeleteParameter(ParameterType.NoneDependencies, o.ToString());
        }

        /// <summary>
        /// Delete program from the all dependencies list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteAllDependenciesCommand(object o)
        {
            DeleteParameter(ParameterType.AllDependencies, o.ToString());
        }

        /// <summary>
        /// Delete program from the ignore copyright list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteIgnoreCopyrightCommand(object o)
        {
            DeleteParameter(ParameterType.IgnoreCopyright, o.ToString());
        }

        /// <summary>
        /// Delete the passed value from the selected parameter
        /// </summary>
        /// <param name="param">Parameter to delete from</param>
        /// <param name="value">Value to remove</param>
        private void DeleteParameter(ParameterType param, string value)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Deleting value '{value}' from param '{param}'.");

            switch (param)
            {
                case ParameterType.Programmer:
                    CurrentParameters.Programmers.Remove(value);
                    break;
                case ParameterType.Tester:
                    CurrentParameters.Testers.Remove(value);
                    break;
                case ParameterType.IgnoredExtensions:
                    CurrentParameters.IgnoredExtentions.Remove(value);
                    break;
                case ParameterType.IgnoreCopyright:
                    CurrentParameters.ProgramsIgnoreSignatureCopyright.Remove(value);
                    break;
                case ParameterType.ResourceDir:
                    CurrentParameters.ProgramsResources.Remove(value);
                    break;
                case ParameterType.ProgramsWithResources:
                    CurrentParameters.ProgramsWithResourceFiles.Remove(value);
                    break;
                case ParameterType.AllDependencies:
                    CurrentParameters.AllDependenciesPrograms.Remove(value);
                    break;
                case ParameterType.NoneDependencies:
                    CurrentParameters.NoneDependenciesPrograms.Remove(value);
                    break;
                default:
                    break;
            }

            CurrentParameters.SetModified();
        }

        #endregion

        #region Save/Cancel

        /// <summary>
        /// Save parameters and return to main page
        /// </summary>
        public ICommand Save => new CommandImplementation(x =>
        {
            if (CurrentParameters.Modified)
            {
                LogManager.WriteToLog(LogLevel.Basic, "Saving parameters.");
                using (ParametersManager paramsManager = new ParametersManager())
                {
                    Globals.AppParameters = CurrentParameters;
                    paramsManager.SaveAllParameters(CurrentParameters);
                }
            }
            else LogManager.WriteToLog(LogLevel.Basic, "Saving - No changes made to parameters.");

            Mediator.Notify("GoToMainPage");
        });

        /// <summary>
        /// Cancel changes and return to main page
        /// </summary>
        public ICommand Cancel => new CommandImplementation(x =>
        {
            if (CurrentParameters.Modified)
                LogManager.WriteToLog(LogLevel.Basic, "Cancelling parameter changes.");

            Mediator.Notify("GoToMainPage");
        });

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
