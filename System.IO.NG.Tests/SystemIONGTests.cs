using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.IO.NG.Tests
{
    [TestClass]
    public class SystemIONGTests
    {
        public const string LettersAndNumbers = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string SpecialChars = "`~!@#$%^&*()_-+={[}]|\\:;\"'<,>.?/";
        public const string AllKeyboardCharacters = LettersAndNumbers + SpecialChars;

        static readonly Random rnd = new Random();
        public static string RandomString(int length, string chars = LettersAndNumbers) => new string(Enumerable.Repeat(chars, length).Select(s => s[rnd.Next(s.Length)]).ToArray());

        [TestMethod]
        public void T01_Functionality()
        {
            try { Thread.CurrentThread.Name = "Unit Test"; }
            catch { }

            // CREATE DIRECTORY
            string pathDir = Path.Combine(Path.GetTempPath(), $"{nameof(SystemIONGTests)}-{nameof(T01_Functionality)}-{DateTime.Now:yyyyMMdd-HHmmssfff}");
            Assert.IsFalse(DirectoryNG.Exists(pathDir));
            DirectoryNG.CreateDirectory(pathDir);
            Assert.IsTrue(DirectoryNG.Exists(pathDir));

            // CREATE SUBDIRECTORIES
            int amountSubDirs = rnd.Next(4, 6);
            for (int dn = 0; dn < amountSubDirs; dn++)
            {
                string dirname = $"{dn}-{RandomString(rnd.Next(5, 12))}";
                string pathSubDir = Path.Combine(pathDir, dirname);
                Assert.IsFalse(DirectoryNG.Exists(pathSubDir));
                DirectoryNG.CreateDirectory(pathSubDir);
                Assert.IsTrue(DirectoryNG.Exists(pathSubDir));
                string[] filesInSubDirReadBack = DirectoryNG.GetFiles(pathSubDir);
                Assert.AreEqual(0, filesInSubDirReadBack.Length);

                // CREATE FILES
                int amountFiles = rnd.Next(4, 6);
                for (int fn = 0; fn < amountFiles; fn++)
                {
                    string filename = RandomString(rnd.Next(5, 12)) + ".txt";
                    string pathfile = Path.Combine(pathSubDir, filename);
                    if (fn % 2 == 1)
                    {
                        // ADD TEXT TO FILE
                        string content = RandomString(rnd.Next(10 * 1024, 200 * 1024), LettersAndNumbers + SpecialChars + " ");
                        Assert.IsFalse(FileNG.Exists(pathfile));
                        FileNG.WriteAllText(pathfile, content);
                        Assert.IsTrue(FileNG.Exists(pathfile));
                        string contentReadBack = FileNG.ReadAllText(pathfile);
                        Assert.AreEqual(content, contentReadBack);

                        // APPEND TEXT TO FILE
                        string contentToAppend = RandomString(rnd.Next(1 * 1024, 2 * 1024), LettersAndNumbers + SpecialChars + " ");
                        FileNG.AppendAllText(pathfile, contentToAppend);
                        contentReadBack = FileNG.ReadAllText(pathfile);
                        Assert.AreEqual(content + contentToAppend, contentReadBack);

                        // MOVE/RENAME FILE
                        string filename2 = $"{Path.GetFileNameWithoutExtension(filename)}-moved.txt";
                        string pathFile2 = Path.Combine(pathSubDir, filename2);
                        Assert.IsFalse(FileNG.Exists(pathFile2));
                        FileNG.Move(pathfile, pathFile2);
                        Assert.IsFalse(FileNG.Exists(pathfile));
                        Assert.IsTrue(FileNG.Exists(pathFile2));
                        contentReadBack = FileNG.ReadAllText(pathFile2);
                        Assert.AreEqual(content + contentToAppend, contentReadBack);
                    }
                    else
                    {
                        // ADD LINES TO FILE
                        string[] content = new string[rnd.Next(5, 10)];
                        for (int line = 0; line < content.Length; line++) content[line] = line + "-" + RandomString(rnd.Next(10, 80), LettersAndNumbers + SpecialChars + " ");
                        Assert.IsFalse(FileNG.Exists(pathfile));
                        FileNG.WriteAllLines(pathfile, content);
                        Assert.IsTrue(FileNG.Exists(pathfile));
                        string[] contentReadBack = FileNG.ReadAllLines(pathfile);
                        Assert.AreEqual(content.Length, contentReadBack.Length);
                        for (int line = 0; line < content.Length; line++) Assert.AreEqual(content[line], contentReadBack[line]);

                        // APPEND LINES TO FILE
                        string[] contentToAppend = new string[rnd.Next(5, 10)];
                        for (int line = 0; line < contentToAppend.Length; line++) contentToAppend[line] = (line + content.Length) + "-" + RandomString(rnd.Next(10, 80), LettersAndNumbers + SpecialChars + " ");
                        FileNG.AppendAllLines(pathfile, contentToAppend);
                        contentReadBack = FileNG.ReadAllLines(pathfile);
                        string[] expected = content.Concat(contentToAppend).ToArray();
                        Assert.AreEqual(expected.Length, contentReadBack.Length);
                        for (int line = 0; line < expected.Length; line++) Assert.AreEqual(expected[line], contentReadBack[line]);
                    }
                }

                // TEST DIRECTORY CAN LIST FILES
                filesInSubDirReadBack = DirectoryNG.GetFiles(pathSubDir);
                Assert.AreEqual(amountFiles, filesInSubDirReadBack.Length);

                // TEST DIRECTORY FILE LISTING SAME AS System.IO
                string[] filesInSubDirReadBackBySysIO = System.IO.Directory.GetFiles(pathSubDir);
                Assert.AreEqual(filesInSubDirReadBackBySysIO.Length, filesInSubDirReadBack.Length);
                for (int fn = 0; fn < filesInSubDirReadBack.Length; fn++) Assert.AreEqual(filesInSubDirReadBackBySysIO[fn], filesInSubDirReadBack[fn]);
            }

            // TEST DIRECTORY CAN LIST FILES (no files created on top directory)
            string[] filesReadBack = DirectoryNG.GetFiles(pathDir);
            Assert.AreEqual(0, filesReadBack.Length);

            // TEST DIRECTORY CAN LIST SUBDIRECTORIES
            string[] dirsReadBack = DirectoryNG.GetDirectories(pathDir);
            Assert.AreEqual(amountSubDirs, dirsReadBack.Length);

            // MOVE/RENAME DIRECTORY
            string pathDirAfterMove = $"{pathDir}-moved";
            Assert.IsFalse(DirectoryNG.Exists(pathDirAfterMove));
            DirectoryNG.Move(pathDir, pathDirAfterMove);
            Assert.IsFalse(DirectoryNG.Exists(pathDir));
            Assert.IsTrue(DirectoryNG.Exists(pathDirAfterMove));

            // TEST SUB-DIRECTORY LISTING SAME AS System.IO
            dirsReadBack = DirectoryNG.GetDirectories(pathDirAfterMove);
            string[] dirsReadBackBySysIO = System.IO.Directory.GetDirectories(pathDirAfterMove);
            Assert.AreEqual(dirsReadBackBySysIO.Length, dirsReadBack.Length);
            for (int dn = 0; dn < dirsReadBackBySysIO.Length; dn++) Assert.AreEqual(dirsReadBackBySysIO[dn], dirsReadBack[dn]);

            foreach (string dir in dirsReadBack)
            {
                // TEST DELETING FILES
                string[] files = DirectoryNG.GetFiles(dir);
                Assert.IsTrue(files.Length > 2);
                for (int i = 0; i < files.Length / 2; i++)
                {
                    string pathFile = files[i];
                    Assert.IsTrue(FileNG.Exists(pathFile));
                    FileNG.Delete(pathFile);
                    Assert.IsFalse(FileNG.Exists(pathFile));
                }
                string[] filesAfterDeletingSome = DirectoryNG.GetFiles(dir);
                Assert.IsTrue(filesAfterDeletingSome.Length >= 1 && filesAfterDeletingSome.Length < files.Length);
            }

            // TEST EMPTYING DIRECTORIES
            for (int d = 0; d < dirsReadBack.Length / 2; d++)
            {
                string pathSubDir = dirsReadBack[d];
                string[] filesInSubDirBeforeEmptying = DirectoryNG.GetFiles(pathSubDir);
                Assert.IsTrue(filesInSubDirBeforeEmptying.Length >= 1);
                DirectoryNG.DeleteAllContents(pathSubDir);
                string[] filesInSubDirAfterEmptying = DirectoryNG.GetFiles(pathSubDir);
                Assert.AreEqual(0, filesInSubDirAfterEmptying.Length);
            }

            // TEST DELETING DIRECTORIES AND SUBDIRECTORIES
            dirsReadBack = DirectoryNG.GetDirectories(pathDirAfterMove);
            filesReadBack = DirectoryNG.GetFiles(pathDirAfterMove);
            Assert.IsTrue(dirsReadBack.Length > 0);
            Assert.AreEqual(0, filesReadBack.Length);
            DirectoryNG.Delete(pathDirAfterMove, true);
            Assert.IsFalse(DirectoryNG.Exists(pathDirAfterMove));
        }

        [TestMethod]
        public void T02_IOPriority()
        {
            // RUN FUNCTIONALITY TESTS UNHINDERED
            Stopwatch swFunctionality = Stopwatch.StartNew();
            T01_Functionality();
            swFunctionality.Stop();

            // CREATE BIG FILE
            Stopwatch swCreateBigFile = Stopwatch.StartNew();
            string pathBigFile = Path.Combine(Path.GetTempPath(), $"{nameof(SystemIONGTests)}-{nameof(T02_IOPriority)}", $"{DateTime.Now:yyyyMMdd-HHmmssfff}-big-file.txt");
            DirectoryNG.CreateDirectory(Path.GetDirectoryName(pathBigFile));
            string pattern = RandomString(10 * 1024 * 1024, LettersAndNumbers + SpecialChars + " ");
            FileNG.WriteAllText(pathBigFile, pattern);
            while (new FileInfo(pathBigFile).Length < 5L * 1024 * 1024 * 1024) FileNG.AppendAllText(pathBigFile, pattern);
            swCreateBigFile.Stop();
            Debug.WriteLine($"Creating the big file took {swCreateBigFile.Elapsed.TotalSeconds:N2} seconds");

            Stopwatch swHighPrio = new Stopwatch();
            Stopwatch swLowPrio = new Stopwatch();
            Stopwatch swTotal = new Stopwatch();
            string pathBigFileHighPrio = Path.Combine(Path.GetTempPath(), $"{nameof(SystemIONGTests)}-{nameof(T02_IOPriority)}", $"{DateTime.Now:yyyyMMdd-HHmmssfff}-big-file-highrio.txt");
            string pathBigFileLowPrio = Path.Combine(Path.GetTempPath(), $"{nameof(SystemIONGTests)}-{nameof(T02_IOPriority)}", $"{DateTime.Now:yyyyMMdd-HHmmssfff}-big-file-lowrio.txt");
            
            Task t1 = Task.Factory.StartNew(() =>
            {
                try { Thread.CurrentThread.Name = $"Copy big file high priority"; }
                catch { }
                Debug.WriteLine($"Starting high prio copy");
                swHighPrio.Start();
                swTotal.Start();
                FileNG.Copy(pathBigFile, pathBigFileHighPrio, iopriority: IOPriorityClass.L03_HighEffort);
                swHighPrio.Stop();
            });
            
            Task t2 = Task.Factory.StartNew(() =>
            {
                try { Thread.CurrentThread.Name = $"Copy big file low priority"; }
                catch { }
                Debug.WriteLine($"Starting low prio copy");
                swLowPrio.Start();
                FileNG.Copy(pathBigFile, pathBigFileLowPrio, iopriority: IOPriorityClass.L00_Idle);
                swLowPrio.Stop();
                swTotal.Stop();
            });

            Task.WaitAll(t1, t2);

            TimeSpan between = swTotal.Elapsed - swHighPrio.Elapsed;

            FileNG.Delete(pathBigFileHighPrio);
            FileNG.Delete(pathBigFileLowPrio);
            FileNG.Delete(pathBigFile);

            Debug.WriteLine($"Low priority copy took {swLowPrio.Elapsed.TotalSeconds:N2}s, and high priority finished {between.TotalSeconds:N2}s earlier taking {swHighPrio.Elapsed.TotalSeconds:N2}s.");
        }

        /// <summary>
        /// This test runs the entire T01_Functionality() test twice, the first time it runs unhindered, the second time it runs while an idle-priority file is being copied. It measures both executions times and we expect the second one to be slower, but not much.
        /// </summary>
        [TestMethod]
        public void T03_Performance()
        {
            // RUN FUNCTIONALITY TESTS UNHINDERED
            Stopwatch swFunctionality = Stopwatch.StartNew();
            T01_Functionality();
            swFunctionality.Stop();

            // CREATE BIG FILE
            Stopwatch swCreateBigFile = Stopwatch.StartNew();
            string pathBigFile = Path.Combine(Path.GetTempPath(), $"{nameof(SystemIONGTests)}-{nameof(T03_Performance)}", $"{DateTime.Now:yyyyMMdd-HHmmssfff}-big-file.txt");
            DirectoryNG.CreateDirectory(Path.GetDirectoryName(pathBigFile));
            string pattern = RandomString(10 * 1024 * 1024, LettersAndNumbers + SpecialChars + " ");
            FileNG.WriteAllText(pathBigFile, pattern);
            while (new FileInfo(pathBigFile).Length < 10L * 1024 * 1024 * 1024) FileNG.AppendAllText(pathBigFile, pattern);
            swCreateBigFile.Stop();
            Debug.WriteLine($"Creating the big file took {swCreateBigFile.Elapsed.TotalSeconds:N2} seconds");

            // START COPYING FILE WHILE SECOND INSTANCE OF FUNCTIONALITY TESTS RUN
            CancellationTokenSource cts = new CancellationTokenSource();
            bool keepCopying = true;
            Task.Factory.StartNew(() =>
            {
                try { Thread.CurrentThread.Name = $"Big file copier"; }
                catch { }
                int copyIteration = 0;
                while (keepCopying)
                {
                    string pathNewCopy = Path.Combine(Path.GetDirectoryName(pathBigFile), $"{Path.GetFileNameWithoutExtension(pathBigFile)}-copy{copyIteration++}.txt");
                    FileNG.Copy(pathBigFile, pathNewCopy, false, IOPriorityClass.L00_Idle, null, cts.Token);
                }
            });

            // RUN SECOND INSTANCE OF FUNCTIONALITY TESTS
            Stopwatch swFunctionalityWhileBigFileBeingCopied = Stopwatch.StartNew();
            T01_Functionality();
            swFunctionalityWhileBigFileBeingCopied.Stop();
            keepCopying = false;
            cts.Cancel();

            string[] allCopies = DirectoryNG.GetFiles(Path.GetDirectoryName(pathBigFile), $"{Path.GetFileNameWithoutExtension(pathBigFile)}*", SearchOption.TopDirectoryOnly);
            foreach (string pathCopy in allCopies) FileNG.Delete(pathCopy);

            Debug.WriteLine($"T01 runs unhindered in {swFunctionality.Elapsed.TotalSeconds:N2} seconds, overloaded takes {swFunctionalityWhileBigFileBeingCopied.Elapsed.TotalSeconds:N2} seconds"); // on my system: 50s, 8.5m, big file created in 3m
        }
    }
}
