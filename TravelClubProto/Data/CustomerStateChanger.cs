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

        public CustomerVacations VacationLists = new CustomerVacations(); //Not sure if this should be created locally, as the number of each class is uncertain

        private void JoinVacation(Vacation CurrentVacation) //Guessing the functions are called on command from the UI with some sort of access to the viewed vacation
        {
            VacationLists.JoinedVacations.Add(CurrentVacation);
            ChangeDateAndType.Add(DateTime.Now, "Joined " + CurrentVacation.ID);
        }
        private void LeaveVacation(Vacation CurrentVacation)
        {
            VacationLists.JoinedVacations.Remove(CurrentVacation);
            ChangeDateAndType.Add(DateTime.Now, "Left " + CurrentVacation.ID);
        }
        private void FavouriteVacation(Vacation CurrentVacation)
        {
            VacationLists.FavouritedVacations.Add(CurrentVacation);
            ChangeDateAndType.Add(DateTime.Now, "Favourited " + CurrentVacation.ID);
        }
        private void UnFavouriteVacation(Vacation CurrentVacation)
        {
            VacationLists.FavouritedVacations.Remove(CurrentVacation);
            ChangeDateAndType.Add(DateTime.Now, "Unfavourited " + CurrentVacation.ID);
        }
    }
}
