using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Final
{
    class Gemeente
    {
        public Gemeente(string id,string naam)
        {
            Id = id;
            Naam = naam;
            Straten = new List<Straat>();
        }
        public string Naam { get; set; }
        public string Id { get; set; }
        public Provincie Provincie { get; set; }
        public List<Straat> Straten { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Gemeente gemeente &&
                   Naam == gemeente.Naam &&
                   Id == gemeente.Id &&
                   EqualityComparer<Provincie>.Default.Equals(Provincie, gemeente.Provincie) &&
                   EqualityComparer<List<Straat>>.Default.Equals(Straten, gemeente.Straten);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Naam, Id, Provincie, Straten);
        }
    }
}
