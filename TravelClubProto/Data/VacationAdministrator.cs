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
       public static int IDInc = 0;

        public void OnStateChange(string state, int ID)
        {
            switch (state)
            {
                case "Published":
                    PublishVacation(ID);
                    break;
                case "Rejected":
                    RejectProposal(ID);
                    break;
                case "Cancelled":
                    CancelVacation(ID);
                    break;
                case "GracePeriod":
                    StartGracePeriod(ID);
                    break;
                case "Completed":
                    CompleteVacation(ID);
                    break;
                default:
                    break;
            }
        }

        public void StartGracePeriod(int ID)
        {

            if (FindVac(ID).State == "Published") 
            {
                AddDateTime("GracePeriodDate", ID);
                GracePeriodVacations.Add(ID);
                PublishedVacations.Remove(ID);
                //TODO add exceptions
            }
        }

        public void CompleteVacation(int ID)
        {
            if (FindVac(ID).State == "GracePeriod")
            {
                AddDateTime("CompletionDate", ID);
                //TODO tilføj til TravelBureauCompletedVacations
                //TODO tilføj til TravelClubCompletedVacations
                //TODO tilføj til CustomerCompletedVacations
                GracePeriodVacations.Remove(ID);
                //TODO add exceptions
            }
        }

        public void PublishVacation(int ID)
        {
            if (FindVac(ID).State == "Published")
            {
                //TODO fjern fra travelbureausProposedVacations
                PublishedVacations.Add(ID);
                ProposedVacations.Remove(ID);
                AddDateTime("PublishDate", ID);
                //Add Exceptions
            }
        }

        public void CancelVacation(int ID)
        {
            PublishedVacations.Remove(ID);
            AddDateTime("CancelDate", ID);
            //TODO fjerne joinedVacation(Customer)
            //TODO fjern fra favouritedVacation(Customer)
        }

        public void RejectProposal(int ID)
        {
            if (FindVac(ID).State == "Proposed")
            {
                ProposedVacations.Remove(ID);
                AddDateTime("RejectionDate", ID);
                //TODO Fjern fra TravelBureauProposedVacations
                //TODO tilføj til RejectedVacations(travelbureau)
                //TODO Add exceptions
            }

        }

        private void AddDateTime (string state, int ID)
        {
            FindVac(ID).Dates.Add(state, DateTime.Now);
        }

        public Vacation FindVac(int ID)
        {
            Vacation currentVac = null;
            foreach (Vacation vac in AllVacations)
            {
                if (vac.ID == ID)
                {
                    currentVac = vac;
                }
            }

            return currentVac;
        }
    }
}
