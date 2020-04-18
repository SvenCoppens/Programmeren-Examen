using System;
using Programmeren_Examen_Tool_1;
using System.Collections.Generic;

namespace Programmeren_Examen_Tool_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DatabaseRequester dR = new DatabaseRequester();
            ReportWriter rW = new ReportWriter(@"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata\Rapporten");
            //Straat straat1 = dR.ReturnStraatVoorStraatId(486);
            //Straat straat2 = dR.ReturnStraatVoorStraatNaam("Antwerpen", "Boomstraat");
            //Straat straat3 = dR.ReturnStraatVoorStraatNaam("Aartselaar", "Boomsesteenweg");
            //List<string> straten = dR.StraatNamenVoorGemeente("Antwerpen");
            #region provincieRapport
            Provincie antwerpen = dR.ProvincieRapport("Oost-Vlaanderen");
            rW.ProvincieRapport(antwerpen);
            #endregion
            #region Aangrenzende Straten
            //List<Straat> straten = dR.AangrenzendeStraten(114);
            //rW.AangrenzendeStratenReport(straten, dR.ReturnStraatVoorStraatId(114));
            #endregion
            Console.WriteLine("Done");
        }
    }
}
