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
                    BouwGraaf(straatIdGraafKoppeling, linkseStraatnaamId, beginKnoop, eindKnoop, verzamelingSegmenten[verzamelingSegmenten.Count - 1]);
                    if (linkseStraatnaamId != rechtsStraatnaamId)
                        BouwGraaf(straatIdGraafKoppeling, rechtsStraatnaamId, beginKnoop, eindKnoop, verzamelingSegmenten[verzamelingSegmenten.Count - 1]);

                }// einde van de loop

            }//dit is het einde van het document lezen, nu hebben we een hoop graven gekoppeld aan straatnaamIDs.
            return straatIdGraafKoppeling;
        }
        public Dictionary<string, Provincie> BouwStraten(string path, Dictionary<string, Graaf> straatIdGraafKoppeling)
        {
            List<string> filter = new List<string>();
            Dictionary<string, List<string>> ProvincieIDGemeenteIDLink = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> gemeenteIdStraatID = new Dictionary<string, List<string>>();

            Dictionary<string, string> GemeenteIdEnNaam = new Dictionary<string, string>();
            Dictionary<string, string> straatIdEnNaam = new Dictionary<string, string>();

            //datatypes
            Dictionary<string, Provincie> provincies = new Dictionary<string, Provincie>();
            Dictionary<string, Gemeente> gemeenten = new Dictionary<string, Gemeente>();
            Dictionary<string, Straat> straten = new Dictionary<string, Straat>();
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
                            if (!provincies.ContainsKey(provincieId))
                            {
                                provincies.Add(provincieId, new Provincie(int.Parse(provincieId), provincieNaam));
                            }
                            if (ProvincieIDGemeenteIDLink.ContainsKey(provincieId))
                            {
                                ProvincieIDGemeenteIDLink[provincieId].Add(gemeenteId);
                            }
                            else
                            {
                                ProvincieIDGemeenteIDLink.Add(provincieId, new List<string>());
                                ProvincieIDGemeenteIDLink[provincieId].Add(gemeenteId);
                            }
                        }
                    }
                }
            }
            //Gemeente naam en ID aan elkaar koppelen.
            using (StreamReader sr = new StreamReader(Path.Combine(path, "WRGemeentenaam.csv")))
            {
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splitLine = line.Split(";");
                    if (splitLine[2] == "nl")
                    {
                        GemeenteIdEnNaam.Add(splitLine[1], splitLine[3]);
                    }
                }

            }
            //gemeenten aanmaken en koppelen aan hun provincie.
            foreach (KeyValuePair<string, List<string>> entry in ProvincieIDGemeenteIDLink)
            {
                foreach (string gemeenteId in entry.Value)
                {
                    Gemeente temp = new Gemeente(int.Parse(gemeenteId), GemeenteIdEnNaam[gemeenteId]);
                    provincies[entry.Key].VoegGemeenteToe(temp);
                    gemeenten.Add(gemeenteId, temp);
                }
            }
            //straatnaamIDs koppelen aan de straatnamen
            using (StreamReader sr = new StreamReader(Path.Combine(path, "WRstraatnamen.csv")))
            {
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
                    gemeente.Value.Provincie.Gemeenten.Remove(gemeente.Value);
                }
            }
            return provincies;

        }
        public static void BouwGraaf(Dictionary<string, Graaf> StraatBouwer, string straatID, Knoop beginknoop, Knoop eindknoop, Segment segment)
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
