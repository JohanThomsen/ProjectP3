using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.AspNetCore.Components;

namespace TravelClubProto.Data
{
    public abstract class Account
    {
        private DateTime LoginDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        private DateTime CreationDate { get; set; }
        public int ID { get; set; }
        

        public DataAccessService DaService { get; set; }

        public Account(string email, string password, DataAccessService daService)
        {
            CreationDate = DateTime.Now;
            Email        = email;
            Password     = password;
            DaService    = daService;
            LoginDate    = CreationDate;
        }
        public void InsertIntoDatabase()
        {
            //Connects to the azure sql database
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                //Prepares the values (hotel, location) into coloums hotel and location on table [dbo].[Destination]
                string query = "INSERT INTO [dbo].[Account] (Email, Password, LoginDate, Type) VALUES(@Email, @Password, @LoginDate, @Type)";
                
                //SqlCommand is used to build up commands
                SqlCommand sqlCommand = new SqlCommand(query, con);
                con.Open();
                sqlCommand.Parameters.AddWithValue("@Email", Email);
                sqlCommand.Parameters.AddWithValue("@Password", Password);
                sqlCommand.Parameters.AddWithValue("@LoginDate", CreationDate);
                sqlCommand.Parameters.AddWithValue("@Type", Type);
                
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

        public async static Task<int> FindAccountInDatabase(string email, string password, DataAccessService daService)
        {

            Account user = new Customer(email, password, daService);
            daService.LoggedIn = false;
            try
            {
                using (SqlConnection myConnection = new SqlConnection(daService.ConnectionString))
                {
                    //The * means all. So data from [dbo].[Destination] table are selected by the database
                    string query = "SELECT * FROM [dbo].[Account] WHERE Email=@Email AND Password=@Password";
                    SqlCommand sqlCommand = new SqlCommand(query, myConnection);
                    sqlCommand.Parameters.AddWithValue("@Email", email);
                    sqlCommand.Parameters.AddWithValue("@Password", password);
                    myConnection.Open();
                    //Reads all the executed sql commands
                    using (SqlDataReader Reader = sqlCommand.ExecuteReader())
                    {
                        // Reads all data and converts to object and type matches
                        while (Reader.Read())
                        {
                            user.ID = Convert.ToInt32(Reader["AccountID"]);
                            if (Reader["Email"] as string == user.Email && Reader["Password"] as string == user.Password)
                            {
                                daService.LoggedIn = true;
                                daService.LoggedInAccountID = user.ID;

                                myConnection.Close();
                                return await Task.FromResult(user.ID);
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
            return await Task.FromResult(-1);
        }


    }
}
