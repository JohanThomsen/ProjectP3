using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class DataAccessService
    {
        private IConfiguration config;
        public DataAccessService(IConfiguration configuration)
        {
            config = configuration;
        }

        private string ConnectionString
        {
            get
            {
                string _server = config.GetValue<string>("DbConfig:ServerName");
                string _database = config.GetValue<string>("DbConfig:DatabaseName");
                string _username = config.GetValue<string>("DbConfig:UserName");
                string _password = config.GetValue<string>("DbConfig:Password");
                return $"Server={_server};Database={_database};User ID={_username};Password={_password};Trusted_Connection=False;MultipleActiveResultSets=true;";
            }
        }

        public async Task<List<DBVacation>> GetAllVacations()
        {
            // Console.WriteLine(ConnectionString);
            List<DBVacation> vacations = new List<DBVacation>();
            DBVacation v;
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionString);
            Console.WriteLine(con);
            SqlDataAdapter da = new SqlDataAdapter("select * from [dbo].[Vacation]", con);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                v = new DBVacation();
                v.ID = Convert.ToInt32(row["ID"]);
                v.Type = row["Type"] as string;
                v.Location = row["Lokation"] as string;
                v.Price = Convert.ToInt32(row["Price"]);
                vacations.Add(v);
            }
            return await Task.FromResult(vacations);
        }


        public async Task<List<DBVacation>> GetSkiVacations()
        {
           // Console.WriteLine(ConnectionString);
            List<DBVacation> vacations = new List<DBVacation>();
            DBVacation v;
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionString);
            Console.WriteLine(con);
            SqlDataAdapter da = new SqlDataAdapter("select * from [dbo].[Vacation] where Type='Ski' ", con);
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                v = new DBVacation();
                v.ID = Convert.ToInt32(row["ID"]);
                v.Type = row["Type"] as string;
                v.Location = row["Lokation"] as string;
                v.Price = Convert.ToInt32(row["Price"]);
                vacations.Add(v);
            }
            return await Task.FromResult(vacations);
        }

    }
}
