using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Vacation
    {

        private Dictionary<string, DateTime> Dates = new Dictionary<string, DateTime>();

        private Dictionary<int, decimal> Prices = new Dictionary<int, decimal>();
        private Destination Destination { get; set; }
        private int VacationID { get; set; }
        private int MinNumberOfUsers { get; set; }
        private bool MinNumberOfUsersExceeded { get; set; }
        private DateTime GracePeriodLength { get; set; }

        private string _state;
        public string State
        {
            get { return _state; }
            set 
            {
                _state = value;
                OnStateChange();
            }
        }


        public Vacation(DateTime proposalDate, DateTime deadline, List<int> stretchGoals, List<decimal> prices, int vacationID)
        {
            Dates.Add("ProposalDate", proposalDate);
            Dates.Add("Deadline", deadline);
            AddPrices(stretchGoals, prices);
            VacationID = vacationID;
        }

        private void AddPrices(List<int> stretchGoals, List<decimal> prices)
        {
            for (int i = 0; i < stretchGoals.Count; i++)
            {
                Prices.Add(stretchGoals[i], prices[i]);
            }
        }

        private void OnStateChange()
        {
            switch (_state)
            {
                case "Published":
                    //Do stuff publishVacation();
                    break;
                case "Rejected":
                    //Do stuff
                    break;
                case "Cancelled":
                    //Do stuff
                    break;
                case "GracePeriod":
                    //Do stuff
                    break;
                case "Completed":
                    //Do stuff
                    break;
                default:
                    break;
            }
            //TODO Add switch for every state, including adding to date to Dict
        }
    }
}
