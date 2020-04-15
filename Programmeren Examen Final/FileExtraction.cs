using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class FileExtraction
    {
        public Dictionary<string,Graaf> MaakSegmenten(string path)
        {
            //List<Straat> straten = new List<Straat>();
            //Dictionary<string, Straat> straatVerzameling = new Dictionary<string, Straat>();
            List<Segment> verzamelingSegmenten = new List<Segment>();
            Dictionary<string, Graaf> straatIdGraafKoppeling = new Dictionary<string, Graaf>();
            string line;
            using (StreamReader r1 = new StreamReader(Path.Combine(path, "WRdata.csv")))
            {
                r1.ReadLine();
                while ((line = r1.ReadLine()) != null)
                {

                    //de string onderverdelen in makkelijker te gebruiken variabelen
                    string[] onteledeDelen = line.Split(";");
                    string wegsegmentID = onteledeDelen[0];
                    string segmentCoördinaten = onteledeDelen[1];
                    string beginWegKnoopID = onteledeDelen[4];
                    string eindWegKnoopID = onteledeDelen[5];
                    string linkseStraatnaamId = onteledeDelen[6];
                    string rechtsStraatnaamId = onteledeDelen[7];

                    // coordinaten ontleden
                    string getrimdeCoordinaten = segmentCoördinaten.Substring(12, segmentCoördinaten.Length - 13);
                    getrimdeCoordinaten = getrimdeCoordinaten.Replace(", ", ",");
                    string[] coordinatenOntleding = getrimdeCoordinaten.Split(",");
                    List<double> xCoordinaten = new List<double>();
                    List<double> yCoordinaten = new List<double>();
                    List<Punt> puntenVerzameling = new List<Punt>();

                    foreach (string xEnY in coordinatenOntleding)
                    {
                        //alle punten in comma's veranderen anders is het in het verkeerde formaat.(mogenlijks wegdoen op laptop.
                        string[] tijdelijkeCoordinaat = xEnY.Split(" ");
                        string tmp = tijdelijkeCoordinaat[0].Replace(".", ",");
                        xCoordinaten.Add(double.Parse(tmp));
                        tmp = tijdelijkeCoordinaat[1].Replace(".", ",");
                        yCoordinaten.Add(double.Parse(tmp));
                    }
                    //coordinaten in punten steken
                    for (int i = 0; i < xCoordinaten.Count; i++)
                    {
                        puntenVerzameling.Add(new Punt(xCoordinaten[i], yCoordinaten[i]));
                    }

                    //de knopen voor dit segment aanmaken
                    Knoop beginKnoop = new Knoop(int.Parse(beginWegKnoopID), puntenVerzameling[0]);
                    Knoop eindKnoop = new Knoop(int.Parse(eindWegKnoopID), puntenVerzameling[puntenVerzameling.Count - 1]);


                    // Het Segment aanmaken:
                    verzamelingSegmenten.Add(new Segment(int.Parse(wegsegmentID), beginKnoop, eindKnoop, puntenVerzameling));

                    //het skelet van mijn straat beginnen bouwen zonder de naam: dus een graaf gelinkt aan een ID
                    BouwStraat(straatIdGraafKoppeling, linkseStraatnaamId, beginKnoop, eindKnoop, verzamelingSegmenten[verzamelingSegmenten.Count - 1]);
                    if (linkseStraatnaamId != rechtsStraatnaamId)
                        BouwStraat(straatIdGraafKoppeling, rechtsStraatnaamId, beginKnoop, eindKnoop, verzamelingSegmenten[verzamelingSegmenten.Count - 1]);

                }// einde van de loop

            }//dit is het einde van het document lezen, nu hebben we een hoop graven gekoppeld aan straatnaamIDs.
            return straatIdGraafKoppeling;
        }
        public static void BouwStraat(Dictionary<string, Graaf> StraatBouwer, string straatID, Knoop beginknoop, Knoop eindknoop, Segment segment)
        {
            //de straten zonder naam eruit filteren, zou ik eigenlijk moeten verplaatsen naar erbuiten
            if (straatID != "-9")
            {
                //kijken of ik al zo een straat heb

                if (StraatBouwer.ContainsKey(straatID))
                {
                    //als de straat al bestaat, kijken of die knoop er al in zit, dan toevoegen aan de lijst, anders de knoop met een nieuwe lijst toevoegen
                    if (StraatBouwer[straatID].Map.ContainsKey(beginknoop))
                    {
                        //kijken of het segment zich niet al in de lijst bevindt(bijvoorbeeld als links en rechts dezelfde straat zijn. of als er dubbele data zou zijn
                        if (!StraatBouwer[straatID].Map[beginknoop].Contains(segment))
                            StraatBouwer[straatID].Map[beginknoop].Add(segment);
                    }
                    else
                        StraatBouwer[straatID].Map.Add(beginknoop, new List<Segment> { segment });
                    //hetzelfde maar met de andere knoop.
                    if (StraatBouwer[straatID].Map.ContainsKey(eindknoop))
                    {
                        //kijken of het segment zich niet al in de lijst bevindt(bijvoorbeeld als links en rechts dezelfde straat zijn. of als er dubbele data zou zijn
                        if (!StraatBouwer[straatID].Map[eindknoop].Contains(segment))
                            StraatBouwer[straatID].Map[eindknoop].Add(segment);
                    }
                    else
                        StraatBouwer[straatID].Map.Add(eindknoop, new List<Segment> { segment });
                }
                else
                {
                    Dictionary<Knoop, List<Segment>> tmp = new Dictionary<Knoop, List<Segment>>();
                    tmp.Add(beginknoop, new List<Segment> { segment });
                    tmp.Add(eindknoop, new List<Segment> { segment });
                    Graaf temp = new Graaf(int.Parse(straatID), tmp);
                    StraatBouwer.Add(straatID, temp);
                }
            }
        }
    }
}
