using System.Diagnostics;
using System.Linq;
using System.Threading;
using CmdOneLinerNET;

namespace System.IO.NG
{
    public static class DirectoryNG
    {
        public static void Move(string sourceFileName, string destFileName, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix) FileNG.Move(sourceFileName, destFileName, false);
            else Directory.Move(sourceFileName, destFileName);
        }

        public static bool Exists(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"test -d \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                return Success;
            }
            else return Directory.Exists(path);
        }

        public static DirectoryInfo CreateDirectory(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"mkdir -p \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return new DirectoryInfo(path);
                else throw new IOException($"Unable to create directory '{path}': {StdErr}");
            }
            else return Directory.CreateDirectory(path);
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\"{(searchOption == SearchOption.TopDirectoryOnly ? " -maxdepth 1" : "")} -type f -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return _parseLinuxFindOutput(path, StdOut);
                else throw new IOException($"Unable to list files in directory '{path}': {StdErr}");
            }
            else return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public static string[] GetFiles(string path, string searchPattern, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type f -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return _parseLinuxFindOutput(path, StdOut);
                else throw new IOException($"Unable to list files in directory '{path}': {StdErr}");
            }
            else return Directory.GetFiles(path, searchPattern);
        }

        public static string[] GetFiles(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type f", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return _parseLinuxFindOutput(path, StdOut);
                else throw new IOException($"Unable to list files in directory '{path}': {StdErr}");
            }
            else return Directory.GetFiles(path);
        }

        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\"{(searchOption == SearchOption.TopDirectoryOnly ? " -maxdepth 1" : "")} -type f -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return _parseLinuxFindOutput(path, StdOut);
                else throw new IOException($"Unable to list subdirectories of '{path}': {StdErr}");
            }
            else return Directory.GetDirectories(path, searchPattern, searchOption);
        }

        public static string[] GetDirectories(string path, string searchPattern, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type d -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return _parseLinuxFindOutput(path, StdOut);
                else throw new IOException($"Unable to list subdirectories of '{path}': {StdErr}");
            }
            else return Directory.GetDirectories(path, searchPattern);
        }

        public static string[] GetDirectories(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type d", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (Success) return _parseLinuxFindOutput(path, StdOut);
                else throw new IOException($"Unable to list subdirectories of '{path}': {StdErr}");
            }
            else return Directory.GetDirectories(path);
        }

        private static string[] _parseLinuxFindOutput(string searchedPath, string stdOut)
        {
            string[] lines = stdOut.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++) lines[i] = Path.GetFullPath(lines[i]); // convert relative to absolute paths
            lines = lines.Except(new[] { Path.GetFullPath(searchedPath) }).ToArray();
            return lines;
        }

        public static void DeleteAllContents(string pathDir, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            foreach (string file in GetFiles(pathDir, iopriority, timeout - sw.Elapsed, canceltoken)) FileNG.Delete(file, iopriority, timeout - sw.Elapsed, canceltoken);
            foreach (string dir in GetDirectories(pathDir, iopriority, timeout - sw.Elapsed, canceltoken)) Delete(dir, iopriority, timeout - sw.Elapsed, canceltoken);
        }

        public static void Delete(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"rmdir \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (!Success) throw new IOException($"Unable to delete directory '{path}': {StdErr}");
            }
            else Directory.Delete(path);
        }

        public static void Delete(string path, bool recursive, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long MaxRamUsedBytes, TimeSpan UserProcessorTime, TimeSpan TotalProcessorTime) = CmdOneLiner.Run($"rm {(recursive ? "-r" : "")} \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority);
                if (!Success) throw new IOException($"Unable to delete directory '{path}': {StdErr}");
            }
            else Directory.Delete(path, recursive);
        }
    }
}
