using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Customer : Account
    {
        private struct PersonalInformation
        {
            string Name         { get; set; }
            string EmailAddress { get; set; } 
        }
        
        private struct TravelPreferences
        {
            Destination Destination { get; set; }
            Activity    Activity    { get; set; }
        }
        
        private PriceAgentManager priceAgentManager { get; set; }

        private CustomerVacations customerVacations { get; set; }

        public Customer(string username, string password, int id) : base(string username, string password, int id)
        {
            ID = id;
        }

        private void ChangePersonalInformation(string name, string emailAddress)
        {
            PersonalInformation.Name         = name;
            PersonalInformation.EmailAddress = emailAddress;
        }

        private void ChangeCustomerPreferences(Destination destination, Activity activity)
        {
            TravelPreferences.Destination = destination;
            TravelPreferences.Activity    = activity;
        }



    }
}
