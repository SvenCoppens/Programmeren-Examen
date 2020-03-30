using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Programmeren_Examen_Final
{
    class Unzipper
    {
        public static DirectoryInfo Unzip(string path)
        {
            string exitpath = path.Substring(0, path.Length - 4);
            if (Directory.Exists(exitpath))
            {
                DeleteCompletely(exitpath);
            }
            ZipFile.ExtractToDirectory(path, exitpath);
            return new DirectoryInfo(exitpath);
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

    }
}
