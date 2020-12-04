using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace TravelClubProto.Data.Tests
{

    [TestClass()]
    public class CustomerTests
    {
        public IConfiguration config = InitConfiguration();
        [TestMethod()]
        public void ChangePersonalInformationTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int custID = CreateAndGetCustomerID(DaService);
            Customer cust = new Customer(custID, "TestEmail", "TestPassword", DaService);

            cust.ChangePersonalInformation("TestNameChange", "TestEmailChange");
            DeleteCustomer(DaService, custID);

            Assert.AreEqual("TestNameChange", cust.PI.Name);            
        }

        [TestMethod()]
        public void GetRelatedVacationsForCustomerTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            int customerID = CreateAndGetCustomerID(DaService);
            Customer cust = new Customer(customerID, "TestEmail", "TestPassword", DaService);

            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");

            List<int> vacids = cust.customerVacations.GetRelatedVacationsForCustomer("Joined").GetAwaiter().GetResult();
            Assert.AreEqual(1, vacids.Count);
            Assert.AreEqual(v.ID, vacids[0]);

            DeleteRelations(DaService, v.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v.ID);
            DeleteDestination(DaService);
        }

        private int CreateAndGetCustomerID(DataAccessService DaService)
        {
            Customer newUser = new Customer("TestEmail", "TestPassword", DaService);
            newUser.InsertIntoDatabase();
            int id = DaService.FindAccountInDatabase("TestEmail", "TestPassword", DaService).GetAwaiter().GetResult();
            return id;
        }
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
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
    }
}