using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Programmeren_Examen_Tool_1;
using System.Data;

namespace Programmeren_Examen_Tool_3
{
    class DatabaseRequester
    {
        private string _connectionString = @"Data Source=DESKTOP-VCI7746\SQLEXPRESS;Initial Catalog=Prog3Examen;Integrated Security=True";
        private SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            return connection;
        }
        public List<int> ReturnStraatIdsVoorGemeente(string gemeenteNaam)
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
        public Straat ReturnStraatVoorStraatId(int straatId)
        {
            List<Punt> straatIds = new List<Punt>();
            string straatnaam = "";

            string straatQuery = "SELECT * FROM dbo.straat WHERE Id=@StraatId";
            string gemeenteQuery = "SELECT * FROM dbo.gemeente JOIN gemeenteStraatLink ON gemeenteStraatLink.GemeenteId=gemeente.Id WHERE gemeenteStraatLink.StraatId = @StraatId";
            string provincieQuery = "SELECT * FROM dbo.provincie JOIN provincieGemeenteLink ON provincie.Id = provincieGemeenteLink.provincieId WHERE gemeenteId = @GemeenteId";
            string segmentQuery = "SELECT * FROM dbo.segment JOIN straatSegmenten ON straatSegmenten.SegmentId = segment.Id WHERE straatId = @StraatId";
            string knoopQuery = "SELECT punt.X,punt.Y FROM dbo.knoop JOIN punt ON knoop.PuntId = Punt.Id WHERE knoop.Id = @KnoopId";
            string puntQuery = "SELECT * FROM dbo.punt JOIN dbo.vertices ON punt.Id=vertices.PuntId WHERE vertices.segmentId = @segmentId";
            SqlConnection connection = getConnection();
            using (SqlCommand puntCommand = connection.CreateCommand())
            using (SqlCommand knoopCommand = connection.CreateCommand())
            using (SqlCommand segmentCommand = connection.CreateCommand())
            using (SqlCommand provincieCommand = connection.CreateCommand())
            using (SqlCommand gemeenteCommand = connection.CreateCommand())
            using (SqlCommand straatCommand = connection.CreateCommand())
            {
                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                straatCommand.Parameters["@StraatId"].Value = straatId;

                gemeenteCommand.CommandText = gemeenteQuery;
                gemeenteCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                gemeenteCommand.Parameters["@StraatId"].Value = straatId;

                provincieCommand.CommandText = provincieQuery;
                provincieCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));

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
                    straatnaam = (string)reader["Naam"];
                    reader.Close();

                    reader = gemeenteCommand.ExecuteReader();
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

                    Graaf tempGraaf = StelGraafOp(segmenten,straatId);

                    Provincie tempProv = new Provincie(provincieId,provincieNaam);
                    Gemeente tempGemeente = new Gemeente(gemeenteId, gemeenteNaam);
                    tempProv.VoegGemeenteToe(tempGemeente);
                    Straat returnStraat = new Straat(straatId, straatnaam, tempGraaf, tempGemeente);
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
        public Straat ReturnStraatVoorStraatNaam(string gemeenteNaam, string straatNaam)
        {
            int straatId = 0;
            string gemeenteQuery = "SELECT * FROM dbo.gemeente JOIN gemeenteStraatLink on gemeente.Id = gemeenteStraatLink.gemeenteId WHERE gemeente.Naam = @GemeenteNaam";
            string straatQuery = "SELECT * FROM dbo.straat JOIN gemeenteStraatLink on straat.Id = gemeenteStraatLink.StraatId WHERE gemeenteStraatLink.GemeenteId = @GemeenteId AND straat.Naam = @StraatNaam";
            SqlConnection connection = getConnection();
            using (SqlCommand straatCommand = connection.CreateCommand())
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = gemeenteQuery;
                command.Parameters.Add(new SqlParameter("@GemeenteNaam", SqlDbType.NVarChar));
                command.Parameters["@GemeenteNaam"].Value = gemeenteNaam;

                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@StraatNaam", SqlDbType.NVarChar));
                straatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                straatCommand.Parameters["@StraatNaam"].Value = straatNaam;

                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int gemeenteId = (int)reader["Id"];
                    reader.Close();

                    straatCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    reader = straatCommand.ExecuteReader();
                    reader.Read();
                    straatId = (int)reader["Id"];

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
            return ReturnStraatVoorStraatId(straatId);
        }
        public List<string> StraatNamenVoorGemeente(string gemeenteNaam)
        {
            List<string> straatnamen = new List<string>();
            string gemeenteQuery = "SELECT * FROM dbo.gemeente JOIN gemeenteStraatLink on gemeente.Id = gemeenteStraatLink.gemeenteId WHERE gemeente.Naam = @GemeenteNaam";
            string straatQuery = "SELECT * FROM dbo.straat JOIN gemeenteStraatLink on straat.Id = gemeenteStraatLink.StraatId WHERE gemeenteStraatLink.GemeenteId = @GemeenteId";
            SqlConnection connection = getConnection();
            using (SqlCommand straatCommand = connection.CreateCommand())
            using (SqlCommand command = connection.CreateCommand())
            {

                command.CommandText = gemeenteQuery;
                command.Parameters.Add(new SqlParameter("@GemeenteNaam", SqlDbType.NVarChar));
                command.Parameters["@GemeenteNaam"].Value = gemeenteNaam;

                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));

                connection.Open();
                try
                {
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int gemeenteId = (int)reader["Id"];
                    reader.Close();

                    straatCommand.Parameters["@GemeenteId"].Value = gemeenteId;
                    reader = straatCommand.ExecuteReader();
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
        public Provincie ProvincieRapport(string provincieNaam)
        {
            SqlConnection connection = getConnection();
            string provincieQuery = "SELECT * FROM dbo.provincie JOIN provincieGemeenteLink ON provincie.Id = provincieGemeenteLink.provincieId WHERE Naam = @ProvincieNaam";

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
                    Provincie provincie = new Provincie(provincieId ,provincieNaam);
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
        public List<Straat> AangrenzendeStraten(int straatId)
        {
            List<int> knoopIds = new List<int>();
            List<int> straatIds = new List<int>();
            List<Straat> straten = new List<Straat>();
            SqlConnection connection = getConnection();
            string segmentQuery = "SELECT * FROM segment JOIN straatSegmenten ON segment.Id = straatSegmenten.SegmentId WHERE StraatId=@Id";
            string beginKnoopQuery = "SELECT * FROM segment JOIN straatSegmenten ON segment.Id = straatSegmenten.SegmentId WHERE BeginKnoopId = @Id";
            string EindKnoopQuery = "SELECT * FROM segment JOIN straatSegmenten ON segment.Id = straatSegmenten.SegmentId WHERE EindKnoopId = @Id";

            using(SqlCommand eindKnoopCommand = connection.CreateCommand())
            using(SqlCommand beginKnoopCommand = connection.CreateCommand())
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
                    foreach(int knoop in knoopIds)
                    {
                        beginKnoopCommand.Parameters["@Id"].Value = knoop;
                        reader = beginKnoopCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            int tempstraatId = (int)reader["StraatId"];
                            if (!straatIds.Contains(tempstraatId))
                                straatIds.Add(tempstraatId);

                        }
                        reader.Close();
                        eindKnoopCommand.Parameters["@Id"].Value = knoop;
                        reader = eindKnoopCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            int tempstraatId = (int)reader["StraatId"];
                            if (!straatIds.Contains(tempstraatId))
                                straatIds.Add(tempstraatId);

                        }
                        reader.Close();
                    }
                    foreach(int straat in straatIds)
                    {
                        straten.Add(ReturnStraatVoorStraatId(straat));
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
        public Graaf StelGraafOp(List<Segment> segmenten,int straatId)
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
        public Gemeente ReturnGemeenteVoorGemeenteId(int gemeenteId)
        {
            string gemeenteQuery = "SELECT * FROM dbo.gemeente WHERE Id=@GemeenteId";
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
                gemeenteCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                gemeenteCommand.Parameters["@GemeenteId"].Value = gemeenteId;

                straatCommand.CommandText = straatQuery;
                straatCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                straatCommand.Parameters["@GemeenteId"].Value = gemeenteId;

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

                        Graaf tempGraaf = StelGraafOp(segmenten,straatIds[index]);
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
    }
}
