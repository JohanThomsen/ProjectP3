using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelClub : Account
    {
        Type = TravelClub;
        public List<Vacation> TravelClubCompletedVacations = new List<Vacation>();
        public List<Customer> CustomerList                 = new List<Customer>();
        public VacationAdministrator vacationAdministrator { get; set; }

        public void CreateCustomer(string username, string password)
        {
            CustomerList.Add(new Customer(username, password, CustomerList.Count + 1));
        }        
    }
}
