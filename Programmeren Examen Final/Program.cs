using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");
            string path = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata";
            FileExtraction fe = new FileExtraction();
            Dictionary<string, Graaf> straatIdGraafKoppeling = fe.MaakSegmenten(path);
            StraatNaamExtraction sne = new StraatNaamExtraction();
            Console.WriteLine("deel 2");
            Dictionary<string, Provincie> provincies = sne.Extract(path, straatIdGraafKoppeling);
            List<Provincie> provinciesLijst = new List<Provincie>();
            foreach(var x in provincies)
            {
                provinciesLijst.Add(x.Value);
            }
            //ReportWriter rw = new ReportWriter(straten);
            //rw.CreateReport();
            //Serializer serializer = new Serializer(provinciesLijst);
            //serializer.Serialize();
            Belgie belgie = new Belgie(provinciesLijst);
            Serializer serializer = new Serializer(belgie);
            serializer.Serialize();
            Console.WriteLine("donezo");


        }
    }
}
