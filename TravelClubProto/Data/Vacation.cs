using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TravelClubProto.Data
{
    
    public class Vacation : IEquatable<Vacation>
    {
        //Hej Bang :D
        public Dictionary<string, DateTime> Dates = new Dictionary<string, DateTime>();

        //public Dictionary<int, decimal> Prices = new Dictionary<int, decimal>();

        public List<int> StretchGoals = new List<int>();
        public List<decimal> Prices = new List<decimal>();
        public Destination Destination { get; set; }
        public int ID { get; set; }
        public int MinNumberOfUsers { get; set; }
        public int MinNumberOfUsersExceeded { get; set; }
        public string Description { get; set; }
        public int FK_DestinationID { get; set; }
        public int FK_PublisherID { get; set; }
        public DateTime GracePeriodLength { get; set; }
        public string ImageLink { get; set; }
        public string DepartureAirport { get; set; }
        public string TravelBureauWebsiteLink { get; set; }
        public DataAccessService DaService { get; set; }
        private VacationAdministrator VacAdmin { get; set; }
        public TravelGroup TravelGroup { get; }
        public async Task<decimal> CurrentPrice()
        {
            int users = await NumberOfJoinedUsers();
            decimal currentPrice = Prices[0];
            for (int i = 0; i < Prices.Count; i++)
            {
                if (users >= StretchGoals[i])
                {
                    currentPrice = Prices[i];
                }
            }

            return await Task.FromResult(currentPrice);
        }

        public async Task<int> NumberOfJoinedUsers()
        {
            int users = (await TravelGroup.GetUserIDsFromRelation(ID, "Joined").ConfigureAwait(false)).Count;
            return await Task.FromResult(users);
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set 
            {
                try
                {
                    VacAdmin.OnStateChange(value, _state);
                }
                catch (InvalidStateException e)
                {
                    Console.WriteLine(e);
                }
                
                _state = value;
                
            }
        }

        //Constructor from insertion form
        public Vacation(List<int> stretchGoals, List<decimal> prices, DataAccessService daService)
        {
            DaService = daService;
            AddPrices(stretchGoals, prices);  
            VacAdmin = new VacationAdministrator(daService, ID);
        }

        //Constructor from Database
        public Vacation(DataAccessService daService, int id, int destinationID)
        {
            ID = id;
            FK_DestinationID = destinationID;
            DaService = daService;
            VacAdmin = new VacationAdministrator(daService, ID);
            TravelGroup = new TravelGroup(ID, daService);
            getDestination(daService);
            getPrices();
        }

        private async void getDestination(DataAccessService daService)
        {
            Destination matchingDestination = new Destination(daService);
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DaService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Destination] WHERE DestinationID=@DestinationID";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@DestinationID", FK_DestinationID);
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

            Destination = matchingDestination;
        }

        private async Task<List<Activity>> GetAllActivitiesByDestID(int FK_DestinationID)
        {
            List<Activity> activities = new List<Activity>();
            Activity a;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].Activities WHERE FK_DestinationID = '{FK_DestinationID}'", con);
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

        private void getPrices()
        {
            Dictionary<int, decimal> prices = new Dictionary<int, decimal>();
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].Prices WHERE FK_VacationID='{ID}'", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    Prices.Add(Convert.ToDecimal(row["Price"]));
                    StretchGoals.Add(Convert.ToInt32(row["JoinAmount"]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        // Everything above get the Vacation from the database
        // Everything below insert the vacation inton the database
        private void AddPrices(List<int> stretchGoals, List<decimal> prices)
        {
            for (int i = 0; i < stretchGoals.Count; i++)
            {
                Prices.Add(prices[i]);
                StretchGoals.Add(stretchGoals[i]);
            }
        }

        public void InsertVacationToDatabase()
        {

            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Vacation] (State, MinNumberOfusers, MinNumberOfUsersExceeded, ProposalDate, Deadline, GracePeriodLength, PriceChangeDate, FK_DestinationID, Description, TravelDate, LeaveDate, ImageLink, DepartureAirport, TravelBureauWebsiteLink, FK_PublisherID)" +
                               " VALUES(@State, @MinNumberOfusers, @MinNumberOfUsersExceeded, @ProposalDate, @Deadline, @GracePeriodLength, @PriceChangeDate, @FK_DestinationID, @Description, @TravelDate, @LeaveDate, @ImageLink, @DepartureAirport, @TravelBureauWebsiteLink, @FK_PublisherID)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@State", State);
                sqlCommand.Parameters.AddWithValue("@MinNumberOfusers", MinNumberOfUsers);
                sqlCommand.Parameters.AddWithValue("@MinNumberOfUsersExceeded", MinNumberOfUsersExceeded);
                sqlCommand.Parameters.AddWithValue("@ProposalDate", Dates["ProposalDate"]);
                sqlCommand.Parameters.AddWithValue("@Deadline", Dates["Deadline"]);
                sqlCommand.Parameters.AddWithValue("@GracePeriodLength", Dates["GracePeriodLength"]);
                sqlCommand.Parameters.AddWithValue("@PriceChangeDate", Dates["PriceChangeDate"]);
                sqlCommand.Parameters.AddWithValue("@FK_DestinationID", FK_DestinationID);
                sqlCommand.Parameters.AddWithValue("@FK_PublisherID", FK_PublisherID);
                sqlCommand.Parameters.AddWithValue("@Description", Description);
                sqlCommand.Parameters.AddWithValue("@TravelDate", Dates["TravelDate"]);
                sqlCommand.Parameters.AddWithValue("@LeaveDate", Dates["LeaveDate"]);
                sqlCommand.Parameters.AddWithValue("@ImageLink", ImageLink);
                sqlCommand.Parameters.AddWithValue("@DepartureAirport", DepartureAirport);
                sqlCommand.Parameters.AddWithValue("@TravelBureauWebsiteLink", TravelBureauWebsiteLink);


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
                getID();
                InsertPricesIntoDatabase();
            }
            
        }
        private void getID()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DaService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[vacation] WHERE ProposalDate=@ProposalDate";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@ProposalDate", Dates["ProposalDate"]);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            ID = Convert.ToInt32(Reader["ID"]);
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InsertPricesIntoDatabase()
        {
            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);

            for (int i = 0; i < Prices.Count; i++)
            {
                try
                {
                    //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                    string query = "INSERT INTO [dbo].[Prices] (Price, JoinAmount, FK_VacationID) VALUES(@Price, @JoinAmount, @FK_VacationID)";
                    //SqlCommand is used to build up commands
                    SqlCommand sqlCommand = new SqlCommand(query, con);
                    con.Open();
                    sqlCommand.Parameters.AddWithValue("@Price", Prices[i]);
                    sqlCommand.Parameters.AddWithValue("@JoinAmount", StretchGoals[i]);
                    sqlCommand.Parameters.AddWithValue("@FK_VacationID", ID);
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
        }
        private async Task<int> GetNumberOfJoinedUsers()
        {
            return (await TravelGroup.GetUserIDsFromRelation(ID, "Joined").ConfigureAwait(false)).Count;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Vacation);
        }

        public bool Equals(Vacation other)
        {
            return other != null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID);
        }
    }
}
