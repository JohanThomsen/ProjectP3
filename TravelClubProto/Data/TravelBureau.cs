using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelBureau : Account
    {
        public string Name { get; set; }

        TravelBureau(string email, string password, DataAccessService daService) : base(email, password, daService) 
        {
            Type = "TravelBureau";
        }

        //SKAL OPDATERES
        public Vacation ProposeVacation(DateTime proposalDate, DateTime deadline, List<int> stretchGoals, List<decimal> prices, VacationData vacData)
        {
            return new Vacation(proposalDate, deadline, stretchGoals, prices, vacData);
        }

    }
}