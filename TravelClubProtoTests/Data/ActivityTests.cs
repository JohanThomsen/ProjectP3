using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelClubProto.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Data.SqlClient;

namespace TravelClubProto.Data.Tests
{
    [TestClass()]
    public class ActivityTests
    {
        public IConfiguration config = InitConfiguration();
        
        [TestMethod()]
        public void InsertActivityIntoDataBaseTest()
        {
            DataAccessService DaService = new DataAccessService(config);
            Activity act = new Activity(DaService, "Testing");
            Destination dest = CreateDestination(DaService);
            int ID = dest.GetID();

            act.InsertActivityIntoDataBase(ID);
            DeleteDestination(DaService);
            int success = DeleteActivity(DaService);

            Assert.AreNotEqual(0, success);
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
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