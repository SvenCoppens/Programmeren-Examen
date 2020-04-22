using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Programmeren_Examen_Tool_1
{
    class Unzipper
    {
        public static void Unzip(string path)
        {
            string exitpath = path.Substring(0, path.Length - 4);
            if (Directory.Exists(exitpath))
            {
                Console.WriteLine("unzipped folder seems to already exist, do you wish to delete it and unzip again? \"Y\" for yes");
                if (Console.ReadLine() == "Y")
                {
                    DeleteCompletely(exitpath);
                    ActualUnzip(path, exitpath);
                }
                else
                    Console.WriteLine("no changes were made");
            }
            else
                ActualUnzip(path, exitpath);
            
        }
        public static void DeleteCompletely(string Path)
        {
            DirectoryInfo temp = new DirectoryInfo(Path);
            foreach (FileInfo file in temp.EnumerateFiles())
                file.Delete();
            foreach (DirectoryInfo dir in temp.EnumerateDirectories())
                DeleteCompletely(dir.FullName);
            temp.Delete();
        }
        private static void ActualUnzip(string path,string exitpath)
        {
            ZipFile.ExtractToDirectory(path, exitpath);
            string tempPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\WRdata-master";
            ZipFile.ExtractToDirectory(Path.Combine(tempPath, "WRdata.zip"), tempPath);
            ZipFile.ExtractToDirectory(Path.Combine(tempPath, "WRstraatnamen.zip"), tempPath);
        }

    }
}
