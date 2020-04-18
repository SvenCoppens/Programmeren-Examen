using Programmeren_Examen_Tool_1;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Programmeren_Examen_Tool_3
{
    class ReportWriter
    {
        public ReportWriter(string path)
        {
            ReportsPath = path;
        }
        public string ReportsPath { get; set; }
        public void CheckAndDeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public void ProvincieRapport(Provincie prov)
        {
            string path = Path.Combine(ReportsPath,$"Provincie rapport {prov.Naam}.txt");
            CheckAndDeleteFile(path);
            using (StreamWriter writer = File.CreateText(path))
            {
                prov.Gemeenten = prov.Gemeenten.OrderBy(g => g.Naam).ToList();
                foreach (Gemeente gem in prov.Gemeenten)
                {
                    writer.WriteLine($"{gem.Naam}: {gem.Straten.Count}");
                    gem.Straten = gem.Straten.OrderBy(s => s.Lengte).ToList();
                    foreach(Straat straat in gem.Straten)
                    {
                        writer.WriteLine($"     ■   {straat.Naam}, {straat.Lengte}");
                    }
                }
            }
        }
        public void AangrenzendeStratenReport(List<Straat> straten,Straat origineleStraat)
        {
            string path = Path.Combine(ReportsPath, $"Aangrenzende straten van {origineleStraat.Naam}, {origineleStraat.Gemeente.Naam}.txt");
            CheckAndDeleteFile(path);
            using (StreamWriter writer = File.CreateText(path))
            {
                writer.WriteLine($"Aantal aangrenzende straten: {straten.Count}");
                foreach(Straat str in straten)
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
                                foreach(Punt punt in seg.Vertices)
                            {
                                writer.WriteLine($"             ({punt.X};{punt.Y})");
                            }
                        }

                    }
            }
            }
        }
    }
}
