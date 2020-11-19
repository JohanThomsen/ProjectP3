using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelClubProto.Data
{
    public class VacationAdministrator
    {
        //TDODO skal nok bruge en dataccesservice
        public DataAccessService DaService { get; set; }
        private int VacID { get; set; }

        public VacationAdministrator(DataAccessService daService, int vacID)
        {
            DaService = daService;
            VacID = vacID;
        }

        public void OnStateChange(string state, string oldState)
        {
            if (oldState != null)
            {
                switch (state)
                {
                    case "Published":
                        if (oldState == "Proposed")
                        {
                            PublishVacation(state);
                        }
                        else
                        {
                            throw new InvalidStateException("Invalid new state");
                        }
                        break;
                    case "Rejected":
                        if (oldState == "Proposed")
                        {
                            RejectProposal(state);
                        }
                        else
                        {
                            throw new InvalidStateException("Invalid new state");
                        }

                        break;
                    case "Canceled":
                        CancelVacation(state);
                        break;
                    case "GracePeriod":
                        if (oldState == "Published")
                        {
                            StartGracePeriod(state);
                        }
                        else
                        {
                            throw new InvalidStateException("Invalid new state");
                        }
                        break;
                    case "Completed":
                        if (oldState == "GracePeriod")
                        {
                            CompleteVacation(state);
                        }
                        else
                        {
                            throw new InvalidStateException("Invalid new state");
                        }
                        break;
                    case "Proposed":
                        ResetVacation();
                        break;
                    default:
                        break;
                }
            }
            
        }


        private void PublishVacation(string state)
        {
            AddDateTimeAndChangeDate(state, "PublishDate");
        }


        private void RejectProposal(string state)
        {
            AddDateTimeAndChangeDate(state, "RejectionDate");
            //TODO add stuff for TravelBureau
        }

        private void CancelVacation(string state)
        {
            AddDateTimeAndChangeDate(state, "CancelDate");
            DeleteVacationRelation();
            //TODO Do stuff for travel Bureau
        }

        private void StartGracePeriod(string state)
        {
            AddDateTimeAndChangeDate(state, "GracePeriodDate");

            //TODO add stuff for TravelBureau
        }



        private void CompleteVacation(string state)
        {
            AddDateTimeAndChangeDate(state, "CompletionDate");

            DeleteVacationRelation();
        }


        private void DeleteVacationRelation()
        {
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = "DELETE FROM [dbo].CustomerVacationRelations WHERE FK_VacationID=@VacID";
                    cmd.Parameters.AddWithValue("@VacID", VacID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddDateTimeAndChangeDate(string state, string DateType)
        {
            DateTime now = DateTime.Now;
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"UPDATE [dbo].Vacation SET State=@state, {DateType}=@now WHERE ID=@VacID";
                    cmd.Parameters.AddWithValue("@state", state);
                    cmd.Parameters.AddWithValue("@now", now);
                    cmd.Parameters.AddWithValue("@VacID", VacID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Not needed in final production. Just usefull for now
        private void ResetVacation()
        {
            try
            {
                using (var sc = new SqlConnection(DaService.ConnectionString))
                using (var cmd = sc.CreateCommand())
                {
                    sc.Open();
                    cmd.CommandText = $"UPDATE [dbo].Vacation SET State='Proposed', PublishDate=NULL, GracePeriodDate=NULL, CancelDate=NULL, CompletionDate=NULL, RejectionDate=NULL WHERE ID=@VacID";
                    cmd.Parameters.AddWithValue("@VacID", VacID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
