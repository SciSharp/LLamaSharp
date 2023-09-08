namespace LLama.Native
{   
     /// <summary>
     /// Severity level of a log message
     /// </summary>
    public enum LLamaLogLevel
    {
        /// <summary>
        /// Logs that are used for interactive investigation during development.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due to a failure.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the application flow, but do not otherwise cause the application execution to stop.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Logs that track the general flow of the application.
        /// </summary>
        Info = 4
    }
}
