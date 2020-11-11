using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TravelClubProto.Data
{
    
    public class Vacation
    {
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
        public DateTime GracePeriodLength { get; set; }
        public DataAccessService DaService { get; set; }
        public VacationAdministrator VacAdmin { get; set; }

        private string _state;
        public string State
        {
            get { return _state; }
            set 
            {
                _state = value;
                VacAdmin.OnStateChange(_state, ID);
            }
        }

        public Vacation(List<int> stretchGoals, List<decimal> prices, DataAccessService daService)
        {
            VacAdmin = new VacationAdministrator();
            AddPrices(stretchGoals, prices);
            DaService = daService;
        }

        public Vacation(DataAccessService daService, int id, int destinationID)
        {
            ID = id;
            FK_DestinationID = destinationID;
            DaService = daService;
            VacAdmin = new VacationAdministrator();
            Destination = getDestination(daService);
            getPrices();
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

        private Destination getDestination(DataAccessService daService)
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
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return matchingDestination;
        }

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
                string query = "INSERT INTO [dbo].[Vacation] (State, MinNumberOfusers, MinNumberOfUsersExceeded, ProposalDate, Deadline, GracePeriodLength, PriceChangeDate, FK_DestinationID, Description)" +
                               " VALUES(@State, @MinNumberOfusers, @MinNumberOfUsersExceeded, @ProposalDate, @Deadline, @GracePeriodLength, @PriceChangeDate, @FK_DestinationID, @Description)";
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
                sqlCommand.Parameters.AddWithValue("@Description", Description);
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
                InsertTravelGroupIntoDatabase();
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

        private void InsertTravelGroupIntoDatabase()
        {
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[TravelGroup] (FK_VacationID) VALUES(@FK_VacationID)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@FK_vacationID", ID);
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
    }
}
