using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace TravelClubProto.Data.Tests
{
    [TestClass()]
    public class PriceAgentTests
    {
        public IConfiguration config = InitConfiguration();
        [TestMethod()]
        public void PriceAgentTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int custID = CreateAndGetCustomerID(DaService);
            Destination dest = CreateDestination(DaService);
            string destPreference = dest.CountryLocationHotel;
            decimal maxPrice = 100000;
            List<string> actPreferences = new List<string>(){ "Testing" };
            PriceAgent agent = new PriceAgent(custID, DaService, destPreference, maxPrice, actPreferences);


            DeleteCustomer(DaService, custID);
            DeleteDestination(DaService);

            Assert.AreEqual(custID, agent.FK_CustomerID);
            Assert.AreEqual(destPreference, agent.DestinationPreference);
            Assert.AreEqual(maxPrice, agent.MaxPrice);
            Assert.AreEqual("Testing", actPreferences[0]);
        }

        [TestMethod()]
        public void LookForRelevantPriceChangesTest()
        {
            DataAccessService DaService = new DataAccessService(config);

            Activity act = new Activity(DaService, "Testing");
            int custID = CreateAndGetCustomerID(DaService);
            Destination dest = CreateDestination(DaService);
            int destID = dest.GetID();
            act.InsertActivityIntoDataBase(destID);
            Vacation v = CreateVacationAndInsertAndGet(DaService, destID);
            v.State = "Published";
            string destPreference = dest.CountryLocationHotel;
            decimal maxPrice = 100000;
            List<string> actPreferences = new List<string>() { "Testing" };
            PriceAgent agent = new PriceAgent(custID, DaService, destPreference, maxPrice, actPreferences);

            List<Vacation> vacs = agent.LookForRelevantPriceChanges();


            DeleteActivity(DaService);
            DeleteRelations(DaService, v.ID);
            DeleteVacation(DaService, v.ID);
            DeleteCustomer(DaService, custID);
            DeleteDestination(DaService);

            Assert.AreEqual(1, vacs.Count);
        }

        [TestMethod()]
        public void InsertIntoDatabaseTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int custID = CreateAndGetCustomerID(DaService);
            Destination dest = CreateDestination(DaService);
            string destPreference = dest.CountryLocationHotel;
            decimal maxPrice = 100000;
            List<string> actPreferences = new List<string>() { "Testing" };
            PriceAgent agent = new PriceAgent(custID, DaService, destPreference, maxPrice, actPreferences);

            agent.InsertIntoDatabase();
            PriceAgent agentFromDatabase = GetPriceAgent(DaService, custID).GetAwaiter().GetResult();

            

            DeletePriceAgent(DaService, custID, agentFromDatabase.PriceAgentID);
            DeleteCustomer(DaService, custID);
            DeleteDestination(DaService);

            Assert.AreEqual(agent.PriceAgentID, agentFromDatabase.PriceAgentID);
            Assert.AreEqual(agent.DestinationPreference, agentFromDatabase.DestinationPreference);
        }

        public async Task<PriceAgent> GetPriceAgent(DataAccessService DaService, int FK_CustomerID)
        {
            List<int> IDs = new List<int>();
            PriceAgent agent = null;
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);

                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].PriceAgent WHERE FK_AccountID='{FK_CustomerID}'", con);

                //Structures the data such that it can be read 
                da.Fill(dt);

                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    IDs.Add(Convert.ToInt32(row["PriceAgentID"]));
                }
                foreach (int ID in IDs)
                {
                    agent = new PriceAgent(FK_CustomerID, ID, DaService);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(agent);
        }

        private int CreateAndGetCustomerID(DataAccessService DaService)
        {
            Customer newUser = new Customer("TestEmail", "TestPassword", DaService);
            newUser.InsertIntoDatabase();
            int id = DaService.FindAccountInDatabase("TestEmail", "TestPassword", DaService).GetAwaiter().GetResult();
            return id;
        }
        private Destination CreateDestination(DataAccessService DaService)
        {
            Destination dest = new Destination(DaService);
            dest.Hotel = "testHotel";
            dest.Location = "TestLocation";
            dest.Country = "TestCountry";
            dest.AddDate = DateTime.Now;
            dest.InsertDestinationIntoDataBase();
            return dest;
        }

        private Vacation CreateVacationAndInsertAndGet(DataAccessService DaService, int ID)
        {
            List<decimal> prices = new List<decimal>()
            {
                62345,
                52345,
                42345,
                32345
            };

            List<int> breaks = new List<int>()
            {
                2,
                6,
                12,
                20
            };
            Vacation v = new Vacation(breaks, prices, DaService);
            v.State = "Proposed";
            v.MinNumberOfUsers = 2;
            v.Dates.Add("ProposalDate", DateTime.Now);
            v.Dates.Add("Deadline", DateTime.Now.AddDays(5));
            v.Dates.Add("GracePeriodLength", DateTime.Now.AddDays(7));
            v.Dates.Add("PriceChangeDate", DateTime.Now);
            v.Dates.Add("TravelDate", DateTime.Now.AddDays(10));
            v.Dates.Add("LeaveDate", DateTime.Now.AddDays(13));
            v.ImageLink = "https://p.bigstockphoto.com/GeFvQkBbSLaMdpKXF1Zv_bigstock-Aerial-View-Of-Blue-Lakes-And--227291596.jpg";
            v.Description = "Testing";
            v.FK_DestinationID = ID;
            v.FK_PublisherID = 16;
            v.DepartureAirport = "Billund";
            v.TravelBureauWebsiteLink = "Google.com";
            v.InsertVacationToDatabase();

            Vacation vDataBase = DaService.GetVacationByID(DaService, v.ID).GetAwaiter().GetResult();
            return vDataBase;
        }
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        private int DeleteDestination(DataAccessService DaService)
        {
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo].Destination WHERE Hotel='testHotel'";
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }

        private int DeleteCustomer(DataAccessService DaService, int ID)
        {
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].Account WHERE AccountID='{ID}'";
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }

        private int DeleteVacation(DataAccessService DaService, int vacID)
        {
            DeleteDiscardedVacations(DaService, vacID);
            DeleteRelations(DaService, vacID);
            DeletePrices(DaService, vacID);
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].Vacation WHERE ID='{vacID}'";
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }

        private void DeleteDiscardedVacations(DataAccessService DaService, int vacID)
        {
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].DiscardedVacations WHERE FK_VacationID='{vacID}'";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DeleteRelations(DataAccessService DaService, int vacID)
        {
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].CustomerVacationRelations WHERE FK_VacationID='{vacID}'";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DeletePriceAgent(DataAccessService DaService, int custID, int agentID)
        {
            DeletePreferences(DaService, agentID);
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].PriceAgent WHERE FK_AccountID='{custID}'";
                    cmd.ExecuteNonQuery();
                }
                using (var sc2 = new SqlConnection(DaService.ConnectionString))
                using (var cmd2 = sc2.CreateCommand())
                {
                    sc2.Open();
                    cmd2.CommandText = $"DELETE FROM [dbo].DiscardedVacations WHERE FK_CustomerID='{custID}'";
                    cmd2.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void DeletePreferences(DataAccessService DaService, int agentID)
        {
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].PriceAgentPreferences WHERE FK_PriceAgentID='{agentID}'";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private int DeletePrices(DataAccessService DaService, int vacID)
        {
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"DELETE FROM [dbo].Prices WHERE FK_VacationID='{vacID}'";
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }

        public int DeleteActivity(DataAccessService DaService)
        {
            int count = 0;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo].Activities WHERE Type='Testing'";
                    count = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return count;
        }
    }
}