using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class TravelBureau : Account
    {
        public string Name { get; set; }


        public TravelBureau(string email, string password, DataAccessService daService) : base(email, password, daService)
        {
            Type = "TravelBureau";
        }

        public TravelBureau(int ID, string email, string password, DataAccessService daService) : base(ID, email, password, daService)
        {
            Type = "TravelBureau";
        }

    }
}
