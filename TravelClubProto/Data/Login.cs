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
    public class Login
    {
        public async Task<Account> GetAccountByID(DataAccessService DaService, int AccountID)
        {
            Account a = null;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].Account WHERE AccountID={AccountID}", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    if (row["Type"] as string == "Customer")
                    {
                        Customer c = new Customer(Convert.ToInt32(row["AccountID"]), row["Email"] as string, row["Password"] as string, DaService);
                        c.PI.Name = row["Name"] as string;
                        c.LoginDate = Convert.ToDateTime(row["LoginDate"]);
                        c.Type = row["Type"] as string;
                        a = c;
                    }
                    else if (row["Type"] as string == "TravelBureau")
                    {
                        TravelBureau tb = new TravelBureau(Convert.ToInt32(row["AccountID"]), row["Email"] as string, row["Password"] as string, DaService);
                        tb.Name = row["Name"] as string;
                        a = tb;
                    }
                    else if (row["Type"] as string == "TravelClub")
                    {
                        TravelClub tc = new TravelClub(Convert.ToInt32(row["AccountID"]), row["Email"] as string, row["Password"] as string, DaService);
                        a = tc;
                    } else
                    {
                        throw new InvalidAccountTypeException("Invalid Account Type");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(a);
        }
    }
}
