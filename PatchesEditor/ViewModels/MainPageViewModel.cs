using PatchesEditor.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using PatchesEditor.Data;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace PatchesEditor.ViewModels
{
    /// <summary>
    /// View Model for MainPage
    /// </summary>
    public class MainPageViewModel : INotifyPropertyChanged
    {
        #region Private variables

        private const string APP_NAME = "Patches Editor";

        // Variables used in code
        private PatchesManager patchesManager;
        private bool windowTitleHasStar;
        private DispatcherTimer timer;
        private string? exportDirectory;
        private string mergeSaveFileName;
        private Dictionary<string, string> origDependenciesDict; // <ProgramName, OrigDependencies>

        // Private variable for properties
        private string _windowTitle;
        private string _mergeLabel;
        private int _currentPatchIndex;
        private bool _spellcheckEnabled;
        private bool _exportWithLineBreaks;
        private bool _mergeMode;
        private bool _nextButtonEnabled;
        private bool _previousButtonEnabled;
        private bool _deleteButtonEnabled;
        private bool _addButtonEnabled;
        private bool _impactTextFieldEnabled;
        private bool _dependenciesTextFieldEnabled;
        private bool _instructionsTextFieldEnabled;
        private bool _programsUsedTextFieldEnabled;

        #endregion

        #region Properties

        public AppParameters AppParameters => Globals.AppParameters;

        /// <summary>
        /// Merge Mode
        /// </summary>
        public bool MergeMode
        {
            get => _mergeMode;
            set
            {
                if (value != _mergeMode)
                {
                    _mergeMode = value;
                    OnPropertyChanged("MergeMode");
                    SetWindowTitle();
                    LogManager.WriteToLog(LogLevel.Everything, $"Merge mode set to: {MergeMode}.");
                }
            }
        }

        /// <summary>
        /// All Patch Data
        /// </summary>
        public ObservableCollection<PatchData> AllPatchData { get; set; }

        /// <summary>
        /// Patch Data displayed on the screen
        /// </summary>
        public PatchData CurrentPatchData { get => AllPatchData[CurrentPatchIndex]; }

        /// <summary>
        /// Current Patch Data index in the AllPatchData list
        /// </summary>
        public int CurrentPatchIndex
        {
            get => _currentPatchIndex;
            set
            {
                if (value != _currentPatchIndex)
                {
                    _currentPatchIndex = value;
                    OnPropertyChanged("CurrentPatchIndex");
                    OnPropertyChanged("CurrentPatchData");
                    LogManager.WriteToLog(LogLevel.Everything, $"CurrentPatchIndex set to: {CurrentPatchIndex}.");
                }
            }
        }

        /// <summary>
        /// Export with line breaks
        /// </summary>
        public bool ExportWithLineBreaks
        {
            get => _exportWithLineBreaks;
            set
            {
                if (value != _exportWithLineBreaks)
                {
                    _exportWithLineBreaks = value;
                    OnPropertyChanged("ExportWithLineBreaks");
                }
            }
        }

        /// <summary>
        /// Spellcheck enabled in the text fields
        /// </summary>
        public bool SpellcheckEnabled
        {
            get => _spellcheckEnabled;
            set
            {
                if (value != _spellcheckEnabled)
                {
                    _spellcheckEnabled = value;
                    OnPropertyChanged("SpellcheckEnabled");
                }
            }
        }

        /// <summary>
        /// Window title
        /// </summary>
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (value != _windowTitle)
                {
                    _windowTitle = value;
                    OnPropertyChanged("WindowTitle");
                }
            }
        }

        /// <summary>
        /// Merge label
        /// </summary>
        public string MergeLabel
        {
            get => _mergeLabel;
            set
            {
                if (value != _mergeLabel)
                {
                    _mergeLabel = value;
                    OnPropertyChanged("MergeLabel");
                }
            }
        }

        /// <summary>
        /// Next button enabled
        /// </summary>
        public bool NextButtonEnabled
        {
            get => _nextButtonEnabled;
            set
            {
                if (value != _nextButtonEnabled)
                {
                    _nextButtonEnabled = value;
                    OnPropertyChanged("NextButtonEnabled");
                }
            }
        }

        /// <summary>
        /// Previous button enabled
        /// </summary>
        public bool PreviousButtonEnabled
        {
            get => _previousButtonEnabled;
            set
            {
                if (value != _previousButtonEnabled)
                {
                    _previousButtonEnabled = value;
                    OnPropertyChanged("PreviousButtonEnabled");
                }
            }
        }

        /// <summary>
        /// Delete button enabled
        /// </summary>
        public bool DeleteButtonEnabled
        {
            get => _deleteButtonEnabled;
            set
            {
                if (value != _deleteButtonEnabled)
                {
                    _deleteButtonEnabled = value;
                    OnPropertyChanged("DeleteButtonEnabled");
                }
            }
        }

        /// <summary>
        /// Add button enabled
        /// </summary>
        public bool AddButtonEnabled
        {
            get => _addButtonEnabled;
            set
            {
                if (value != _addButtonEnabled)
                {
                    _addButtonEnabled = value;
                    OnPropertyChanged("AddButtonEnabled");
                }
            }
        }

        /// <summary>
        /// Impact text field enabled
        /// </summary>
        public bool ImpactTextFieldEnabled
        {
            get => _impactTextFieldEnabled;
            set
            {
                if (value != _impactTextFieldEnabled)
                {
                    _impactTextFieldEnabled = value;
                    OnPropertyChanged("ImpactTextFieldEnabled");
                }
            }
        }

        /// <summary>
        /// Dependencies text field enabled
        /// </summary>
        public bool DependenciesTextFieldEnabled
        {
            get => _dependenciesTextFieldEnabled;
            set
            {
                if (value != _dependenciesTextFieldEnabled)
                {
                    _dependenciesTextFieldEnabled = value;
                    OnPropertyChanged("DependenciesTextFieldEnabled");
                }
            }
        }

        /// <summary>
        /// Instructions text field enabled
        /// </summary>
        public bool InstructionsTextFieldEnabled
        {
            get => _instructionsTextFieldEnabled;
            set
            {
                if (value != _instructionsTextFieldEnabled)
                {
                    _instructionsTextFieldEnabled = value;
                    OnPropertyChanged("InstructionsTextFieldEnabled");
                }
            }
        }

        /// <summary>
        /// Programs Used text field enabled
        /// </summary>
        public bool ProgramsUsedTextFieldEnabled
        {
            get => _programsUsedTextFieldEnabled;
            set
            {
                if (value != _programsUsedTextFieldEnabled)
                {
                    _programsUsedTextFieldEnabled = value;
                    OnPropertyChanged("ProgramsUsedTextFieldEnabled");
                }
            }
        }

        #endregion

        #region Commands

        public ICommand Save => new CommandImplementation(SaveCommand);
        public ICommand Clipboard => new CommandImplementation(ClipboardCommand);
        public ICommand Import => new CommandImplementation(ImportCommand);
        public ICommand Exit => new CommandImplementation(ExitCommand);
        public ICommand SaveAs => new CommandImplementation(SaveAsCommand);
        public ICommand New => new CommandImplementation(NewCommand);
        public ICommand Open => new CommandImplementation(OpenCommand);
        public ICommand Merge => new CommandImplementation(ToggleMergeCommand);
        public ICommand ExportAll => new CommandImplementation(ExportAllCommand);
        public ICommand AddProgrammer => new CommandImplementation(AddProgrammerCommand);
        public ICommand DeleteProgrammer => new CommandImplementation(DeleteProgrammerCommand);
        public ICommand AddTester => new CommandImplementation(AddTesterCommand);
        public ICommand DeleteTester => new CommandImplementation(DeleteTesterCommand);
        public ICommand ImportDefaultImpact => new CommandImplementation(ImportDefaultImpactCommand);
        public ICommand RefreshInstructions => new CommandImplementation(RefreshInstructionsCommand);
        public ICommand RefreshProgramsUsed => new CommandImplementation(RefreshProgramsUsedCommand);
        public ICommand Bulletin => new CommandImplementation(BulletinCommand);
        public ICommand TaskUpdate => new CommandImplementation(TaskUpdateCommand);
        public ICommand AddPatch => new CommandImplementation(AddPatchCommand);
        public ICommand DeletePatch => new CommandImplementation(DeletePatchCommand);
        public ICommand NextPatch => new CommandImplementation(NextPatchCommand);
        public ICommand PreviousPatch => new CommandImplementation(PreviousPatchCommand);
        public ICommand ToggleImpactTextFieldEnabled => new CommandImplementation(ToggleImpactTextFieldEnabledCommand);
        public ICommand ToggleDependenciesTextFieldEnabled => new CommandImplementation(ToggleDependenciesTextFieldEnabledCommand);
        public ICommand ToggleInstructionsTextFieldEnabled => new CommandImplementation(ToggleInstructionsTextFieldEnabledCommand);
        public ICommand ToggleProgramsUsedTextFieldEnabled => new CommandImplementation(ToggleProgramsUsedTextFieldEnabledCommand);
        public ICommand Parameters => new CommandImplementation(x => { Mediator.Notify("GoToParametersPage"); });

        #endregion

        #region Constructor & Startup

        /// <summary>
        /// Main constructor
        /// </summary>
        public MainPageViewModel()
        {
            Startup();
            LogManager.WriteToLog(LogLevel.Everything, $"MainPage startup type: none.");
        }

        /// <summary>
        /// Constructor with startup file
        /// </summary>
        /// <param name="startupFile">Startup file</param>
        public MainPageViewModel(string startupFile)
        {
            Startup();
            LogManager.WriteToLog(LogLevel.Everything, $"MainPage startup type: start from file.");
            OpenCommand(startupFile);
        }

        /// <summary>
        /// Constructor with startup type
        /// </summary>
        /// <param name="startupType">Startup type</param>
        public MainPageViewModel(StartupType startupType)
        {
            Startup();
            LogManager.WriteToLog(LogLevel.Everything, $"MainPage startup type: {startupType}.");
            DoStartup(startupType);
        }

        /// <summary>
        /// Starting tasks
        /// </summary>
        private void Startup()
        {
            // Set up new patch data
            PatchData newPatch = new PatchData();
            AllPatchData = new ObservableCollection<PatchData>() { newPatch };
            CurrentPatchData.ResetModified();
            CurrentPatchIndex = 0;
            patchesManager = new PatchesManager();

            // Set up local variables
            windowTitleHasStar = false;
            exportDirectory = string.Empty;
            mergeSaveFileName = string.Empty;
            origDependenciesDict = new Dictionary<string, string>();

            // Set up properties
            ExportWithLineBreaks = true;
            SpellcheckEnabled = false;
            MergeMode = false;
            AddButtonEnabled = true;
            NextButtonEnabled = false;
            PreviousButtonEnabled = false;
            DeleteButtonEnabled = false;
            ImpactTextFieldEnabled = false;
            DependenciesTextFieldEnabled = false;
            InstructionsTextFieldEnabled = false;
            ProgramsUsedTextFieldEnabled = false;
            SetWindowTitle();
            MergeLabel = "No Patches Selected";

            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        /// <summary>
        /// Perform the startup operations
        /// </summary>
        /// <param name="start">Startup type</param>
        public void DoStartup(StartupType start)
        {
            switch (start)
            {
                case StartupType.New:
                    StartNewPatch();
                    break;
                case StartupType.Open:
                    OpenCommand(null);
                    break;
                case StartupType.Import:
                    ImportCommand(null);
                    break;
                case StartupType.Merge:
                    MergeMode = true;
                    ToggleMergeCommand(null);
                    break;
                case StartupType.Nothing:
                default:
                    break;
            }
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        public void ExitCommand(object o)
        {
            AskToSave();
            Application.Current.MainWindow.Close();
        }

        #endregion

        #region Open/Load Methods

        /// <summary>
        /// Import patch notes from existing patch notes file
        /// </summary>
        public void ImportCommand(object? o)
        {
            LogManager.WriteToLog(LogLevel.Everything, "Starting Import command.");

            AskToSave();
            string file = OpenFileWindow();
            if (string.IsNullOrEmpty(file)) return;

            LoadPatchInfo(patchesManager.ImportFile(file));
            CurrentPatchData.FilePath = string.Empty;
            CurrentPatchData.ResetModified();
            SetWindowTitle();
            OnPropertyChanged("CurrentPatchData");
        }

        /// <summary>
        /// Clears the current patches
        /// </summary>
        public void NewCommand(object? o)
        {
            LogManager.WriteToLog(LogLevel.Everything, "Starting new patch.");

            AskToSave();
            CurrentPatchData.ClearAllFields();
            SetWindowTitle();
            OnPropertyChanged("CurrentPatchData");
        }

        /// <summary>
        /// Open existing patch notes
        /// </summary>
        public void OpenCommand(object? o)
        {
            string? fileName;
            if (o != null && string.IsNullOrEmpty(o.ToString()) == false)
            {
                fileName = o.ToString();
            }
            else
            {
                AskToSave();
                fileName = OpenFileWindow();
            }

            if (string.IsNullOrEmpty(fileName)) return;

            LogManager.WriteToLog(LogLevel.Basic, $"Loading from file: {fileName}.");
            List<PatchData> patches;
            patches = patchesManager.ImportFile(fileName);

            LoadPatchInfo(patches);
            CurrentPatchData.ResetModified();
            SetWindowTitle();
            OnPropertyChanged("CurrentPatchData");
        }

        /// <summary>
        /// Start a new patch
        /// </summary>
        private void StartNewPatch()
        {
            SaveCommand(null);
            if (string.IsNullOrWhiteSpace(CurrentPatchData.FilePath)) return;
            RefreshProgramsUsedCommand(null);
            RefreshInstructionsCommand(null);
            SaveCommand(null);
        }

        /// <summary>
        /// Opens the open file dialog and returns the file path selected
        /// </summary>
        private string OpenFileWindow()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "All Files (*.*)|*.*|Text Files (.txt)|*.txt|Patch Files (.patch)|*.patch"
            };

            while (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                if (Globals.IsTxtFile(fileName) || Globals.IsPatchFile(fileName))
                    return fileName;

                string message = "OpenFileWindow: Please select a valid file to open (.txt or .patch)."; ;
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
                MessageBox.Show(message);
            }

            return string.Empty;
        }

        /// <summary>
        /// Update the CurrentPatchData displayed on the screen from the passed patch data
        /// </summary>
        private void LoadPatchInfo(PatchData newPatchData)
        {
            if (newPatchData == null) return;

            LogManager.WriteToLog(LogLevel.Basic, $"Loading new patch. Ticket: {newPatchData.Ticket}.");

            AllPatchData[CurrentPatchIndex] = newPatchData;
            CheckForService();
            EnableImpactDependenciesTextFields();
        }

        /// <summary>
        /// Updates the AllPatchData list + CurrentPatchData displayed on the screen from the passed data.
        /// Clears all existing patches in AllPatchData list and resets current patch index to 0.
        /// </summary>
        /// <param name="newPatchData">List of patches to load</param>
        private void LoadPatchInfo(List<PatchData> newPatchData)
        {
            if (newPatchData == null || newPatchData.Count == 0) return;

            LogManager.WriteToLog(LogLevel.Basic, $"Loading {newPatchData.Count} patches.");

            AllPatchData.Clear();
            foreach (PatchData patch in newPatchData)
            {
                AllPatchData.Add(patch);
            }

            CurrentPatchIndex = 0;
            CheckForService();
            EnableImpactDependenciesTextFields();

            if (newPatchData.Count > 1)
            {
                MergeMode = true;
                ToggleMergeCommand(MergeMode);
            }
        }

        #endregion

        #region Save/Export Methods

        /// <summary>
        /// Asks the user if they would like to save their changes
        /// </summary>
        public void AskToSave()
        {
            if (CurrentPatchData.Modified)
            {
                string message = "Would you like to save your changes?";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
                if (MessageBox.Show(message, APP_NAME, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveCommand(CurrentPatchData);
                }
            }
        }

        /// <summary>
        /// Copy bulletin text to clipboard
        /// </summary>
        public void BulletinCommand(object o)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Export bulletin.");

            Mouse.OverrideCursor = Cursors.Wait;
            string export = patchesManager.ExportBulletin(AllPatchData.ToList());
            System.Windows.Clipboard.SetText(export);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Copy task update text to clipboard
        /// </summary>
        public void TaskUpdateCommand(object o)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Export task update.");

            Mouse.OverrideCursor = Cursors.Wait;
            string export = patchesManager.ExportTaskUpdate(CurrentPatchData);
            System.Windows.Clipboard.SetText(export);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Copy the formatted patch notes to the clipboard
        /// </summary>
        public void ClipboardCommand(object o)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Export to clipboard.");

            Mouse.OverrideCursor = Cursors.Wait;
            string export = GenerateExport();
            System.Windows.Clipboard.SetText(export);
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Save patch notes
        /// </summary>
        public void SaveCommand(object? o)
        {
            if (MergeMode)
            {
                SaveCurrentMerge();
            }
            else
            {
                SaveFileWindow();
                if (string.IsNullOrWhiteSpace(CurrentPatchData.FilePath)) return;
                SaveToFile();
            }
        }

        /// <summary>
        /// Save As patch notes
        /// </summary>
        private void SaveAsCommand(object? o)
        {
            SaveFileWindow(true);
            if (string.IsNullOrWhiteSpace(CurrentPatchData.FilePath)) return;
            SaveToFile();
        }

        /// <summary>
        /// Opens the save file dialog and saves the path selected to the current patch data
        /// </summary>
        /// <param name="saveAs">Force save as</param>
        private void SaveFileWindow(bool saveAs = false)
        {
            LogManager.WriteToLog(LogLevel.Everything, "Saving file.");

            if (saveAs || string.IsNullOrWhiteSpace(CurrentPatchData.FilePath))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Filter = "All Files (*.*)|*.*|Text Files (.txt)|*.txt|Patch Files (.patch)|*.patch",
                    FileName = "readme.patch",
                    Title = "Save Patch Notes"
                };

                while (saveFileDialog.ShowDialog() == true)
                {
                    string fileName = saveFileDialog.FileName;

                    if (Globals.IsTxtFile(fileName) || Globals.IsPatchFile(fileName))
                    {
                        CurrentPatchData.FilePath = fileName;
                        SetWindowTitle();
                        return;
                    }

                    string message = "SaveFileWindow: Invalid filename. Please save to .txt file or .patch file.";
                    LogManager.WriteToLog(LogLevel.Everything, $"MessageBox: {message}");
                    MessageBox.Show(message);

                    saveFileDialog.FileName = "readme.patch";
                }
            }
            else SaveToFile(CurrentPatchData.FilePath);
        }

        /// <summary>
        /// Save the current patch data to a text or patch file
        /// </summary>
        private void SaveToFile(string saveFile = "")
        {
            if (string.IsNullOrEmpty(saveFile))
                saveFile = CurrentPatchData.FilePath;

            LogManager.WriteToLog(LogLevel.Basic, $"Save to file: {saveFile}.");

            // Save to json
            if (Globals.IsPatchFile(saveFile))
            {
                patchesManager.SaveToPatchFile(AllPatchData.ToList(), saveFile);
            }
            // Save to txt
            else if (Globals.IsTxtFile(saveFile))
            {
                string export = GenerateExport();
                using (FileStream fileStream = new FileStream(saveFile, FileMode.Create))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(export);
                }
            }
            else return;

            CurrentPatchData.ResetModified();
        }

        /// <summary>
        /// Generates the export data of CurrentPatchData as a string
        /// </summary>
        private string GenerateExport()
        {
            LogManager.WriteToLog(LogLevel.Basic, "Starting to generate export.");
            CheckForService();
            return patchesManager.GenerateExport(CurrentPatchData, ExportWithLineBreaks);
        }

        /// <summary>
        /// Export all patch notes
        /// </summary>
        private void ExportAllCommand(object o)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Export all.");
            if (MergeMode)
            {
                // Ensure all patches have a short description
                StringBuilder str = new StringBuilder();
                foreach (PatchData patch in AllPatchData)
                {
                    if (string.IsNullOrWhiteSpace(patch.ShortDescription))
                        str.Append(patch.Ticket + "; ");
                }

                if (str.Length > 0)
                {
                    string tickets = str.ToString().Trim().TrimEnd(';');

                    string message = $"ExportAllCommand: Please enter a short description for the following tickets: {tickets}";
                    LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
                    MessageBox.Show(message);
                    return;
                }

                // Get the merging folder
                SaveFileWindowMerge();
                SaveCurrentMerge();
            }
            else
            {
                CheckForService();
                SaveFileWindow();
                exportDirectory = Path.GetDirectoryName(CurrentPatchData.FilePath);
            }

            if (string.IsNullOrEmpty(exportDirectory)) return;

            // Get final programs list
            Mouse.OverrideCursor = Cursors.Wait;
            LogManager.WriteToLog(LogLevel.Basic, $"Get list of all programs being patched out from directory: {exportDirectory}.");
            List<ProgramData> finalProgramsList = patchesManager.GenerateProgramDataList(exportDirectory);

            // Check for NeedsCommonFiles and NeedsSqlScripts
            foreach (PatchData patch in AllPatchData)
            {
                if (patch.NeedsCommonFiles && patch.ProgramsUsed.Any(x => Globals.IsCommonFiles(x.ProgramName)) == false)
                    if (FindMissingProgram(patch, finalProgramsList, "CommonFiles.exe") == false)
                    {
                        Mouse.OverrideCursor = Cursors.Arrow;
                        return;
                    }

                if (patch.NeedsSqlScripts && patch.ProgramsUsed.Any(x => Globals.IsSqlScripts(x.ProgramName)) == false)
                    if (FindMissingProgram(patch, finalProgramsList, "SqlScripts.sql") == false)
                    {
                        Mouse.OverrideCursor = Cursors.Arrow;
                        return;
                    }
            }

            // Get the merged patch notes
            LogManager.WriteToLog(LogLevel.Basic, "Generate merged patch notes.");
            Dictionary<string, string> mergedPatches = patchesManager.MergePatches(AllPatchData.ToList(), finalProgramsList);

            // Save to text files
            LogManager.WriteToLog(LogLevel.Basic, "Save merged patch notes to files.");
            foreach (var patch in mergedPatches)
            {
                string patchFileName = Path.ChangeExtension(patch.Key, ".txt");
                string filePath = Path.Combine(exportDirectory, patchFileName);
                string originalText = string.Empty;

                LogManager.WriteToLog(LogLevel.Everything, $"Writing patch notes for: {patchFileName}.");

                if (File.Exists(filePath))
                    originalText = File.ReadAllText(filePath);
                using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(patch.Value + originalText);
                }
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Checks for missing program in the patch notes. If it can find it in the finalProgramsList, updates current patch notes.
        /// </summary>
        /// <param name="patch">Patch notes</param>
        /// <param name="finalProgramsList">Final programs list</param>
        /// <param name="programName">Missing program name</param>
        private bool FindMissingProgram(PatchData patch, List<ProgramData> finalProgramsList, string programName)
        {
            ProgramData? exportProgram = finalProgramsList.FirstOrDefault(x => string.Compare(x.ProgramName, programName, 
                StringComparison.OrdinalIgnoreCase) == 0);

            if (exportProgram == null)
            {
                // Mouse cursor is 'Cursors.Wait' when entering this method
                Mouse.OverrideCursor = Cursors.Arrow;
                MessageBox.Show($"Missing {programName} for patch: {patch.Ticket}.");
                Mouse.OverrideCursor = Cursors.Wait;
                return false;
            }
            else
            {
                // Add it to the list of programs and update instructions
                LogManager.WriteToLog(LogLevel.Basic, $"Updated patch {patch.Ticket} with {programName} from export directory.");

                patch.ProgramsUsed.Add(exportProgram);
                List<string> programNames = new List<string>();
                patch.ProgramsUsed.ToList().ForEach(x => programNames.Add(x.ProgramName));

                patch.Instructions = patchesManager.GenerateInstructions(programNames);
                return true;
            }
        }

        #endregion

        #region UI Methods

        /// <summary>
        /// Sets the window title
        /// </summary>
        /// <param name="windowTitle">String for the window title</param>
        private void SetWindowTitle(string windowTitle = "")
        {
            string mode = "";
            if (MergeMode) mode = "(MERGE)";

            if (string.IsNullOrWhiteSpace(windowTitle) == false)
                WindowTitle = windowTitle;
            else if (string.IsNullOrWhiteSpace(CurrentPatchData.FilePath) == false)
                SetWindowTitle($"{mode} Patches Editor - {Path.GetDirectoryName(CurrentPatchData.FilePath).Split(Path.DirectorySeparatorChar).Last()}" +
                    $" - {Path.GetFileName(CurrentPatchData.FilePath)}");
            else
                SetWindowTitle($"{mode} Patches Editor");

            Application.Current.MainWindow.Title = WindowTitle;
        }

        /// <summary>
        /// Timer tick
        /// </summary>
        private void TimerTick(object sender, EventArgs e)
        {
            // If there are unsaved changes, add star to the title bar
            if (windowTitleHasStar == false && CurrentPatchData.Modified)
            {
                SetWindowTitle(WindowTitle + "*");
                windowTitleHasStar = true;
            }
            else if (windowTitleHasStar && CurrentPatchData.Modified == false)
            {
                SetWindowTitle(WindowTitle.TrimEnd('*'));
                windowTitleHasStar = false;
            }
        }

        /// <summary>
        /// Toggles the impact text field enabled property from current state to opposite state
        /// </summary>
        private void ToggleImpactTextFieldEnabledCommand(object o)
        {
            ImpactTextFieldEnabled = !ImpactTextFieldEnabled;
        }

        /// <summary>
        /// Toggles the dependencies text field enabled property from current state to opposite state
        /// </summary>
        private void ToggleDependenciesTextFieldEnabledCommand(object o)
        {
            DependenciesTextFieldEnabled = !DependenciesTextFieldEnabled;
        }

        /// <summary>
        /// Toggles the instructions text field enabled property from current state to opposite state
        /// </summary>
        private void ToggleInstructionsTextFieldEnabledCommand(object o)
        {
            InstructionsTextFieldEnabled = !InstructionsTextFieldEnabled;
        }

        /// <summary>
        /// Toggles the programs used text field enabled property from current state to opposite state
        /// </summary>
        private void ToggleProgramsUsedTextFieldEnabledCommand(object o)
        {
            ProgramsUsedTextFieldEnabled = !ProgramsUsedTextFieldEnabled;
        }

        /// <summary>
        /// Enables the impact and/or dependencies text fields from the loaded patch.
        /// If the impact text is not default, then enable.
        /// If the dependencies text is not blank, then enable.
        /// </summary>
        private void EnableImpactDependenciesTextFields()
        {
            if (string.IsNullOrEmpty(CurrentPatchData.Impact.AllImpact) == false &&
                string.Compare(Globals.AppParameters.NoneImpact, CurrentPatchData.Impact.AllImpact, StringComparison.OrdinalIgnoreCase) != 0)
            {
                ImpactTextFieldEnabled = true;
            }

            if (string.IsNullOrEmpty(CurrentPatchData.AllDependencies) == false)
                DependenciesTextFieldEnabled = true;
        }

        #endregion

        #region Merge Methods

        /// <summary>
        /// Opens the save file dialog to select the export directory for the merge. 
        /// </summary>
        private void SaveFileWindowMerge()
        {
            LogManager.WriteToLog(LogLevel.Basic, "Opening save file window for merge.");

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "All Files (*.*)|*.*",
                Title = "Select Export Folder",
                CheckFileExists = false,
                CheckPathExists = false,
                FileName = "NO FILE NAME",
                OverwritePrompt = false
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                exportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                UpdateMergeSaveFileLocation();
            }
        }

        /// <summary>
        /// Enter/Exit merge mode
        /// </summary>
        private void ToggleMergeCommand(object? o)
        {
            // MergeMode is true when entering merge mode and false when leaving
            if (MergeMode)
            {
                // If there is no patch currently loaded, open file dialog to load first patch
                if (string.IsNullOrEmpty(CurrentPatchData.Ticket))
                {
                    OpenCommand(null);
                }

                ExportWithLineBreaks = true;
                UpdateMergeLabel();
                UpdateMergeNavigationButtons();
                UpdateMergeSaveFileLocation();
                SaveCurrentMerge();
            }
        }

        /// <summary>
        /// Updates the enabled/disabled status of the merge mode navigation buttons
        /// </summary>
        private void UpdateMergeNavigationButtons()
        {
            AddButtonEnabled = true;

            if (AllPatchData.Count > 1)
            {
                PreviousButtonEnabled = true;
                NextButtonEnabled = true;
                DeleteButtonEnabled = true;

                if (CurrentPatchIndex == 0)
                {
                    PreviousButtonEnabled = false;
                }
                else if (CurrentPatchIndex == AllPatchData.Count - 1)
                {
                    NextButtonEnabled = false;
                }
            }
            else
            {
                PreviousButtonEnabled = false;
                NextButtonEnabled = false;
                DeleteButtonEnabled = false;
            }
        }

        /// <summary>
        /// Save the current merge data to a patch file
        /// </summary>
        private void SaveCurrentMerge()
        {
            if (string.IsNullOrEmpty(exportDirectory))
            {
                SaveFileWindowMerge();
                if (string.IsNullOrEmpty(exportDirectory)) return;
            }

            LogManager.WriteToLog(LogLevel.Everything, $"Current merge saved to: {mergeSaveFileName}.");
            SaveToFile(mergeSaveFileName);
            CurrentPatchData.ResetModified();
        }

        /// <summary>
        /// Updates the merge title
        /// </summary>
        private void UpdateMergeLabel()
        {
            if (AllPatchData.Count == 0)
                MergeLabel = "No Patches Selected";
            else
                MergeLabel = $"Patch {CurrentPatchIndex + 1}/{AllPatchData.Count}";
        }

        /// <summary>
        /// Updates the save file location for the merge save file
        /// </summary>
        private void UpdateMergeSaveFileLocation()
        {
            if (string.IsNullOrEmpty(exportDirectory) && string.IsNullOrEmpty(CurrentPatchData.FilePath) == false)
            {
                // Assume the current directory is the merge directory for saving purposes
                exportDirectory = Path.GetDirectoryName(CurrentPatchData.FilePath);
            }
            if (string.IsNullOrEmpty(exportDirectory)) return;

            // Move existing merge save file to new directory
            string newFileName = Path.Combine(exportDirectory, Globals.AppParameters.MergeSaveFileName);
            if (string.Compare(mergeSaveFileName, newFileName, StringComparison.OrdinalIgnoreCase) != 0)
            {
                try
                {
                    if (File.Exists(mergeSaveFileName))
                    {
                        File.Move(mergeSaveFileName, newFileName);
                    }
                    mergeSaveFileName = newFileName;
                }
                catch (Exception e)
                {
                    string message = $"UpdateMergeSaveFileLocation: Exception occured when moving merge save file from " +
                        $"{mergeSaveFileName} to {newFileName}. {e.Message}";
                    LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
                    MessageBox.Show(message);
                }
            }
        }

        /// <summary>
        /// Add a new patch in AllPatchData
        /// </summary>
        private void AddPatchCommand(object o)
        {
            string file = OpenFileWindow();
            if (string.IsNullOrEmpty(file)) return;

            LogManager.WriteToLog(LogLevel.Everything, $"Add new patch to current merge: {file}.");

            List<PatchData> newPatches = patchesManager.ImportFile(file);

            foreach (PatchData patch in newPatches)
            {
                // Check for duplicates
                PatchData? existing = AllPatchData.FirstOrDefault(x => string.Compare(x.Ticket, patch.Ticket, 
                    StringComparison.OrdinalIgnoreCase) == 0);
                if (existing != null)
                {
                    string message = $"At least one patch with ticket {patch.Ticket} has already been added. " +
                        $"Are you sure you want to add another? (Description for existing: {existing.ShortDescription}; " +
                        $"New: {patch.ShortDescription}).";
                    LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
                    if (MessageBox.Show(message, APP_NAME, MessageBoxButton.YesNo) == MessageBoxResult.No)
                        continue;
                }

                AllPatchData.Add(patch);
            }

            CurrentPatchIndex = AllPatchData.Count - newPatches.Count;
            LoadPatchInfo(AllPatchData[CurrentPatchIndex]);

            UpdateMergeTasks();
        }

        /// <summary>
        /// Move to the next patch in AllPatchData
        /// </summary>
        private void NextPatchCommand(object o)
        {
            if (CurrentPatchIndex < AllPatchData.Count - 1)
            {
                CurrentPatchIndex++;
                LoadPatchInfo(AllPatchData[CurrentPatchIndex]);
                PreviousButtonEnabled = true;
            }
            else NextButtonEnabled = false;

            UpdateMergeTasks();
        }

        /// <summary>
        /// Move to previous patch in AllPatchData
        /// </summary>
        private void PreviousPatchCommand(object o)
        {
            if (CurrentPatchIndex > 0)
            {
                CurrentPatchIndex--;
                LoadPatchInfo(AllPatchData[CurrentPatchIndex]);
                NextButtonEnabled = true;
            }
            else PreviousButtonEnabled = false;

            UpdateMergeTasks();
        }

        /// <summary>
        /// Delete the current patch in AllPatchData
        /// </summary>
        private void DeletePatchCommand(object o)
        {
            string message = $"Are you sure you want to delete ticket {CurrentPatchData.Ticket}?";
            LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
            if (MessageBox.Show(message, APP_NAME, MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            if (AllPatchData.Count > 1)
            {
                AllPatchData.RemoveAt(CurrentPatchIndex);
                if (CurrentPatchIndex > 0) CurrentPatchIndex--;
                LoadPatchInfo(AllPatchData[CurrentPatchIndex]);
            }
            else DeleteButtonEnabled = false;

            UpdateMergeTasks();
        }

        /// <summary>
        /// Tasks performed every time the current patch is changed in merge mode
        /// </summary>
        private void UpdateMergeTasks()
        {
            UpdateMergeLabel();
            SetWindowTitle();
            UpdateMergeNavigationButtons();
            SaveCurrentMerge();
            OnPropertyChanged("CurrentPatchData");
        }

        #endregion

        #region Programmer/Tester Methods

        /// <summary>
        /// Adds a new programmer to the list
        /// </summary>
        /// <param name="o"></param>
        private void AddProgrammerCommand(object o)
        {
            if (o == null || string.IsNullOrEmpty(o.ToString())) return;
            CurrentPatchData.Programmers.Add(o.ToString());
        }

        /// <summary>
        /// Removes a programmer from the list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteProgrammerCommand(object o)
        {
            if (o == null || string.IsNullOrEmpty(o.ToString())) return;
            CurrentPatchData.Programmers.Remove(o.ToString());
        }

        /// <summary>
        /// Adds a new tester to the list
        /// </summary>
        /// <param name="o"></param>
        private void AddTesterCommand(object o)
        {
            if (o == null || string.IsNullOrEmpty(o.ToString())) return;
            CurrentPatchData.Testers.Add(o.ToString());
        }

        /// <summary>
        /// Removes a tester from the list
        /// </summary>
        /// <param name="o"></param>
        private void DeleteTesterCommand(object o)
        {
            if (o == null || string.IsNullOrEmpty(o.ToString())) return;
            CurrentPatchData.Testers.Remove(o.ToString());
        }

        #endregion

        #region Impact Methods

        /// <summary>
        /// Imports the default Impact
        /// </summary>
        private void ImportDefaultImpactCommand(object o)
        {
            CurrentPatchData.Impact.AllImpact = Globals.AppParameters.NoneImpact;
        }

        /// <summary>
        /// Sets the HasService flag on the Impact by checking if the service is in the list of Programs Used
        /// </summary>
        private void CheckForService()
        {
            LogManager.WriteToLog(LogLevel.Everything, "Check for Service.exe in programs used.");
            CurrentPatchData.Impact.HasService = CurrentPatchData.ProgramsUsed.Any(x => Globals.IsService(x.ProgramName));
        }

        #endregion

        #region Dependencies Methods

        /// <summary>
        /// Generates dependencies for each program used.
        /// </summary>
        public void GenerateDependencies()
        {
            string version = patchesManager.GetCurrentVersionString(CurrentPatchData.ProgramsUsed.ToList());
            string origDependencies = string.Empty;
            LogManager.WriteToLog(LogLevel.Basic, $"Enter GenerateDependencies. Current version: {version}. " +
                $"Current dependencies count: {CurrentPatchData.Dependencies.Count}");

            // Add new and update existing dependencies
            foreach (ProgramData mainProgram in CurrentPatchData.ProgramsUsed)
            {
                // Get the original dependencies from the released patch notes and store them in dictionary to reuse later
                // if necessary (so we don't keep prompting every refresh)
                string dictName = Globals.GetAndroidName(mainProgram.ProgramName, true);
                if (GetDependenciesType(mainProgram.ProgramName) == DependenciesType.Some)
                {
                    if (origDependenciesDict.ContainsKey(dictName))
                    {
                        LogManager.WriteToLog(LogLevel.Everything, $"Using orig dependencies from dictionary for '{mainProgram.ProgramName}'.");
                        origDependencies = origDependenciesDict[dictName];
                    }
                    else
                    {
                        origDependencies = ImportDependencies(mainProgram.ProgramName, version);
                        origDependenciesDict.Add(dictName, origDependencies);
                    }
                }

                // If program already exists in the list with no programdependencies, create the list
                DependenciesData? existingDepData = CurrentPatchData.Dependencies.FirstOrDefault(x => 
                    string.Compare(Globals.GetAndroidName(x.ProgramName, true), dictName, StringComparison.OrdinalIgnoreCase) == 0);
                if (existingDepData != null && existingDepData.ProgramDependencies.Count == 0)
                {
                    LogManager.WriteToLog(LogLevel.Everything, $"Generate DependencyPrograms list for '{existingDepData.ProgramName}'.");

                    foreach (var item in GenerateDependencyPrograms(mainProgram.ProgramName, origDependencies))
                    {
                        existingDepData.ProgramDependencies.Add(item);
                    }

                    existingDepData.GenerateText();
                }
                // If program already exists in the list, with some programdependencies, update them
                else if (existingDepData != null)
                {
                    LogManager.WriteToLog(LogLevel.Everything, $"Update DependencyPrograms list for '{existingDepData.ProgramName}'.");

                    List<DependencyProgram> origProgDependencies = new List<DependencyProgram>(existingDepData.ProgramDependencies);
                    List<DependencyProgram> newProgDependencies = new List<DependencyProgram>(GenerateDependencyPrograms(mainProgram.ProgramName, 
                        origDependencies));

                    // Add new programs
                    foreach (DependencyProgram newProg in newProgDependencies)
                    {
                        if (existingDepData.ProgramDependencies.Any(x => string.Compare(x.ProgramName, newProg.ProgramName, 
                            StringComparison.OrdinalIgnoreCase) == 0) == false)
                        {
                            LogManager.WriteToLog(LogLevel.Everything, $"Adding new program '{newProg.ProgramName}' with version {newProg.Version}.");
                            existingDepData.ProgramDependencies.Add(newProg);
                        }
                    }

                    // Remove obsolete programs
                    foreach (DependencyProgram origProg in origProgDependencies)
                    {
                        if (newProgDependencies.Any(x => string.Compare(x.ProgramName, origProg.ProgramName, 
                            StringComparison.OrdinalIgnoreCase) == 0) == false)
                        {
                            LogManager.WriteToLog(LogLevel.Everything, $"Removing old program '{origProg.ProgramName}'.");
                            existingDepData.ProgramDependencies.Remove(origProg);
                        }
                    }

                    // Update existing programs (version only)
                    foreach (DependencyProgram newProg in newProgDependencies)
                    {
                        DependencyProgram? origProg = existingDepData.ProgramDependencies.FirstOrDefault(x => 
                            string.Compare(x.ProgramName, newProg.ProgramName, StringComparison.OrdinalIgnoreCase) == 0);
                        if (origProg != null && string.Compare(newProg.Version, origProg.Version, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            LogManager.WriteToLog(LogLevel.Everything, $"Updating program version for '{origProg.ProgramName}' " +
                                $"from {origProg.Version} to {newProg.Version}.");
                            origProg.Version = newProg.Version;
                        }
                    }

                    // For the Android, update the main program name if the version changed
                    if (Globals.IsAndroid(existingDepData.ProgramName) && string.Compare(existingDepData.ProgramName, 
                        mainProgram.ProgramName, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        LogManager.WriteToLog(LogLevel.Everything, $"Updating main program name for Android from '{existingDepData.ProgramName}' " +
                            $"to '{mainProgram.ProgramName}'.");
                        existingDepData.ProgramName = mainProgram.ProgramName;
                    }

                    existingDepData.GenerateText();
                }
                // Create new dependencies record
                else
                {
                    DependenciesData dependenciesData = new DependenciesData()
                    {
                        ProgramName = mainProgram.ProgramName,
                        Type = GetDependenciesType(mainProgram.ProgramName)
                    };

                    LogManager.WriteToLog(LogLevel.Everything, $"Add dependencies for '{dependenciesData.ProgramName}'. Type: {dependenciesData.Type}.");

                    if (dependenciesData.Type == DependenciesType.Some)
                    {
                        // Check the orig dependencies to see if they were 'All' or 'None'
                        if (CheckAllDependencies(origDependencies))
                        {
                            dependenciesData.Type = DependenciesType.All;
                            LogManager.WriteToLog(LogLevel.Everything, $"Changed dependencies type for '{dependenciesData.ProgramName}' to All.");
                        }
                        else if (CheckNoneDependencies(origDependencies))
                        {
                            dependenciesData.Type = DependenciesType.None;
                            LogManager.WriteToLog(LogLevel.Everything, $"Changed dependencies type for '{dependenciesData.ProgramName}' to None.");
                        }
                    }

                    foreach (var item in GenerateDependencyPrograms(mainProgram.ProgramName, origDependencies))
                    {
                        dependenciesData.ProgramDependencies.Add(item);
                    }

                    dependenciesData.GenerateText();
                    CurrentPatchData.Dependencies.Add(dependenciesData);
                }
            }

            // Remove obsolete programs from main dependencies list
            List<string> origProgramList = new List<string>(CurrentPatchData.Dependencies.Select(x => x.ProgramName).Distinct());
            foreach (string origProgram in origProgramList)
            {
                bool programFound = false;
                foreach (ProgramData program in CurrentPatchData.ProgramsUsed)
                {
                    if (string.Compare(Globals.GetAndroidName(origProgram, true), Globals.GetAndroidName(program.ProgramName, true), 
                        StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        programFound = true;
                        break;
                    }
                }

                if (programFound == false)
                {
                    var removeDep = new List<DependenciesData>(CurrentPatchData.Dependencies.Where(x => string.Compare(origProgram, x.ProgramName) == 0));
                    foreach (DependenciesData depData in removeDep)
                    {
                        LogManager.WriteToLog(LogLevel.Everything, $"Removing dependencies data for '{origProgram}'.");
                        CurrentPatchData.Dependencies.Remove(depData);
                    }
                }
            }

            LogManager.WriteToLog(LogLevel.Everything, $"Done GenerateDependencies. Current dependencies count: {CurrentPatchData.Dependencies.Count}");
        }

        /// <summary>
        /// Generates the list of dependency programs for a selected program.
        /// </summary>
        /// <param name="currentProgram">Current program</param>
        /// <param name="origDependencies">Original dependencies string from patch notes</param>
        /// <returns></returns>
        private ObservableCollection<DependencyProgram> GenerateDependencyPrograms(string currentProgram, string origDependencies)
        {
            // Add the programs from the previous dependencies (original patch notes)
            ObservableCollection<DependencyProgram> dependencies = ParseDependencies(origDependencies);
            if (dependencies == null) dependencies = new ObservableCollection<DependencyProgram>();

            // Add the new programs from this patch
            foreach (ProgramData prog in CurrentPatchData.ProgramsUsed)
            {
                // Skip the current program
                if (string.Compare(prog.ProgramName, currentProgram, StringComparison.OrdinalIgnoreCase) == 0)
                    continue;

                // Skip CommonFiles
                if (Globals.IsCommonFiles(prog.ProgramName)) continue;

                // Skip Android
                if (Globals.IsAndroid(prog.ProgramName)) continue;

                // If the program is already in the list, update the version
                DependencyProgram? depProg = dependencies.FirstOrDefault(x => string.Compare(x.ProgramName, prog.ProgramName, 
                    StringComparison.OrdinalIgnoreCase) == 0);
                if (depProg != null)
                {
                    depProg.Version = prog.VersionString;
                }
                // If it doesn't already exist, add it to the list
                else
                {
                    depProg = new DependencyProgram(prog.ProgramName, DependencyStatus.NotRequired, prog.VersionString);
                    dependencies.Add(depProg);
                }
            }

            return new ObservableCollection<DependencyProgram>(dependencies.OrderBy(x => x.ProgramName));
        }

        /// <summary>
        /// Imports dependencies from existing patches for the passed program.
        /// </summary>
        private string ImportDependencies(string programName, string version)
        {
            string result = string.Empty;
            Mouse.OverrideCursor = Cursors.Wait;
            LogManager.WriteToLog(LogLevel.Basic, $"Get dependencies from released patch notes for: '{programName}'.");

            if (string.IsNullOrEmpty(version) == false)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                string filename = patchesManager.GetReleasedPatchNotesFileName(programName, version);
                if (string.IsNullOrEmpty(filename) == false)
                {
                    // Commented out for demo version to reduce error messages when not finding old patch notes
                    /*string message = $"Could not find released patch notes for {programName}. Would you like to manually select a file?";
                    LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");

                    if (MessageBox.Show(message, APP_NAME, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        filename = OpenFileWindow();
                        if (string.IsNullOrEmpty(filename) == false)
                        {
                            result = patchesManager.GetSectionFromReleasedPatches(programName, filename, Section.Dependencies);
                        }
                    }*/
                }
                else
                {
                    result = patchesManager.GetSectionFromReleasedPatches(programName, filename, Section.Dependencies);
                }
                Mouse.OverrideCursor = Cursors.Wait;
                LogManager.WriteToLog(LogLevel.Basic, $"Got dependencies: '{result}' for {programName}.");
            }
            else
            {
                string message = "ImportDependencies: Could not determine version.";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}");
                MessageBox.Show(message);
            }

            Mouse.OverrideCursor = Cursors.Arrow;
            return result;
        }

        /// <summary>
        /// Parses the dependencies string and returns a list of dependencies
        /// </summary>
        /// <param name="origDependencies">Original dependencies string from patch notes</param>
        /// <returns></returns>
        private ObservableCollection<DependencyProgram> ParseDependencies(string origDependencies)
        {
            ObservableCollection<DependencyProgram> results = new ObservableCollection<DependencyProgram>();
            DependencyProgram depProg = new DependencyProgram();

            if (string.IsNullOrEmpty(origDependencies)) return results;

            // Splitting on the space character to check each word individually
            origDependencies = origDependencies.Replace(Environment.NewLine, " ");
            LogManager.WriteToLog(LogLevel.Basic, $"Parse dependencies: '{origDependencies}'.");
            foreach (string word in origDependencies.Split(' '))
            {
                // If the word contains a period not at the end of the word, it's probably a program or a version
                if (word.Contains('.') && word.IndexOf('.') < word.Length - 1)
                {
                    LogManager.WriteToLog(LogLevel.Everything, $"Found valid word: '{word}'.");

                    // Save program name
                    if (string.IsNullOrEmpty(depProg.ProgramName))
                    {
                        depProg.ProgramName = word.TrimEnd('.');
                        LogManager.WriteToLog(LogLevel.Everything, $"Using '{depProg.ProgramName}' as program name.");
                    }
                    // Save version, store in results and reset
                    else
                    {
                        depProg.Version = word.TrimEnd('.');
                        LogManager.WriteToLog(LogLevel.Everything, $"Using '{depProg.Version}' as version.");
                        depProg.Status = DependencyStatus.Previous;
                        results.Add(new DependencyProgram(depProg));
                        depProg = new DependencyProgram();
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the dependencies type for the program based on the parameters
        /// </summary>
        /// <param name="programName">Program name</param>
        /// <returns></returns>
        private DependenciesType GetDependenciesType(string programName)
        {
            if (Globals.AppParameters.AllDependenciesPrograms.Any(x => string.Compare(x, programName,
                StringComparison.OrdinalIgnoreCase) == 0))
            {
                return DependenciesType.All;
            }

            if (Globals.AppParameters.NoneDependenciesPrograms.Any(x => string.Compare(x, programName,
                StringComparison.OrdinalIgnoreCase) == 0))
            {
                return DependenciesType.None;
            }

            return DependenciesType.Some;
        }

        /// <summary>
        /// Checks whether the dependencies from existing patches have all dependencies
        /// </summary>
        /// <param name="dependencies">Original dependencies string from patch notes</param>
        /// <returns></returns>
        private bool CheckAllDependencies(string dependencies)
        {
            if (string.IsNullOrEmpty(dependencies)) return false;
            return string.Compare(dependencies.Trim(), Globals.AppParameters.AllDependencies, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Checks whether the dependencies from existing patches have no dependencies
        /// </summary>
        /// <param name="dependencies">Original dependencies string from patch notes</param>
        /// <returns></returns>
        private bool CheckNoneDependencies(string dependencies)
        {
            if (string.IsNullOrEmpty(dependencies)) return true;
            return string.Compare(dependencies.Trim(), Globals.AppParameters.NoneDependencies, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion

        #region Programs Used Methods

        /// <summary>
        /// Refresh the programs used from the current folder
        /// </summary>
        private void RefreshProgramsUsedCommand(object? o)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Update programs used list.");

            // Get folder where the programs are
            SaveCommand(null);
            if (string.IsNullOrWhiteSpace(CurrentPatchData.FilePath)) return;
            string? workingDirectory = Path.GetDirectoryName(CurrentPatchData.FilePath);
            if (string.IsNullOrEmpty(workingDirectory)) return;

            // Re-generate the programs used list
            Mouse.OverrideCursor = Cursors.Wait;
            CurrentPatchData.ProgramsUsed.Clear();
            List<ProgramData> allProgs = patchesManager.GenerateProgramDataList(workingDirectory);
            foreach (ProgramData program in allProgs)
            {
                LogManager.WriteToLog(LogLevel.Everything, $"Adding program used: {program.ProgramName}.");
                CurrentPatchData.ProgramsUsed.Add(program);
            }
            CurrentPatchData.SetModified();

            // Check programs for resource files and obfuscation
            List<string> messages = patchesManager.CheckPrograms(CurrentPatchData.ProgramsUsed);
            Mouse.OverrideCursor = Cursors.Arrow;

            foreach (string message in messages)
            {
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                MessageBox.Show(message);
            }

            CheckForService();
            GenerateDependencies();
            RefreshInstructionsCommand(o);
        }

        #endregion

        #region Instructions Methods

        /// <summary>
        /// Refresh instructions from the programs used
        /// </summary>
        private void RefreshInstructionsCommand(object? o)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Update instructions.");

            // If no programs, return
            if (CurrentPatchData.ProgramsUsed.Count == 0) return;

            // Extract list of program names
            List<string> programNames = new List<string>();
            foreach (var program in CurrentPatchData.ProgramsUsed)
            {
                programNames.Add(program.ProgramName);
            }

            CurrentPatchData.Instructions = patchesManager.GenerateInstructions(programNames);
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
