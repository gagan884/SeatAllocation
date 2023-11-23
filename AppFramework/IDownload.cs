using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public interface IDownload
    {        
        DataTable GetAllCandidates(int roundno);
        DataTable GetEligibleCandiate(int roundno);
        DataTable GetChoice(int roundno);
        DataTable GetAllotment(int roundno);
        DataTable GetAllotmentSummary(int roundno);
        DataTable GetORCR(int roundno);
        DataTable PSeatBeforeProcessing(int roundno);
        DataTable PSeatAfterProcessing(int roundno);
    }
}
