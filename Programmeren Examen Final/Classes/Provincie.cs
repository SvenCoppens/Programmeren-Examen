using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    [Serializable]
    public class Provincie
    {
        public Provincie(int id,string naam)
        {
            Id=id;
            Naam = naam;
            Gemeenten = new List<Gemeente>();
        }
        public List<Gemeente> Gemeenten { get; set; }
        public string Naam { get; set; }
        public int Id { get; set; }
        public void VoegGemeenteToe(Gemeente gemeente)
        {
            Gemeenten.Add(gemeente);
            gemeente.Provincie = this;
        }
        public void VerwijderGemeente(Gemeente gemeente)
        {
            gemeente.Provincie = null;
            Gemeenten.Remove(gemeente);
        }

        public override bool Equals(object obj)
        {
            return obj is Provincie provincie &&
                   EqualityComparer<List<Gemeente>>.Default.Equals(Gemeenten, provincie.Gemeenten) &&
                   Naam == provincie.Naam &&
                   Id == provincie.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Gemeenten, Naam, Id);
        }
        public override string ToString()
        {
            return Naam;
        }
    }
}
