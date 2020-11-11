using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class VacationAdministrator
    {
        //TDODO skal nok bruge en dataccesservice
        public VacationAdministrator()
        {
        }

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

            //if (VacData.FindVac(vacationID).State == "GracePeriod")
            //{
                //AddDateTime("GracePeriodDate", vacationID);
                //VacData.GracePeriodVacations.Add(vacationID);
                //VacData.PublishedVacations.Remove(vacationID);
                //TODO add exceptions
            //}
        }

        public void CompleteVacation(int vacationID)
        {
            //if (VacData.FindVac(vacationID).State == "Completed")
            //{
                //AddDateTime("CompletionDate", vacationID);
                //TODO tilføj til TravelBureauCompletedVacations
                //TODO tilføj til TravelClubCompletedVacations
                //TODO tilføj til CustomerCompletedVacations
                //VacData.GracePeriodVacations.Remove(vacationID);
                //TODO add exceptions
            //}
        }

        public void PublishVacation(string state, int vacationID)
        {
            if (state == "Published")
            {
                //TODO fjern fra travelbureausProposedVacations
               // VacData.PublishedVacations.Add(vacationID);
               // VacData.ProposedVacations.Remove(vacationID);
                //AddDateTime("PublishDate", vacationID);
                //Add Exceptions
            }
        }

        public void CancelVacation(int vacationID)
        {
            //VacData.PublishedVacations.Remove(vacationID);
            //AddDateTime("CancelDate", vacationID);
            //TODO fjerne joinedVacation(Customer)
            //TODO fjern fra favouritedVacation(Customer)
        }

        public void RejectProposal(int vacationID)
        {


        }

        private void AddDateTime(string state, int vacationID)
        {
        }

    }
}
