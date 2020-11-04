using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class VacationAdministrator
    {
       public List<Vacation> AllVacations = new List<Vacation>();

       public List<int> ProposedVacations = new List<int>();
       public List<int> PublishedVacations = new List<int>();
       public List<int> GracePeriodVacations = new List<int>();
       public List<int> RecentlyChangedVacations = new List<int>();
       public static int IDInc = 1;

        public void OnStateChange(string state, int vacationID)
        {
            switch (state)
            {
                case "Published":
                    PublishVacation(state, vacationID);
                    break;
                case "Rejected":
                    RejectProposal(vacationID);
                    break;
                case "Cancelled":
                    CancelVacation(vacationID);
                    break;
                case "GracePeriod":
                    StartGracePeriod(vacationID);
                    break;
                case "Completed":
                    CompleteVacation(vacationID);
                    break;
                default:
                    break;
            }
        }

        public void StartGracePeriod(int vacationID)
        {

            if (FindVac(vacationID).State == "GracePeriod") 
            {
                AddDateTime("GracePeriodDate", vacationID);
                GracePeriodVacations.Add(vacationID);
                PublishedVacations.Remove(vacationID);
                //TODO add exceptions
            }
        }

        public void CompleteVacation(int vacationID)
        {
            if (FindVac(vacationID).State == "Completed")
            {
                AddDateTime("CompletionDate", vacationID);
                //TODO tilføj til TravelBureauCompletedVacations
                //TODO tilføj til TravelClubCompletedVacations
                //TODO tilføj til CustomerCompletedVacations
                GracePeriodVacations.Remove(vacationID);
                //TODO add exceptions
            }
        }

        public void PublishVacation(string state, int vacationID)
        {
            if (state == "Published")
            {
                //TODO fjern fra travelbureausProposedVacations
                PublishedVacations.Add(vacationID);
                ProposedVacations.Remove(vacationID);
                AddDateTime("PublishDate", vacationID);
                //Add Exceptions
            }
        }

        public void CancelVacation(int vacationID)
        {
            PublishedVacations.Remove(vacationID);
            AddDateTime("CancelDate", vacationID);
            //TODO fjerne joinedVacation(Customer)
            //TODO fjern fra favouritedVacation(Customer)
        }

        public void RejectProposal(int vacationID)
        {
            if (FindVac(vacationID).State == "Proposed")
            {
                ProposedVacations.Remove(vacationID);
                AddDateTime("RejectionDate", vacationID);
                //TODO Fjern fra TravelBureauProposedVacations
                //TODO tilføj til RejectedVacations(travelbureau)
                //TODO Add exceptions
            }

        }

        private void AddDateTime (string state, int vacationID)
        {
            Vacation currentVac = FindVac(vacationID);
            currentVac.Dates.Add(state, DateTime.Now);
        }

        public Vacation FindVac(int vacationID)
        {
            Vacation currentVac = null;
            Console.WriteLine(AllVacations);
            for (int i = 0; i < AllVacations.Count; i++)
            {
                if (AllVacations[i].ID == vacationID)
                {
                    currentVac = AllVacations[i];
                    Console.WriteLine("In if" + currentVac.ID);
                }
            }

            Console.WriteLine(currentVac.ID);
            return currentVac;
        }
    }
}
