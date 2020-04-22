using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Programmeren_Examen_Tool_1;

namespace Programmeren_Examen_Tool_2
{
    class SerializationExtraction
    {
        public Belgie Unserialize(string binPath)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(binPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Belgie belg = (Belgie)formatter.Deserialize(stream);
            stream.Close();
            return belg;
        }
    }
}
