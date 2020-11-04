using System;
using System.Collections.Generic;
using System.Text;

namespace TravelClubProto.Data
{
    class PriceAgent
    {

        private List<Vacation> DiscardedVacations;

        public List<string> Preferences;

        public PriceAgent(List<string> IncomingPreferences)
        {
            Preferences = IncomingPreferences;
        }

        

        public List<Vacation> LookForRelevantPriceChanges(List<string> preferences)
        {
            List<Vacation> RelevantChangedVacations = new List<Vacation>();
            // Check each Property in the recently changed vacations againt the preferences of the agent, an add them to a list
            // Return the changed list to the manager who called it.
            return RelevantChangedVacations;
        }

    }
}
