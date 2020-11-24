using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    //Having functions that change the class that it is aggregated to, isnt great, as its hard to move up the chain
    public class CustomerStateChanger
    {
        public Dictionary<DateTime, string> ChangeDateAndType = new Dictionary<DateTime, string>();
    }
}
