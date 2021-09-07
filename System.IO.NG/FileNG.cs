using CmdOneLinerNET;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="iopriority"></param>
        /// <param name="timeout"></param>
        /// <param name="canceltoken"></param>
        public static void Copy(string sourceFileName, string destFileName, bool overwrite = false, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"cp --no-dereference --preserve=links --preserve=all \"{sourceFileName}\" \"{destFileName}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (!Success) throw new IOException($"Unable to copy file '{sourceFileName}' to '{destFileName}': {StdErr}");
            }
            else File.Copy(sourceFileName, destFileName, overwrite);
        }

        public static void Move(string sourceFileName, string destFileName, bool overwrite = false, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"mv {(overwrite ? "-f" : "-n")} \"{sourceFileName}\" \"{destFileName}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (!Success) throw new IOException($"Unable to move file '{sourceFileName}' to '{destFileName}': {StdErr}");
            }
            else File.Move(sourceFileName, destFileName /*, overwrite*/); // https://docs.microsoft.com/en-us/dotnet/api/system.io.file.move Why is this overload not supported?
        }

        public static void Delete(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"rm \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (!Success) throw new IOException($"Unable to delete file '{path}': {StdErr}");
            }
            else File.Delete(path);
        }

        public static bool Exists(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"test -f \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                return Success;
            }
            else return File.Exists(path);
        }

        public static void WriteAllText(string path, string contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"tee \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, StdIn: contents);
                if (!Success) throw new IOException($"Error while writing contents to '{path}': {StdErr}");
            }
            else File.WriteAllText(path, contents);
        }


        public static void WriteAllLines(string path, string[] contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => WriteAllText(path, string.Join(Environment.NewLine, contents), iopriority, timeout, canceltoken);

        public static void AppendAllText(string path, string contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"tee -a \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, StdIn: contents);
                if (!Success) throw new IOException($"Error while writing contents to '{path}': {StdErr}");
            }
            else File.AppendAllText(path, contents);
        }

        public static void AppendAllLines(string path, string[] lines, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => AppendAllText(path, Environment.NewLine + string.Join(Environment.NewLine, lines), iopriority, timeout, canceltoken);

        public static string ReadAllText(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"cat \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success)
                {
                    if (StdOut.EndsWith('\r') || StdOut.EndsWith('\n')) return StdOut.Substring(0, StdOut.Length - 1); // since CmdOneLiner reads command stdout line by line, it always adds an extra EOL
                    else return StdOut;
                }
                else throw new IOException($"Error while reading contents of '{path}': {StdErr}");
            }
            else return File.ReadAllText(path);
        }

        public static string[] ReadAllLines(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            string content = ReadAllText(path, iopriority, timeout, canceltoken);
            string[] lines = content.Replace("\r\n", "\n").Replace("\n\r", "\n").Split('\r', '\n');
            return lines;
        }

        public static async Task<string> ReadAllTextAsync(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => await Task.Factory.StartNew(() => ReadAllText(path, iopriority, timeout, canceltoken));

        public static async Task WriteAllTextAsync(string path, string contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => await Task.Factory.StartNew(() => WriteAllText(path, contents, iopriority, timeout, canceltoken));
    }
}
