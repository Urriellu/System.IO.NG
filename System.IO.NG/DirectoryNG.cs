using System.Diagnostics;
using System.Linq;
using System.Threading;
using CmdOneLinerNET;
using Humanizer.Bytes;

namespace System.IO.NG
{
    public static class DirectoryNG
    {
        public static void Move(string sourceFileName, string destFileName, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (sourceFileName.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {sourceFileName}");
            if (destFileName.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {destFileName}");
            if (Environment.OSVersion.Platform == PlatformID.Unix) FileNG.Move(sourceFileName, destFileName, false);
            else Directory.Move(sourceFileName, destFileName);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static bool Exists(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"test -d \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    return Success;
                }
                else return Directory.Exists(path);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static DirectoryInfo CreateDirectory(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"mkdir -p \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return new DirectoryInfo(path);
                    else throw new IOException($"Unable to create directory '{path}': {StdErr}");
                }
                else return Directory.CreateDirectory(path);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\"{(searchOption == SearchOption.TopDirectoryOnly ? " -maxdepth 1" : "")} -type f -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return _parseLinuxFindOutput(path, StdOut);
                    else throw new IOException($"Unable to list files in directory '{path}': {StdErr}");
                }
                else return Directory.GetFiles(path, searchPattern, searchOption);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static string[] GetFiles(string path, string searchPattern, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type f -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return _parseLinuxFindOutput(path, StdOut);
                    else throw new IOException($"Unable to list files in directory '{path}': {StdErr}");
                }
                else return Directory.GetFiles(path, searchPattern);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static string[] GetFiles(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type f", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return _parseLinuxFindOutput(path, StdOut);
                    else throw new IOException($"Unable to list files in directory '{path}': {StdErr}");
                }
                else return Directory.GetFiles(path);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\"{(searchOption == SearchOption.TopDirectoryOnly ? " -maxdepth 1" : "")} -type d -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return _parseLinuxFindOutput(path, StdOut);
                    else throw new IOException($"Unable to list subdirectories of '{path}': {StdErr}");
                }
                else return Directory.GetDirectories(path, searchPattern, searchOption);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static string[] GetDirectories(string path, string searchPattern, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type d -name \"{searchPattern}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return _parseLinuxFindOutput(path, StdOut);
                    else throw new IOException($"Unable to list subdirectories of '{path}': {StdErr}");
                }
                else return Directory.GetDirectories(path, searchPattern);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static string[] GetDirectories(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"find \"{path}\" -maxdepth 1 -type d", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success) return _parseLinuxFindOutput(path, StdOut);
                    else throw new IOException($"Unable to list subdirectories of '{path}': {StdErr}");
                }
                else return Directory.GetDirectories(path);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
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
            if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
            if (pathDir.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {pathDir}");
            foreach (string file in GetFiles(pathDir, iopriority, timeout - sw.Elapsed, canceltoken)) FileNG.Delete(file, iopriority, timeout - sw.Elapsed, canceltoken);
            foreach (string dir in GetDirectories(pathDir, iopriority, timeout - sw.Elapsed, canceltoken)) Delete(dir, iopriority, timeout - sw.Elapsed, canceltoken);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static void Delete(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
            if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"rmdir \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                if (!Success) throw new IOException($"Unable to delete directory '{path}': {StdErr}");
            }
            else Directory.Delete(path);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static void Delete(string path, bool recursive, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = FileNG.DefaultTimeout;
            if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                if (recursive)
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"rm {(recursive ? "-r" : "")} -f \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (!Success) throw new IOException($"Unable to delete directory '{path}': {StdErr}");
                }
                else Delete(path, iopriority, timeout, canceltoken);
            }
            else Directory.Delete(path, recursive);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static ByteSize GetSize(string pathDirectory) => GetSize(new DirectoryInfo(pathDirectory));

        public static ByteSize GetSize(DirectoryInfo d)
        {
            ByteSize size = ByteSize.FromBytes(0);

            // Calculate file sizes
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                try { size += fi.GetSize(); }
                catch { }
            }

            // Calculate subdirectory sizes
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis) size += GetSize(di);
            return size;
        }
    }
}
