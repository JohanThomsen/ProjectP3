using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace TravelClubProto.Data
{
    public class DataAccessService
    {

        //public Boolean LoggedIn { get { } set {  } }
        public bool LoggedIn { get; set; }


        private IConfiguration config;
        //IConfiguration gets key/value from appsettings.json
        public DataAccessService(IConfiguration configuration)
        {
            config = configuration;
        }

        //ConnectionString uses the data received from "Iconfiguration config" above which will be used to connect the project code with the azure sql database
        public string ConnectionString
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

            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Destination] (Hotel, Location) VALUES(@Hotel, @Location)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Hotel", hotel);
                sqlCommand.Parameters.AddWithValue("@Location", location);
                //The built commands are executed
                sqlCommand.ExecuteNonQuery();
            }
            //Catches the error and prints it
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
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Destination] WHERE DestinationID=@DestinationID";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@DestinationID", ID);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            matchingDestination.ID = Convert.ToInt32(Reader["DestinationID"]);
                            matchingDestination.Hotel = Reader["Hotel"] as string;
                            matchingDestination.Location = Reader["Location"] as string;
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return await Task.FromResult(matchingDestination);
        }

        public async Task<List<Destination>> GetAllDestinations()
        {
            List<Destination> Destinations = new List<Destination>();
            Destination d;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [dbo].Destination", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    d = new Destination();
                    d.ID = Convert.ToInt32(row["DestinationID"]);
                    d.Hotel = row["Hotel"] as string;
                    d.Location = row["Location"] as string;
                    Destinations.Add(d);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(Destinations);
        }

        public void DeleteDestinationByLocation(string location)
        {
            try
            {
                using (var sc = new SqlConnection(ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo].Destination WHERE Location=@location";
                    cmd.Parameters.AddWithValue("@location", location);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<List<Activity>> GetAllActivities()
        {
            List<Activity> activities = new List<Activity>();
            Activity a;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [dbo].Activities", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    a = new Activity();
                    a.ID = Convert.ToInt32(row["ActivityID"]);
                    a.Type = row["Type"] as string;
                    activities.Add(a);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(activities);
        }
    }
}
