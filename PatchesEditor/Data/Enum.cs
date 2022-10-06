namespace PatchesEditor.Data
{
    /// <summary>
    /// Section of the patch notes
    /// </summary>
    public enum Section
    {
        /// <summary>
        /// General section (Date, Programmer, Tester, Ticket)
        /// </summary>
        General,

        /// <summary>
        /// Background
        /// </summary>
        Background,

        /// <summary>
        /// Impact
        /// </summary>
        Impact,

        /// <summary>
        /// Dependencies
        /// </summary>
        Dependencies,

        /// <summary>
        /// Description of Changes
        /// </summary>
        DescriptionOfChanges,

        /// <summary>
        /// Instructions
        /// </summary>
        Instructions,

        /// <summary>
        /// Programs Used
        /// </summary>
        ProgramsUsed
    }

    /// <summary>
    /// Type of additional reference
    /// </summary>
    public enum ReferenceType
    {
        /// <summary>
        /// Reference Number
        /// </summary>
        Reference,

        /// <summary>
        /// Quote Number
        /// </summary>
        Quote,

        /// <summary>
        /// Defect Number
        /// </summary>
        Defect
    }

    /// <summary>
    /// Application startup type
    /// </summary>
    public enum StartupType
    {
        /// <summary>
        /// Start new
        /// </summary>
        New,

        /// <summary>
        /// Open existing
        /// </summary>
        Open,

        /// <summary>
        /// Import from existing
        /// </summary>
        Import,

        /// <summary>
        /// Start merge
        /// </summary>
        Merge,

        /// <summary>
        /// Do nothing, just go to main page
        /// </summary>
        Nothing
    }

    /// <summary>
    /// Dependencies type for a patched program
    /// </summary>
    public enum DependenciesType
    {
        /// <summary>
        /// All dependencies
        /// </summary>
        All,

        /// <summary>
        /// Some dependencies
        /// </summary>
        Some,

        /// <summary>
        /// No dependencies
        /// </summary>
        None
    }

    /// <summary>
    /// Status of a dependency program
    /// </summary>
    public enum DependencyStatus
    {
        /// <summary>
        /// Dependency required for current patch
        /// </summary>
        Required,

        /// <summary>
        /// Dependency required for previous patch
        /// </summary>
        Previous,

        /// <summary>
        /// Dependency not required
        /// </summary>
        NotRequired
    }

    /// <summary>
    /// Log level
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Basic log level
        /// </summary>
        Basic,

        /// <summary>
        /// Log everything
        /// </summary>
        Everything
    }

    /// <summary>
    /// Parameter type used by the Parameters screen to know which param to delete from
    /// </summary>
    public enum ParameterType
    {
        Programmer,
        Tester,
        IgnoredExtensions,
        ProgramsWithResources,
        IgnoreCopyright,
        ResourceDir,
        AllDependencies,
        NoneDependencies
    }
}
