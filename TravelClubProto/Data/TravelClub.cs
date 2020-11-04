using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelClub : Account
    {
        private string Type = "TravelClub";
        public List<Vacation> TravelClubCompletedVacations = new List<Vacation>();
        public List<Customer> CustomerList = new List<Customer>();
        public VacationData vacationAdministrator { get; set; }


        public TravelClub(string username, string password) : base(username, password)
        {
            
        }

        public void CreateCustomer(string username, string password)
        {
            CustomerList.Add(new Customer(username, password, CustomerList.Count + 1));
        }        
    }
}
