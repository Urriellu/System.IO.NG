using CmdOneLinerNET;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.NG
{
    public static class FileNG
    {
        public static TimeSpan DefaultTimeout = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same name is not allowed.
        /// </summary>
        /// <param name="pathSrcFile">The file to copy.</param>
        /// <param name="pathDstFile"></param>
        /// <param name="overwrite">true if the destination file can be overwritten; otherwise, false</param>
        /// <param name="iopriority"></param>
        /// <param name="timeout"></param>
        /// <param name="canceltoken"></param>
        public static void Copy(string pathSrcFile, string pathDstFile, bool overwrite = false, bool createDstDirectory = false, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = DefaultTimeout;
            if (pathSrcFile.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {pathSrcFile}");
            if (pathDstFile.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {pathDstFile}");
            string pathDstDir = Path.GetDirectoryName(pathDstFile);
            if (!DirectoryNG.Exists(pathDstDir, iopriority)) DirectoryNG.CreateDirectory(pathDstDir, iopriority, timeout, canceltoken);
            if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"cp -rP \"{pathSrcFile}\" \"{pathDstFile}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                if (!Success && canceltoken?.IsCancellationRequested == false) throw new IOException($"Unable to copy file '{pathSrcFile}' to '{pathDstFile}': {StdErr}");
            }
            else File.Copy(pathSrcFile, pathDstFile, overwrite);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static void Move(string sourceFileName, string destFileName, bool overwrite = false, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = DefaultTimeout;
            if (sourceFileName.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {sourceFileName}");
            if (destFileName.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {destFileName}");
            if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"mv {(overwrite ? "-f" : "-n")} \"{sourceFileName}\" \"{destFileName}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                if (!Success) throw new IOException($"Unable to move file '{sourceFileName}' to '{destFileName}': {StdErr}");
            }
            else File.Move(sourceFileName, destFileName /*, overwrite*/); // https://docs.microsoft.com/en-us/dotnet/api/system.io.file.move Why is this overload not supported?
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static void Delete(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = DefaultTimeout;
            if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
            if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"rm \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                if (!Success) throw new IOException($"Unable to delete file '{path}': {StdErr}");
            }
            else File.Delete(path);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static bool Exists(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
                {
                    // we need to do 'test' twice because 'test -f' would return false on non-regular files, so we use 'test -e' to see if it exists (either as regular, non-regular, or directory) and then exclude directories
                    var cmdIsDirectory = CmdOneLiner.Run($"test -d \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    var cmdIsAnyKindOfFileOrDirectoryOrSpecial = CmdOneLiner.Run($"test -e \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    return !cmdIsDirectory.Success && cmdIsAnyKindOfFileOrDirectoryOrSpecial.Success;
                }
                else return File.Exists(path);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
        }

        public static void WriteAllText(string path, string contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = DefaultTimeout;
            if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
            if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
            {
                if (path.Contains("\\") || path.Contains("\"")) throw new System.NotImplementedException($"Certain characters not supported.");
                (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"tee \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true, StdIn: contents);
                if (!Success) throw new IOException($"Error while writing contents to '{path}': {StdErr}");
            }
            else File.WriteAllText(path, contents);
            StorageNG.RecordStatistics(sw.Elapsed);
        }


        public static void WriteAllLines(string path, string[] contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => WriteAllText(path, string.Join(Environment.NewLine, contents), iopriority, timeout, canceltoken);

        public static void AppendAllText(string path, string contents, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            if (!timeout.HasValue) timeout = DefaultTimeout;
            if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
            if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
            {
                (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"tee -a \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true, StdIn: contents);
                if (!Success) throw new IOException($"Error while writing contents to '{path}': {StdErr}");
            }
            else File.AppendAllText(path, contents);
            StorageNG.RecordStatistics(sw.Elapsed);
        }

        public static void AppendAllLines(string path, string[] lines, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => AppendAllText(path, Environment.NewLine + string.Join(Environment.NewLine, lines), iopriority, timeout, canceltoken);

        public static string ReadAllText(string path, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                if (!timeout.HasValue) timeout = DefaultTimeout;
                if (path.Length > 255) throw new ArgumentOutOfRangeException($"Path too long: {path}");
                if (Environment.OSVersion.Platform == PlatformID.Unix && (iopriority != IOPriorityClass.L02_NormalEffort || Thread.CurrentThread.Priority != ThreadPriority.Normal || StorageNG.ProcessPriority != ProcessPriorityClass.Normal))
                {
                    (int ExitCode, bool Success, string StdOut, string StdErr, long? MaxRamUsedBytes, TimeSpan? UserProcessorTime, TimeSpan? TotalProcessorTime) = CmdOneLiner.Run($"cat \"{path}\"", Environment.CurrentDirectory, timeout, canceltoken, iopriority.GetSimilarProcessPriority(), iopriority, ignoreStatistics: true);
                    if (Success)
                    {
                        if (StdOut.EndsWith('\r') || StdOut.EndsWith('\n')) return StdOut.Substring(0, StdOut.Length - 1); // since CmdOneLiner reads command stdout line by line, it always adds an extra EOL
                        else return StdOut;
                    }
                    else throw new IOException($"Error while reading contents of '{path}': {StdErr}");
                }
                else return File.ReadAllText(path);
            }
            finally
            {
                StorageNG.RecordStatistics(sw.Elapsed);
            }
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
