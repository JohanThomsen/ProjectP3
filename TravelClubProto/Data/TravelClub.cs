using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelClub : Account
    {
        public TravelClub(int ID, string email, string password, DataAccessService daService) : base(ID, email, password, daService) 
        {
            Type = "TravelClub";
        }
        public TravelClub(string email, string password, DataAccessService daService) : base(email, password, daService)
        {
            Type = "TravelClub";
        }
    }
}
