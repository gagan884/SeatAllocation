using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public interface IAllocation
    {
        ActionOutput InItAllocation(int roundno);         
        ActionOutput PrepareSeat();
        ActionOutput PrepareEligibleCandidate();
        ActionOutput PreparePreviousAllotment();
        ActionOutput CreateVirtualChoice();        
        ActionOutput AllotSeat();       
        ActionOutput AllotSeatNew();       
        ActionOutput PrepareAllotmentSummary();
        ActionOutput UpdateApplicationAfterAllocation();
        DataTable GetVirtualChoice(string rollno);
        DataTable GetOriginalChoice(string rollno);
        DataSet GetCandidateDetails(string rollno);
        ActionOutput GetAllotmentOverview(int roundNo);
        ActionOutput UnloadAllocation();
        ActionOutput VirtualCreationNew();
        void SetRoundNo(int roundno);

    }
}
