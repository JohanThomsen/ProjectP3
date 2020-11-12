using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TravelClubProto.Data;

namespace TravelClubProto
{
    public class Destination
    {
        public int ID { get; set; }
        public string Location { get; set; }
        public string Hotel { get; set; }
        public DateTime AddDate { get; set; }

        public DataAccessService DaService;

        public Destination(DataAccessService daService)
        {
            DaService = daService;
        }

        public void InsertDestinationIntoDataBase()
        {

            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Destination] (Hotel, Location, AddDate) VALUES(@Hotel, @Location, @AddDate)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Hotel", Hotel);
                sqlCommand.Parameters.AddWithValue("@Location", Location);
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
    }
}
