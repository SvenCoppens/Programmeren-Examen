using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Programmeren_Examen_Tool_1;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace Programmeren_Examen_Tool_3
{
    class DatabaseQueryer
    {
        private string _connectionString = @"Data Source=DESKTOP-VCI7746\SQLEXPRESS;Initial Catalog=Prog3Examen;Integrated Security=True";
        private SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            return connection;
        }

        //opdracht 2
        public Straat ReturnStraatVoorId(int straatId)
        {
            SqlConnection connection = getConnection();
            string gemeenteQuery = "SELECT * FROM dbo.gemeente JOIN gemeenteStraatLink ON gemeenteStraatLink.GemeenteId=gemeente.Id WHERE gemeenteStraatLink.StraatId = @StraatId";
            string provincieQuery = "SELECT * FROM dbo.provincie JOIN provincieGemeenteLink ON provincie.Id = provincieGemeenteLink.provincieId WHERE gemeenteId = @GemeenteId";
            using (SqlCommand provincieCommand = connection.CreateCommand())
            using (SqlCommand gemeenteCommand = connection.CreateCommand())
            {
                gemeenteCommand.CommandText = gemeenteQuery;
                gemeenteCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                gemeenteCommand.Parameters["@StraatId"].Value = straatId;

                provincieCommand.CommandText = provincieQuery;
                provincieCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));

                connection.Open();
                try
                {
                    SqlDataReader reader = gemeenteCommand.ExecuteReader();
                    reader.Read();
                    string gemeenteNaam = (string)reader["Naam"];
                    int gemeenteId = (int)reader["gemeenteId"];
                    reader.Close();

                    provincieCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    reader = provincieCommand.ExecuteReader();
                    reader.Read();
                    int provincieId = (int)reader["Id"];
                    string provincieNaam = (string)reader["Naam"];
                    reader.Close();

                    Provincie provincie = new Provincie(provincieId, provincieNaam);
                    Gemeente gemeente = new Gemeente(gemeenteId, gemeenteNaam);
                    provincie.VoegGemeenteToe(gemeente);
                    return ReturnStraatInGemeente(straatId, gemeente);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }

        }                                      
        //opdracht 3
        public Straat ReturnStraatVoorNaam(string gemeenteNaam, string straatnaam)
        {
            SqlConnection connection = getConnection();
            string gemeenteQuery = "SELECT * FROM dbo.gemeente JOIN gemeenteStraatLink ON gemeenteStraatLink.GemeenteId=gemeente.Id WHERE Naam = @GemeenteNaam";
            string straatQuery = "SELECT Id FROM dbo.straat JOIN gemeenteStraatLink ON gemeenteStraatLink.StraatId = straat.Id WHERE straat.Naam = @StraatNaam AND GemeenteId = @GemeenteId";
            string provincieQuery = "SELECT * FROM dbo.provincie JOIN provincieGemeenteLink ON provincie.Id = provincieGemeenteLink.provincieId WHERE gemeenteId = @GemeenteId";
            using (SqlCommand StraatCommand = connection.CreateCommand())
            using (SqlCommand provincieCommand = connection.CreateCommand())
            using (SqlCommand gemeenteCommand = connection.CreateCommand())
            {
                StraatCommand.CommandText = straatQuery;
                StraatCommand.Parameters.Add(new SqlParameter("@StraatNaam", SqlDbType.NVarChar));
                StraatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                StraatCommand.Parameters["@StraatNaam"].Value = straatnaam;

                gemeenteCommand.CommandText = gemeenteQuery;
                gemeenteCommand.Parameters.Add(new SqlParameter("@GemeenteNaam", SqlDbType.NVarChar));
                gemeenteCommand.Parameters["@GemeenteNaam"].Value = gemeenteNaam;

                provincieCommand.CommandText = provincieQuery;
                provincieCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));

                connection.Open();
                try
                {
                    SqlDataReader reader = gemeenteCommand.ExecuteReader();
                    reader.Read();
                    int gemeenteId = (int)reader["GemeenteId"];
                    reader.Close();

                    StraatCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    reader = StraatCommand.ExecuteReader();
                    reader.Read();
                    int straatId = (int)reader["Id"];
                    reader.Close();

                    provincieCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    reader = provincieCommand.ExecuteReader();
                    reader.Read();
                    int provincieId = (int)reader["Id"];
                    string provincieNaam = (string)reader["Naam"];
                    reader.Close();

                    Provincie provincie = new Provincie(provincieId, provincieNaam);
                    Gemeente gemeente = new Gemeente(gemeenteId, gemeenteNaam);
                    provincie.VoegGemeenteToe(gemeente);
                    return ReturnStraatInGemeente(straatId, gemeente);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }           
        public Straat ReturnStraatInGemeente(int straatId, Gemeente gemeente)
        {
            string straatQuery = "SELECT * FROM dbo.straat WHERE Id=@StraatId";
            string segmentQuery = "SELECT * FROM dbo.segment JOIN straatSegmenten ON straatSegmenten.SegmentId = segment.Id WHERE straatId = @StraatId";
            string knoopQuery = "SELECT punt.X,punt.Y FROM dbo.knoop JOIN punt ON knoop.PuntId = Punt.Id WHERE knoop.Id = @KnoopId";
            string puntQuery = "SELECT * FROM dbo.punt JOIN dbo.vertices ON punt.Id=vertices.PuntId WHERE vertices.segmentId = @segmentId ORDER BY vertices.PuntId";
            SqlConnection connection = getConnection();
            using (SqlCommand puntCommand = connection.CreateCommand())
            using (SqlCommand knoopCommand = connection.CreateCommand())
            using (SqlCommand segmentCommand = connection.CreateCommand())
            using (SqlCommand straatCommand = connection.CreateCommand())
            {
                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                straatCommand.Parameters["@StraatId"].Value = straatId;

                segmentCommand.CommandText = segmentQuery;
                segmentCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                segmentCommand.Parameters["@StraatId"].Value = straatId;

                knoopCommand.CommandText = knoopQuery;
                knoopCommand.Parameters.Add(new SqlParameter("@KnoopId", SqlDbType.Int));

                puntCommand.CommandText = puntQuery;
                puntCommand.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int));
                connection.Open();

                try
                {
                    List<Segment> segmenten = new List<Segment>();

                    SqlDataReader reader = straatCommand.ExecuteReader();
                    reader.Read();
                    string straatnaam = (string)reader["Naam"];
                    reader.Close();

                    reader = segmentCommand.ExecuteReader();
                    List<int> segmentIds = new List<int>();
                    List<int> beginKnoopIds = new List<int>();
                    List<int> eindKnoopIds = new List<int>();
                    while (reader.Read())
                    {
                        segmentIds.Add((int)reader["Id"]);
                        beginKnoopIds.Add((int)reader["BeginKnoopId"]);
                        eindKnoopIds.Add((int)reader["EindKnoopId"]);
                    }
                    reader.Close();

                    //hier beginnen we te loopen om onze segmenten op te bouwen.
                    for (int i = 0; i < segmentIds.Count; i++)
                    {
                        knoopCommand.Parameters["@KnoopId"].Value = beginKnoopIds[i];
                        reader = knoopCommand.ExecuteReader();
                        reader.Read();
                        double beginX = (double)reader["X"];
                        double beginY = (double)reader["Y"];
                        reader.Close();

                        knoopCommand.Parameters["@KnoopId"].Value = eindKnoopIds[i];
                        reader = knoopCommand.ExecuteReader();
                        reader.Read();
                        double eindX = (double)reader["X"];
                        double eindY = (double)reader["Y"];
                        reader.Close();

                        puntCommand.Parameters["@SegmentId"].Value = segmentIds[i];
                        reader = puntCommand.ExecuteReader();
                        Knoop beginKnoop = new Knoop(beginKnoopIds[i], new Punt(beginX, beginY));
                        Knoop eindKnoop = new Knoop(eindKnoopIds[i], new Punt(eindX, eindY));
                        List<Punt> punten = new List<Punt>() { beginKnoop.SegmentPunt };
                        while (reader.Read())
                        {
                            punten.Add(new Punt((double)reader["X"], (double)reader["Y"]));
                        }
                        reader.Close();
                        punten.Add(eindKnoop.SegmentPunt);
                        segmenten.Add(new Segment(segmentIds[i], beginKnoop, eindKnoop, punten));
                    }

                    Graaf tempGraaf = BouwGraaf(segmenten, straatId);

                    Straat returnStraat = new Straat(straatId, straatnaam, tempGraaf, gemeente);
                    return returnStraat;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        //opdracht 5
        public Provincie ReturnProvincieCompleteVoorNaam(string provincieNaam)
        {
            SqlConnection connection = getConnection();
            string provincieQuery = "SELECT * FROM dbo.provincie WHERE Naam = @ProvincieNaam";

            using (SqlCommand provincieCommand = connection.CreateCommand())
            {
                provincieCommand.CommandText = provincieQuery;
                provincieCommand.Parameters.Add(new SqlParameter("@ProvincieNaam", SqlDbType.NVarChar));
                provincieCommand.Parameters["@ProvincieNaam"].Value = provincieNaam;
                connection.Open();
                try
                {
                    SqlDataReader reader = provincieCommand.ExecuteReader();
                    reader.Read();
                    int provincieId = (int)reader["Id"];

                    return ReturnProvincieCompleteVoorId(provincieId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }


            }

        }
        public Provincie ReturnProvincieCompleteVoorId(int provincieId)
        {
            SqlConnection connection = getConnection();
            string provincieQuery = "SELECT * FROM dbo.provincie JOIN provincieGemeenteLink ON provincie.Id = provincieGemeenteLink.provincieId WHERE provincieId = @Id";

            using (SqlCommand provincieCommand = connection.CreateCommand())
            {
                provincieCommand.CommandText = provincieQuery;
                provincieCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                provincieCommand.Parameters["@Id"].Value = provincieId;
                connection.Open();
                try
                {
                    SqlDataReader reader = provincieCommand.ExecuteReader();
                    reader.Read();
                    string provincieNaam = (string)reader["Naam"];
                    Provincie provincie = new Provincie(provincieId, provincieNaam);
                    List<int> gemeenteIds = new List<int> { (int)reader["GemeenteId"] };
                    while (reader.Read())
                    {
                        gemeenteIds.Add((int)reader["GemeenteId"]);
                    }

                    foreach (int gemeenteId in gemeenteIds)
                    {
                        provincie.VoegGemeenteToe(ReturnGemeenteVoorGemeenteId(gemeenteId));
                    }
                    return provincie;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }


            }

        }

        //hulpmethode
        public Gemeente ReturnGemeenteVoorGemeenteId(int gemeenteId)
        {
            string gemeenteQuery = "SELECT * FROM dbo.gemeente WHERE Id=@GemeenteId";
            string straatQuery = "SELECT * FROM dbo.straat JOIN gemeenteStraatLink ON gemeenteStraatLink.StraatId = straat.Id WHERE gemeenteStraatLink.GemeenteId = @GemeenteId";

            SqlConnection connection = getConnection();
            using (SqlCommand gemeenteCommand = connection.CreateCommand())
            using (SqlCommand straatCommand = connection.CreateCommand())
            {
                gemeenteCommand.CommandText = gemeenteQuery;
                gemeenteCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                gemeenteCommand.Parameters["@GemeenteId"].Value = gemeenteId;

                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                straatCommand.Parameters["@GemeenteId"].Value = gemeenteId;

                connection.Open();

                try
                {
                    SqlDataReader reader = gemeenteCommand.ExecuteReader();
                    reader.Read();
                    string gemeenteNaam = (string)reader["Naam"];
                    reader.Close();
                    List<int> straatIds = new List<int>();
                    List<string> straatNamen = new List<string>();
                    reader = straatCommand.ExecuteReader();
                    Gemeente gemeente = new Gemeente(gemeenteId, gemeenteNaam);
                    while (reader.Read())
                    {
                        straatIds.Add((int)reader["Id"]);
                        straatNamen.Add((string)reader["Naam"]);
                    }
                    reader.Close();
                    for (int index = 0; index < straatIds.Count; index++)
                    {
                        ReturnStraatInGemeente(straatIds[index], gemeente);

                    }
                    return gemeente;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }




            }
        }
        //extra ter vervollediging
        public Gemeente ReturnGemeenteVoorGemeenteNaam(string gemeenteNaam)
        {
            string gemeenteQuery = "SELECT * FROM dbo.gemeente WHERE Naam=@gemeenteNaam";
            string straatQuery = "SELECT * FROM dbo.straat JOIN gemeenteStraatLink ON gemeenteStraatLink.StraatId = straat.Id WHERE gemeenteStraatLink.GemeenteId = @GemeenteId";

            string segmentQuery = "SELECT * FROM dbo.segment JOIN straatSegmenten ON straatSegmenten.SegmentId = segment.Id WHERE straatId = @StraatId";
            string knoopQuery = "SELECT punt.X,punt.Y FROM dbo.knoop JOIN punt ON knoop.PuntId = Punt.Id WHERE knoop.Id = @KnoopId";
            string puntQuery = "SELECT * FROM dbo.punt JOIN dbo.vertices ON punt.Id=vertices.PuntId WHERE vertices.segmentId = @segmentId";

            SqlConnection connection = getConnection();
            using (SqlCommand knoopCommand = connection.CreateCommand())
            using (SqlCommand puntCommand = connection.CreateCommand())
            using (SqlCommand segmentCommand = connection.CreateCommand())
            using (SqlCommand gemeenteCommand = connection.CreateCommand())
            using (SqlCommand straatCommand = connection.CreateCommand())
            {
                gemeenteCommand.CommandText = gemeenteQuery;
                gemeenteCommand.Parameters.Add(new SqlParameter("@GemeenteNaam", SqlDbType.NVarChar));
                gemeenteCommand.Parameters["@GemeenteNaam"].Value = gemeenteNaam;

                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));

                segmentCommand.CommandText = segmentQuery;
                segmentCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));

                knoopCommand.CommandText = knoopQuery;
                knoopCommand.Parameters.Add(new SqlParameter("@KnoopId", SqlDbType.Int));

                puntCommand.CommandText = puntQuery;
                puntCommand.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int));

                connection.Open();

                try
                {
                    SqlDataReader reader = gemeenteCommand.ExecuteReader();
                    reader.Read();
                    int gemeenteId = (int)reader["Id"];
                    reader.Close();
                    List<int> straatIds = new List<int>();
                    List<string> straatNamen = new List<string>();
                    straatCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    reader = straatCommand.ExecuteReader();
                    Gemeente gemeente = new Gemeente(gemeenteId, gemeenteNaam);
                    while (reader.Read())
                    {
                        straatIds.Add((int)reader["Id"]);
                        straatNamen.Add((string)reader["Naam"]);
                    }
                    reader.Close();
                    for (int index = 0; index < straatIds.Count; index++)
                    {
                        segmentCommand.Parameters["@StraatId"].Value = straatIds[index];
                        reader = segmentCommand.ExecuteReader();
                        List<Segment> segmenten = new List<Segment>();
                        List<int> segmentIds = new List<int>();
                        List<int> beginKnoopIds = new List<int>();
                        List<int> eindKnoopIds = new List<int>();
                        while (reader.Read())
                        {
                            segmentIds.Add((int)reader["Id"]);
                            beginKnoopIds.Add((int)reader["BeginKnoopId"]);
                            eindKnoopIds.Add((int)reader["EindKnoopId"]);
                        }
                        reader.Close();

                        //hier beginnen we te loopen om onze segmenten op te bouwen.
                        for (int i = 0; i < segmentIds.Count; i++)
                        {
                            knoopCommand.Parameters["@KnoopId"].Value = beginKnoopIds[i];
                            reader = knoopCommand.ExecuteReader();
                            reader.Read();
                            double beginX = (double)reader["X"];
                            double beginY = (double)reader["Y"];
                            reader.Close();

                            knoopCommand.Parameters["@KnoopId"].Value = eindKnoopIds[i];
                            reader = knoopCommand.ExecuteReader();
                            reader.Read();
                            double eindX = (double)reader["X"];
                            double eindY = (double)reader["Y"];
                            reader.Close();

                            puntCommand.Parameters["@SegmentId"].Value = segmentIds[i];
                            reader = puntCommand.ExecuteReader();
                            List<Punt> punten = new List<Punt>();
                            while (reader.Read())
                            {
                                punten.Add(new Punt((double)reader["X"], (double)reader["Y"]));
                            }
                            reader.Close();
                            Knoop beginKnoop = new Knoop(beginKnoopIds[i], new Punt(beginX, beginY));
                            Knoop eindKnoop = new Knoop(eindKnoopIds[i], new Punt(eindX, eindY));
                            segmenten.Add(new Segment(segmentIds[i], beginKnoop, eindKnoop, punten));
                        }

                        Graaf tempGraaf = BouwGraaf(segmenten, straatIds[index]);
                        Straat tempstraat = new Straat(straatIds[index], straatNamen[index], tempGraaf, gemeente);

                    }



                    return gemeente;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }




            }
        }

        //opdracht 4
        public List<string> StraatNamenVoorGemeenteNaam(string gemeenteNaam)
        {
            string gemeenteQuery = "SELECT * FROM dbo.gemeente WHERE gemeente.Naam = @GemeenteNaam";
            SqlConnection connection = getConnection();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = gemeenteQuery;
                SqlParameter parameter = new SqlParameter("@GemeenteNaam", SqlDbType.NVarChar);
                parameter.Value = gemeenteNaam;
                command.Parameters.Add(parameter);
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int gemeenteId = (int)reader["Id"];
                    reader.Close();

                    List<string> straatNamen = HaalStraatNamenVoorGemeenteId(gemeenteId);
                    straatNamen.Sort();
                    return straatNamen;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        private List<string> HaalStraatNamenVoorGemeenteId(int gemeenteId)
        {
            List<string> straatnamen = new List<string>();
            string straatQuery = "SELECT * FROM dbo.straat JOIN gemeenteStraatLink on straat.Id = gemeenteStraatLink.StraatId WHERE gemeenteStraatLink.GemeenteId = @GemeenteId";
            SqlConnection connection = getConnection();
            using (SqlCommand straatCommand = connection.CreateCommand())
            {

                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));

                connection.Open();
                try
                {

                    straatCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    SqlDataReader reader = straatCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        straatnamen.Add((string)reader["Naam"]);
                    }
                    straatnamen.Sort();
                    return straatnamen;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        //opdracht 6
        public List<Straat> ReturnStratenAangrenzendAanId(int straatId)
        {
            List<int> knoopIds = new List<int>();
            List<int> straatIds = new List<int>();
            List<Straat> straten = new List<Straat>();
            SqlConnection connection = getConnection();
            string segmentQuery = "SELECT * FROM segment JOIN straatSegmenten ON segment.Id = straatSegmenten.SegmentId WHERE StraatId=@Id";
            string beginKnoopQuery = "SELECT * FROM segment JOIN straatSegmenten ON segment.Id = straatSegmenten.SegmentId WHERE BeginKnoopId = @Id";
            string EindKnoopQuery = "SELECT * FROM segment JOIN straatSegmenten ON segment.Id = straatSegmenten.SegmentId WHERE EindKnoopId = @Id";

            using (SqlCommand eindKnoopCommand = connection.CreateCommand())
            using (SqlCommand beginKnoopCommand = connection.CreateCommand())
            using (SqlCommand command = connection.CreateCommand())
            {
                eindKnoopCommand.CommandText = EindKnoopQuery;
                eindKnoopCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));

                beginKnoopCommand.CommandText = beginKnoopQuery;
                beginKnoopCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));

                command.CommandText = segmentQuery;
                command.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                command.Parameters["@Id"].Value = straatId;

                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int beginKnoopId = (int)reader["BeginKnoopId"];
                        int eindKnoopId = (int)reader["EindKnoopId"];

                        if (!knoopIds.Contains(beginKnoopId))
                            knoopIds.Add(beginKnoopId);
                        if (!knoopIds.Contains(eindKnoopId))
                            knoopIds.Add(eindKnoopId);
                    }
                    reader.Close();
                    foreach (int knoop in knoopIds)
                    {
                        beginKnoopCommand.Parameters["@Id"].Value = knoop;
                        reader = beginKnoopCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            int tempstraatId = (int)reader["StraatId"];
                            if (!straatIds.Contains(tempstraatId) && tempstraatId != straatId)
                                straatIds.Add(tempstraatId);

                        }
                        reader.Close();
                        eindKnoopCommand.Parameters["@Id"].Value = knoop;
                        reader = eindKnoopCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            int tempstraatId = (int)reader["StraatId"];
                            if (!straatIds.Contains(tempstraatId) && tempstraatId != straatId)
                                straatIds.Add(tempstraatId);

                        }
                        reader.Close();
                    }
                    foreach (int straat in straatIds)
                    {
                        straten.Add(ReturnStraatVoorId(straat));
                    }
                    return straten;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }

            }
        }


        //opdracht 1
        public List<int> ReturnStraatIdsVoorGemeenteNaam(string gemeenteNaam)
        {
            List<int> straatIds = new List<int>();
            string query = "SELECT * FROM gemeenteStraatLink JOIN gemeente ON gemeente.Id = gemeenteStraatLink.GemeenteId WHERE gemeente.Naam = @GemeenteNaam";
            SqlConnection connection = getConnection();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@GemeenteNaam", SqlDbType.NVarChar));
                command.Parameters["@GemeenteNaam"].Value = gemeenteNaam;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        straatIds.Add((int)reader["StraatId"]);
                    }
                    reader.Close();
                    return straatIds;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public List<int> ReturnStraatIdsVoorGemeenteId(int gemeenteId)
        {
            List<int> straatIds = new List<int>();
            string query = "SELECT * FROM gemeenteStraatLink WHERE GemeenteId = @GemeenteId";
            SqlConnection connection = getConnection();
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                command.Parameters["@GemeenteId"].Value = gemeenteId;
                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        straatIds.Add((int)reader["StraatId"]);
                    }
                    reader.Close();
                    return straatIds;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        //hulpmethode
        public Graaf BouwGraaf(List<Segment> segmenten, int straatId)
        {
            Dictionary<Knoop, List<Segment>> map = new Dictionary<Knoop, List<Segment>>();

            foreach (Segment seg in segmenten)
            {
                if (map.ContainsKey(seg.BeginKnoop))
                {
                    map[seg.BeginKnoop].Add(seg);
                }
                else
                    map.Add(seg.BeginKnoop, new List<Segment> { seg });
                if (map.ContainsKey(seg.EindKnoop))
                {
                    map[seg.EindKnoop].Add(seg);
                }
                else
                    map.Add(seg.EindKnoop, new List<Segment> { seg });
            }
            Graaf result = new Graaf(straatId, map);
            return result;
        }
    }
}
