using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class PriceAgentManager
    {
        public string Email;
        public List<PriceAgent> Agents;
        public List<Vacation> ReleventVacations = new List<Vacation>();
        public List<Vacation> DiscardedVacations = new List<Vacation>();
        public int FK_CustomerID;
        DataAccessService DaService;

        public PriceAgentManager(DataAccessService daService, string email, int customerID)
        {
            Email = email;
            DaService = daService;
            FK_CustomerID = customerID;
            Agents = GetPriceAgentsIDs().GetAwaiter().GetResult();
        }

        public async Task<List<PriceAgent>> GetPriceAgentsIDs()
        {
            List<int> IDs = new List<int>();
            List<PriceAgent> agents = new List<PriceAgent>();
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
                    agents.Add(new PriceAgent(FK_CustomerID, ID, DaService));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Waits for Task to be finished and then returns the list of Destinations
            return await Task.FromResult(agents);
        }

        public void CreatePriceAgent(string destinationPreference, List<string> activityPreferences, decimal maxPrice)
        {
            PriceAgent newAgent = new PriceAgent(FK_CustomerID, DaService, destinationPreference, maxPrice, activityPreferences);
            newAgent.InsertIntoDatabase();
            Agents.Add(newAgent);

        }

        public void GatherVacations()
        {
            List<Vacation> AllRelevantVacations = new List<Vacation>();
            foreach (PriceAgent agent in Agents)
            {
                List<Vacation> tempRelevant = agent.LookForRelevantPriceChanges();
                foreach (Vacation vac in tempRelevant)
                {
                    if (!(AllRelevantVacations.Contains(vac)))
                    {
                        AllRelevantVacations.Add(vac);
                    }
                }
            }

            if (AllRelevantVacations.Count != 0)
            {
                SendNotification(AllRelevantVacations);
            }
        }

        private void SendNotification(List<Vacation> AllRelevantVacations)
        {
            string vacsToSend = "";
            //Send email
            foreach (Vacation vac in AllRelevantVacations)
            {
                //Construct Email
                vacsToSend += vac.ToString();
            }
            Console.WriteLine(vacsToSend);
        }
    }
}
