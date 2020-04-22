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
            Console.WriteLine("Hello World! \n");
            string reportPath = @"D:\Programmeren Data en Bestanden\Wegen Examen\Rapporten";
            iDataFetcher queryer = new DatabaseQueryer();
            ReportWriter reportWriter = new ReportWriter(reportPath);

            ////opdracht 1
            //string gemeenteOpdracht1 = "Melle";
            //List<int> straatIds = queryer.ReturnStraatIdsVoorGemeenteNaam(gemeenteOpdracht1);
            //reportWriter.StraatIdsReport(straatIds, gemeenteOpdracht1);

            ////opdracht 2
            //Straat straat1 = queryer.ReturnStraatVoorId(15);
            //reportWriter.StraatReport(straat1);

            ////opdracht 3
            //Straat straat2 = queryer.ReturnStraatVoorNaam("Melle", "Schauwegemstraat");
            //reportWriter.StraatReport(straat2);

            ////opdracht 4
            //string gemeenteOpdracht4 = "Melle";
            //List<string> straten = queryer.StraatNamenVoorGemeenteNaam(gemeenteOpdracht4);
            //reportWriter.StraatNamenReport(straten, gemeenteOpdracht4);

            ////opdracht 5
            ////string provincieNaamOpdracht5 = "Oost-Vlaanderen";
            ////Provincie provincie = dQ.ReturnProvincieCompleteVoorNaam(provincieNaamOpdracht5);
            ////rW.ProvincieRapport(provincie);

            ////opdracht 6
            //int straatIdOpdracht6 = 3;
            //List<Straat> aangrenzendeStraten = queryer.ReturnStratenAangrenzendAanId(straatIdOpdracht6);
            //reportWriter.AangrenzendeStratenReport(aangrenzendeStraten, queryer.ReturnStraatVoorId(straatIdOpdracht6));
            //Console.WriteLine("Finished");


            #region alternatief
            DatabaseReports Reporter = new DatabaseReports(queryer, reportPath);

            //opdracht 1
            string alternatiefOpdracht1 = "Melle";
            Reporter.StraatIdsReport(alternatiefOpdracht1);

            //opdracht 2
            int alternatiefOpdracht2 = 15;
            Reporter.StraatReport(alternatiefOpdracht2);

            //opdracht 3
            string alternatiefGemeenteOpdracht3 = "Melle";
            string alternatiefStraatOpdracht3 = "Schauwegemstraat";
            Reporter.StraatReport(alternatiefGemeenteOpdracht3, alternatiefStraatOpdracht3);

            //opdracht 4
            string alternatiefOpdracht4 = "Melle";
            Reporter.StraatNamenReport(alternatiefOpdracht4);

            ////opdracht 5
            //string alternatiefOpdracht5 = "Oost-Vlaanderen";
            //Reporter.ProvincieRapport(alternatiefOpdracht5);

            //opdracht 6
            int alternatiefOpdracht6 = 3;
            Reporter.AangrenzendeStratenReport(alternatiefOpdracht6);

            #endregion
        }
    }
}
