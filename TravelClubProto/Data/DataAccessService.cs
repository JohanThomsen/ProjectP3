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
        public bool LoggedIn { get; set; }
        public int LoggedInAccountID { get; set; }
        public string LoggedInType { get; set; }


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
                vacations = await FillVacationList(dt);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(vacations);
        }

        public async Task<List<Vacation>> FillVacationList(DataTable dt)
        {
            List<Vacation> vacations = new List<Vacation>();
            Vacation v;
            foreach (DataRow row in dt.Rows)
            {
                v = new Vacation(this, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["FK_DestinationID"]));
                v.State = row["State"] as string;
                v.MinNumberOfUsers = Convert.ToInt32(row["MinNumberOfUsers"]);
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
                v.FK_PublisherID = Convert.ToInt32(row["FK_PublisherID"]);
                v.Description = row["Description"] as string;
                v.ImageLink = row["ImageLink"] as string;
                v.DepartureAirport = row["DepartureAirport"] as string;
                v.TravelBureauWebsiteLink = row["TravelBureauWebsiteLink"] as string;
                vacations.Add(v);
            }
            return await Task.FromResult(vacations);
        }

        public async Task<Vacation> GetVacationByID(DataAccessService DaService, int vacID)
        {
            Vacation v = null;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].Vacation WHERE ID='{vacID}'", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                v = await FillVacation(dt, DaService);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(v);
        }

        private Task<Vacation> FillVacation(DataTable dt, DataAccessService DaService)
        {
            Vacation v = null;
            foreach (DataRow row in dt.Rows)
            {
                v = new Vacation(DaService, Convert.ToInt32(row["ID"]), Convert.ToInt32(row["FK_DestinationID"]));
                v.State = row["State"] as string;
                v.MinNumberOfUsers = Convert.ToInt32(row["MinNumberOfUsers"]);
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
                v.FK_PublisherID = Convert.ToInt32(row["FK_PublisherID"]);
                v.Description = row["Description"] as string;
                v.ImageLink = row["ImageLink"] as string;
                v.DepartureAirport = row["DepartureAirport"] as string;
                v.TravelBureauWebsiteLink = row["TravelBureauWebsiteLink"] as string;
            }
            return Task.FromResult(v);
        }

        public async Task<int> FindAccountInDatabase(string email, string password, DataAccessService daService)
        {
            Account user = new Customer(email, password, daService);
            daService.LoggedIn = false;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(daService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Account] WHERE Email=@Email AND Password=@Password";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@Email", email);
                    sqlCommand.Parameters.AddWithValue("@Password", password);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            user.ID = Convert.ToInt32(Reader["AccountID"]);
                            if (Reader["Email"] as string == user.Email && Reader["Password"] as string == user.Password)
                            {
                                daService.LoggedIn = true;
                                daService.LoggedInAccountID = user.ID;

                                myConnection.Close();
                                return await Task.FromResult(user.ID);
                            }
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return await Task.FromResult(-1);
        }

        public async Task<Account> GetAccountById(int id)
        {
            Account foundAccount = null;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Account] WHERE AccountID=@id";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@id", id);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {

                            switch (Reader["Type"] as string)
                            {
                                case "Customer":
                                    foundAccount = new Customer(id, Reader["Email"] as string, Reader["Password"] as string, this);
                                    break;
                                case "TravelBureau":
                                    foundAccount = new TravelBureau(id, Reader["Email"] as string, Reader["Password"] as string, this);
                                    break;
                                case "TravelClub":
                                    foundAccount = new TravelClub(id, Reader["Email"] as string, Reader["Password"] as string, this);
                                    break;
                                default:
                                    break;
                            }
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return await Task.FromResult(foundAccount);
        }

        public async Task<Destination> getDestinationByID(int id)
        {
            Destination matchingDestination = new Destination(this);
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Destination] WHERE DestinationID=@DestinationID";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@DestinationID", id);
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
                            matchingDestination.Country = Reader["Country"] as string;
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            matchingDestination.Activities = await GetAllActivitiesByDestID(matchingDestination.ID);

            return await Task.FromResult(matchingDestination);
        }


        public async Task<List<Activity>> GetAllActivitiesByDestID(int FK_DestinationID)
        {
            List<Activity> activities = new List<Activity>();
            Activity a;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].Activities WHERE FK_DestinationID = '{FK_DestinationID}'", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    a = new Activity(this);
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
