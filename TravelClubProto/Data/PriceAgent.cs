﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    class PriceAgent
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
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DaService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Account] WHERE FK_PriceAgentID=@FK_PriceAgentID";
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
            List<Vacation> ignorableVacations = GetIgnorableVacations().GetAwaiter().GetResult();
            foreach (Vacation vac in RecentlyChangedVacations)
            {
                if (vac.Destination.CountryLocationHotel == DestinationPreference && vac.CurrentPrice().GetAwaiter().GetResult() <= MaxPrice)
                {
                    bool contains = true;
                    foreach (Activity act in vac.Destination.Activities)
                    {
                        if (!(ActPreferences.Contains(act.Type)))
                        {
                            contains = false;
                        }
                    }
                    if (contains == true && (!(ignorableVacations.Contains(vac))))
                    {
                        RelevantChangedVacations.Add(vac);
                    }
                }
            }
            return RelevantChangedVacations;
        }

        private async Task<List<Vacation>> GetIgnorableVacations()
        {
            List<Vacation> vacations = new List<Vacation>();
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
                vacations = await DaService.FillVacationList(dt, DaService);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(vacations);
        }

        public void InsertIntoDatabase()
        {
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[PriceAgent] (FK_AccountID) VALUES(@FK_CustomerID)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@FK_CustomerID", FK_CustomerID);
                //The built commands are executed
                sqlCommand.ExecuteNonQuery();

                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string queryPrice = "INSERT INTO [dbo].[PriceAgentPreferences] (Preference, PreferenceType) VALUES(@Preference, @PreferenceType)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommandPrice = new SqlCommand(queryPrice, con);
                sqlCommandPrice.Parameters.AddWithValue("@Preference", MaxPrice);
                sqlCommandPrice.Parameters.AddWithValue("@PreferenceType", "MaxPrice");
                //The built commands are executed
                sqlCommandPrice.ExecuteNonQuery();

                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string queryLocation = "INSERT INTO [dbo].[PriceAgentPreferences] (Preference, PreferenceType) VALUES(@Preference, @PreferenceType)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommandLocation = new SqlCommand(queryLocation, con);
                sqlCommandLocation.Parameters.AddWithValue("@Preference", DestinationPreference);
                sqlCommandLocation.Parameters.AddWithValue("@PreferenceType", "Destination");
                //The built commands are executed
                sqlCommandLocation.ExecuteNonQuery();

                foreach (string act in ActPreferences)
                {
                    //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                    string queryAct = "INSERT INTO [dbo].[PriceAgentPreferences] (Preference, PreferenceType) VALUES(@Preference, @PreferenceType)";
                    //SqlCommand is used to build up commands
                    SqlCommand sqlCommandAct = new SqlCommand(queryAct, con);
                    sqlCommandAct.Parameters.AddWithValue("@Preference", act);
                    sqlCommandAct.Parameters.AddWithValue("@PreferenceType", "Activity");
                    //The built commands are executed
                    sqlCommandAct.ExecuteNonQuery();
                }
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
