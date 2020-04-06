using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    [Serializable]
    class Segment
    {
        public Segment(int iD, Knoop beginKnoop, Knoop eindKnoop, List<Punt> punten)
        {
            SegmentID = iD;
            BeginKnoop = beginKnoop;
            EindKnoop = eindKnoop;
            Vertices= punten;
            BerekenLengte();
        }

        public Knoop BeginKnoop { get; set; }
        public Knoop EindKnoop { get; set; }
        public int SegmentID { get; set; }
        public List<Punt> Vertices { get; set; }
        public double Lengte { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is Segment)
            {
                Segment temp = obj as Segment;
                return (SegmentID == temp.SegmentID && BeginKnoop == temp.BeginKnoop && EindKnoop == temp.EindKnoop && Vertices == temp.Vertices);
            }
            else return false;
        }
        public override int GetHashCode()
        {
            return (SegmentID.GetHashCode()^BeginKnoop.GetHashCode()^EindKnoop.GetHashCode()^Vertices.GetHashCode());
        }
        public override string ToString()
        {
            return $"Segment: {SegmentID}, BeginKnoop: {BeginKnoop.KnoopID}, Eindknoop: {EindKnoop.KnoopID}";
        }
        public void BerekenLengte()
        {
            double lengte = 0;
            for(int i = 1; i < Vertices.Count; i++)
            {
                double tempLengte = Math.Sqrt(Math.Pow(Vertices[i].X - Vertices[i - 1].X, 2) + (Math.Pow(Vertices[i].Y - Vertices[i - 1].Y, 2)));
                lengte += tempLengte;
            }
            Lengte = lengte;
        }
    }
}
