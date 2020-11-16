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

    }
}