using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class Program
    {
        static void Main()
        {
            //setup
            Console.WriteLine("Hello World!");
            string zipPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata.zip";
            Unzipper.Unzip(zipPath);
            string path = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\WRdata-master";
            FileExtraction fe = new FileExtraction();
            StraatNaamExtraction sne = new StraatNaamExtraction();

            Console.WriteLine("deel 1");
            Dictionary<string, Graaf> straatIdGraafKoppeling = fe.MaakSegmenten(path);
            Console.WriteLine("deel 2");
            Dictionary<string, Provincie> provincies = sne.Extract(path, straatIdGraafKoppeling);
            List<Provincie> provinciesLijst = new List<Provincie>();
            foreach(var x in provincies)
            {
                provinciesLijst.Add(x.Value);
            }
            Belgie belgie = new Belgie(provinciesLijst);

            string customPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\";
            Console.WriteLine("Writing Report");
            ReportWriter rw = new ReportWriter(belgie,customPath);
            rw.CompleteReport();

            Serializer serializer = new Serializer(belgie,customPath);
            serializer.Serialize();
            Console.WriteLine("donezo");


        }
    }
}
