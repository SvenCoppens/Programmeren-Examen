using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Programmeren_Examen_Tool_1
{
    class ReportWriter
    {
        Belgie belg;
        public string ReportPath;
        public ReportWriter(string exportPath)
        {
            ReportPath = exportPath;
        }
        public void CompleteReport(Belgie belg)
        {
            int totaalAantalStraten = 0;
            int[] aantalStraten = new int[belg.Provincies.Count];
            List<Provincie> provincies = belg.Provincies.OrderBy(p => p.Naam).ToList();
            for(int i=0;i< belg.Provincies.Count;i++)
            {
                provincies[i].Gemeenten = provincies[i].Gemeenten.OrderBy(g => g.Naam).ToList();
                foreach(Gemeente gem in provincies[i].Gemeenten)
                {
                    gem.Straten = gem.Straten.OrderBy(s => s.Lengte).ToList();
                    totaalAantalStraten += gem.Straten.Count;
                    aantalStraten[i] += gem.Straten.Count;
                }
            }
            using (StreamWriter writer = File.CreateText(Path.Combine(ReportPath, "Rapport tool 1.txt")))
            {
                writer.WriteLine($"Totaal aantal straten: {totaalAantalStraten}");
                writer.WriteLine();
                writer.WriteLine("Aantal straten per provincie");
                for(int i = 0; i < aantalStraten.Length; i++)
                {
                    writer.WriteLine($"    -   {provincies[i].Naam}:{aantalStraten[i]}");
                }
                writer.WriteLine();
                foreach (Provincie prov in belg.Provincies)
                {
                    writer.WriteLine($"Straatinfo {prov.Naam}");
                    foreach (Gemeente gem in prov.Gemeenten)
                    {

                        writer.WriteLine($"    -{gem.Naam}: {gem.Straten.Count} straten met een totale lengte van: {gem.BerekenTotaleLengte()}m");
                        //voor het geval dat er meerder straten zijn met dezelfde lengte
                        int i = 0;
                        if (gem.Straten.Count != 0)
                        {
                            while (gem.Straten[i].Lengte == gem.Straten[0].Lengte)
                            {
                                writer.WriteLine($"         Kortste Straat - ID: {gem.Straten[i].StraatID}; {gem.Straten[i].Naam}: {gem.Straten[i].Lengte}m");
                                i++;
                            }

                            i = gem.Straten.Count - 1;
                            while (gem.Straten[i].Lengte == gem.Straten[gem.Straten.Count - 1].Lengte)
                            {
                                writer.WriteLine($"         Langste Straat - ID: {gem.Straten[i].StraatID}; {gem.Straten[i].Naam}: {gem.Straten[i].Lengte}m");
                                i--;
                            }
                        }
                    }
                    writer.WriteLine();
                }
            }
        }
    }
}
