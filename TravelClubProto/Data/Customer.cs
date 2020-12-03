using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Customer : Account
    {
        public int customerID { get; set; }
        public PriceAgentManager priceAgentManager { get; set; }
        public CustomerVacations customerVacations { get; set; }

        public Customer(int ID, string email, string password, DataAccessService daService) : base(ID, email, password, daService)
        {
            customerID = ID;
            Type = "Customer";
            customerVacations = new CustomerVacations(daService, ID);
            priceAgentManager = new PriceAgentManager(daService, email, ID);
            PI.EmailAddress = email;
        }

        public Customer(string email, string password, DataAccessService daService) : base(email, password, daService)
        {
            Type = "Customer";
            customerVacations = new CustomerVacations(daService, ID);
            priceAgentManager = new PriceAgentManager(daService, email, ID);
            PI.EmailAddress = email;
        }
        public struct PersonalInformation
        {
            public string Name        { get; set; }
            public string EmailAddress { get; set; } 
        }

        public PersonalInformation PI;

        public int ChangePersonalInformation(string name, string emailAddress)
        {
            PI.Name         = name;
            PI.EmailAddress = emailAddress;

            int count = 0;
            SqlConnection con = new SqlConnection(DaService.ConnectionString);
            try
            {
                using (var sc2 = new SqlConnection(DaService.ConnectionString))
                using (var cmd2 = sc2.CreateCommand())
                {
                    sc2.Open();
                    cmd2.CommandText = "UPDATE [dbo].Account SET Email = @emailAddress, Name = @name WHERE AccountID=@CustomerID";
                    cmd2.Parameters.AddWithValue("@emailAddress", emailAddress);
                    cmd2.Parameters.AddWithValue("@name", name);
                    cmd2.Parameters.AddWithValue("@CustomerID", customerID);

                    count = cmd2.ExecuteNonQuery();
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
            return count;
        }
    }
}
