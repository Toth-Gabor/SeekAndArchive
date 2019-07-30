using System;
using System.Collections.Generic;
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
                    if (fileName == fi.Name)
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
            
            foreach (var file in foundFiles)
            {
                Console.WriteLine(file.FullName);
            }
            Console.ReadLine();
        }

    }
}
