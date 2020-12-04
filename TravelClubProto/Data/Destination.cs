using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TravelClubProto.Data;

namespace TravelClubProto
{
    public class Destination : IEquatable<Destination>
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public string Hotel { get; set; }
        public DateTime AddDate { get; set; }
        public string Country { get; set; }
        public string CountryLocationHotel => $"{Country} ({Location}) : {Hotel}"; 
        public DataAccessService DaService;
        public List<Activity> Activities { get; set; }

        public Destination(DataAccessService daService)
        {
            DaService = daService;
            AddActivity().GetAwaiter().GetResult();
        }
        public async Task AddActivity()
        {
            Activities = await GetAllActivitiesFromDest();
        }
        

        public void InsertDestinationIntoDataBase()
        {

            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Destination] (Hotel, Location, AddDate, Country) VALUES(@Hotel, @Location, @AddDate, @Country)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Hotel", Hotel);
                sqlCommand.Parameters.AddWithValue("@Location", Location);
                sqlCommand.Parameters.AddWithValue("@Country", Country);
                sqlCommand.Parameters.AddWithValue("@AddDate", AddDate);
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

        public int GetID()
        {
            int returnid = 0;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(DaService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Destination] WHERE AddDate=@AddDate";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@AddDate", AddDate);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            returnid = Convert.ToInt32(Reader["DestinationID"]);
                            ID = returnid;
                        }
                        myConnection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return returnid;
        }
        


        public async Task<List<Activity>> GetAllActivitiesFromDest()
        {
            List<Activity> activities = new List<Activity>();
            Activity a;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);

                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].Activities WHERE FK_DestinationID='{ID}'", con);

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

        public override bool Equals(object obj)
        {
            return Equals(obj as Destination);
        }

        public bool Equals(Destination other)
        {
            return other != null &&
                   ID == other.ID &&
                   AddDate == other.AddDate &&
                   CountryLocationHotel == other.CountryLocationHotel;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, AddDate, CountryLocationHotel);
        }
    }
}
