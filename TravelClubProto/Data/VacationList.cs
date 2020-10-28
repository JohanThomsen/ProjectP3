using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class VacationAdmin
    {
        public int Age = 10;

        public List<Vacation> PublishedVacations { get; } = new List<Vacation>();

        public void PublishVacation(Vacation newVac)
        {
            PublishedVacations.Add(newVac);
        }
    }
}
