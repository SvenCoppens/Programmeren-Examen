using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_2
{
    [Serializable]
    class Belgie
    {
        List<Provincie> Provincies { get; set; }
        public Belgie(List<Provincie> provincies)
        {
            Provincies = provincies;
        }
    }
}
