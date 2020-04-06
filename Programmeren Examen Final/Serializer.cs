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
        //List<Provincie> Provincies;
        //string BinPath;
        //public Serializer(List<Provincie> provincies)
        //{
        //    Provincies = provincies;
        //    BinPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata";
        //}
        //public void Serialize()
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = new FileStream(Path.Combine(BinPath, "Provincies.bin"), FileMode.Create, FileAccess.Write, FileShare.None);
        //    formatter.Serialize(stream, Provincies);
        //    stream.Close();
        //}
        Belgie Belg;
        string BinPath;
        public Serializer(Belgie belg)
        {
            Belg = belg;
            BinPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata";
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
