using System.Threading;

namespace System.IO.NG
{
    public static class FileInfoExtensions
    {
        public static bool Exists(this FileInfo fi, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => FileNG.Exists(fi.FullName, iopriority, timeout, canceltoken);

        public static void Delete(this FileInfo fi, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeout = null, CancellationToken? canceltoken = null) => FileNG.Delete(fi.FullName, iopriority, timeout, canceltoken);
    }
}
