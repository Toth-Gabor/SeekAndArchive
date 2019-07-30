using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace SeekAndArchive
{
    class Program
    {
        private static List<FileInfo> foundFiles = new List<FileInfo>();

        private void FileSearcher(DirectoryInfo parent, string fileName)
        {
            DirectoryInfo[] dis = parent.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    Match m = Regex.Match(fi.Name, fileName);
                    if (m.Success)
                    {
                        foundFiles.Add(fi);
                    }
                }
                FileSearcher(di, fileName);
            }
        }

        static void Main(string[] args)
        {
            string fileName = args[0];
            string directory = args[1];

            DirectoryInfo parent = new DirectoryInfo(directory);
            Program p = new Program();
            p.FileSearcher(parent, fileName);

            if (!parent.Exists)
            {
                Console.WriteLine("The specified directory does not exist.");
                return;
            }
      
            foreach (var file in foundFiles)
            {
                Console.WriteLine(file.FullName);
            }
            Console.ReadLine();
        }

    }
}
