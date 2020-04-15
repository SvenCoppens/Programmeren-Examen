using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Programmeren_Examen_Tool_1;
using System.Data;

namespace Programmeren_Examen_Tool_2
{
    class DataProcessing
    {
        public Belgie Belgie;
        private string _connectionString = @"Data Source=DESKTOP-VCI7746\SQLEXPRESS;Initial Catalog=Prog3Examen;Integrated Security=True";
        public DataProcessing(Belgie belg)
        {
            Belgie = belg;
        }
        private SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            return connection;
        }
        public void CompletelyFillDataBase()
        {
            foreach(Provincie prov in Belgie.Provincies)
            {
                FillDataBaseWithProvincie(prov);
            }

        }
        public void FillDataBaseWithProvincie(Provincie prov)
        {
            AddProvincie(prov);
            foreach (Gemeente gem in prov.Gemeenten)
            {
                FillDatabaseWithGemeente(gem);
            }
        }
        public void FillDatabaseWithGemeente(Gemeente gem)
        {
            AddGemeentePlusLink(gem);
            foreach (Straat str in gem.Straten)
            {
                AddStraatPlusLink(str);
                AddSegmenten(str);
            }
        }
        public void AddProvincie(Provincie prov)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.provincie VALUES(@id, @naam)";
            using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                    command.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    command.CommandText = query;
                    command.Parameters["@id"].Value = prov.Id;
                    command.Parameters["@naam"].Value = prov.Naam;

                    command.ExecuteNonQuery();
                }catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddGemeentePlusLink(Gemeente gem)
        {
            SqlConnection connection = getConnection();
            string gemeenteQuery = "INSERT INTO dbo.gemeente VALUES(@Id,@Naam)";
            string linkQuery = "INSERT INTO dbo.provincieGemeenteLink VALUES(@ProvincieId,@GemeenteId)";
            using(SqlCommand linkCommand = connection.CreateCommand())
            using (SqlCommand gemCommand = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                linkCommand.Transaction = transaction;
                gemCommand.Transaction = transaction;
                try
                {
                    
                    gemCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    gemCommand.Parameters.Add(new SqlParameter("@Naam", SqlDbType.NVarChar));
                    gemCommand.CommandText = gemeenteQuery;
                    gemCommand.Parameters["@Id"].Value = gem.Id;
                    gemCommand.Parameters["@Naam"].Value = gem.Naam;

                    linkCommand.Parameters.Add(new SqlParameter("@ProvincieId", SqlDbType.Int));
                    linkCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                    linkCommand.CommandText = linkQuery;
                    linkCommand.Parameters["@ProvincieId"].Value = gem.Provincie.Id;
                    linkCommand.Parameters["@GemeenteId"].Value = gem.Id;

                    gemCommand.ExecuteNonQuery();
                    linkCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                    Console.WriteLine($"Gemeente met Id {gem.Id} onsuccesvol toegevoegd");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddStraatPlusLink(Straat straat)
        {
            SqlConnection connection = getConnection();
            string straatQuery = "INSERT INTO dbo.straat VALUES(@Id,@Naam)";
            string linkQuery = "INSERT INTO dbo.gemeenteStraatLink VALUES(@StraatId,@GemeenteId)";
            using (SqlCommand linkCommand = connection.CreateCommand())
            using (SqlCommand gemCommand = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                linkCommand.Transaction = transaction;
                gemCommand.Transaction = transaction;
                try
                {

                    gemCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    gemCommand.Parameters.Add(new SqlParameter("@Naam", SqlDbType.NVarChar));
                    gemCommand.CommandText = straatQuery;
                    gemCommand.Parameters["@Id"].Value = straat.StraatID;
                    gemCommand.Parameters["@Naam"].Value = straat.Naam;

                    linkCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                    linkCommand.Parameters.Add(new SqlParameter("@GemeenteId", SqlDbType.Int));
                    linkCommand.CommandText = linkQuery;
                    linkCommand.Parameters["@StraatId"].Value = straat.StraatID;
                    linkCommand.Parameters["@GemeenteId"].Value = straat.Gemeente.Id;

                    gemCommand.ExecuteNonQuery();
                    linkCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                    Console.WriteLine($"straat met ID {straat.StraatID} onsuccesvol toegevoegd");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void LinkSegment(Segment seg, int straatId)
        {
            SqlConnection connection = getConnection();
            string linkQuery = "INSERT INTO dbo.straatSegmenten VALUES(@StraatId,@SegmentId)";
            using (SqlCommand gemCommand = connection.CreateCommand())
            {
                connection.Open();
                try
                {

                    gemCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                    gemCommand.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.NVarChar));
                    gemCommand.CommandText = linkQuery;
                    gemCommand.Parameters["@StraatId"].Value = straatId;
                    gemCommand.Parameters["@SegmentId"].Value = seg.SegmentID;


                    gemCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine($"Segment met ID {seg.SegmentID} onsuccesvol toegevoegd aan straat met id: {straatId}");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddSegmenten(Straat str)
        {
            #region SegmentCollectie
            List<Segment> segmenten = new List<Segment>();
            //unieke segmenten ophalen zodat we niet met dubbele entries belanden.
            foreach(KeyValuePair<Knoop,List<Segment>> collectie in str.Graaf.Map)
            {
                foreach(Segment seg in collectie.Value)
                {
                    if (!segmenten.Contains(seg))
                    {
                        segmenten.Add(seg);
                    }
                }
            }
            #endregion
            for (int i = 0; i < segmenten.Count; i++)
            {
                //segmenten kunnen tot meerdere straten behoren, dus perfect mogenlijk dat dit segment al in de databank is toegevoegd door een andere straat
                if (CheckSegment(segmenten[i]))
                {
                    //hier moet ik enkel de koppeling tussen het segment en de straat aanmaken
                    LinkSegment(segmenten[i],str.StraatID);
                }
                //als het Segment zich nog niet in de databank bevindt.
                else
                {
                    //hier moet ik het segment EN de koppeling aanmaken.
                    AddSegmentWithLink(segmenten[i],str.StraatID);
                }
            }
        }
        public void AddSegmentWithLink(Segment seg,int straatId)
        {
            //controleren of de knoop zich al in de databank bevindt(perfect mogenlijk dat 1 van de 2 er zich al in bevindt maar de andere niet, dus moet met 2 iffen werken)
            #region KnoopControle  
            if (!CheckKnoop(seg.BeginKnoop)){
                AddKnoop(seg.BeginKnoop);
            }
            if (!CheckKnoop(seg.EindKnoop))
            {
                AddKnoop(seg.EindKnoop);
            }
            #endregion

            //het effectieve toevoegen van het Segment in de Databank, Inclusief punten om zo het aantal connecties te minimaliseren. De voorkeur gelegd op dubbele code over een unieke connectie voor elk individueel punt.
            SqlConnection connection = getConnection();
            string queryPunt = "INSERT INTO dbo.punt (X,Y) output INSERTED.ID VALUES(@X, @Y)";
            string querySegment = "INSERT INTO dbo.Segment VALUES(@SegmentId,@BeginKnoopId,@EindKnoopId)";
            string queryVertices = "INSERT INTO dbo.vertices VALUES(@SegmentId,@PuntId)";
            string queryStraatLink = "INSERT INTO dbo.straatSegmenten VALUES(@StraatId,@SegmentId)";

            using(SqlCommand straatCommand = connection.CreateCommand())
            using (SqlCommand puntCommand = connection.CreateCommand())
            using(SqlCommand verticesCommand = connection.CreateCommand())
            using (SqlCommand segmentCommand = connection.CreateCommand())
            {

                puntCommand.CommandText = queryPunt;
                puntCommand.Parameters.Add(new SqlParameter("@X", SqlDbType.Float));
                puntCommand.Parameters.Add(new SqlParameter("@Y", SqlDbType.Float));

                segmentCommand.CommandText = querySegment;
                segmentCommand.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int));
                segmentCommand.Parameters.Add(new SqlParameter("@BeginKnoopId", SqlDbType.Int));
                segmentCommand.Parameters.Add(new SqlParameter("@EindKnoopId", SqlDbType.Int));
                segmentCommand.Parameters["@SegmentId"].Value = seg.SegmentID;
                segmentCommand.Parameters["@BeginKnoopId"].Value = seg.BeginKnoop.KnoopID;
                segmentCommand.Parameters["@EindKnoopId"].Value = seg.EindKnoop.KnoopID;

                verticesCommand.CommandText = queryVertices;
                verticesCommand.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int));
                verticesCommand.Parameters.Add(new SqlParameter("@PuntId", SqlDbType.Int));
                verticesCommand.Parameters["@SegmentId"].Value = seg.SegmentID;

                straatCommand.CommandText = queryStraatLink;
                straatCommand.Parameters.Add(new SqlParameter("@StraatId", SqlDbType.Int));
                straatCommand.Parameters.Add(new SqlParameter("@SegmentId", SqlDbType.Int));
                straatCommand.Parameters["@SegmentId"].Value = seg.SegmentID;
                straatCommand.Parameters["@StraatId"].Value = straatId;
                // segment toevoegen -> punt toevoegen -> vertices vaststellen, zo hoef ik maar 1 loop te doen.
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                puntCommand.Transaction = transaction;
                segmentCommand.Transaction = transaction;
                verticesCommand.Transaction = transaction;
                straatCommand.Transaction = transaction;
                try
                {
                    segmentCommand.ExecuteNonQuery();

                    foreach(Punt punt in seg.Vertices)
                    {
                        puntCommand.Parameters["@X"].Value = punt.X;
                        puntCommand.Parameters["@Y"].Value = punt.Y;
                        int puntId = ((int)puntCommand.ExecuteScalar());

                        verticesCommand.Parameters["@PuntId"].Value = puntId;
                        verticesCommand.ExecuteNonQuery();
                    }
                    straatCommand.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void AddKnoop(Knoop knoop)
        {
            SqlConnection connection = getConnection();
            string knoopQuery = "INSERT INTO dbo.knoop VALUES(@Id, @puntId)";
            string puntQuery = "INSERT INTO dbo.punt (X,Y) output INSERTED.ID VALUES(@X, @Y)";
            using(SqlCommand puntCommand = connection.CreateCommand())
            using (SqlCommand knoopCommand = connection.CreateCommand())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                puntCommand.Transaction = transaction;
                knoopCommand.Transaction = transaction;
                try
                {
                    puntCommand.Parameters.Add(new SqlParameter("@X", SqlDbType.Float));
                    puntCommand.Parameters.Add(new SqlParameter("@Y", SqlDbType.Float));
                    puntCommand.CommandText = puntQuery;
                    puntCommand.Parameters["@X"].Value = knoop.SegmentPunt.X;
                    puntCommand.Parameters["@Y"].Value = knoop.SegmentPunt.Y;
                    int puntId = (int)puntCommand.ExecuteScalar();

                    knoopCommand.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    knoopCommand.Parameters.Add(new SqlParameter("@puntId", SqlDbType.Int));
                    knoopCommand.CommandText = knoopQuery;
                    knoopCommand.Parameters["@Id"].Value = knoop.KnoopID;
                    knoopCommand.Parameters["@puntId"].Value = puntId;

                    knoopCommand.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public bool CheckSegment(Segment seg)
        {
            SqlConnection connection = getConnection();
            string query = "SELECT COUNT(*) AS count FROM dbo.segment WHERE Id=@Id";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.DbType = DbType.Int32;
                paramId.Value = seg.SegmentID;
                command.Parameters.Add(paramId);
                connection.Open();
                
                try
                {
                    IDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int segCount = (int)reader["count"];

                    if (segCount == 0)
                    {
                        return false;
                    }
                    else
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new Exception("Incorrect SQL syntax in CheckSegment");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public bool CheckKnoop(Knoop knoop)
        {
            SqlConnection connection = getConnection();
            string query = "SELECT COUNT(*) AS count FROM dbo.knoop WHERE Id=@Id";
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                SqlParameter paramId = new SqlParameter();
                paramId.ParameterName = "@Id";
                paramId.DbType = DbType.Int32;
                paramId.Value = knoop.KnoopID;
                command.Parameters.Add(paramId);
                connection.Open();
                try
                {
                    IDataReader reader = command.ExecuteReader();
                    reader.Read();
                    int knoopCount = (int)reader["count"];

                    if (knoopCount == 1)
                    {
                        return true;
                    }
                    else return false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new Exception("Incorrect SQL syntax in CheckKnoop");
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
