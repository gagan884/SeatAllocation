using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework
{
    public interface IValidation
    {
        /// <summary>
        /// Custome Response values
        /// Code = PS > Test passed 
        /// Code = FL > Test failed 
        /// Code = NI > Test not implementes
        /// Code = NA > Test not applicable
        /// </summary>


        

        //Output Validation
        ActionOutput TotalSeatsWithAllotted(int roundNo);
        ActionOutput OldRetainCandidateAllocation(int roundNo);
        ActionOutput Dereservation(int roundNo);
        DataTable RankViolation(int roundNo);        
        DataTable ValidateVChoiceWithAllotmentSummary(int roundno, string rollno);
       


    }
}
