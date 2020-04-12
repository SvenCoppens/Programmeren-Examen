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
            DatabaseRequests dR = new DatabaseRequests();
            //Straat straat1 = dR.ReturnStraatVoorStraatId(486);
            //Straat straat2 = dR.ReturnStraatVoorStraatNaam("Antwerpen", "Boomstraat");
            //Straat straat3 = dR.ReturnStraatVoorStraatNaam("Aartselaar", "Boomsesteenweg");
            //List<string> straten = dR.StraatNamenVoorGemeente("Antwerpen");
            //Provincie antwerpen = dR.ProvincieRapport("Antwerpen");
            List<Straat> straten = dR.AangrenzendeStraten(114);
            Console.WriteLine();
        }
    }
}
