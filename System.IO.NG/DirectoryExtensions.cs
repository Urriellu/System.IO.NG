namespace System.IO.NG
{
    public static class DirectoryExtensions
    {
        public static void CopyContentsOfRecursively(string sourcePath, string destinationPath, IOPriorityClass iopriority = IOPriorityClass.L02_NormalEffort, TimeSpan? timeoutPerFile = null)
        {
            //Now Create all of the directories
            var dirs = DirectoryNG.GetDirectories(sourcePath, "*", SearchOption.AllDirectories, iopriority: iopriority);
            foreach (string dirPath in dirs) DirectoryNG.CreateDirectory(dirPath.Replace(sourcePath, destinationPath), iopriority, timeoutPerFile);

            //Copy all the files & Replaces any files with the same name
            var fs = DirectoryNG.GetFiles(sourcePath, "*", SearchOption.AllDirectories, iopriority: iopriority);
            foreach (string newPath in fs) FileNG.Copy(newPath, newPath.Replace(Path.GetFullPath(sourcePath), Path.GetFullPath(destinationPath)), overwrite: true, iopriority: iopriority, timeout: timeoutPerFile);
        }
    }
}
