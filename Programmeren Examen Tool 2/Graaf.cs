using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_2
{
    [Serializable]
    class Graaf
    {
        public Graaf(int ID, Dictionary<Knoop,List<Segment>> lijst)
        {
            GraafID = ID;
            Map = lijst;
        }
        public int GraafID { get; set; }
        public Dictionary<Knoop,List<Segment>> Map { get; set; }
        public List<Knoop> getKnopen()
        {
            List<Knoop> temp = new List<Knoop>();
            foreach (Knoop entry in Map.Keys)
                temp.Add(entry);
            return temp;
        }
        public void ShowGraaf()
        {
            Console.WriteLine($"Graaf {GraafID}: ");
            foreach(KeyValuePair<Knoop,List<Segment>> entry in Map)
            {
                
                Console.Write("           " + entry.Key+": [");
                foreach(Segment seg in entry.Value)
                {
                    Console.Write($" {seg},");
                   
                }
                Console.Write("]./n");
            }
        }
        public override string ToString()
        {
            return ($"ID: {GraafID},count: {Map.Count}");
        }
        public override bool Equals(object obj)
        {
            if(obj is Graaf)
            {
                Graaf temp = obj as Graaf;
                return (GraafID == temp.GraafID && Map == temp.Map);
            }
            else
            return false;
        }
        public override int GetHashCode()
        {
            return (GraafID.GetHashCode() ^ Map.GetHashCode());
        }

    }
}
