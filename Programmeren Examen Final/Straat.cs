using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Final
{
    class Straat
    {
        public string Naam { get; set; }
        public List<Knoop> GetKnopen()
        {
            return Graaf.getKnopen();
        }
        public void showStraat()
        {
            Console.Write($"Straat: {StraatID}: {Naam}: [");
            foreach (Knoop entry in GetKnopen())
            {
                Console.Write($" {entry},");
            }
            Console.Write("]");
        }
        public Straat(int Id,string naam, Graaf graaf,Gemeente gemeente)
        {
            Graaf = graaf;
            StraatID = Id;
            Naam = naam;
            Gemeente = gemeente;
            gemeente.Straten.Add(this);
        }
        public Graaf Graaf { get; set; }
        public int StraatID { get; set; }
        public Gemeente Gemeente { get; set; }
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
    }
}
