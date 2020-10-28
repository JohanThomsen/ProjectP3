using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class Vacation
    {
        public string Type { get; set; }
        public string Destination { get; set; }
        public int Price { get; set; }

        public Vacation(string type, string destination, int price)
        {
            Type = type;
            Destination = destination;
            Price = price;
        }
    }
}
