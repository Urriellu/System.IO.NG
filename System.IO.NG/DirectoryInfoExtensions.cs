using System.Linq;
using System.Threading;

namespace System.IO.NG
{
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Delete all files and subdirectories, but keep the directory itself, emptied.
        /// </summary>
        /// <param name="di"></param>
        public static void DeleteAllContents(this DirectoryInfo di, IOPriorityClass iopriority) => DirectoryNG.Delete(di.FullName, recursive: true, iopriority: iopriority);

        public static void Delete(this DirectoryInfo di, bool recursive = false, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => DirectoryNG.Delete(di.FullName, recursive, iopriority, timeout, canceltoken);

        public static DirectoryInfo[] GetDirectories(this DirectoryInfo di, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => DirectoryNG.GetDirectories(di.FullName, iopriority, timeout, canceltoken).Select(d => new DirectoryInfo(d)).ToArray();

        public static bool Exists(this DirectoryInfo di, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => DirectoryNG.Exists(di.FullName, iopriority, timeout, canceltoken);
    }
}
