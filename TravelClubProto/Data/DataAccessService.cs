using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace TravelClubProto.Data
{
    public class DataAccessService
    {
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

        public async Task<List<Destination>> GetAllDestinations(DataAccessService DaService)
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
                    d = new Destination(DaService);
                    d.ID = Convert.ToInt32(row["DestinationID"]);
                    d.Hotel = row["Hotel"] as string;
                    d.Location = row["Location"] as string;
                    d.Country = row["Country"] as string;
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

        public async Task<List<Activity>> GetAllActivities(DataAccessService DaService)
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
                    a = new Activity(DaService);
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

        public async Task<List<Vacation>> GetAllVacations(DataAccessService DaService)
        {
            List<Vacation> vacations = new List<Vacation>();
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [dbo].Vacation", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                vacations = FillVacationList(dt, DaService);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(vacations);
        }

        private List<Vacation> FillVacationList(DataTable dt, DataAccessService DaService)
        {
            List<Vacation> vacations = new List<Vacation>();
            Vacation v;
            foreach (DataRow row in dt.Rows)
            {
                v = new Vacation(DaService, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["FK_DestinationID"]));
                v.State = row["State"] as string;
                v.MinNumberOfUsers = Convert.ToInt32(row["MinNumberOfUsers"]);
                v.MinNumberOfUsersExceeded = Convert.ToInt32(row["MinNumberOfUsersExceeded"]);
                v.Dates.Add("ProposalDate", Convert.ToDateTime(row["ProposalDate"]));
                if (!(row["PublishDate"] is DBNull)) v.Dates.Add("PublishDate", Convert.ToDateTime(row["PublishDate"]));
                if (!(row["GracePeriodDate"] is DBNull)) v.Dates.Add("GracePeriodDate", Convert.ToDateTime(row["GracePeriodDate"]));
                if (!(row["CancelDate"] is DBNull)) v.Dates.Add("CancelDate", Convert.ToDateTime(row["CancelDate"]));
                if (!(row["CompletionDate"] is DBNull)) v.Dates.Add("CompletionDate", Convert.ToDateTime(row["CompletionDate"]));
                if (!(row["Deadline"] is DBNull)) v.Dates.Add("Deadline", Convert.ToDateTime(row["Deadline"]));
                if (!(row["GracePeriodLength"] is DBNull)) v.Dates.Add("GracePeriodLength", Convert.ToDateTime(row["GracePeriodLength"]));
                if (!(row["RejectionDate"] is DBNull)) v.Dates.Add("RejectionDate", Convert.ToDateTime(row["RejectionDate"]));
                if (!(row["TravelDate"] is DBNull)) v.Dates.Add("TravelDate", Convert.ToDateTime(row["TravelDate"]));
                if (!(row["LeaveDate"] is DBNull)) v.Dates.Add("LeaveDate", Convert.ToDateTime(row["LeaveDate"]));
                v.Dates.Add("PriceChangeDate", Convert.ToDateTime(row["PriceChangeDate"]));
                v.FK_DestinationID = Convert.ToInt32(row["FK_DestinationID"]);
                v.Description = row["Description"] as string;
                v.ImageLink = row["ImageLink"] as string;
                v.DepartureAirport = row["DepartureAirport"] as string;
                v.TravelBureauWebsiteLink = row["TravelBureauWebsiteLink"] as string;
                vacations.Add(v);
            }
            return vacations;
        }

        public int ClearTable(string table)
        {
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo]." + table;
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }

        public int DeleteCustomers()
        {
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo].Account WHERE Type='Customer'";
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }
    }

   
}
