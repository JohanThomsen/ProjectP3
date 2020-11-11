using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelBureau : Account
    {
        public string    Name                             { get; set; }
        public List<int> TravelBureauCompletedVacations = new List<int>();
        public List<int> RejectedVacations              = new List<int>();
        public List<int> TravelBureauProposedVacations  = new List<int>();

        TravelBureau(string username, string password, int id) : base(username, password) 
        {
            ID = id;
        }
    }
}