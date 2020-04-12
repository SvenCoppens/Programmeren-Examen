using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_3
{
    public class ID_Generator
    {
        private static int CurrentGraafID { get; set; } = 100;
        public static int GraafIDToekennen()
        {
            return CurrentGraafID++;
        }
    }
}
