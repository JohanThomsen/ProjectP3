using System;
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
        public int MinNumberOfUsers { get; set; }
        public bool MinNumberOfUsersExceeded { get; set; }
        private DateTime GracePeriodLength { get; set; }


        public VacationData VacData { get; set; }

        public VacationAdministrator VacAdmin { get; set; }

        private string _state;
        public string State
        {
            get { return _state; }
            set 
            {
                _state = value;
                VacAdmin.OnStateChange(_state, ID);
            }
        }


        public Vacation(DateTime proposalDate, DateTime deadline, List<int> stretchGoals, List<decimal> prices, VacationData vacData)
        {
            VacData = vacData;
            VacAdmin = new VacationAdministrator(vacData);
            Dates.Add("ProposalDate", proposalDate);
            Dates.Add("Deadline", deadline);
            AddPrices(stretchGoals, prices);
        }

        public Vacation(VacationData vacData, List<int> stretchGoals, List<decimal> prices)
        {
            VacData = vacData;
            VacAdmin = new VacationAdministrator(vacData);
            AddPrices(stretchGoals, prices);
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
