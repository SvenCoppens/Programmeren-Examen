using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Programmeren_Examen_Tool_1
{
    class StraatNaamExtraction
    {
        public Dictionary<string,Provincie> Extract(string path, Dictionary<string, Graaf> straatIdGraafKoppeling)
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
                                provincies.Add(provincieId, new Provincie(int.Parse(provincieId),provincieNaam));
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
            foreach(KeyValuePair<string,Gemeente> gemeente in gemeenten)
            {
                if (gemeente.Value.Straten.Count == 0)
                {
                    gemeente.Value.Provincie.Gemeenten.Remove(gemeente.Value);
                }
            }
                return provincies;

        }
    }
}
