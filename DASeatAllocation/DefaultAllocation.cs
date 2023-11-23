using AppFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DASeatAllocation
{
    public class DefaultAllocation : IAllocation
    {
        SQLHelper objSql=null;
        Queue<string> Applicants = default(Queue<string>);
        Dictionary<string, int> Seats = default(Dictionary<string, int>);
        Dictionary<string, List<WaitListCandidate>> WaitListArray = default(Dictionary<string, List<WaitListCandidate>>);
        Dictionary<string, AllotmentDetail> allotmentDetails = default(Dictionary<string, AllotmentDetail>);

        DataTable dtSeats;
        Dictionary<string, Int16> PIList = default(Dictionary<string, Int16>);
        SqlConnection objSqlConnection = default(SqlConnection);


        public CustomeResponse PrepareSeat(int roundno)
        {
            
        }        

        public CustomeResponse PrepareEligibleCandidate(int roundno)
        {
            throw new NotImplementedException();
        }

        public CustomeResponse CreateVirtualChoice(int roundno)
        {
            throw new NotImplementedException();
        }

        public DataTable GetVirtualChoice(int roundno, string rollno)
        {
            throw new NotImplementedException();
        }

        public CustomeResponse AllotSeat(int roundno)
        {
            throw new NotImplementedException();
        }

        public CustomeResponse DereserveSeat(int roundno)
        {
            throw new NotImplementedException();
        }        

        public CustomeResponse PrepareAllotmentSummary(int roundno)
        {
            throw new NotImplementedException();
        }

        

        
    }
}
