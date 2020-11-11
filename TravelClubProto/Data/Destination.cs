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

        public DataAccessService DaService;

        public Destination(DataAccessService daService)
        {
            DaService = daService;
        }
        public Destination()
        {
               
        }

        public void InsertDestinationIntoDataBase()
        {

            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Destination] (Hotel, Location) VALUES(@Hotel, @Location)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Hotel", Hotel);
                sqlCommand.Parameters.AddWithValue("@Location", Location);
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
