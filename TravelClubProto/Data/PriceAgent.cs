using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class PriceAgent
    {
        public int PriceAgentID { get; set; }
        public int FK_CustomerID { get; set; }
        public string DestinationPreference { get; set; }
        public List<string> ActPreferences { get; set; }
        public DataAccessService DaService { get; set; }
        public decimal MaxPrice { get; set; }

        public PriceAgent( int customerID, DataAccessService daService, string destinationPreference, decimal maxPrice, List<string> actPreferences)
        {
            FK_CustomerID = customerID;
            DaService = daService;
            DestinationPreference = destinationPreference;
            MaxPrice = maxPrice;
            ActPreferences = actPreferences;

        }

        public PriceAgent(int customerID, int priceAgentID, DataAccessService daService)
        {
            PriceAgentID = priceAgentID;
            FK_CustomerID = customerID;
            DaService = daService;
            GetPreferences();
        }

        private void GetPreferences()
        {
            ActPreferences = new List<string>();
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DaService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[PriceAgentPreferences] WHERE FK_PriceAgentID=@FK_PriceAgentID";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@FK_PriceAgentID", PriceAgentID);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            switch (Reader["PreferenceType"] as string)
                            {
                                case "MaxPrice":
                                    MaxPrice = Convert.ToInt32(Reader["Preference"]);
                                    break;
                                case "Destination":
                                    DestinationPreference = Reader["Preference"] as string;
                                    break;
                                case "Activity":
                                    ActPreferences.Add(Reader["Preference"] as string);
                                    break;
                                default:
                                    throw new ArgumentException("Prefencetype is invalid");
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
        }

        public List<Vacation> LookForRelevantPriceChanges()
        {
            List<Vacation> allVacs = DaService.GetAllVacations(DaService).GetAwaiter().GetResult();
            List<Vacation> RecentlyChangedVacations = allVacs.Where(v => v.Dates["PriceChangeDate"] > DateTime.Now.AddDays(-1)).ToList();
            List<Vacation> RelevantChangedVacations = new List<Vacation>();
            List<int> ignorableVacations = GetIgnorableVacations().GetAwaiter().GetResult();
            foreach (Vacation vac in RecentlyChangedVacations)
            {
                if (vac.Destination.CountryLocationHotel == DestinationPreference && vac.CurrentPrice().GetAwaiter().GetResult() <= MaxPrice)
                {
                    bool contains = true;
                    foreach (string act in ActPreferences)
                    {
                        bool tempContains = false;
                        foreach (Activity VacAct in vac.Destination.Activities)
                        {
                            
                            if (act == VacAct.Type)
                            {
                                tempContains = true;
                            }
                        }
                        if (tempContains == false)
                        {
                            contains = false;
                            break;
                        }
                    }

                    
                    if (contains == true && (!(ignorableVacations.Contains(vac.ID))))
                    {
                        RelevantChangedVacations.Add(vac);
                        InsertIntoDiscardedVacations(vac.ID);
                    }
                }
            }
            return RelevantChangedVacations;
        }

        private void InsertIntoDiscardedVacations(int vacID)
        {
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                DateTime dateMixer = DateTime.Now;
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[DiscardedVacations] (FK_PriceAgentID, FK_VacationID) VALUES(@FK_PriceAgentID, @FK_VacationID)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@FK_PriceAgentID", PriceAgentID);
                sqlCommand.Parameters.AddWithValue("@FK_VacationID", vacID);
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

        private async Task<List<int>> GetIgnorableVacations()
        {
            List<int> vacations = new List<int>();
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].DiscardedVacations WHERE FK_PriceAgentID='{PriceAgentID}'", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    vacations.Add(Convert.ToInt32(row["FK_VacationID"]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(vacations);
        }

        public async void InsertIntoDatabase()
        {
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                DateTime dateMixer = DateTime.Now;
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[PriceAgent] (FK_AccountID, CreationDate) VALUES(@FK_CustomerID, @CreationDate)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@FK_CustomerID", FK_CustomerID);
                sqlCommand.Parameters.AddWithValue("@CreationDate", dateMixer);
                //The built commands are executed
                sqlCommand.ExecuteNonQuery();

                PriceAgentID = await GetAgentID(dateMixer);
                
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
            InsertPreferences(PriceAgentID);
        }

        private void InsertPreferences(int PriceAgentID)
        {
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
            string queryPrice = "INSERT INTO [dbo].[PriceAgentPreferences] (Preference ,FK_PriceAgentID, PreferenceType) VALUES(@Preference, @FK_PriceAgentID, @PreferenceType)";
            //SqlCommand is used to build up commands
            SqlCommand sqlCommandPrice = new SqlCommand(queryPrice, con);
            con.Open();
            sqlCommandPrice.Parameters.AddWithValue("@Preference", MaxPrice);
            sqlCommandPrice.Parameters.AddWithValue("@FK_PriceAgentID", PriceAgentID);
            sqlCommandPrice.Parameters.AddWithValue("@PreferenceType", "MaxPrice");
            //The built commands are executed
            sqlCommandPrice.ExecuteNonQuery();

            //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
            string queryLocation = "INSERT INTO [dbo].[PriceAgentPreferences] (Preference, FK_PriceAgentID, PreferenceType) VALUES(@Preference, @FK_PriceAgentID, @PreferenceType)";
            //SqlCommand is used to build up commands
            SqlCommand sqlCommandLocation = new SqlCommand(queryLocation, con);
            sqlCommandLocation.Parameters.AddWithValue("@Preference", DestinationPreference);
            sqlCommandLocation.Parameters.AddWithValue("@FK_PriceAgentID", PriceAgentID);
            sqlCommandLocation.Parameters.AddWithValue("@PreferenceType", "Destination");
            //The built commands are executed
            sqlCommandLocation.ExecuteNonQuery();

            foreach (string act in ActPreferences)
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string queryAct = "INSERT INTO [dbo].[PriceAgentPreferences] (Preference, FK_PriceAgentID, PreferenceType) VALUES(@Preference, @FK_PriceAgentID, @PreferenceType)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommandAct = new SqlCommand(queryAct, con);
                sqlCommandAct.Parameters.AddWithValue("@Preference", act);
                sqlCommandAct.Parameters.AddWithValue("@FK_PriceAgentID", PriceAgentID);
                sqlCommandAct.Parameters.AddWithValue("@PreferenceType", "Activity");
                //The built commands are executed
                sqlCommandAct.ExecuteNonQuery();
            }
        }

        private async Task<int> GetAgentID(DateTime DateMixer)
        {
            int id = -1;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DaService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[PriceAgent] WHERE CreationDate=@DateMixer";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@DateMixer", DateMixer);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            id = Convert.ToInt32(Reader["PriceAgentID"]);
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return await Task.FromResult(id);
        }
    }
}
