using System;
using Programmeren_Examen_Tool_1;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Programmeren_Examen_Tool_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DatabaseQueryer dQ = new DatabaseQueryer();
            ReportWriter rW = new ReportWriter(@"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\Rapporten");

            //opdracht 1
            string gemeenteOpdracht1 = "Melle";
            List<int> straatIds = dQ.ReturnStraatIdsVoorGemeenteNaam(gemeenteOpdracht1);
            rW.StraatIdsReport(straatIds, gemeenteOpdracht1);

            //opdracht 2
            Straat straat1 = dQ.ReturnStraatVoorId(15);
            rW.StraatReport(straat1);

            //opdracht 3
            Straat straat2 = dQ.ReturnStraatVoorNaam("Melle", "Schauwegemstraat");
            rW.StraatReport(straat2);

            //opdracht 4
            string gemeenteOpdracht4 = "Melle";
            List<string> straten = dQ.StraatNamenVoorGemeenteNaam(gemeenteOpdracht4);
            rW.StraatNamenReport(straten, gemeenteOpdracht4);

            //opdracht 5
            string provincieNaamOpdracht5 = "Oost-Vlaanderen";
            Provincie provincie = dQ.ReturnProvincieCompleteVoorNaam(provincieNaamOpdracht5);
            rW.ProvincieRapport(provincie);

            //opdracht 6
            int straatIdOpdracht6 = 3;
            List<Straat> aangrenzendeStraten = dQ.ReturnStratenAangrenzendAanId(straatIdOpdracht6);
            rW.AangrenzendeStratenReport(aangrenzendeStraten, dQ.ReturnStraatVoorId(straatIdOpdracht6));
            Console.WriteLine("Finished");

        }
    }
}
