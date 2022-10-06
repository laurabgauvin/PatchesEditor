using PatchesEditor.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Windows;
using PatchesEditor.Views;

namespace PatchesEditor.Helpers
{
    /// <summary>
    /// Class that performs patch notes operations
    /// </summary>
    public class PatchesManager
    {
        #region Private Variables

        private const string PATCH_SEPARATOR = "***************************************************";
        private const string LAST_LINE = "+++";
        private const int MAX_CHAR_LINE = 100;

        #endregion Private Variables

        #region Export & Save Methods

        /// <summary>
        /// Export the patch notes to a fully formatted string
        /// </summary>
        /// <param name="patchData">Patch Data to export</param>
        /// <param name="lineBreaks">Should the lines have line breaks based on MAX_CHAR_LINE?</param>
        /// <returns></returns>
        public string GenerateExport(PatchData patchData, bool lineBreaks)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Generate export for: {patchData.Ticket}.");

            List<string> programmers = patchData.Programmers.ToList();
            List<string> testers = patchData.Testers.ToList();
            programmers.Sort();
            testers.Sort();

            StringBuilder patchNotes = new StringBuilder();
            patchNotes.AppendLine(PATCH_SEPARATOR);
            patchNotes.AppendLine($"Date: {patchData.PatchDate:MMMM d/yyyy}");
            patchNotes.AppendLine($"Programmer: {string.Join(", ", programmers)}");
            patchNotes.AppendLine($"Tester: {string.Join(", ", testers)}");
            patchNotes.AppendLine($"Ticket #: {patchData.Ticket.Trim()}");
            if (string.IsNullOrWhiteSpace(patchData.ReferenceNumber) == false)
            {
                string refNum = patchData.ReferenceNumber.Trim();
                switch (patchData.ReferenceType)
                {
                    case ReferenceType.Reference:
                        patchNotes.AppendLine($"Reference #: {refNum}");
                        break;
                    case ReferenceType.Quote:
                        patchNotes.AppendLine($"Quote #: {refNum}");
                        break;
                    case ReferenceType.Defect:
                        patchNotes.AppendLine($"Defect #: {refNum}");
                        break;
                    default:
                        break;
                }
            }
            patchNotes.AppendLine();

            patchNotes.AppendLine("Background:");
            foreach (string line in DoLineBreaks(patchData.Background.Trim(), lineBreaks))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            patchNotes.AppendLine("Impact:");
            string impact = GenerateImpactText(patchData.Impact, patchData.ProgramsUsed.ToList());
            foreach (string line in DoLineBreaks(impact.Trim(), lineBreaks))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            patchNotes.AppendLine("Dependencies:");
            string dependencies = GenerateDependenciesText(patchData.AllDependencies, patchData.Dependencies.ToList());
            foreach (string line in DoLineBreaks(dependencies.Trim(), lineBreaks))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            patchNotes.AppendLine("Description of Changes:");
            foreach (string line in DoLineBreaks(patchData.DescriptionOfChanges.Trim(), lineBreaks))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            patchNotes.AppendLine("Instructions:");
            foreach (string line in DoLineBreaks(patchData.Instructions.Trim(), lineBreaks))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            patchNotes.AppendLine("Programs Used:");
            List<string> formattedProgramsUsed = new List<string>();
            foreach (ProgramData program in patchData.ProgramsUsed)
            {
                formattedProgramsUsed.Add(GenerateProgramsUsedText(program));
            }
            patchNotes.AppendLine(string.Join(Environment.NewLine, formattedProgramsUsed));
            patchNotes.AppendLine();

            patchNotes.AppendLine(PATCH_SEPARATOR);
            patchNotes.AppendLine(LAST_LINE);
            return patchNotes.ToString();
        }

        /// <summary>
        /// Exports all of the patch notes for the bulletin
        /// </summary>
        /// <param name="patches">Patches to export</param>
        /// <returns></returns>
        public string ExportBulletin(List<PatchData> patches)
        {
            StringBuilder bulletin = new StringBuilder();

            foreach (PatchData patch in patches)
            {
                LogManager.WriteToLog(LogLevel.Everything, $"Exporting {patch.Ticket} for bulletin.");
                string version = GetCurrentVersionString(patch.ProgramsUsed.ToList());

                bulletin.AppendLine($"Ticket #{patch.Ticket}");
                bulletin.AppendLine(patch.Background);
                bulletin.AppendLine();
                bulletin.AppendLine(patch.DescriptionOfChanges);
                bulletin.AppendLine();
                bulletin.AppendLine($"This has been patched out in {version}.");
                bulletin.AppendLine();
            }

            return bulletin.ToString().TrimEnd();
        }

        /// <summary>
        /// Exports the text for the task update
        /// </summary>
        /// <param name="patchData">Patch to export</param>
        /// <returns></returns>
        public string ExportTaskUpdate(PatchData patchData)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Exporting {patchData.Ticket} for task update.");

            StringBuilder taskUpdate = new StringBuilder();
            string version = GetCurrentVersionString(patchData.ProgramsUsed.ToList());

            taskUpdate.AppendLine($"Tested in {version}");
            taskUpdate.AppendLine();
            taskUpdate.AppendLine("Programs used:");

            foreach (ProgramData program in patchData.ProgramsUsed)
            {
                if (Globals.IsCommonFiles(program.ProgramName)) continue;

                program.GenerateVersionString();
                string programName = program.ProgramName;

                if (Globals.IsAndroid(program.ProgramName))
                {
                    programName = Globals.GetAndroidName(program.ProgramName) + ".apk";
                }

                if (string.IsNullOrWhiteSpace(program.VersionString)) taskUpdate.AppendLine($"{programName}");
                else taskUpdate.AppendLine($"{programName}, version {program.VersionString}");
            }

            return taskUpdate.ToString().TrimEnd();
        }

        /// <summary>
        /// Save the patch data to a json file
        /// </summary>
        /// <param name="patches">List of patch data to save</param>
        /// <param name="saveFileName">Path and name of file to save to</param>
        public void SaveToPatchFile(List<PatchData> patches, string saveFileName)
        {
            if (Globals.IsPatchFile(saveFileName) == false)
            {
                string message = "SaveToPatchFile error: Invalid file extention, should be .patch.";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                MessageBox.Show(message);
                return;
            }

            try
            {
                using (StreamWriter strWriter = File.CreateText(saveFileName))
                {
                    JsonSerializer serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented
                    };
                    serializer.Serialize(strWriter, patches);
                }
            }
            catch (Exception e)
            {
                string message = $"SaveToPatchFile error: Exception while saving file or serializing JSON: {e.Message}";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                MessageBox.Show(message);
            }
        }

        #endregion Export & Save Methods

        #region Import & Load Methods

        /// <summary>
        /// Import the file
        /// </summary>
        /// <param name="filepath">Path of file to import</param>
        /// <returns></returns>
        public List<PatchData> ImportFile(string filepath)
        {
            if (File.Exists(filepath))
            {
                // Import from json file
                if (Globals.IsPatchFile(filepath))
                {
                    List<PatchData> result = LoadFromPatchFile(filepath);
                    if (result == null) return new List<PatchData>();
                    return result;
                }
                // Import from txt file
                else if (Globals.IsTxtFile(filepath))
                {
                    return new List<PatchData>() { ImportFromTextFile(filepath) };
                }
            }

            return new List<PatchData>();
        }

        /// <summary>
        /// Import patch data from a txt file (single patch)
        /// </summary>
        /// <param name="filePath">Text file to import from</param>
        /// <returns></returns>
        private PatchData ImportFromTextFile(string filePath)
        {
            LogManager.WriteToLog(LogLevel.Basic, $"Import from text file: {filePath}.");
            PatchData patchData = new PatchData();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                Section currentSection = Section.General;
                bool changedSection = false;
                StringBuilder strBuilder = new StringBuilder();

                while (streamReader.EndOfStream == false)
                {
                    string? line = streamReader.ReadLine();
                    if (line == null) continue;

                    if (currentSection == Section.General && string.IsNullOrWhiteSpace(line))
                    {
                        // Skip blank lines in the top section
                        continue;
                    }
                    else if (string.Compare(line, LAST_LINE, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Stop at the end of the current patch notes
                        break;
                    }
                    else if (line.StartsWith("Date:", StringComparison.OrdinalIgnoreCase))
                    {
                        if (line.Length < 6) patchData.PatchDate = DateTime.Today;
                        else
                        {
                            string tmp = line.Substring(6);
                            if (DateTime.TryParse(tmp, out DateTime result))
                                patchData.PatchDate = result;
                        }
                    }
                    else if (line.StartsWith("Programmer:", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmp = line.Substring(12);
                        patchData.Programmers = new ObservableCollection<string>(ParseSeparated(tmp));
                    }
                    else if (line.StartsWith("Tester:", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmp = line.Substring(8);
                        patchData.Testers = new ObservableCollection<string>(ParseSeparated(tmp));
                    }
                    else if (line.StartsWith("Ticket #:", StringComparison.OrdinalIgnoreCase) ||
                        line.StartsWith("Tickets #:", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmp = line.Substring(10);
                        patchData.Ticket = tmp;
                    }
                    else if (line.StartsWith("Reference #:", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmp = line.Substring(13);
                        patchData.ReferenceNumber = tmp;
                        patchData.ReferenceType = ReferenceType.Reference;
                    }
                    else if (line.StartsWith("Quote #:", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmp = line.Substring(9);
                        patchData.ReferenceNumber = tmp;
                        patchData.ReferenceType = ReferenceType.Quote;
                    }
                    else if (line.StartsWith("Defect #:", StringComparison.OrdinalIgnoreCase))
                    {
                        string tmp = line.Substring(10);
                        patchData.ReferenceNumber = tmp;
                        patchData.ReferenceType = ReferenceType.Defect;
                    }
                    else if (line.StartsWith("Background", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSection = Section.Background;
                        changedSection = true;
                    }
                    else if (line.StartsWith("Impact", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSection = Section.Impact;
                        changedSection = true;
                    }
                    else if (line.StartsWith("Dependencies", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSection = Section.Dependencies;
                        changedSection = true;
                    }
                    else if (line.StartsWith("Description of Changes", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSection = Section.DescriptionOfChanges;
                        changedSection = true;
                    }
                    else if (line.StartsWith("Instructions:", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSection = Section.Instructions;
                        changedSection = true;
                    }
                    else if (line.StartsWith("Programs used:", StringComparison.OrdinalIgnoreCase))
                    {
                        currentSection = Section.ProgramsUsed;
                        changedSection = true;
                    }
                    else
                    {
                        // New section, clear string builder
                        if (changedSection)
                        {
                            strBuilder.Clear();
                            changedSection = false;
                        }

                        ProgramData programData = new ProgramData();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            // New paragraph
                            strBuilder = TrimEnd(strBuilder);
                            strBuilder.Append(Environment.NewLine + Environment.NewLine);
                        }
                        else
                        {
                            switch (currentSection)
                            {
                                case Section.Background:
                                case Section.Impact:
                                case Section.Dependencies:
                                case Section.DescriptionOfChanges:
                                case Section.Instructions:
                                    if (line.StartsWith("-") || StartsWithNumber(line))
                                    {
                                        if (strBuilder.Length != 0)
                                        {
                                            strBuilder = TrimEnd(strBuilder);
                                            strBuilder.Append(Environment.NewLine);
                                        }
                                    }
                                    strBuilder.Append(line.Trim() + " ");
                                    break;
                                case Section.ProgramsUsed:
                                    programData.ImportProgramsUsed(line.Trim());
                                    break;
                                default:
                                    break;
                            }
                        }

                        // Save current data
                        switch (currentSection)
                        {
                            case Section.Background:
                                patchData.Background = strBuilder.ToString().Trim();
                                break;
                            case Section.Impact:
                                patchData.Impact.AllImpact = strBuilder.ToString().Trim();
                                break;
                            case Section.Dependencies:
                                string dep = strBuilder.ToString().Trim();
                                if (string.Compare(dep, Globals.AppParameters.AllDependencies, StringComparison.OrdinalIgnoreCase) == 0 ||
                                    string.Compare(dep, Globals.AppParameters.NoneDependencies, StringComparison.OrdinalIgnoreCase) == 0)
                                    dep = string.Empty;
                                patchData.AllDependencies = dep;
                                break;
                            case Section.DescriptionOfChanges:
                                patchData.DescriptionOfChanges = strBuilder.ToString().Trim();
                                break;
                            case Section.Instructions:
                                patchData.Instructions = strBuilder.ToString().Trim();
                                break;
                            case Section.ProgramsUsed:
                                if (string.IsNullOrEmpty(programData.ProgramName)) break;
                                patchData.ProgramsUsed.Add(programData);
                                break;
                        }
                    }
                }
            }

            patchData.FilePath = filePath;
            return patchData;
        }

        /// <summary>
        /// Finds the file name for the released patch notes for the passed program and version.
        /// </summary>
        /// <param name="programName">Name of the program</param>
        /// <param name="version">Version</param>
        /// <returns></returns>
        public string GetReleasedPatchNotesFileName(string programName, string version)
        {
            string patchesPath = string.Format(Globals.AppParameters.PatchesDirectory, version);
            string patchNotesFileName = Path.ChangeExtension(programName, ".txt");
            bool isAndroid = Globals.IsAndroid(programName);
            string androidFileName = Globals.GetAndroidName(programName);
            string? importFileName = string.Empty;

            try
            {
                // Find patch notes for current program
                string[] allFiles = Directory.GetFiles(patchesPath, "*.txt", SearchOption.AllDirectories);
                if (isAndroid)
                {
                    importFileName = allFiles.FirstOrDefault(x => string.Compare(androidFileName, Globals.GetAndroidName(Path.GetFileName(x)), StringComparison.OrdinalIgnoreCase) == 0);
                }
                else
                {
                    importFileName = allFiles.FirstOrDefault(x => string.Compare(patchNotesFileName, Path.GetFileName(x), StringComparison.OrdinalIgnoreCase) == 0);
                }
            }
            catch (Exception e)
            {
                // Commented out for demo version to reduce error messages when not finding old patch notes
                /*string message = $"Exception when looking for patches {patchNotesFileName} in {patchesPath}: {e.Message}";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                MessageBox.Show(message);*/
            }

            if (string.IsNullOrEmpty(importFileName)) return string.Empty;
            return importFileName;
        }

        /// <summary>
        /// Load the patch data from a json file
        /// </summary>
        /// <param name="filePath">Json file path</param>
        /// <returns></returns>
        public List<PatchData> LoadFromPatchFile(string filePath)
        {
            List<PatchData> patches = new List<PatchData>();
            if (Globals.IsPatchFile(filePath) == false)
            {
                string message = "LoadFromPatchFile error: Invalid file extention, should be .patch.";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                MessageBox.Show(message);
                return patches;
            }

            try
            {
                using (StreamReader strReader = new StreamReader(filePath))
                {
                    string json = strReader.ReadToEnd();
                    var tmp = JsonConvert.DeserializeObject<List<PatchData>>(json);
                    if (tmp != null)
                        patches = tmp;
                }
            }
            catch (Exception e)
            {
                string message = $"LoadFromPatchFile error: Exception while deserializing JSON: {e.Message}";
                LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                MessageBox.Show(message);
                return patches;
            }

            foreach (PatchData patch in patches)
            {
                patch.ResetModified();
                patch.FilePath = filePath;
            }

            return patches;
        }

        #endregion Import & Load Methods

        #region Merge Methods

        /// <summary>
        /// Does the patch merging
        /// </summary>
        /// <param name="allPatches">The list of patches to merge</param>
        /// <param name="mergeDirPrograms">List of final programs in merge directory</param>
        /// <returns>Dictionary: program name, formatted patch notes for that program</returns>
        public Dictionary<string, string> MergePatches(List<PatchData> allPatches, List<ProgramData> mergeDirPrograms)
        {
            LogManager.WriteToLog(LogLevel.Basic, $"Generate merge for {allPatches.Count} patches.");
            Dictionary<string, string> result = new Dictionary<string, string>(); // <program name, final export patch notes text for that program>

            // Make list of all patches per program
            Dictionary<string, List<PatchData>> patchesPerProgram = new Dictionary<string, List<PatchData>>(); // <program name, list of patches for that program>
            foreach (PatchData patch in allPatches)
            {
                foreach (ProgramData program in patch.ProgramsUsed)
                {
                    string programName = program.ProgramName;
                    if (Globals.IsAndroid(program.ProgramName))
                    {
                        // Remove version from file name
                        programName = Globals.GetAndroidName(program.ProgramName) + ".apk";
                    }

                    if (patchesPerProgram.ContainsKey(programName) == false)
                    {
                        // Add new key to dictionary
                        patchesPerProgram.Add(programName, new List<PatchData>() { patch });
                    }
                    else
                    {
                        // Update existing key in the dictionary
                        List<PatchData> tmp = new List<PatchData>();
                        tmp.AddRange(patchesPerProgram[programName]);
                        if (tmp.Contains(patch) == false)
                        {
                            tmp.Add(patch);
                            patchesPerProgram[programName] = tmp;
                        }
                    }
                }
            }

            if (Globals.AppParameters.LogLevel == LogLevel.Everything)
            {
                foreach (KeyValuePair<string, List<PatchData>> keyValuePair in patchesPerProgram)
                {
                    LogManager.WriteToLog(LogLevel.Everything, $"Found {keyValuePair.Value.Count} patches containing program {keyValuePair.Key}.");
                }
            }

            // Generate patch notes for each program
            foreach (string program in patchesPerProgram.Keys)
            {
                string fullProgramName = program;

                if (Globals.IsAndroid(program))
                {
                    // Restore version in file name for export using final file name in merge directory
                    foreach (ProgramData mergeDirProgram in mergeDirPrograms)
                    {
                        if (Globals.IsAndroid(mergeDirProgram.ProgramName) &&
                            string.Compare(Globals.GetAndroidName(mergeDirProgram.ProgramName), Path.GetFileNameWithoutExtension(program), StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            fullProgramName = mergeDirProgram.ProgramName;
                            break;
                        }
                    }
                }

                result.Add(fullProgramName, GenerateMergedPatchText(patchesPerProgram[program], fullProgramName, mergeDirPrograms));
            }

            return result;
        }

        #endregion Merge Methods

        #region Dependencies Methods

        /// <summary>
        /// Gets a single string from the passed file name for the passed section
        /// </summary>
        /// <param name="programName">Name of the program</param>
        /// <param name="fileName">File path + name to read from</param>
        /// <param name="section">Section to import (Impact or Dependencies).</param>
        /// <returns></returns>
        public string GetSectionFromReleasedPatches(string programName, string fileName, Section section)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Getting {section} from released patched notes: {fileName}.");
            List<string> results = ImportPartialPatchFromTextFile(fileName, section);
            if (results.Count == 1) return results[0];

            string result = string.Empty;
            if (results.Count > 1)
            {
                UserStringSelectionWindow window = new UserStringSelectionWindow(programName, section, results);
                if (window.ShowDialog() == true)
                {
                    result = window.SelectedString;
                }
            }

            return result;
        }

        /// <summary>
        /// Imports only one section from a text file. Will return list that contains each instance of that section.
        /// </summary>
        /// <param name="filePath">Text file to import</param>
        /// <param name="section">Section to import (Impact or Dependencies).</param>
        /// <returns></returns>
        private List<string> ImportPartialPatchFromTextFile(string filePath, Section section)
        {
            List<string> results = new List<string>();
            bool foundAllSection = false;
            if (string.IsNullOrEmpty(filePath)) return results;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                StringBuilder strBuilder = new StringBuilder();
                int count = 0;
                int previousCount = 0;

                while (!streamReader.EndOfStream)
                {
                    string? line = streamReader.ReadLine();
                    if (line == null) continue;

                    if (string.Compare(line, LAST_LINE, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Stop at the end of the current patch notes
                        break;
                    }
                    else if (section == Section.Impact && line.StartsWith("Impact", StringComparison.OrdinalIgnoreCase))
                    {
                        if (foundAllSection == false)
                        {
                            if (string.Compare(line.Trim(), "Impact (All):", StringComparison.OrdinalIgnoreCase) == 0)
                                foundAllSection = true;
                        }

                        count++;
                    }
                    else if (line.StartsWith("Dependencies", StringComparison.OrdinalIgnoreCase))
                    {
                        if (section == Section.Dependencies && foundAllSection == false)
                        {
                            if (string.Compare(line.Trim(), "Dependencies (All):", StringComparison.OrdinalIgnoreCase) == 0)
                                foundAllSection = true;
                        }

                        if (section == Section.Dependencies)
                            count++;
                        else
                            break;
                    }
                    else if (line.StartsWith("Description of Changes", StringComparison.OrdinalIgnoreCase))
                    {
                        // You've gone too far and need to stop here
                        break;
                    }
                    else
                    {
                        // Found a new section
                        if (count > previousCount)
                        {
                            // Don't record the other results if we found an 'All' section
                            if (count > 1 && foundAllSection)
                                break;

                            // Record previous data
                            if (count > 1)
                                results.Add(strBuilder.ToString().Trim());

                            strBuilder.Clear();
                            previousCount = count;
                        }

                        // Record text to string builder
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            // New paragraph
                            strBuilder = TrimEnd(strBuilder);
                            strBuilder.Append(Environment.NewLine + Environment.NewLine);
                        }
                        else
                            strBuilder.Append(line.Trim() + " ");
                    }
                }

                // Record last section
                if (count > 0)
                    results.Add(strBuilder.ToString().Trim());
            }

            return results;
        }

        #endregion Dependencies Methods

        #region Instruction Methods

        /// <summary>
        /// Generates the instructions for the list of programs passed
        /// </summary>
        /// <param name="programNames">List of program names (file name with extension)</param>
        /// <returns></returns>
        public string GenerateInstructions(List<string> programNames)
        {
            List<string> programsInstall = new List<string>();
            List<string> programsResources = new List<string>();
            List<string> programsAndroid = new List<string>();
            bool hasService = false;
            bool hasCommonFiles = false;
            bool hasSqlScripts = false;
            programNames = programNames.OrderBy(x => x).ToList();

            // Determine where each program goes
            foreach (string program in programNames)
            {
                // Special programs
                if (Globals.IsService(program))
                {
                    hasService = true;
                    programsInstall.Add(program);
                }
                else if (Globals.IsCommonFiles(program))
                {
                    hasCommonFiles = true;
                }
                else if (Globals.IsAndroid(program))
                {
                    programsAndroid.Add(program);
                }
                else if (Globals.IsSqlScripts(program))
                {
                    hasSqlScripts = true;
                }
                // Regular programs
                else
                {
                    if (Globals.AppParameters.ProgramsResources.Contains(program))
                    {
                        programsResources.Add(program);
                    }
                    else
                    {
                        programsInstall.Add(program);
                    }
                }
            }

            // Format each line of instructions
            string installInstructions = GenerateInstructionLine(programsInstall, "Install");
            string resourcesInstructions = GenerateInstructionLine(programsResources, @"Install/Resources");

            // Build instruction steps
            List<string> listInstructions = new List<string>();
            int step = 1;
            if (hasService)
            {
                listInstructions.Add($"{step}) Close all programs, including the service.");
                step++;
            }

            listInstructions.AddRange(BuildInstructionSteps(ref step, installInstructions, resourcesInstructions));

            if (hasService)
            {
                listInstructions.Add($"{step}) Restart the service.");
                step++;
            }

            if (hasSqlScripts)
            {
                listInstructions.Add($" {step}) Run the SQL scripts.");
                step++;
            }

            if (programsAndroid.Count > 0)
            {
                listInstructions.AddRange(BuildAndroidInstructionSteps(programsAndroid, ref step));
            }

            if (hasCommonFiles)
            {
                listInstructions.Add($"{step}) Copy the CommonFiles.exe to the install directory and run it. It will update data files and documentation.");
                step++;
            }

            return string.Join(Environment.NewLine, listInstructions);
        }

        #endregion Instruction Methods

        #region Programs Methods

        /// <summary>
        /// Generates the programs list from the working directory passed.
        /// </summary>
        /// <param name="workingDirectory">Working directory</param>
        /// <returns></returns>
        public List<ProgramData> GenerateProgramDataList(string workingDirectory)
        {
            List<string> fileList = GetAllPrograms(workingDirectory);

            // Format the program list
            List<ProgramData> allPrograms = new List<ProgramData>();
            foreach (var filePath in fileList)
            {
                // If a program with the same name already exists in the list, skip adding it again
                if (allPrograms.Any(x => string.Compare(x.ProgramName, Path.GetFileName(filePath), StringComparison.OrdinalIgnoreCase) == 0)) continue;

                LogManager.WriteToLog(LogLevel.Everything, $"Adding program to programs list: {filePath}.");
                ProgramData programData = new ProgramData(filePath);
                allPrograms.Add(programData);
            }

            return allPrograms.OrderBy(x => x.ProgramName).ToList();
        }

        /// <summary>
        /// Returns the list of programs in the working directory and all sub directories, that are not in the ignored extentions list.
        /// Also checks the copyright year.
        /// </summary>
        /// <param name="workingDirectory">Working directory</param>
        /// <returns></returns>
        private List<string> GetAllPrograms(string workingDirectory)
        {
            // Get all programs in the directory and sub directories, ignore specific extensions
            List<string> fileList = Directory.GetFiles(workingDirectory).Where(x => Globals.AppParameters.IgnoredExtentions.Contains(Path.GetExtension(x)) == false).ToList();
            foreach (string subDirectory in Directory.GetDirectories(workingDirectory, "*", SearchOption.AllDirectories))
            {
                fileList.AddRange(Directory.GetFiles(subDirectory).Where(x => Globals.AppParameters.IgnoredExtentions.Contains(Path.GetExtension(x.ToLower())) == false));
            }

            // Verify programs
            foreach (string file in fileList)
            {
                // Check programs have the correct copyright year
                // (except Android and list from parameters)
                if (Globals.IsAndroid(file) == false &&
                    Globals.AppParameters.ProgramsIgnoreSignatureCopyright.Contains(Path.GetFileName(file)) == false)
                {
                    if (CheckCopyright(file) == false)
                    {
                        string message = $"Program {Path.GetFileName(file)} has an incorrect copyright year.";
                        LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                        MessageBox.Show(message);
                    }
                }
            }

            return fileList.OrderBy(x => Path.GetFileName(x)).ToList();
        }

        /// <summary>
        /// Check program list for missing or orphaned resource files and obfuscation. 
        /// Returns an error message for each error found.
        /// </summary>
        /// <param name="programList">Program list to check</param>
        public List<string> CheckPrograms(ObservableCollection<ProgramData> programList)
        {
            LogManager.WriteToLog(LogLevel.Basic, "Checking programs list for missing or orphaned resource files + obfuscation.");

            List<string> messages = new List<string>();
            foreach (ProgramData program in programList)
            {
                // Check for missing resource files
                if (Globals.AppParameters.ProgramsWithResourceFiles.Contains(program.ProgramName))
                {
                    string resourceFileName = Path.GetFileNameWithoutExtension(program.ProgramName) + ".resources.dll";
                    if (programList.Any(x => x.ProgramName == resourceFileName) == false)
                        messages.Add($"Resource file {resourceFileName} missing.");
                }

                if (program.ProgramName.Contains("resources.dll"))
                {
                    string mainFileName = program.ProgramName.Split('.')[0] + ".dll";
                    var mainProgram = programList.FirstOrDefault(x => x.ProgramName == mainFileName);

                    // Check for orphaned resource files
                    if (mainProgram == null)
                        messages.Add($"Resource file {program.ProgramName} does not have a matching dll ({mainFileName} is missing).");
                    else
                    {
                        // Check that resource file version matches main dll version
                        if (string.Compare(mainProgram.VersionString, program.VersionString, StringComparison.OrdinalIgnoreCase) != 0)
                            messages.Add($"Version of resource file {program.ProgramName} ({program.VersionString}) " +
                                $"does not match version of dll {mainProgram.ProgramName} ({mainProgram.VersionString}).");
                    }
                }
                // Check obfuscation (not for resource files, they are not obfuscated)
                else
                {
                    if (Globals.IsAndroid(program.ProgramName) == false)
                    {
                        try
                        {
                            Assembly assembly = Assembly.LoadFrom(program.FullPath);

                            if (assembly.GetType("DotfuscatorAttribute") == null)
                                messages.Add($"Program {program.ProgramName} is not obfuscated");
                        }
                        catch (Exception)
                        {
                            // Exceptions happen when checking non-C# programs, do nothing here
                        }
                    }
                }
            }

            return messages;
        }

        /// <summary>
        /// Check whether the program's copyright's end year matches the year of the modified date
        /// </summary>
        /// <param name="filePath">Program full path</param>
        /// <returns></returns>
        private bool CheckCopyright(string filePath)
        {
            var fileInfo = FileVersionInfo.GetVersionInfo(filePath);
            string? copyright = fileInfo.LegalCopyright;
            if (copyright == null) return false;

            DateTime modifiedDate = File.GetLastWriteTime(filePath);

            LogManager.WriteToLog(LogLevel.Everything, $"{Path.GetFileName(filePath)} - Copyright: '{copyright}'; Modified year: {modifiedDate.Year}.");

            string year = string.Empty;
            foreach (char c in copyright)
            {
                if (int.TryParse(c.ToString(), out _))
                    year += c;

                if (year.Length == 4)
                {
                    int copyrightYear = int.Parse(year);
                    if (copyrightYear == modifiedDate.Year)
                        return true;
                    else
                        year = string.Empty;
                }
            }

            return false;
        }

        #endregion Programs Methods

        #region General Methods

        /// <summary>
        /// Gets the version of the current patches, in a string format. Returns string.Empty if can't find it.
        /// </summary>
        /// <param name="programList">List of programs for the current patches</param>
        /// <returns></returns>
        public string GetCurrentVersionString(List<ProgramData> programList)
        {
            string version = string.Empty;
            foreach (ProgramData program in programList)
            {
                program.GenerateVersionString();
                if (string.IsNullOrEmpty(program.VersionString) == false && (string.Compare(program.VersionString, "0.0.0", StringComparison.OrdinalIgnoreCase) != 0))
                {
                    version = program.VersionString;
                    LogManager.WriteToLog(LogLevel.Everything, $"Found program with version: {version}.");
                    break;
                }
            }

            string[] tmp = version.Split('.');
            if (tmp.Count() > 3) version = $"{tmp[0]}.{tmp[1]}.{tmp[2]}";
            LogManager.WriteToLog(LogLevel.Everything, $"Using version: {version}.");

            return version;
        }

        /// <summary>
        /// Parse a string based on the separator and returns a list of strings
        /// </summary>
        /// <param name="line">String to parse</param>
        /// <param name="separator">Separator</param>
        /// <returns></returns>
        private List<string> ParseSeparated(string line, char separator = ',')
        {
            List<string> result = new List<string>();
            foreach (string item in line.Split(separator))
            {
                string tmp = item.Trim();
                if (string.IsNullOrWhiteSpace(tmp) == false)
                    result.Add(tmp);
            }
            return result;
        }

        /// <summary>
        /// Returns whether the current line starts with a number
        /// </summary>
        /// <param name="line">Line string</param>
        /// <returns></returns>
        private bool StartsWithNumber(string line)
        {
            if (string.IsNullOrEmpty(line)) return false;

            line = line.Trim();
            if (line.Length == 0) return false;
            return int.TryParse(line[0].ToString(), out _);
        }

        /// <summary>
        /// Trims the trailing spaces from a StringBuilder
        /// </summary>
        /// <param name="sb">String builder to trim</param>
        /// <returns></returns>
        private StringBuilder TrimEnd(StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return new StringBuilder();

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }

        /// <summary>
        /// Split the text in lines based on the MAX_CHAR_LINE param
        /// </summary>
        /// <param name="text">Text to split</param>
        /// <returns></returns>
        private List<string> DoLineBreaks(string text, bool doLineBreaks = true)
        {
            // If not doing line breaks, just return text as is
            if (doLineBreaks == false) return new List<string>() { text };

            // Return text as is if it fits on one line
            if (text.Length <= MAX_CHAR_LINE) return new List<string>() { text };

            text = text.Replace(Environment.NewLine, $" {Environment.NewLine}"); // Add space in front of line breaks so they get split
            List<string> result = new List<string>();
            int numberOfLeadingSpaces = 0;
            int i = 0;
            StringBuilder line = new StringBuilder();

            foreach (string word in text.Split(new string[] { " " }, StringSplitOptions.None))
            {
                // First word
                if (i == 0)
                {
                    numberOfLeadingSpaces = GetNumberOfLeadingSpaces(word);
                }

                if (word == "") continue;
                else if (word.Contains(Environment.NewLine))
                {
                    if (line.Length > 0) result.Add(line.ToString().TrimEnd()); // Record existing line
                    else result.Add(""); // Add blank line if previous line was not blank
                    line.Clear(); // Reset line

                    string tmp = word.Replace(Environment.NewLine, ""); // Remove line break from word
                    if (tmp.Length > 0)
                    {
                        numberOfLeadingSpaces = GetNumberOfLeadingSpaces(tmp);
                        line.Append(tmp + " "); // Start new line
                    }
                }
                else if (line.Length + word.Length <= MAX_CHAR_LINE)
                    line.Append(word + " "); // Append to existing line
                else
                {
                    result.Add(line.ToString().TrimEnd()); // Record existing line
                    line.Clear(); // Reset line
                    line.Append(string.Empty.PadLeft(numberOfLeadingSpaces)); // Start next line with the leading spaces
                    line.Append(word + " "); // Start new line
                }

                i++;
            }

            // Record final line
            if (line.Length > 0)
                result.Add(line.ToString().TrimEnd());

            return result;
        }

        /// <summary>
        /// For lines that start with either a dash or a number (Description of Changes or Instructions, mostly), 
        /// count the number of leading spaces required for subsequent lines
        /// </summary>
        /// <param name="word">First word of the line</param>
        /// <returns></returns>
        private int GetNumberOfLeadingSpaces(string word)
        {
            int result = 0;
            word = word.Trim();
            if (word.Length == 0) return result;

            // Starts with dash "- "
            if (word.StartsWith("-"))
            {
                result = 1;
                foreach (char letter in word)
                {
                    if (letter == '-')
                        result++;
                    else
                        break;
                }
            }
            // Starts with number "1) "
            else if (int.TryParse(word[0].ToString(), out _))
            {
                result = 2;
                foreach (char letter in word)
                {
                    if (int.TryParse(letter.ToString(), out _))
                        result++;
                    else
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Builds the regular instruction steps
        /// </summary>
        /// <param name="step">Step number</param>
        /// <param name="instructions">Each instruction step, in order that it should be added</param>
        /// <returns></returns>
        private List<string> BuildInstructionSteps(ref int step, params string[] instructions)
        {
            List<string> result = new List<string>();

            foreach (string line in instructions)
            {
                if (string.IsNullOrWhiteSpace(line) == false)
                {
                    result.Add($"{step}) {line}");
                    step++;
                }
            }

            return result;
        }

        #endregion General Methods

        #region Single Patch

        /// <summary>
        /// Generate the Impact text string for a single patch
        /// </summary>
        /// <param name="impactData">Impact data</param>
        /// <param name="allPrograms">List of all programs for the patch</param>
        /// <returns></returns>
        private string GenerateImpactText(ImpactData impactData, List<ProgramData> allPrograms)
        {
            string impact = impactData.AllImpact;

            // If there is a service, use that impact
            if (impactData.HasService)
            {
                impactData.ServiceImpact = GenerateServiceImpactText(impactData.ServiceDoesCheckpoint, allPrograms);

                // If the AllImpact is not the default 'none impact' and is not the same as the service impact,
                // then use AllImpact + service impact
                if (string.IsNullOrEmpty(impact) == false &&
                    string.Compare(impact, Globals.AppParameters.NoneImpact, StringComparison.OrdinalIgnoreCase) != 0 &&
                    string.Compare(impact, impactData.ServiceImpact, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    impact += Environment.NewLine + Environment.NewLine + impactData.ServiceImpact;
                }
                // If the AllImpact is blank or 'no impact', use only service impact
                else
                {
                    impact = impactData.ServiceImpact;
                }
            }

            if (string.IsNullOrEmpty(impact)) impact = Globals.AppParameters.NoneImpact;
            return impact;
        }

        /// <summary>
        /// Generates the Dependencies text string for a single patch
        /// </summary>
        /// <param name="allDependencies">All dependencies string</param>
        /// <param name="dependenciesData">List of DependenciesData</param>
        /// <returns></returns>
        private string GenerateDependenciesText(string allDependencies, List<DependenciesData> dependenciesData)
        {
            string dependencies = string.Empty;
            if (string.IsNullOrEmpty(allDependencies) == false) dependencies = allDependencies + Environment.NewLine + Environment.NewLine;

            // If ANY program has all dependencies, use 'all dependencies' text
            if (dependenciesData.Any(x => x.Type == DependenciesType.All))
            {
                dependencies += Globals.AppParameters.AllDependencies;
            }
            // If ALL programs have none dependencies, use 'none' text (or just allDependencies text if set)
            else if (dependenciesData.All(x => x.Type == DependenciesType.None))
            {
                if (dependencies.Length == 0)
                    dependencies = Globals.AppParameters.NoneDependencies;
            }
            else
            {
                foreach (DependenciesData dep in dependenciesData)
                {
                    if (dep.Type == DependenciesType.Some)
                    {
                        dependencies += dep.Text + Environment.NewLine + Environment.NewLine;
                    }
                }
            }

            if (string.IsNullOrEmpty(dependencies)) dependencies = Globals.AppParameters.NoneDependencies;
            return dependencies;
        }

        #endregion Single Patch

        #region Merged Patches

        /// <summary>
        /// Merges the passed patch notes
        /// </summary>
        /// <param name="patchList">Patches to merge</param>
        /// <param name="program">Name of the program for these patch notes</param>
        /// <param name="allProgramsInMergeDir">All the "final" programs in the merge directory</param>
        /// <returns></returns>
        private string GenerateMergedPatchText(List<PatchData> patchList, string program, List<ProgramData> allProgramsInMergeDir)
        {
            LogManager.WriteToLog(LogLevel.Basic, $"Generating merged patch notes for: {program}.");

            // Sort patches by ticket
            patchList = patchList.OrderBy(x => x.Ticket).ThenBy(x => x.PatchDate).ToList();

            // Remove duplicate programmers, testers and tickets
            List<string> programmers = new List<string>();
            List<string> testers = new List<string>();
            List<string> tickets = new List<string>();
            foreach (PatchData singlePatch in patchList)
            {
                foreach (string programmer in singlePatch.Programmers)
                {
                    if (programmers.Contains(programmer) == false)
                        programmers.Add(programmer);
                }

                foreach (string tester in singlePatch.Testers)
                {
                    if (testers.Contains(tester) == false)
                        testers.Add(tester);
                }

                if (tickets.Contains(singlePatch.Ticket) == false)
                    tickets.Add(singlePatch.Ticket);
            }

            // General section
            StringBuilder patchNotes = new StringBuilder();
            patchNotes.AppendLine(PATCH_SEPARATOR);
            patchNotes.AppendLine($"Date: {DateTime.Today:MMMM d/yyyy}");
            patchNotes.AppendLine($"Programmer: {string.Join(", ", programmers)}");
            patchNotes.AppendLine($"Tester: {string.Join(", ", testers)}");
            patchNotes.AppendLine($"Ticket #: {string.Join(", ", tickets)}");
            patchNotes.AppendLine();

            // Background
            foreach (PatchData singlePatch in patchList)
            {
                if (patchList.Count == 1) patchNotes.AppendLine("Background:");
                else patchNotes.AppendLine($"Background ({singlePatch.ShortDescription}):");
                foreach (string line in DoLineBreaks(singlePatch.Background.Trim()))
                {
                    patchNotes.AppendLine(line);
                }
                patchNotes.AppendLine();
            }

            // Impact
            string impact = GenerateImpactTextMerged(program, patchList, allProgramsInMergeDir);
            foreach (string line in DoLineBreaks(impact.Trim()))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            // Dependencies
            string dependencies = GenerateDependenciesTextMerged(program, patchList, allProgramsInMergeDir);
            foreach (string line in DoLineBreaks(dependencies.Trim()))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            // Description of changes
            foreach (PatchData singlePatch in patchList)
            {
                if (patchList.Count == 1) patchNotes.AppendLine("Description of Changes:");
                else patchNotes.AppendLine($"Description of Changes ({singlePatch.ShortDescription}):");
                foreach (string line in DoLineBreaks(singlePatch.DescriptionOfChanges.Trim()))
                {
                    patchNotes.AppendLine(line);
                }
                patchNotes.AppendLine();
            }

            // Get the most recent ProgramData values (from the merge folder) for each unique program used in these patch notes
            List<ProgramData> programsUsed = new List<ProgramData>();
            foreach (PatchData singlePatch in patchList)
            {
                foreach (ProgramData patchProgram in singlePatch.ProgramsUsed)
                {
                    string currentPatchProgramName = Globals.GetAndroidName(patchProgram.ProgramName, true);
                    bool programFound = false;
                    foreach (ProgramData exportProgram in allProgramsInMergeDir)
                    {
                        string currentExportProgramName = Globals.GetAndroidName(exportProgram.ProgramName, true);
                        if (string.Compare(currentPatchProgramName, currentExportProgramName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            programFound = true;
                            if (programsUsed.Contains(exportProgram) == false)
                                programsUsed.Add(exportProgram);
                            break;
                        }
                    }

                    // Fallback to using the data from the original patches if can't find in merge directory
                    if (programFound == false)
                        programsUsed.Add(patchProgram);
                }
            }
            programsUsed = programsUsed.OrderBy(x => x.ProgramName).ToList();

            // Instructions
            List<string> programNames = new List<string>();
            foreach (ProgramData prog in programsUsed)
            {
                if (programNames.Contains(prog.ProgramName) == false)
                    programNames.Add(prog.ProgramName);
            }
            string instructions = GenerateInstructions(programNames);
            patchNotes.AppendLine("Instructions:");
            foreach (string line in DoLineBreaks(instructions))
            {
                patchNotes.AppendLine(line);
            }
            patchNotes.AppendLine();

            // Programs used
            List<string> formattedProgramsUsed = new List<string>();
            foreach (ProgramData programInUse in programsUsed)
            {
                formattedProgramsUsed.Add(GenerateProgramsUsedText(programInUse));
            }
            patchNotes.AppendLine("Programs Used:");
            patchNotes.AppendLine(string.Join(Environment.NewLine, formattedProgramsUsed));
            patchNotes.AppendLine();
            patchNotes.AppendLine(PATCH_SEPARATOR);
            patchNotes.AppendLine(LAST_LINE);

            return patchNotes.ToString();
        }

        /// <summary>
        /// Generate Impact string for merged patches
        /// </summary>
        /// <param name="program">Program name</param>
        /// <param name="patchList">List of PatchData</param>
        /// <param name="allProgramsInMergeDir">List of all programs being patched out</param>
        /// <returns></returns>
        private string GenerateImpactTextMerged(string program, List<PatchData> patchList, List<ProgramData> allProgramsInMergeDir)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Generating merged impact for: {program}.");

            StringBuilder impactStr = new StringBuilder();
            Dictionary<string, List<string>> impactList = new Dictionary<string, List<string>>(); // <Impact, List of ShortDescriptions>
            List<string> allList = new List<string> { "All" };

            // Service: generate one Service impact for all of them
            if (Globals.IsService(program))
            {
                // If one does a checkpoint, they all do a checkpoint
                bool checkpoint = patchList.Any(x => x.Impact.ServiceDoesCheckpoint);
                string cpImpact = GenerateServiceImpactText(checkpoint, allProgramsInMergeDir);
                impactList.Add(cpImpact, allList);

                if (checkpoint) LogManager.WriteToLog(LogLevel.Everything, "Service generates checkpoint.");
                else LogManager.WriteToLog(LogLevel.Everything, "Service does not generate checkpoint.");
            }

            // Get impact from each patch
            foreach (PatchData singlePatch in patchList)
            {
                string impact = singlePatch.Impact.AllImpact;
                if (string.IsNullOrEmpty(impact) == false &&
                    string.Compare(Globals.AppParameters.NoneImpact, impact, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    if (impactList.ContainsKey(impact) == false)
                    {
                        impactList.Add(impact, new List<string>() { singlePatch.ShortDescription });
                    }
                    else
                    {
                        List<string> tmp = impactList[impact];
                        tmp.Add(singlePatch.ShortDescription);
                        impactList[impact] = tmp;
                    }

                    LogManager.WriteToLog(LogLevel.Everything, $"Found impact '{impact}' from ticket: {singlePatch.Ticket} ({singlePatch.ShortDescription}).");
                }
            }

            // Generate final string
            if (impactList.Count == 0)
            {
                // Add default 'none' impact if there were no other impacts
                impactList.Add(Globals.AppParameters.NoneImpact, allList);
                LogManager.WriteToLog(LogLevel.Everything, "Using default 'none' impact.");
            }

            if (patchList.Count == 1 || (impactList.Count == 1 && impactList.ContainsValue(allList)))
            {
                impactStr.AppendLine("Impact:");
                foreach (string imp in impactList.Keys)
                {
                    impactStr.AppendLine(imp);
                }
                impactStr.AppendLine();
            }
            else
            {
                foreach (KeyValuePair<string, List<string>> impact in impactList)
                {
                    impactStr.AppendLine($"Impact ({string.Join(", ", impact.Value)}):");
                    impactStr.AppendLine(impact.Key);
                    impactStr.AppendLine();
                }
            }

            return impactStr.ToString();
        }

        /// <summary>
        /// Generate Dependencies string for merged patches
        /// </summary>
        /// <param name="program">Program name</param>
        /// <param name="patchList">List of PatchData</param>
        /// <returns></returns>
        private string GenerateDependenciesTextMerged(string program, List<PatchData> patchList, List<ProgramData> allProgramsInMergeDir)
        {
            LogManager.WriteToLog(LogLevel.Everything, $"Generating merged dependencies for: {program}.");

            DependenciesData finalDepData = new DependenciesData()
            {
                ProgramName = program,
                Type = DependenciesType.Some
            };

            string compareProgramName = Globals.GetAndroidName(program, true);

            // Use parameters to determine the dependencies type for this program
            if (Globals.AppParameters.AllDependenciesPrograms.Contains(compareProgramName))
                finalDepData.Type = DependenciesType.All;
            else if (Globals.AppParameters.NoneDependenciesPrograms.Contains(compareProgramName))
                finalDepData.Type = DependenciesType.None;

            // If it was not in the parameters, check the dependencies for this program for each patch and see if they are all 'All' or 'None'
            if (finalDepData.Type == DependenciesType.Some)
            {
                if (patchList.All(p => p.Dependencies.Where(d => string.Compare(Globals.GetAndroidName(d.ProgramName, true), compareProgramName, StringComparison.OrdinalIgnoreCase) == 0).All(d => d.Type == DependenciesType.All)))
                    finalDepData.Type = DependenciesType.All;
                else if (patchList.All(p => p.Dependencies.Where(d => string.Compare(Globals.GetAndroidName(d.ProgramName, true), compareProgramName, StringComparison.OrdinalIgnoreCase) == 0).All(d => d.Type == DependenciesType.None)))
                    finalDepData.Type = DependenciesType.None;
            }

            LogManager.WriteToLog(LogLevel.Everything, $"Using dependencies type: {finalDepData.Type}.");

            // If it's still 'Some', then add all the required & previous programs for each patch
            if (finalDepData.Type == DependenciesType.Some)
            {
                // List of all Programs Used for this patch
                List<string> allProgramsUsed = new List<string>();
                patchList.ForEach(patch => patch.ProgramsUsed.ToList().ForEach(prog => allProgramsUsed.Add(Globals.GetAndroidName(prog.ProgramName, true))));

                foreach (PatchData singlePatch in patchList)
                {
                    DependenciesData? singleDepData = singlePatch.Dependencies.FirstOrDefault(x => string.Compare(compareProgramName, Globals.GetAndroidName(x.ProgramName, true), StringComparison.OrdinalIgnoreCase) == 0);
                    if (singleDepData != null && singleDepData.ProgramDependencies != null)
                    {
                        foreach (DependencyProgram depProg in singleDepData.ProgramDependencies)
                        {
                            if (depProg.Status != DependencyStatus.NotRequired)
                            {
                                // Check if program is already in the list with a different status: 'Required' status wins
                                // *Only update if the program is also in the list of programs being patched out for this program
                                var progFromList = finalDepData.ProgramDependencies.FirstOrDefault(x => string.Compare(Globals.GetAndroidName(depProg.ProgramName, true), Globals.GetAndroidName(x.ProgramName, true), StringComparison.OrdinalIgnoreCase) == 0);
                                if (progFromList != null && depProg.Status != progFromList.Status && progFromList.Status == DependencyStatus.Required &&
                                    allProgramsUsed.Contains(Globals.GetAndroidName(depProg.ProgramName, true)))
                                {
                                    LogManager.WriteToLog(LogLevel.Everything, $"Changing status of '{depProg.ProgramName}' from '{depProg.Status}' to '{progFromList.Status}'.");
                                    depProg.Status = progFromList.Status;
                                    finalDepData.ProgramDependencies.Remove(progFromList);
                                }
                                else if (progFromList != null) continue;

                                // Update program version from allProgramsInMergeDir list
                                // *Only update if the program is also in the list of programs being patched out for this program
                                var progFromMerge = allProgramsInMergeDir.FirstOrDefault(x => string.Compare(Globals.GetAndroidName(depProg.ProgramName, true), Globals.GetAndroidName(x.ProgramName, true), StringComparison.OrdinalIgnoreCase) == 0);
                                if (progFromMerge != null && allProgramsUsed.Contains(Globals.GetAndroidName(depProg.ProgramName, true)))
                                {
                                    LogManager.WriteToLog(LogLevel.Everything, $"Found '{depProg.ProgramName}' in merge folder, updated version from '{depProg.Version}' to '{progFromMerge.VersionString}'.");
                                    depProg.Version = progFromMerge.VersionString;
                                }

                                finalDepData.ProgramDependencies.Add(depProg);
                                LogManager.WriteToLog(LogLevel.Everything, $"Added program '{depProg.ProgramName}' with version '{depProg.Version}' and status '{depProg.Status}'.");

                            }
                        }
                    }
                }
            }

            // Check if any patches have AllDependencies
            Dictionary<string, List<string>> allDependenciesList = new Dictionary<string, List<string>>();
            foreach (PatchData singlePatch in patchList)
            {
                if (string.IsNullOrEmpty(singlePatch.AllDependencies) == false)
                {
                    if (allDependenciesList.ContainsKey(singlePatch.AllDependencies))
                    {
                        List<string> tmp = allDependenciesList[singlePatch.AllDependencies];
                        tmp.Add(singlePatch.ShortDescription);
                        allDependenciesList[singlePatch.AllDependencies] = tmp;
                    }
                    else
                    {
                        allDependenciesList.Add(singlePatch.AllDependencies, new List<string>() { singlePatch.ShortDescription });
                    }

                    LogManager.WriteToLog(LogLevel.Everything, $"Found all dependencies '{singlePatch.AllDependencies}' from ticket: {singlePatch.Ticket} ({singlePatch.ShortDescription}).");
                }
            }

            // Generate final string
            StringBuilder str = new StringBuilder();
            finalDepData.GenerateText();
            if (allDependenciesList.Count > 0) str.AppendLine("Dependencies (All):");
            else str.AppendLine("Dependencies:");
            str.AppendLine(finalDepData.Text);
            str.AppendLine();

            if (allDependenciesList.Count > 0)
            {
                foreach (var allDep in allDependenciesList)
                {
                    str.AppendLine($"Dependencies ({string.Join(", ", allDep.Value)}):");
                    str.AppendLine(allDep.Key);
                    str.AppendLine();
                }

                str.AppendLine();
            }

            return str.ToString();
        }

        #endregion Merged Patches

        #region General

        /// <summary>
        /// Builds the line of instructions from the list of programs and the folder name
        /// </summary>
        /// <param name="programsList">List of programs</param>
        /// <param name="directoryName">Name of the directory that the instructions will indicate to copy to</param>
        /// <returns></returns>
        private string GenerateInstructionLine(List<string> programsList, string directoryName)
        {
            if (programsList.Count == 0) return string.Empty;

            return $"Copy the {Globals.BuildFormattedList(programsList)} to the {directoryName} directory.";
        }

        /// <summary>
        /// Builds the Android instructions
        /// </summary>
        /// <param name="androidPrograms">List of Android apks</param>
        /// <param name="step">Step number</param>
        /// <returns></returns>
        private List<string> BuildAndroidInstructionSteps(List<string> androidPrograms, ref int step)
        {
            List<string> result = new List<string>();
            string formattedAndroidList = Globals.BuildFormattedList(androidPrograms);

            result.Add($@"{step}) Copy the {formattedAndroidList} to the Install\Android directory.");
            step++;
            result.Add($"{step}) Copy the .apk file to the root folder of the Android Device.");
            step++;
            result.Add($"{step}) Navigate to the home screen on the Android device.");
            step++;
            result.Add($"{step}) Tap the menu button and find File Browser in the list of applications installed on the device.");
            step++;
            result.Add($"{step}) Open File Browser on the Android Device.");
            step++;
            result.Add($"{step}) Tap the .apk file then tap the Install button.");
            step++;

            return result;
        }

        /// <summary>
        /// Generates the impact for the service
        /// </summary>
        /// <param name="serviceDoesCp">Does the service create a Cp?</param>
        /// <param name="programsUsed">Programs used for the patch notes (should contain service)</param>
        private string GenerateServiceImpactText(bool serviceDoesCp, List<ProgramData> programsUsed)
        {
            if (serviceDoesCp)
            {
                // Get the version of the service that will generate the Cp
                ProgramData? serviceProgram = programsUsed.FirstOrDefault(x => Globals.IsService(x.ProgramName));
                if (serviceProgram == null)
                {
                    string message = "GenerateServiceImpact: Could not find service getting patched out in directory to generate patch impact. " +
                        "Using 'none' impact.";
                    LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                    MessageBox.Show(message);
                    return Globals.AppParameters.NoneImpact;
                }

                // Generate the string with that service version
                return string.Format(Globals.AppParameters.CheckpointImpact, serviceProgram.VersionString);
            }
            else
            {
                // Get previous impact from already patched service
                string fileName = GetReleasedPatchNotesFileName("Service.exe", GetCurrentVersionString(programsUsed));
                string prevImpact = string.Empty;
                if (string.IsNullOrEmpty(fileName) == false)
                    prevImpact = GetSectionFromReleasedPatches("Service.exe", fileName, Section.Impact);

                if (string.IsNullOrEmpty(prevImpact))
                {
                    string message = "GenerateServiceImpact: Could not get impact for service from released patch notes. " +
                        "Using 'none' impact.";
                    LogManager.WriteToLog(LogLevel.Basic, $"MessageBox: {message}.");
                    MessageBox.Show(message);
                    return Globals.AppParameters.NoneImpact;
                }
                else
                    return prevImpact;
            }
        }

        /// <summary>
        /// Formatted string for Programs Used
        /// </summary>
        public string GenerateProgramsUsedText(ProgramData programUsed)
        {
            if (string.IsNullOrEmpty(programUsed.VersionString))
            {
                return $"{programUsed.ProgramName}, created {programUsed.CreatedDate:MM/dd/yyyy h:mmtt}, size {programUsed.Size}KB";
            }
            else
            {
                return $"{programUsed.ProgramName}, created {programUsed.CreatedDate:MM/dd/yyyy h:mmtt}, size {programUsed.Size}KB, version {programUsed.VersionString}";
            }
        }

        #endregion General
    }
}
