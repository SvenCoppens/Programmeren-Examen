using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Programmeren_Examen_Tool_1
{
    public class ReportWriter
    {
        public string ReportsPath;
        public ReportWriter(string exportPath)
        {
            ReportsPath = exportPath;
        }
        public void CompleteReport(Belgie belg)
        {
            //het oude rapport verwijderen indien er een is
            string exitPath = Path.Combine(ReportsPath, "Rapport tool 1.txt");
            CheckAndDeleteFile(exitPath);


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

            using (StreamWriter writer = File.CreateText(exitPath))
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
            Console.WriteLine($"Report was placed in {exitPath}");
        }
        public void StraatReport(Straat straat)
        {
            Console.WriteLine($"StraatReport van {straat.ToString()}");
            straat.ShowStraat();
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------------------------------\n\n");
        }
        public void StraatIdsReport(List<int> straatIds, string gemeenteNaam)
        {
            string exitPath = Path.Combine(ReportsPath, $"StraatIDs in {gemeenteNaam}.txt");
            CheckAndDeleteFile(exitPath);
            using (StreamWriter writer = File.CreateText(exitPath))
            {
                writer.WriteLine($"Lijst van straatIDs in de gemeente van: {gemeenteNaam}");
                writer.WriteLine($"Totaal aantal van {straatIds.Count}");
                foreach (int straatId in straatIds)
                {
                    writer.WriteLine($"        -   {straatId}");
                }
            }
            Console.WriteLine($"StraatnaamReport weggeschreven in {exitPath}\n\n");
        }
        public void StraatNamenReport(List<string> straten, string gemeente)
        {
            string exitPath = Path.Combine(ReportsPath, $"Straatnamen in {gemeente}.txt");
            CheckAndDeleteFile(exitPath);
            using (StreamWriter writer = File.CreateText(exitPath))
            {
                writer.WriteLine($"Lijst van straatnamen in de gemeente van: {gemeente}");
                writer.WriteLine($"Totaal aantal van {straten.Count}");
                foreach (string straatNaam in straten)
                {
                    writer.WriteLine($"        -   {straatNaam}");
                }
            }
            Console.WriteLine($"StraatnaamReport weggeschreven in {exitPath}\n\n");
        }

        private void CheckAndDeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public void ProvincieRapport(Provincie prov)
        {
            string path = Path.Combine(ReportsPath, $"Provincie rapport {prov.Naam}.txt");
            CheckAndDeleteFile(path);
            using (StreamWriter writer = File.CreateText(path))
            {
                prov.Gemeenten = prov.Gemeenten.OrderBy(g => g.Naam).ToList();
                foreach (Gemeente gem in prov.Gemeenten)
                {
                    writer.WriteLine($"{gem.Naam}: {gem.Straten.Count}");
                    gem.Straten = gem.Straten.OrderBy(s => s.Lengte).ToList();
                    foreach (Straat straat in gem.Straten)
                    {
                        writer.WriteLine($"     ■   {straat.Naam}, {straat.Lengte}");
                    }
                }
            }
            Console.WriteLine($"PrivincieReport weggeschreven in {path}\n\n");
        }
        public void AangrenzendeStratenReport(List<Straat> straten, Straat origineleStraat)
        {
            string exitPath = Path.Combine(ReportsPath, $"Aangrenzende straten van {origineleStraat.Naam}, {origineleStraat.Gemeente.Naam}.txt");
            CheckAndDeleteFile(exitPath);
            using (StreamWriter writer = File.CreateText(exitPath))
            {
                writer.WriteLine($"Aantal aangrenzende straten: {straten.Count} \n");
                foreach (Straat str in straten)
                {
                    List<Segment> segmenten = str.Graaf.GetSegmenten();
                    writer.WriteLine($"{str.StraatID}, {str.Naam}, {str.Gemeente.Naam}, {str.Gemeente.Provincie.Naam}");
                    writer.WriteLine($"aantal knopen: {str.Graaf.getKnopen().Count}");
                    writer.WriteLine($"aantal wegsegmenten: {segmenten.Count}");
                    foreach (KeyValuePair<Knoop, List<Segment>> pair in str.Graaf.Map)
                    {
                        Punt temp = pair.Key.SegmentPunt;
                        writer.WriteLine($"Knoop[{pair.Key.KnoopID},[{temp.X};{temp.Y}]]");
                        foreach (Segment seg in pair.Value)
                        {
                            writer.WriteLine($"     [segment:{seg.SegmentID},begin:{seg.BeginKnoop.KnoopID},eind:{seg.EindKnoop.KnoopID}]");
                            foreach (Punt punt in seg.Vertices)
                            {
                                writer.WriteLine($"             ({punt.X};{punt.Y})");
                            }
                        }

                    }
                }
            }
            Console.WriteLine($"Aangrenzende straten Report weggeschreven in {exitPath}\n\n");
        }
    }
}
