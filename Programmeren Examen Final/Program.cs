using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Final
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
            Dictionary<string, Straat> straten = sne.Extract(path, straatIdGraafKoppeling);
            Console.WriteLine("donezo");


        }
    }
}
