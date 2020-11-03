﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Vacation
    {

        public Dictionary<string, DateTime> Dates = new Dictionary<string, DateTime>();

        private Dictionary<int, decimal> Prices = new Dictionary<int, decimal>();
        private Destination Destination { get; set; }
        public int ID { get; set; }
        private int MinNumberOfUsers { get; set; }
        private bool MinNumberOfUsersExceeded { get; set; }
        private DateTime GracePeriodLength { get; set; }

        public VacationAdministrator VacationAdministrator = new VacationAdministrator();

        private string _state;
        public string State
        {
            get { return _state; }
            set 
            {
                _state = value;
                VacationAdministrator.OnStateChange(_state, ID);
            }
        }


        public Vacation(DateTime proposalDate, DateTime deadline, List<int> stretchGoals, List<decimal> prices)
        {
            Dates.Add("ProposalDate", proposalDate);
            Dates.Add("Deadline", deadline);
            AddPrices(stretchGoals, prices);
            ID = IncrementID();
        }

        private int IncrementID()
        {
            return VacationAdministrator.IDInc++;
        }

        private void AddPrices(List<int> stretchGoals, List<decimal> prices)
        {
            for (int i = 0; i < stretchGoals.Count; i++)
            {
                Prices.Add(stretchGoals[i], prices[i]);
            }
        }
    }
}
