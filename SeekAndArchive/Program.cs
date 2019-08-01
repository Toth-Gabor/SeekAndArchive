using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace SeekAndArchive
{
    class Program
    {
        static List<FileInfo> foundFiles;
        static List<FileSystemWatcher> watchers;
        static List<DirectoryInfo> archiveDirs;

        static void Main(string[] args)
        {
            foundFiles = new List<FileInfo>();
            watchers = new List<FileSystemWatcher>();
            archiveDirs = new List<DirectoryInfo>();

            string fileName = args[0];
            string directory = Path.GetFullPath(args[1]);
            DirectoryInfo parent = new DirectoryInfo(directory);

            if (!parent.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                Console.ReadLine();
                return;
            }

            RecursiveSearch(foundFiles, parent, fileName);
            Console.WriteLine("Found {0} files.", foundFiles.Count);

            foreach (FileInfo fileInfo in foundFiles)
            {
                FileSystemWatcher Watcher = new FileSystemWatcher(fileInfo.DirectoryName, fileInfo.Name);
                Watcher.Changed += new FileSystemEventHandler(Watcher_Changed);
                Watcher.Renamed += new RenamedEventHandler(Watcher_Changed);
                Watcher.Deleted += new FileSystemEventHandler(Watcher_Changed);
                Watcher.EnableRaisingEvents = true;
                watchers.Add(Watcher);

                Console.WriteLine("Searched file: {0}", fileInfo.FullName);
            }

            for (int i = 0; i < foundFiles.Count; i++)
            {
                archiveDirs.Add(Directory.CreateDirectory("archive" + i.ToString()));
            }

            Console.ReadKey();

        }
        static void RecursiveSearch(List<FileInfo> foundFiles, DirectoryInfo currentDirectory, string fileName)
        {
            foreach (FileInfo fi in currentDirectory.GetFiles())
            {
                if (Regex.IsMatch(fi.Name, fileName))
                {
                    foundFiles.Add(fi);
                }
            }

            foreach (DirectoryInfo dir in currentDirectory.GetDirectories())
            {
                RecursiveSearch(foundFiles, dir, fileName);
            }
        }

        static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            FileSystemWatcher senderWatcher = (FileSystemWatcher)sender;
            int index = watchers.IndexOf(senderWatcher);

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed!", e.FullPath);
                ArchiveFile(archiveDirs[index], foundFiles[index]);
                //Console.WriteLine("Archived to {0}", archiveDirs[index].GetFiles()[0].FullName);
            }
            else if (e.ChangeType == WatcherChangeTypes.Renamed)
            {
                Console.WriteLine("{0} has been renamed!", e.FullPath);
            }
            else if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                Console.WriteLine("{0} has been deleted!", e.FullPath);
                watchers.Remove(senderWatcher);
            }
        }
        static void ArchiveFile(DirectoryInfo archiveDir, FileInfo fileToArchive)
        {
            FileStream input = fileToArchive.OpenRead();
            FileStream output = File.Create(archiveDir.FullName + @"" + fileToArchive.Name + ".gz");
            GZipStream Compressor = new GZipStream(output, CompressionMode.Compress);
            int b = input.ReadByte();

            while (b != -1)
            {
                Compressor.WriteByte((byte)b);
                b = input.ReadByte();
            }
            Compressor.Close(); input.Close(); output.Close();
        }
    }
}
