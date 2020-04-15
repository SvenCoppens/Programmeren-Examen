using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Programmeren_Examen_Tool_1
{
    class Serializer
    {
        Belgie Belg;
        string BinPath;
        public Serializer(Belgie belg,string path)
        {
            Belg = belg;
            BinPath = path;
        }
        public void Serialize()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(Path.Combine(BinPath, "Belgie.bin"), FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, Belg);
            stream.Close();
        }
    }
}
