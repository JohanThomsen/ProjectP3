using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelClubProto.Data;

namespace TravelClubProto
{
    public class Destination
    {
        public string Location { get; set; }
        public string Hotel { get; set; }
        public List<Activity> Activities;
    }
}
