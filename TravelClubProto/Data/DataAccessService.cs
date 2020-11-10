using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class DataAccessService
    {
        private IConfiguration config;
        public DataAccessService(IConfiguration configuration)
        {
            config = configuration;
        }

        private string ConnectionString
        {
            get
            {
                string _server = config.GetValue<string>("DbConfig:ServerName");
                string _database = config.GetValue<string>("DbConfig:DatabaseName");
                string _username = config.GetValue<string>("DbConfig:UserName");
                string _password = config.GetValue<string>("DbConfig:Password");
                return $"Server={_server};Database={_database};User ID={_username};Password={_password};Trusted_Connection=False;MultipleActiveResultSets=true;";
            }
        }
        public void InsertNewDestination(string hotel, string location)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                string query = "INSERT INTO [dbo].[Destination] (Hotel, Location) VALUES(@Hotel, @Location)";
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Hotel", hotel);
                sqlCommand.Parameters.AddWithValue("@Location", location);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                con.Close();
            }
        }

        public async Task<Destination> GetDestinationByID(int ID)
        {
            Destination matchingDestination = new Destination();
            using (SqlConnection myConnection = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM [dbo].[Destination] WHERE DestinationID=@DestinationID";
                SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                sqlCommand.Parameters.AddWithValue("@DestinationID", ID);
                myConnection.Open();
                using (SqlDataReader oReader = sqlCommand.ExecuteReader())
                {
                    while (oReader.Read())
                    {
                        matchingDestination.ID = Convert.ToInt32(oReader["DestinationID"]);
                        matchingDestination.Hotel = oReader["Hotel"] as string;
                        matchingDestination.Location = oReader["Location"] as string;
                    }

                    myConnection.Close();
                }
            }
            return matchingDestination;
        }

        public async Task<List<Destination>> GetAllDestinations() 
        {
            List<Destination> Destinations = new List<Destination>();
            Destination d;
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter("select * from [dbo].Destination", con);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                d = new Destination();
                d.ID = Convert.ToInt32(row["DestinationID"]);
                d.Hotel = row["Hotel"] as string;
                d.Location = row["Location"] as string;
                Destinations.Add(d);
            }
            return await Task.FromResult(Destinations);
        }


    }
}
