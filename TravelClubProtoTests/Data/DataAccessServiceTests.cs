using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Threading.Tasks;

namespace TravelClubProto.Data.Tests
{
    [TestClass()]
    public class DataAccessServiceTests
    {
        public IConfiguration config = InitConfiguration();
        [TestMethod()]
        public void GetAllDestinationsTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);

            List<Destination> dests = DaService.GetAllDestinations(DaService).GetAwaiter().GetResult();

            DeleteDestination(DaService);

            Assert.AreNotEqual(0, dests.Count);
        }

        [TestMethod()]
        public void GetAllActivitiesTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Activity act = new Activity(DaService, "Testing");
            Destination dest = CreateDestination(DaService);
            act.InsertActivityIntoDataBase(dest.ID);

            List<Activity> acts = DaService.GetAllActivities(DaService).GetAwaiter().GetResult();

            DeleteActivity(DaService);
            DeleteDestination(DaService);

            Assert.AreNotEqual(0, acts.Count);
        }

        [TestMethod()]
        public void GetAllPublishedVacationsTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int destID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, destID);
            v.State = "Published";

            List<Vacation> vacs = DaService.GetAllPublishedVacations(DaService).GetAwaiter().GetResult();

            DeleteDestination(DaService);
            DeleteVacation(DaService, v.ID);

            Assert.AreNotEqual(0, vacs.Count);
        }

        [TestMethod()]
        public void GetAllVacationsTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int destID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, destID);

            List<Vacation> vacs = DaService.GetAllPublishedVacations(DaService).GetAwaiter().GetResult();

            DeleteDestination(DaService);
            DeleteVacation(DaService, v.ID);

            Assert.AreNotEqual(0, vacs.Count);
        }

        [TestMethod()]
        public void GetVacationByIDTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int destID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, destID);

            Vacation vac = DaService.GetVacationByID(DaService, v.ID).GetAwaiter().GetResult();

            DeleteDestination(DaService);
            DeleteVacation(DaService, v.ID);

            Assert.AreEqual(v.ID, vac.ID);
        }

        [TestMethod()]
        public void FindAccountInDatabaseTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int custID = CreateAndGetCustomerID(DaService);
            Customer cust = new Customer(custID, "TestEmail", "TestPassword", DaService);

            int result = DaService.FindAccountInDatabase("TestEmail", "TestPassword", DaService).GetAwaiter().GetResult();

            DeleteCustomer(DaService, custID);

            Assert.AreEqual(custID, cust.customerID);
        }

        [TestMethod()]
        public void GetAccountByIdTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int custID = CreateAndGetCustomerID(DaService);
            Customer cust = new Customer(custID, "TestEmail", "TestPassword", DaService);
            Customer newCust = null;
            Account acc = DaService.GetAccountById(custID).GetAwaiter().GetResult();
            if (acc is Customer)
            {
                newCust = (Customer)acc;
            }
            DeleteCustomer(DaService, custID);

            Assert.AreEqual(custID, newCust.customerID);
        }

        [TestMethod()]
        public void GetAllCustomersTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int custID = CreateAndGetCustomerID(DaService);
            Customer cust = new Customer(custID, "TestEmail", "TestPassword", DaService);

            List<Customer> result = DaService.GetAllCustomers().GetAwaiter().GetResult();

            DeleteCustomer(DaService, custID);

            Assert.AreNotEqual(0, result.Count);
        }

        [TestMethod()]
        public void getDestinationByIDTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int destID = dest.GetID();

            Destination result = DaService.getDestinationByID(destID).GetAwaiter().GetResult();

            DeleteDestination(DaService);

            Assert.AreEqual(dest.ID, result.ID);
            Assert.AreEqual(dest.CountryLocationHotel, result.CountryLocationHotel);
        }

        [TestMethod()]
        public void GetAllActivitiesByDestIDTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Activity act = new Activity(DaService, "Testing");
            Destination dest = CreateDestination(DaService);
            int destID = dest.GetID();
            act.InsertActivityIntoDataBase(destID);

            List<Activity> acts = DaService.GetAllActivitiesByDestID(destID).GetAwaiter().GetResult();

            DeleteActivity(DaService);
            DeleteDestination(DaService);

            Assert.AreNotEqual(0, acts.Count);
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

        private void DeletePriceAgent(DataAccessService DaService, int custID)
        {
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