using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Programmeren_Examen_Tool_2
{
    class Serialization_extraction
    {
        Belgie Belgie;
        string BinPath;
        public Serialization_extraction()
        {
            Belgie = new Belgie(new List<Provincie>());
            BinPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\Belgie.bin";
        }
        public Belgie Serialize()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(BinPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Belgie = (Belgie)formatter.Deserialize(stream);
            stream.Close();
            return Belgie;
        }
    }
}
