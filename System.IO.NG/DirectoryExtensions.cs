namespace System.IO.NG
{
    public static class DirectoryExtensions
    {
        public static void CopyContentsOfRecursively(string sourcePath, string destinationPath, IOPriorityClass iopriority)
        {
            //Now Create all of the directories
            var dirs = DirectoryNG.GetDirectories(sourcePath, "*", SearchOption.AllDirectories, iopriority: iopriority);
            foreach (string dirPath in dirs) DirectoryNG.CreateDirectory(dirPath.Replace(sourcePath, destinationPath), iopriority: iopriority);

            //Copy all the files & Replaces any files with the same name
            var fs = DirectoryNG.GetFiles(sourcePath, "*", SearchOption.AllDirectories, iopriority: iopriority);
            foreach (string newPath in fs) FileNG.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true, iopriority: iopriority);
        }
    }
}
