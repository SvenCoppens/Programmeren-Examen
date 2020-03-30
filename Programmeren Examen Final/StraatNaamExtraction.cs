using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Programmeren_Examen_Final
{
    class StraatNaamExtraction
    {
        public Dictionary<string,Straat> Extract(string path, Dictionary<string, Graaf> straatIdGraafKoppeling)
        {
            List<string> filter = new List<string>();
            Dictionary<string, List<string>> ProvincieIDGemeenteIDLink = new Dictionary<string, List<string>>();
            Dictionary<string, List<String>> gemeenteIdStraatID = new Dictionary<string, List<string>>();

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
                                provincies.Add(provincieId, new Provincie(provincieNaam));
                            }
                            if (ProvincieIDGemeenteIDLink.ContainsKey(provincieId))
                            {
                                ProvincieIDGemeenteIDLink[provincieId].Add(gemeenteId);
                            }
                            else
                            {
                                ProvincieIDGemeenteIDLink.Add(provincieId, new List<string>() { gemeenteId });
                            }
                        }
                    }
                }
            }
            //Gemeente naam en IDaan elkaar koppelen.
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
                    Gemeente temp = new Gemeente(gemeenteId, GemeenteIdEnNaam[gemeenteId]);
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
                    straatIdEnNaam.Add(splitLine[0], splitLine[1]);
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
                        Straat tempStraat = new Straat(ID_Generator.StraatIDToekennen(), naam, tempGraaf, tempGemeente);
                        straten.Add(straatId, tempStraat);
                    }
                }
            }
            int antwerpen = 0;
            int limburg = 0;
            int vlaamsbrab = 0;
            int west = 0;
            int oost = 0;

                foreach(KeyValuePair<string,Gemeente> entry in gemeenten)
            {
                if (entry.Value.Provincie.Naam == "Antwerpen")
                    antwerpen += entry.Value.Straten.Count;
                else if (entry.Value.Provincie.Naam == "Limburg")
                    limburg += entry.Value.Straten.Count;
                else if (entry.Value.Provincie.Naam == "West-Vlaanderen")
                    west += entry.Value.Straten.Count;
                else if (entry.Value.Provincie.Naam == "Oost-Vlaanderen")
                    oost += entry.Value.Straten.Count;
                else vlaamsbrab += entry.Value.Straten.Count;
            }
            return straten;

        }
    }
}
