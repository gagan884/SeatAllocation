using AppFramework;
using System.Data;
using System;

namespace BAL
{
    public abstract class AbstractDownload:IDownload
    {      

        public abstract DataTable GetAllCandidates(int roundno);

        public virtual DataTable GetEligibleCandiate(int roundno)
        {
            DataTable dt = new DAL().GetDataTableUsingCommand("select * from XT_EligibleCandidate");
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        public abstract DataTable GetChoice(int roundno);

        public virtual DataTable GetAllotment(int roundno)
        {
            DataTable dt = new DAL().GetDataTableUsingCommand("select * from XT_Allotment");
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        public virtual DataTable GetAllotmentSummary(int roundno)
        {
            DataTable dt = new DAL().GetDataTableUsingCommand("select * from XT_AllotmentSummary");
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        public abstract DataTable GetORCR(int roundno);

        public virtual DataTable PSeatBeforeProcessing(int roundno)
        {
            DataTable dt = new DAL().GetDataTableUsingCommand("select * from XT_Seat");
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }

        public virtual DataTable PSeatAfterProcessing(int roundno)
        {
            DataTable dt = new DAL().GetDataTableUsingCommand("select  Instcd,brcd,Sequence,NewSeats tSeat,Allotted aSeat, NewSeats-Allotted bSeat from XT_AllotmentSummary");
            if (dt != null && dt.Rows.Count > 0)
                return dt;
            else
                return null;
        }
        
    }
}
