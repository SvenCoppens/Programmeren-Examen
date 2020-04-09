using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    [Serializable]
    public class Punt
    {
        public Punt(double x,double y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Punt)
            {
                Punt temp = obj as Punt;
                return (X == temp.X && Y == temp.Y);
            }
            else return false;
        }
        public override int GetHashCode()
        {
            return (X.GetHashCode()^Y.GetHashCode());
        }
        public double X { get; set; }
        public double Y { get; set; }
        public override string ToString()
        {
            return($"Punt X:{X},Y{Y};");
        }
    }
}
