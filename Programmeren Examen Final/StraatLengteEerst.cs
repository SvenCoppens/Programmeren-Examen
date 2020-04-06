using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class StraatLengteEerst : Comparer<Straat>
    {
        public override int Compare( Straat x, Straat y)
        {
            if (x.Lengte == y.Lengte)
            {
                return x.Naam.CompareTo(y.Naam);
            }
            else return x.Lengte.CompareTo(y.Lengte);
        }
    }
}
