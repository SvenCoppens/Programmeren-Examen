using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    [Serializable]
    public class Straat
    {
        public string Naam { get; set; }
        public List<Knoop> GetKnopen()
        {
            return Graaf.getKnopen();
        }
        public List<Segment> GetSegmenten()
        {
            return Graaf.GetSegmenten();
        }
        public void ShowStraat()
        {
            List<Segment> segmenten = Graaf.GetSegmenten();
            Console.WriteLine($"{StraatID}, {Naam}, {Gemeente.Naam}, {Gemeente.Provincie.Naam}");
            Console.WriteLine($"aantal knopen: {Graaf.getKnopen().Count}");
            Console.WriteLine($"aantal wegsegmenten: {segmenten.Count}");
            foreach (KeyValuePair<Knoop, List<Segment>> pair in Graaf.Map)
            {
                Punt temp = pair.Key.SegmentPunt;
                Console.WriteLine($"Knoop[{pair.Key.KnoopID},[{temp.X};{temp.Y}]]");
                foreach (Segment seg in pair.Value)
                {
                    Console.WriteLine($"     [segment:{seg.SegmentID},begin:{seg.BeginKnoop.KnoopID},eind:{seg.EindKnoop.KnoopID}]");
                    foreach (Punt punt in seg.Vertices)
                    {
                        Console.WriteLine($"             ({punt.X};{punt.Y})");
                    }
                }

            }
        }
        public Straat(int Id,string naam, Graaf graaf,Gemeente gemeente)
        {
            Graaf = graaf;
            StraatID = Id;
            Naam = naam;
            Gemeente = gemeente;
            gemeente.Straten.Add(this);
            BerekenLengte();
        }
        public Graaf Graaf { get; set; }
        public int StraatID { get; set; }
        public Gemeente Gemeente { get; set; }
        public double Lengte { get; set; }
        public override string ToString()
        {
            return $"Straat: {Naam}, in: {Gemeente.Naam},Graaf: {Graaf.GraafID}";
        }
        public override bool Equals(object obj)
        {
            if (obj is Straat)
            {
                Straat temp = obj as Straat;
                return (StraatID == temp.StraatID && Naam == temp.Naam && Graaf == temp.Graaf && Gemeente==temp.Gemeente);
            }
            else return false;
        }
        public void BerekenLengte()
        {
            double lengte = 0;
            List<Segment> uniekeSegmenten = new List<Segment>();
            foreach (KeyValuePair<Knoop, List<Segment>> segmentCombo in Graaf.Map)
            {
                foreach (Segment seg in segmentCombo.Value)
                {
                    if (!uniekeSegmenten.Contains(seg))
                    {
                        uniekeSegmenten.Add(seg);
                    }
                }
            }
            foreach (Segment seg in uniekeSegmenten)
            {
                lengte += seg.Lengte;
            }
            Lengte = lengte;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Naam, Graaf, StraatID, Gemeente, Lengte);
        }
    }
}
