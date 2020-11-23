using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Customer : Account
    {
        public struct PersonalInformation
        {
            public string Name        { get; set; }
            public string EmailAddress { get; set; } 
        }
        
        public struct TravelPreferences
        {
            public Destination Dest { get; set; }
            public Activity Act    { get; set; }
        }
        
        private PriceAgentManager priceAgentManager { get; set; }

        private CustomerVacations customerVacations { get; set; }

        public PersonalInformation PI;
        public TravelPreferences TP;

        public Customer(string email, string password, DataAccessService daService) : base(email, password, daService)
        {
            Type = "Customer";
        }

        public void ChangePersonalInformation(string name, string emailAddress)
        {
            PI.Name         = name;
            PI.EmailAddress = emailAddress;
        }

        private void ChangeCustomerPreferences(Destination destination, Activity activity)
        {
            TP.Dest = destination;
            TP.Act  = activity;
        }



    }
}
