using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Programmeren_Examen_Tool_1
{
    class FileExtraction
    {
        public Belgie ExtractFromFiles(string path)
        {
            Dictionary<string, Graaf> graafIdkoppeling = MaakGraven(path);
            Dictionary<string, Provincie> provincies = BouwStraten(path, graafIdkoppeling);
            List<Provincie> provinciesLijst = new List<Provincie>();
            foreach (var x in provincies)
            {
                provinciesLijst.Add(x.Value);
            }
            Belgie belg = new Belgie(provinciesLijst);
            return belg;

        }
        public Dictionary<string,Graaf> MaakGraven(string path)
        {
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
                    // (217368.75 181577.0159999989, 217400.1099999994 181499.5159999989) voorbeeld
                    string getrimdeCoordinaten = segmentCoördinaten.Substring(12, segmentCoördinaten.Length - 13);
                    getrimdeCoordinaten = getrimdeCoordinaten.Replace(", ", ",");
                    string[] coordinatenOntleding = getrimdeCoordinaten.Split(",");

                    //collecties aanmaken
                    List<double> xCoordinaten = new List<double>();
                    List<double> yCoordinaten = new List<double>();
                    List<Punt> puntenVerzameling = new List<Punt>();

                    //alle punten in comma's veranderen anders is het in het verkeerde formaat.
                    //alle combinaties in specifieke x en y verzamelingen steken
                    foreach (string xEnY in coordinatenOntleding)
                    {
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
                    Segment seg = new Segment(int.Parse(wegsegmentID), beginKnoop, eindKnoop, puntenVerzameling);

                    //het skelet van mijn straat beginnen bouwen zonder de naam: dus een graaf gelinkt aan een ID
                    BouwGravenVerzameling(straatIdGraafKoppeling, linkseStraatnaamId, seg);
                    if (linkseStraatnaamId != rechtsStraatnaamId)
                        BouwGravenVerzameling(straatIdGraafKoppeling, rechtsStraatnaamId, seg);

                }// einde van de loop

            }//dit is het einde van het document lezen, nu hebben we een hoop graven gekoppeld aan straatnaamIDs.
            return straatIdGraafKoppeling;
        }
        public Dictionary<string, Provincie> BouwStraten(string path, Dictionary<string, Graaf> straatIdGraafKoppeling)
        {
            #region setup
            List<string> filter = new List<string>();
            //ID links
            Dictionary<string, List<string>> provincieIDGemeenteIDLink = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> gemeenteIdStraatID = new Dictionary<string, List<string>>();

            //ID en naam links
            Dictionary<string, string> gemeenteIdEnNaam = new Dictionary<string, string>();
            Dictionary<string, string> straatIdEnNaam = new Dictionary<string, string>();

            //datatypes
            Dictionary<string, Provincie> provincies = new Dictionary<string, Provincie>();
            Dictionary<string, Gemeente> gemeenten = new Dictionary<string, Gemeente>();
            Dictionary<string, Straat> straten = new Dictionary<string, Straat>();
            #endregion
            #region file extracting
            using (StreamReader sr = new StreamReader(Path.Combine(path, "ProvincieIDsVlaanderen.csv")))
            {
                string[] filterIDs = sr.ReadLine().Split(",");
                foreach (string id in filterIDs)
                {
                    filter.Add(id);
                }
            }

            //provincies aanmaken en gemeenteIDs koppelen
            using (StreamReader sr = new StreamReader(Path.Combine(path, "ProvincieInfo.csv")))
            {
                    //gemeenteId; provincieId; taalCodeProvincieNaam; provincieNaam
                    //1;1;nl;Antwerpen
                sr.ReadLine();
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(";");
                    string provincieId = splitLine[1];
                    string provincieNaam = splitLine[3];
                    string gemeenteId = splitLine[0];
                    if (splitLine[2] == "nl")
                    {
                        if (filter.Contains(provincieId))
                        {
                            //kijken of mijn verzameling provincies deze al bevat
                            if (!provincies.ContainsKey(provincieId))
                            {
                                provincies.Add(provincieId, new Provincie(int.Parse(provincieId), provincieNaam));
                            }

                            //mijn verzameling van provincieIDs gelinkt aan gemeenteIDs in orde brengen.
                            if (provincieIDGemeenteIDLink.ContainsKey(provincieId))
                            {
                                provincieIDGemeenteIDLink[provincieId].Add(gemeenteId);
                            }
                            else
                            {
                                provincieIDGemeenteIDLink.Add(provincieId, new List<string>());
                                provincieIDGemeenteIDLink[provincieId].Add(gemeenteId);
                            }
                        }
                    }
                }
            }
            //Gemeente naam en ID aan elkaar koppelen.
            using (StreamReader sr = new StreamReader(Path.Combine(path, "WRGemeentenaam.csv")))
            {
                //gemeenteNaamId; gemeenteId; taalCodeGemeenteNaam; gemeenteNaam
                //1; 1; nl; Aartselaar
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(";");
                    if (splitLine[2] == "nl")
                    {
                        gemeenteIdEnNaam.Add(splitLine[1], splitLine[3]);
                    }
                }

            }
            //gemeenten aanmaken en koppelen aan hun provincie.
            foreach (KeyValuePair<string, List<string>> entry in provincieIDGemeenteIDLink)
            {
                foreach (string gemeenteId in entry.Value)
                {
                    Gemeente temp = new Gemeente(int.Parse(gemeenteId), gemeenteIdEnNaam[gemeenteId]);
                    provincies[entry.Key].VoegGemeenteToe(temp);
                    gemeenten.Add(gemeenteId, temp);
                }
            }
            //straatnaamIDs koppelen aan de straatnamen
            using (StreamReader sr = new StreamReader(Path.Combine(path, "WRstraatnamen.csv")))
            {
                //EXN; LOS
                //-9; NULL
                //1; Acacialaan
                sr.ReadLine();
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(";");
                    straatIdEnNaam.Add(splitLine[0], splitLine[1].Trim());
                }

            }
            //gemeenteIDs koppelen aan de straatnaamIDs
            using (StreamReader sr = new StreamReader(Path.Combine(path, "WRGemeenteID.csv")))
            {
                //straatNaamId; gemeenteId
                //1; 1
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(";");
                    if (gemeenten.ContainsKey(splitLine[1]))
                    {
                        if (gemeenteIdStraatID.ContainsKey(splitLine[1]))
                            gemeenteIdStraatID[splitLine[1]].Add(splitLine[0]);
                        else
                        {
                            gemeenteIdStraatID.Add(splitLine[1], new List<string>());
                            gemeenteIdStraatID[splitLine[1]].Add(splitLine[0]);
                        }
                    }
                }
            }
            #endregion
            //effectieve straten aanmaken
            foreach (KeyValuePair<string, List<string>> entry in gemeenteIdStraatID)
            {
                foreach (string straatId in entry.Value)
                {
                    if (straatIdGraafKoppeling.ContainsKey(straatId))
                    { //184418 was not present in the dictionary error gegeven dus dit geschreven
                      //de gemeente wordt in de Straat constructor in order gebracht zodat die zijn lijst van straten correct blijft.
                        string naam = straatIdEnNaam[straatId];
                        Graaf tempGraaf = straatIdGraafKoppeling[straatId];
                        Gemeente tempGemeente = gemeenten[entry.Key];
                        Straat tempStraat = new Straat(int.Parse(straatId), naam, tempGraaf, tempGemeente);
                        straten.Add(straatId, tempStraat);
                    }
                }
            }

            //Lege Provincies weghalen uit de verzameling gemeenten
            foreach (KeyValuePair<string, Gemeente> gemeente in gemeenten)
            {
                if (gemeente.Value.Straten.Count == 0)
                {
                    gemeente.Value.Provincie.VerwijderGemeente(gemeente.Value);
                }
            }
            return provincies;

        }
        public static void BouwGravenVerzameling(Dictionary<string, Graaf> idGraafVerzameling, string straatID, Segment segment)
        {

            //de straten zonder naam eruit filteren, zou ik eigenlijk moeten verplaatsen naar erbuiten
            if (straatID != "-9")
            {
                Knoop beginKnoop = segment.BeginKnoop;
                Knoop eindKnoop = segment.EindKnoop;
                //kijken of ik al zo een straat heb
                if (idGraafVerzameling.ContainsKey(straatID))
                {
                    //als de straat al bestaat, kijken of die knoop er al in zit, dan toevoegen aan de lijst, anders de knoop met een nieuwe lijst toevoegen
                    if (idGraafVerzameling[straatID].Map.ContainsKey(beginKnoop))
                    {
                        //kijken of het segment zich niet al in de lijst bevindt(bijvoorbeeld als links en rechts dezelfde straat zijn. of als er dubbele data zou zijn
                        if (!idGraafVerzameling[straatID].Map[beginKnoop].Contains(segment))
                            idGraafVerzameling[straatID].Map[beginKnoop].Add(segment);
                    }
                    else
                        idGraafVerzameling[straatID].Map.Add(beginKnoop, new List<Segment> { segment });
                    //hetzelfde maar met de andere knoop.
                    if (idGraafVerzameling[straatID].Map.ContainsKey(eindKnoop))
                    {
                        //kijken of het segment zich niet al in de lijst bevindt(bijvoorbeeld als links en rechts dezelfde straat zijn. of als er dubbele data zou zijn
                        if (!idGraafVerzameling[straatID].Map[eindKnoop].Contains(segment))
                            idGraafVerzameling[straatID].Map[eindKnoop].Add(segment);
                    }
                    else
                        idGraafVerzameling[straatID].Map.Add(eindKnoop, new List<Segment> { segment });
                }
                else
                {
                    Dictionary<Knoop, List<Segment>> nieuweMapEntry = new Dictionary<Knoop, List<Segment>>();
                    nieuweMapEntry.Add(beginKnoop, new List<Segment> { segment });
                    nieuweMapEntry.Add(eindKnoop, new List<Segment> { segment });
                    Graaf nieuweGraaf = new Graaf(int.Parse(straatID), nieuweMapEntry);
                    idGraafVerzameling.Add(straatID, nieuweGraaf);
                }
            }
        }
    }
}
