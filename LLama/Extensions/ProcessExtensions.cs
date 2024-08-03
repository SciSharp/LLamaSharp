using System.Diagnostics;
using System.Text;
using System;

namespace LLama.Extensions;

internal static class ProcessExtensions
{
    public static void SafeKill(this Process process, bool entireProcessTree = true)
    {
        if (process.HasExited)
            return;

        // There's a race here! If the process closed between the above check
        // and the below `Kill` call then an `InvalidOperationException` will
        // be thrown! Catch it and move on.

        try
        {
#if NET5_0_OR_GREATER
            process.Kill(entireProcessTree);
#else
            process.Kill();
#endif
            process.WaitForExit(55);
        }
        catch (InvalidOperationException)
        {
        }
    }

    /// <summary>
    /// Run a process for a certain amount of time and then terminate it
    /// </summary>
    /// <param name="process"></param>
    /// <param name="timeout"></param>
    /// <returns>return code, standard output, standard error, flag indicating if process exited or was terminated</returns>
    public static (int ret, string stdOut, string stdErr, bool ok) SafeRun(this Process process, TimeSpan timeout)
    {
        var stdOut = new StringBuilder();
        process.OutputDataReceived += (s, e) =>
        {
            stdOut.Append(e.Data);
        };

        var stdErr = new StringBuilder();
        process.ErrorDataReceived += (s, e) =>
        {
            stdErr.Append(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var ok = process.WaitForExit((int)timeout.TotalMilliseconds);
        process.SafeKill();

        return (process.ExitCode, stdOut.ToString(), stdErr.ToString(), ok);
    }
}