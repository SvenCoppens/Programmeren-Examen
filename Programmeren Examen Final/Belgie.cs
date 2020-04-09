using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    [Serializable]
    public class Belgie
    {
        public List<Provincie> Provincies { get; set; }
        public Belgie(List<Provincie> provincies)
        {
            Provincies = provincies;
        }
    }
}
