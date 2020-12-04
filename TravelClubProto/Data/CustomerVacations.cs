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
    public class CustomerVacations
    {

        public int CustomerID { get; set; }

        public DataAccessService DaService { get; set; }
        public CustomerVacations(DataAccessService daService, int customerID)
        {
            DaService = daService;
            CustomerID = customerID;
        }

        public async Task<List<int>> GetRelatedVacationsForCustomer(string relation)
        {
            List<int> relatedVacations = new List<int>();
            try
            {
                //Creates a table
                DataTable dt = new DataTable();
                SqlConnection con = new SqlConnection(DaService.ConnectionString);
                //Gets data from the sql database
                SqlDataAdapter da = new SqlDataAdapter($"SELECT * FROM [dbo].CustomerVacationRelations WHERE RelationType='{relation}' AND FK_CustomerID='{CustomerID}'", con);
                //Structures the data such that it can be read 
                da.Fill(dt);
                //Reads data into designated class
                foreach (DataRow row in dt.Rows)
                {
                    relatedVacations.Add(Convert.ToInt32(row["FK_VacationID"]));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return await Task.FromResult(relatedVacations);
        }
    }
}
