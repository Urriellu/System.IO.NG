using CmdOneLinerNET;
using System;
using System.Diagnostics;
using System.Threading;

namespace System.IO.NG
{
    public static class FileNG
    {
        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is not allowed.
        /// </summary>
        /// <param name="sourceFileName">The file to copy.</param>
        /// <param name="destFileName"></param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false</param>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite = false, TimeSpan? timeout = null, CancellationToken? canceltoken = null, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"cp --no-dereference --preserve=links --preserve=all \"{sourceFileName}\" \"{destFileName}\"", Environment.CurrentDirectory, timeout, canceltoken, ProcessPriorityClass.Normal, iopriority);
                if (!Success) throw new Exception($"Unable to copy file '{sourceFileName}' to '{destFileName}': {StdErr}");
            }
            else File.Copy(sourceFileName, destFileName, overwrite);
        }
    }
}
