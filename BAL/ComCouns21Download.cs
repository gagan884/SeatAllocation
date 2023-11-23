using System;
using System.Data;

namespace BAL
{
    public class ComCouns21Download : AbstractDownload
    {
        public override DataTable GetAllCandidates(int roundno)
        {
            throw new NotImplementedException();
        }

        public override DataTable GetChoice(int roundno)
        {
            string query = @"select rollNo,optNo,instituteId,programId from App_Choice order by rollno,optno";
            return new DAL().GetDataTableUsingCommand(query);
        }

        public override DataTable GetORCR(int roundno)
        {
            string query = @"SELECT A.Instcd,A.Brcd,	
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'OPNO' and Examtype='AIEEE') THEN AIR END),0) AS minOPJEE,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'OPNO' and Examtype='AIEEE') THEN AIR END),0) AS maxOPJEE,          

	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'OPNO' and Examtype='WBJEE') THEN AIR END),0) AS minOP,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'OPNO' and Examtype='WBJEE') THEN AIR END),0) AS maxOP,  

    ISNULL(MIN(CASE WHEN (A.AllottedCat = 'OPWB' and Examtype='WBJEE') THEN AIR END),0) AS minOPWB,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'OPWB' and Examtype='WBJEE') THEN AIR END),0) AS maxOPWB, 
          										 
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'OPPH' and Examtype='WBJEE') THEN AIR END),0) AS minOPPD,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'OPPH' and Examtype='WBJEE') THEN AIR END),0) AS maxOPPD,          
          										 
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'SCNO' and Examtype='WBJEE') THEN AIR END),0) AS minSC,            
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'SCNO' and Examtype='WBJEE') THEN AIR END),0) AS maxSC,          
          										
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'SCPH' and Examtype='WBJEE') THEN AIR END),0) AS minSCPD,            
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'SCPH' and Examtype='WBJEE') THEN AIR END),0) AS maxSCPD,          
          										
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'STNO' and Examtype='WBJEE') THEN AIR END),0) AS minST,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'STNO' and Examtype='WBJEE') THEN AIR END),0) AS maxST,          
          										
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'STPH' and Examtype='WBJEE') THEN AIR END),0) AS minSTPD,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'STPH' and Examtype='WBJEE') THEN AIR END),0) AS maxSTPD,          
          										
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'BANO' and Examtype='WBJEE') THEN AIR END),0) AS minOBCA,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'BANO' and Examtype='WBJEE') THEN AIR END),0) AS maxOBCA,          
          										 
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'BAPH' and Examtype='WBJEE') THEN AIR END),0) AS minOBCA_PD,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'BAPH' and Examtype='WBJEE') THEN AIR END),0) AS maxOBCA_PD,
                            					
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'BBNO' and Examtype='WBJEE') THEN AIR END),0) AS minOBCB,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'BBNO' and Examtype='WBJEE') THEN AIR END),0) AS maxOBCB,          
          										 
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'BBPH' and Examtype='WBJEE') THEN AIR END),0) AS minOBCB_PD,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'BBPH' and Examtype='WBJEE') THEN AIR END),0) AS maxOBCB_PD,
												
	ISNULL(MIN(CASE WHEN (A.AllottedCat = 'TFNO' and Examtype='WBJEE') THEN AIR END),0) AS minTF,          
	ISNULL(MAX(CASE WHEN (A.AllottedCat = 'TFNO' and Examtype='WBJEE') THEN AIR END),0) AS maxTF  
   FROM Allotted A   
   WHERE RoundNo=(Select isnull(max(roundno),0) from Allotted)    
   GROUP BY A.Instcd,A.Brcd
";
            return new DAL().GetDataTableUsingCommand(query);
        }
    }
}
