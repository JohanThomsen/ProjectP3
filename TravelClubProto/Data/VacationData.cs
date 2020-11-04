using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class VacationData
    {
       public List<Vacation> AllVacations = new List<Vacation>();

       public List<int> ProposedVacations = new List<int>();
       public List<int> PublishedVacations = new List<int>();
       public List<int> GracePeriodVacations = new List<int>();
       public List<int> RecentlyChangedVacations = new List<int>();
       public int IDInc { get; set; }

        public Vacation FindVac(int vacationID)
        {
            for (int i = 0; i < AllVacations.Count; i++)
            {
                if (AllVacations[i].ID == vacationID)
                {
                    return AllVacations[i];
                }
            }
            return null;
        }
    }
}
