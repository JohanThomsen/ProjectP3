﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TravelClubProto.Data
{
    class PriceAgent
    {
        public int PriceAgentID { get; set; }

        private List<Vacation> DiscardedVacations;

        public List<string> Preferences;

        public PriceAgent(List<string> IncomingPreferences, int id)
        {
            Preferences = IncomingPreferences;
            PriceAgentID = id;
        }

        

        public List<Vacation> LookForRelevantPriceChanges(List<string> preferences)
        {
            List<Vacation> RelevantChangedVacations;
            // Check each Property in the recently changed vacations againt the preferences of the agent, an add them to a list
            // Return the changed list to the manager who called it.
        }

    }
}
