using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TravelClubProto.Data
{
    
    public class Vacation
    {
        public Dictionary<string, DateTime> Dates = new Dictionary<string, DateTime>();

        private Dictionary<int, decimal> Prices = new Dictionary<int, decimal>();
        private Destination Destination { get; set; }
        public int ID { get; set; }
        public int MinNumberOfUsers { get; set; }
        public int MinNumberOfUsersExceeded { get; set; }
        private DateTime GracePeriodLength { get; set; }

        public DataAccessService DaService { get; set; }
        public int FK_DestinationID { get; set; }
        public VacationData VacData { get; set; }

        public string Description { get; set; }
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


        public Vacation(DateTime proposalDate, DateTime deadline, List<int> stretchGoals, List<decimal> prices, VacationData vacData)
        {
            VacData = vacData;
            VacAdmin = new VacationAdministrator(vacData);
            Dates.Add("ProposalDate", proposalDate);
            Dates.Add("Deadline", deadline);
            AddPrices(stretchGoals, prices);
        }

        public Vacation(VacationData vacData, List<int> stretchGoals, List<decimal> prices, DataAccessService daService)
        {
            VacData = vacData;
            VacAdmin = new VacationAdministrator(vacData);
            AddPrices(stretchGoals, prices);
            DaService = daService;
        }

        private void AddPrices(List<int> stretchGoals, List<decimal> prices)
        {
            for (int i = 0; i < stretchGoals.Count; i++)
            {
                Prices.Add(stretchGoals[i], prices[i]);
            }
        }

        public void InsertVacationToDatabase()
        {

            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Vacation] (State, MinNumberOfusers, MinNumberOfUsersExceeded, ProposalDate, Deadline, GracePeriodLength, PriceChangeDate, FK_DestinationID)" +
                               " VALUES(@State, @MinNumberOfusers, @MinNumberOfUsersExceeded, @ProposalDate, @Deadline, @GracePeriodLength, @PriceChangeDate, @FK_DestinationID)";
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

            foreach (KeyValuePair<int, decimal> pricePair in Prices)
            {
                try
                {
                    //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                    string query = "INSERT INTO [dbo].[Prices] (Price, JoinAmount, FK_VacationID) VALUES(@Price, @JoinAmount, @FK_VacationID)";
                    //SqlCommand is used to build up commands
                    SqlCommand sqlCommand = new SqlCommand(query, con);
                    con.Open();
                    sqlCommand.Parameters.AddWithValue("@Price", pricePair.Value);
                    sqlCommand.Parameters.AddWithValue("@JoinAmount", pricePair.Key);
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
