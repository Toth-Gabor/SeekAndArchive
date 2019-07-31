using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SeekAndArchive
{
    class Program
    {
        static List<FileInfo> foundFiles;
        static List<FileSystemWatcher> watchers;

        static void RecursiveSearch(List<FileInfo> foundFiles, DirectoryInfo currentDirectory, string fileName)
        {
            Console.WriteLine(currentDirectory.FullName);
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

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                Console.WriteLine("{0} has been changed!", e.FullPath);
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

        static void Main(string[] args)
        {
            foundFiles = new List<FileInfo>();
            watchers = new List<FileSystemWatcher>();
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


            
            Console.ReadLine();
        }
    }
}
