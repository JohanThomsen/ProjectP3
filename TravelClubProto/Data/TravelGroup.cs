using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelGroup
    {
        public int FK_VacationID { get; }
        DataAccessService DaService { get; set; }
        public TravelGroup(int fK_VacationID, DataAccessService daService)
        {
            FK_VacationID = fK_VacationID;
            DaService = daService;
        }

        /// <summary>
        /// newRelation has to be either "Joined" or "Favourited"
        /// </summary>
        public void ChangeVacationRelation(int customerID, int FK_VacationID, string newRelation)
        {
            if (CheckForAndChangeRelations(customerID, newRelation) == false)
            {
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                try
                {
                    //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                    string query = "INSERT INTO [dbo].CustomerVacationRelations (FK_CustomerID, FK_VacationID, RelationType) VALUES(@FK_CustomerID, @FK_VacationID, @RelationType)";
                    //SqlCommand is used to build up commands
                    SqlCommand sqlCommand = new SqlCommand(query, con);
                    con.Open();
                    sqlCommand.Parameters.AddWithValue("@FK_CustomerID", customerID);
                    sqlCommand.Parameters.AddWithValue("@FK_VacationID", FK_VacationID);
                    sqlCommand.Parameters.AddWithValue("@RelationType", newRelation);

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

        public async Task<bool> CheckForRelation(int customerID, string relation)
        {
            bool succes = false;
            try
            {
                using var sc = new SqlConnection(DaService.ConnectionString);
                using var cmd = sc.CreateCommand();
                sc.Open();
                cmd.CommandText = "SELECT COUNT(*) FROM [dbo].CustomerVacationRelations WHERE FK_CustomerID=@customerID AND RelationType=@relation";
                cmd.Parameters.AddWithValue("@customerID", customerID);
                cmd.Parameters.AddWithValue("@relation", relation);

                int exists = (int)cmd.ExecuteScalar();
                if (exists == 1)
                {
                    succes = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return (succes);
        }


        public bool CheckForAndChangeRelations(int customerID, string newRelation)
        {
            string oldRelation = (newRelation == "Joined") ? "Favourited" : "Joined";
            bool isRelated = false;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "SELECT COUNT(*) FROM [dbo].CustomerVacationRelations WHERE FK_CustomerID=@customerID AND RelationType=@oldRelation OR RelationType=@newRelation";
                    cmd.Parameters.AddWithValue("@customerID", customerID);
                    cmd.Parameters.AddWithValue("@oldRelation", oldRelation);
                    cmd.Parameters.AddWithValue("@newRelation", newRelation);
                    int exists = (int)cmd.ExecuteScalar();
                    if (exists == 1)
                    {
                        isRelated = true;
                        using (var sc2 = new SqlConnection(DaService.ConnectionString))
                        using (var cmd2 = sc.CreateCommand())
                        {
                            sc2.Open();
                            cmd2.CommandText = "UPDATE [dbo].CustomerVacationRelations SET RelationType = @newRelation WHERE FK_CustomerID=@CustomerID";
                            cmd2.Parameters.AddWithValue("@newRelation", newRelation);
                            cmd2.Parameters.AddWithValue("@CustomerID", customerID);
                            cmd2.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return isRelated;
        }

        public void LeaveVacation(int customerID)
        {
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo].CustomerVacationRelations WHERE FK_CustomerID=@CustomerID";
                    cmd.Parameters.AddWithValue("@CustomerID", customerID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// newRelation has to be either "Joined" or "Favourited"
        /// </summary>
        public async Task<List<int>> GetUserIDsFromRelation(int vacationID, string relation)
        {
            List<int> IDS = new List<int>();
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].CustomerVacationRelations WHERE RelationType='{relation}' AND FK_VacationID='{vacationID}'", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    IDS.Add(Convert.ToInt32(row["FK_CustomerID"]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(IDS);
        }
        private void SendNotification()
        {
            //:q-ping:
        }
    }
}
