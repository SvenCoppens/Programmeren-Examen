using Programmeren_Examen_Tool_1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Programmeren_Examen_Tool_3
{
    interface iDataFetcher
    {
        //opdracht 1
        public List<int> ReturnStraatIdsVoorGemeenteNaam(string gemeenteNaam);
        //opdracht 2
        public Straat ReturnStraatVoorId(int straatId);
        //opdracht 3
        public Straat ReturnStraatVoorNaam(string gemeenteNaam, string straatnaam);
        //opdracht 4
        public List<string> StraatNamenVoorGemeenteNaam(string gemeenteNaam);
        //opdracht 5
        public Provincie ReturnProvincieCompleteVoorNaam(string provincieNaam);
        //opdracht 6
        public List<Straat> ReturnStratenAangrenzendAanId(int straatId);

    }
}
