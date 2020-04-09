using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    [Serializable]
    public class Knoop
    {
        public Knoop(int ID, Punt punt)
        {
            SegmentPunt = punt;
            KnoopID = ID;
        }
        public int KnoopID { get; set; }
        public Punt SegmentPunt { get; set; }
        public override string ToString()
        {
            return ($"KnoopID:{KnoopID}, X:{SegmentPunt.X}, Y:{SegmentPunt.Y}");
        }
        public override int GetHashCode()
        {
            return KnoopID.GetHashCode() ^ SegmentPunt.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is Knoop)
            {
                Knoop temp = obj as Knoop;
                return (KnoopID == temp.KnoopID && SegmentPunt == temp.SegmentPunt);
            }
            else return false;
        }
    }
}
