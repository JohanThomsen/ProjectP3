using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace TravelClubProtoTests.Data.Tests
{
    [TestClass()]
    public class VacationTests
    {
        public IConfiguration config = InitConfiguration();

        [TestMethod()]
        public void InsertVacationToDatabaseTest()
        {
            DataAccessService DaService = new DataAccessService(config);

            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsert(DaService, ID);

            int success = DeleteVacation(DaService, v.ID);
            Assert.AreNotEqual(0, success);

            DeleteDestination(DaService);
        }



        [TestMethod()]
        public void MinNumberOfUsersExceededTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            int vacID = CreateVacationAndInsert(DaService, ID).ID;
            int customerID = CreateAndGetCustomerID(DaService);

            Customer newUser1 = new Customer("TestEmail2", "TestPassword", DaService);
            newUser1.InsertIntoDatabase();
            int id = DaService.FindAccountInDatabase("TestEmail2", "TestPassword", DaService).GetAwaiter().GetResult();
            int customerID2 =  id;

            Customer newUser2 = new Customer("TestEmail3", "TestPassword", DaService);
            newUser2.InsertIntoDatabase();
            int id3 = DaService.FindAccountInDatabase("TestEmail3", "TestPassword", DaService).GetAwaiter().GetResult();
            int customerID3 = id3;

            Vacation vac = DaService.GetVacationByID(DaService, vacID).GetAwaiter().GetResult();

            int result = vac.MinNumberOfUsersExceeded();

            Assert.AreEqual(0, result);

            vac.TravelGroup.ChangeVacationRelation(customerID, "Joined");
            vac.TravelGroup.ChangeVacationRelation(customerID2, "Joined");
            vac.TravelGroup.ChangeVacationRelation(customerID3, "Joined");
            result = vac.MinNumberOfUsersExceeded();

            


            DeleteRelations(DaService, vac.ID);
            DeleteCustomer(DaService, customerID);
            DeleteCustomer(DaService, customerID2);
            DeleteCustomer(DaService, customerID3);
            DeleteVacation(DaService, vacID);
            DeleteDestination(DaService);

            Assert.AreEqual(1, result);
        }

        [TestMethod()]
        public void CurrentPriceTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            int vacID = CreateVacationAndInsert(DaService, ID).ID;

            Vacation vac = DaService.GetVacationByID(DaService, vacID).GetAwaiter().GetResult();

            decimal result = vac.CurrentPrice().GetAwaiter().GetResult();

            Assert.AreEqual(62345, result);

            DeleteVacation(DaService, vacID);
            DeleteDestination(DaService);
        }

        [TestMethod()]
        public void VacationTest()
        {
            DataAccessService DaService = new DataAccessService(config);
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

            Assert.AreEqual(4, v.Prices.Count);
            Assert.AreEqual(4, v.StretchGoals.Count);
        }

        /// <summary>
        /// TravelGroup Tests. Share alot of init function os makes sense to keep in here
        /// </summary>
        [TestMethod()]
        public void NumberOfJoinedUsersTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            int customerID = CreateAndGetCustomerID(DaService);

            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");

            int joinedUsers = v.TravelGroup.NumberOfJoinedUsers().GetAwaiter().GetResult();

            Assert.AreEqual(1, joinedUsers);

            DeleteRelations(DaService, v.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v.ID);
            DeleteDestination(DaService);
        }

        [TestMethod()]
        public void ChangeVacationRelationTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            int customerID = CreateAndGetCustomerID(DaService);

            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");

            bool result = v.TravelGroup.CheckForRelation(customerID, "Joined").GetAwaiter().GetResult();

            Assert.AreEqual(true, result);

            DeleteRelations(DaService, v.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v.ID);
            DeleteDestination(DaService);
        }

        [TestMethod()]
        public void CheckForAndChangeRelationsTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            int customerID = CreateAndGetCustomerID(DaService);

            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");
            bool result = v.TravelGroup.CheckForRelation(customerID, "Joined").GetAwaiter().GetResult();
            Assert.AreEqual(true, result);

            v.TravelGroup.ChangeVacationRelation(customerID, "Favourited");
            bool result2 = v.TravelGroup.CheckForRelation(customerID, "Favourited").GetAwaiter().GetResult();
            Assert.AreEqual(true, result2);

            v.TravelGroup.ChangeVacationRelation(customerID, "Favourited");
            bool result3 = v.TravelGroup.CheckForRelation(customerID, "Favourited").GetAwaiter().GetResult();
            Assert.AreEqual(true, result3);

            DeleteRelations(DaService, v.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v.ID);
            DeleteDestination(DaService);
        }

        [TestMethod()]
        public void LeaveVacationTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            int customerID = CreateAndGetCustomerID(DaService);

            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");
            bool result = v.TravelGroup.CheckForRelation(customerID, "Joined").GetAwaiter().GetResult();
            Assert.AreEqual(true, result);

            v.TravelGroup.LeaveVacation(customerID);

            v.TravelGroup.ChangeVacationRelation(customerID, "Favourited");
            bool result2 = v.TravelGroup.CheckForRelation(customerID, "Favourited").GetAwaiter().GetResult();
            Assert.AreEqual(true, result);

            DeleteRelations(DaService, v.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v.ID);
            DeleteDestination(DaService);

        }

        [TestMethod()]
        public void GetUserIDsFromRelationTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            int customerID = CreateAndGetCustomerID(DaService);

            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");
            bool result = v.TravelGroup.CheckForRelation(customerID, "Joined").GetAwaiter().GetResult();
            Assert.AreEqual(true, result);

            List<int> ids = v.TravelGroup.GetUserIDsFromRelation(v.ID, "Joined").GetAwaiter().GetResult();
            Assert.AreEqual(1, ids.Count);
            Assert.AreEqual(customerID, ids[0]);

            DeleteRelations(DaService, v.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v.ID);
            DeleteDestination(DaService);
        }
        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        [TestMethod()]
        public void OnStateChangeTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            int customerID = CreateAndGetCustomerID(DaService);
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();
            Vacation v = CreateVacationAndInsertAndGet(DaService, ID);
            

            v.State = "Published";
            GetVacState(DaService, v.ID);
            Assert.AreEqual("Published", GetVacState(DaService, v.ID));
            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");

            v.State = "GracePeriod";
            GetVacState(DaService, v.ID);
            Assert.AreEqual("GracePeriod", GetVacState(DaService, v.ID));

            v.State = "Completed";
            GetVacState(DaService, v.ID);
            Assert.AreEqual("Completed", GetVacState(DaService, v.ID));
            bool result = v.TravelGroup.CheckForRelation(customerID, "Participated").GetAwaiter().GetResult();
            Assert.AreEqual(true, result);
            DeleteRelations(DaService, v.ID);
            DeleteVacation(DaService, v.ID);

            Destination dest2 = CreateDestination(DaService);
            int ID2 = dest2.GetID();
            Vacation v2 = CreateVacationAndInsertAndGet(DaService, ID2);

            v2.State = "Published";
            GetVacState(DaService, v2.ID);
            Assert.AreEqual("Published", GetVacState(DaService, v2.ID));
            v.TravelGroup.ChangeVacationRelation(customerID, "Joined");
            v2.State = "Canceled";
            Assert.AreEqual("Canceled", GetVacState(DaService, v2.ID));

            bool result2 = v2.TravelGroup.CheckForRelation(customerID, "Participated").GetAwaiter().GetResult();
            Assert.AreEqual(false, result2);

            DeleteRelations(DaService, v2.ID);
            DeleteCustomer(DaService, customerID);
            DeleteVacation(DaService, v2.ID);
            DeleteDestination(DaService);
        }

        private string GetVacState(DataAccessService daService, int iD)
        {
            Vacation v = daService.GetVacationByID(daService, iD).GetAwaiter().GetResult();
            return v.State;
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

        private Vacation CreateVacationAndInsert(DataAccessService DaService, int ID)
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
            return v;
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


    }
}