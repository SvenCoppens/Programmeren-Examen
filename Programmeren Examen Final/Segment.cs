using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Final
{
    class Segment
    {
        public Segment(int iD, Knoop beginKnoop, Knoop eindKnoop, List<Punt> punten)
        {
            SegmentID = iD;
            BeginKnoop = beginKnoop;
            EindKnoop = eindKnoop;
            Vertices= punten;
        }

        public Knoop BeginKnoop { get; set; }
        public Knoop EindKnoop { get; set; }
        public int SegmentID { get; set; }
        public List<Punt> Vertices { get; set; }
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
    }
}
