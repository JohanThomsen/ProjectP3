using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Activity : IEquatable<Activity>
    {

        public string Type { get; set; }
        public int ID { get; set; }
        public int FK_DestinationID { get; set; }
        public DataAccessService DaService { get; set; }

        public Activity(DataAccessService daService)
        {
            DaService = daService;
        }

        public Activity(DataAccessService daService, string type)
        {
            DaService = daService;
            Type = type;
        }
        public void InsertActivityIntoDataBase(int destID)
        {
            FK_DestinationID = destID;
            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Activities] (Type, FK_DestinationID) VALUES(@Type, @DestinationID)";
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Type", Type);
                sqlCommand.Parameters.AddWithValue("@DestinationID", FK_DestinationID);
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

        public override bool Equals(object obj)
        {
            return Equals(obj as Activity);
        }

        public bool Equals(Activity other)
        {
            return other != null &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }
    }
}
