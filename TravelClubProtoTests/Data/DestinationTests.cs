using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using TravelClubProto.Data;
using Microsoft.Data.SqlClient;

namespace TravelClubProto.Tests
{
    [TestClass()]
    public class DestinationTests
    {
        public IConfiguration config = InitConfiguration();
        [TestMethod()]
        public void InsertDestinationIntoDataBaseTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = new Destination(DaService);
            dest.Hotel = "testHotel";
            dest.Location = "TestLocation";
            dest.Country = "TestCountry";
            dest.AddDate = DateTime.Now;
            dest.InsertDestinationIntoDataBase();

            int ID = dest.GetID();

            Assert.AreEqual(ID, dest.ID);

            DeleteDestination(DaService);
        }


        [TestMethod()]
        public void GetAllActivitiesFromDestTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Destination dest = new Destination(DaService);
            dest.Hotel = "testHotel";
            dest.Location = "TestLocation";
            dest.Country = "TestCountry";
            dest.AddDate = DateTime.Now;
            dest.InsertDestinationIntoDataBase();
            int ID = dest.GetID();

            Activity test1 = new Activity(DaService, "Testing");
            test1.InsertActivityIntoDataBase(ID);
            Activity test2 = new Activity(DaService, "Testing");
            test2.InsertActivityIntoDataBase(ID);
            Activity test3 = new Activity(DaService, "Testing");
            test3.InsertActivityIntoDataBase(ID);

            List<Activity> testActs = dest.GetAllActivitiesFromDest().GetAwaiter().GetResult();

            Assert.AreEqual(3, testActs.Count);
            DeleteDestination(DaService);
            DeleteActivity(DaService);
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

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
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