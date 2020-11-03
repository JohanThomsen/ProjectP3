using System;
using System.Collections.Generic;
using System.Text;

namespace TravelClubProto.Data
{
    class PriceAgentManager
    {
        private string Email;

        private List<PriceAgent> PriceAgents;

        private List<Vacation> RelevantVacations;

        public PriceAgentManager(string email)
        {
            Email = email;
        }

        private void GatherVacations()
        {
            foreach(PriceAgent agent in PriceAgents)
            {
                RelevantVacations.AddRange(agent.LookForRelevantPriceChanges(agent.Preferences));
            }
        }

        private void SendNotification()
        {
            //Create notification and send to Email
            RelevantVacations.Clear();
        }


    }
}
