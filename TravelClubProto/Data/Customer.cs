using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Customer : Account
    {
        private PriceAgentManager priceAgentManager { get; set; }
        private CustomerVacations customerVacations { get; set; }

        public Customer(string email, string password, DataAccessService daService) : base(email, password, daService)
        {
            Type = "Customer";
            customerVacations = new CustomerVacations(daService, ID);
            priceAgentManager = new PriceAgentManager(daService, email, ID);
            PI.EmailAddress = email;
        }
        public struct PersonalInformation
        {
            public string Name        { get; set; }
            public string EmailAddress { get; set; } 
        }

        public PersonalInformation PI;

        public void ChangePersonalInformation(string name, string emailAddress)
        {
            PI.Name         = name;
            PI.EmailAddress = emailAddress;
        }
    }
}
