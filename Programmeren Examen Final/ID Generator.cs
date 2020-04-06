using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class ID_Generator
    {
        private static int CurrentGraafID { get; set; } = 100;
        private static int CurrentStraatID { get; set; } = 100;
        public static int GraafIDToekennen()
        {
            return CurrentGraafID++;
        }
        public static int StraatIDToekennen()
        {
            return CurrentStraatID++;
        }
    }
}
