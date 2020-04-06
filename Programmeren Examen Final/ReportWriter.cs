using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Programmeren_Examen_Tool_1
{
    class ReportWriter
    {
        Dictionary<string, Straat> Straten;
        List<Provincie> Provincies;
        public string Wegenpath;
        public ReportWriter(Dictionary<string, Straat> straten)
        {
            Straten = straten;
            HaalProvincies();
            Wegenpath = @"D:\Programmeren Data en Bestanden\Wegen Examen\WRdata";
        }
        public void CreateReport()
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(Wegenpath, "Rapport.txt")))
            {
                writer.WriteLine($"Totaal aantal straten: {Straten.Count}");
                writer.WriteLine();
                writer.WriteLine("Aantal straten per provincie");
                foreach (Provincie prov in Provincies)
                {
                    int aantalStraten = 0;
                    foreach (Gemeente gem in prov.Gemeenten)
                    {
                        aantalStraten += gem.Straten.Count;
                    }
                    writer.WriteLine($"    -   {prov.Naam}:{aantalStraten}");

                }

                foreach (Provincie prov in Provincies)
                {
                    writer.WriteLine($"Straatinfo {prov.Naam}");
                    foreach (Gemeente gem in prov.Gemeenten)
                    {

                        writer.WriteLine($"    -{gem.Naam}: {gem.Straten.Count}, {gem.BerekenTotaleLengte()}");
                        gem.Straten.Sort(new StraatLengteEerst());
                        int i = 0;
                        if (gem.Straten.Count != 0)
                        {
                            while (gem.Straten[i].Lengte == gem.Straten[0].Lengte)
                            {
                                writer.WriteLine($"         -   {gem.Straten[i].StraatID},{gem.Straten[i].Naam},{gem.Straten[i].Lengte}");
                                i++;
                            }

                            i = gem.Straten.Count - 1;
                            while (gem.Straten[i].Lengte == gem.Straten[gem.Straten.Count - 1].Lengte)
                            {
                                writer.WriteLine($"         -   {gem.Straten[i].StraatID},{gem.Straten[i].Naam},{gem.Straten[i].Lengte}");
                                i--;
                            }
                        }
                    }
                }
            }
        }
        public void WriteReport()
        {
            Console.WriteLine($"Totaal aantal straten: {Straten.Count}");
            Console.WriteLine();
            Console.WriteLine("Aantal straten per provincie");
            foreach(Provincie prov in Provincies)
            {
                int aantalStraten = 0;
                foreach(Gemeente gem in prov.Gemeenten)
                {
                    aantalStraten += gem.Straten.Count;
                }
                Console.WriteLine($"    -   {prov.Naam}:{aantalStraten}");
                
            }

            foreach (Provincie prov in Provincies)
            {
                Console.WriteLine($"Straatinfo {prov.Naam}");
                foreach(Gemeente gem in prov.Gemeenten)
                {
                    
                    Console.WriteLine($"    -{gem.Naam}: {gem.Straten.Count}, {gem.BerekenTotaleLengte()}");
                    gem.Straten.Sort( new StraatLengteEerst());
                    int i = 0;
                    if (gem.Straten.Count != 0)
                    {
                        while (gem.Straten[i].Lengte == gem.Straten[0].Lengte)
                        {
                            Console.WriteLine($"         -   {gem.Straten[i].StraatID},{gem.Straten[i].Naam},{gem.Straten[i].Lengte}");
                            i++;
                        }

                        i = gem.Straten.Count - 1;
                        while (gem.Straten[i].Lengte == gem.Straten[gem.Straten.Count - 1].Lengte)
                        {
                            Console.WriteLine($"         -   {gem.Straten[i].StraatID},{gem.Straten[i].Naam},{gem.Straten[i].Lengte}");
                            i--;
                        }
                    }
                }
            }
        }
        public void HaalProvincies()
        {
            List<Provincie> temp = new List<Provincie>();
            foreach(var x in Straten)
            {
                if (!temp.Contains(x.Value.Gemeente.Provincie))
                    temp.Add(x.Value.Gemeente.Provincie);
            }
            Provincies = temp;
        }
        
    }
}
