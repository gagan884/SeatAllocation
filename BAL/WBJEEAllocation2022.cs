using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace BAL
{
    public class WBJEEAllocation2022 : ComCouns21Allocation
    {
        public WBJEEAllocation2022() : base()
        {
            // board Id 
            boardId = "134112221";

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate_WBJEEB";

            //Procedure to prepare Previous allotment or seat gaurnatee candidates
            DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment_WBJEEB";

            //Procedure to dereserve seats
            DBProc_DereserveSeat = "XApp_CC_DereserveSeat";

            //Procedure to update the allotment,allotment summary and other output table
            //after completion of allocation
            DBProc_UpdateApplication = "XApp_CC_UpdateApplication_WBJEEB";

            //Procedure to get eligible candidates for processing
            DBProc_GetEligibleCandidate = "XP_GetEligibleCandidate";

            //Procedure to get mapping between institute-program and rank type
            DBProc_GetSeatRankTypeMapping = "XApp_CC_GetSeatRankTypeMapping";

            //Procedure to get mapping between institute-program and Stream
            DBProc_GetStreamMapping = "XApp_CC_InstituteProgramStreamMaster";

            //Procedure to get mapping between institute-program and seat type
            DBProc_GetSeatTypeMapping = "XApp_CC_InstituteProgramSeatTypeMaster";

            //Procedure to get mapping between institute-program and group type
            DBProc_GetGroupMapping = "XApp_CC_InstituteProgramGroupMaster";

            //Procedure to get mapping between institute-program and Quota
            DBProc_GetQuotaMapping = "XApp_CC_InstituteProgramQuotaMaster";

            //Procedure to get mapping between institute-program and gender
            DBProc_GetGenderMapping = "XApp_CC_InstituteProgramGenderMaster";

            
            DBProc_GetAllotmentSequence = "XApp_CC_GetAllotmentSequence";
            DBProc_CheckOutputTablesClean = "XApp_CC_AreOutputTablesClean";
            DBProc_CleanOutputTables = "XApp_CC_CleanOutputTables";            
            DBProc_GetCandidateChoice = "XApp_CC_GetCandidateChoice_WBJEEB";            
            DBProc_GetGetSymbolCatSubcatOptions = "XApp_CC_GetSymbolCatSubcatOptions";
            AllotmentSequnceMode = "Code";//"DB"/"Code"
            DBProc_GetCandidateAllChoices = "XApp_CC_GetCandidateChoiceAll";
        }


        string options = string.Empty;
        public override ActionOutput CreateVirtualChoice()
        {
            LoadPreviousAllotment();
            LoadSeatDetails();
            LoadQualificationMarksDetails();
            LoadQualificationSubjectMarksDetails();
            DataTable dtAllChoices = new DataTable();
            dtAllChoices.Columns.Add("rollNo");
            dtAllChoices.Columns.Add("totalChoices");
            dtAllChoices.Columns.Add("totalVChoices");
            dtAllChoices.Columns.Add("virtualChoices");
            int procOptno = 0;
            bool isValidChoice = false;
            Double rank;
            StringBuilder choices = new StringBuilder();
            int isRetained = 0, allottedOptno = 0, choiceOptNo = 0;

            SqlConnection connChoice = new SqlConnection(connectionString);
            SqlCommand cmdChoice;
            cmdChoice = new SqlCommand(DBProc_GetCandidateChoice, connChoice);

            cmdChoice.CommandType = CommandType.StoredProcedure;
            cmdChoice.Parameters.Add("@" + DBParam_RollNo, SqlDbType.VarChar);
            cmdChoice.Parameters.Add("@" + DBParam_BoardId, SqlDbType.Int);
            SqlDataReader rdrChoice = default(SqlDataReader);
            connChoice.Open();

            DataTable dtCand = SqlHelper.ExecuteDataset(connectionString, DBProc_GetEligibleCandidate).Tables[0];
            Dictionary<string, double> candRanks = null;
            string candRollNo = string.Empty;
            string candCategory = string.Empty;
            string candSubcategory = string.Empty;
            string candGender = string.Empty;
            string candDomicile = string.Empty;
            string candAdditionalInfo = string.Empty;
            string candSymbolData = string.Empty;
            string candRankData = string.Empty;
            string candWillingness = string.Empty;
            string choiceInstCd = string.Empty;
            string choiceBrCd = string.Empty;
            string choiceStreamId = string.Empty;
            string choiceRankType = string.Empty;
            string waitListKey = string.Empty;
            foreach (DataRow drCand in dtCand.Rows)
            {
                procOptno = 0;
                choices.Clear();
                candRollNo = drCand[DBParam_RollNo].ToString();
                candCategory = drCand[DBParam_CandCategory].ToString();
                candSubcategory = drCand[DBParam_CandSubcategory].ToString();
                candGender = drCand[DBParam_CandGender].ToString();
                candDomicile = drCand[DBParam_CandDomicile].ToString();
                candAdditionalInfo = drCand[DBParam_CandAddionalInfo].ToString();
                candSymbolData = drCand[DBParam_CandSymbol].ToString();
                candRankData = drCand[DBParam_CandRank].ToString();
                candWillingness = drCand[DBParam_CandWiilingness].ToString();
                candRanks = new Dictionary<string, double>();
                try
                {
                    foreach (string s in candRankData.Split(','))
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            candRanks.Add(s.Substring(0, 5), Convert.ToDouble(s.Substring(6)));
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ActionOutput(ActionStatus.Error, "Error:Invalid rank for Rollno:" + candRollNo + "," + ex.Message);
                }


                string catSubCatOptions = string.Empty;
                if (AllotmentSequnceMode.Equals("DB"))
                {
                    catSubCatOptions = GetAllotmentSequenceFromDB(candRollNo, candCategory, candSubcategory, candGender, candDomicile, candAdditionalInfo, candSymbolData, candRankData);
                }
                else
                {
                    catSubCatOptions = GetAllotmentSequence(candRollNo, candCategory, candSubcategory, candGender, candDomicile, candAdditionalInfo, candSymbolData, candRankData);
                }
                if (string.IsNullOrEmpty(catSubCatOptions))
                {
                    continue;
                }
                isRetained = 0;
                allottedOptno = 0;
                if (objAllAllotmentDetails.ContainsKey(candRollNo) && objAllAllotmentDetails[candRollNo].IsRetained != 0)
                {
                    isRetained = objAllAllotmentDetails[candRollNo].IsRetained;
                    allottedOptno = objAllAllotmentDetails[candRollNo].OptNo;
                }

                cmdChoice.Parameters["@" + DBParam_RollNo].SqlValue = candRollNo;
                cmdChoice.Parameters["@" + DBParam_BoardId].SqlValue = boardId;
                rdrChoice = cmdChoice.ExecuteReader();
                while ((rdrChoice.Read()))
                {
                    choiceInstCd = rdrChoice["instcd"].ToString().Trim();
                    choiceBrCd = rdrChoice["brcd"].ToString().Trim();
                    choiceOptNo = Convert.ToInt32(rdrChoice["optno"]);

                    if (isRetained == 0)
                    {
                        isValidChoice = true;
                    }
                    else
                    {
                        switch (candWillingness)
                        {
                            case "FL":
                                isValidChoice = (allottedOptno >= choiceOptNo);
                                break;
                            case "SL":
                                isValidChoice = (allottedOptno >= choiceOptNo && objAllAllotmentDetails[candRollNo].Instcd.Trim() == choiceInstCd);
                                break;
                            case "FR":
                                isValidChoice = (allottedOptno == choiceOptNo && objAllAllotmentDetails[candRollNo].Instcd.Trim() == choiceInstCd && objAllAllotmentDetails[candRollNo].Brcd.Trim() == choiceBrCd);
                                break;
                            case "RF":
                                isValidChoice = (allottedOptno > choiceOptNo);
                                break;
                            default:
                                throw new Exception("Invalid willingness");
                        }
                    }
                    if (isValidChoice)
                    {
                        string StreamId = "03";
                        if (choiceBrCd.Substring(0, 5).Equals("11633") && !choiceInstCd.Equals("104637"))
                        {
                            StreamId = "04";
                        }

                        string eligibilityType = isEligible(candRollNo, choiceInstCd, choiceBrCd);

                        foreach (string option in catSubCatOptions.Split(','))
                        {
                            if ((choiceBrCd.EndsWith("T") && !option.Equals("HS.NO.TF"))
                                || (!choiceBrCd.EndsWith("T") && option.Equals("HS.NO.TF"))
                               )
                            {
                                continue;
                            }


                            if (option.Substring(3, 5).Equals("OP.NO") && eligibilityType[0].Equals('N'))
                            {
                                SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text,
                                    string.Format("insert into XApp_InvalidChoiceOptions values('{0}', {1}, '{2}', '{3}', '{4}')",
                                    candRollNo, choiceOptNo, choiceInstCd, choiceBrCd, option
                                    ));
                                continue;
                            }
                            else if (eligibilityType[1].Equals('N'))
                            {
                                SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text,
                                    string.Format("insert into XApp_InvalidChoiceOptions values('{0}', {1}, '{2}', '{3}', '{4}')",
                                    candRollNo, choiceOptNo, choiceInstCd, choiceBrCd, option
                                    ));
                                continue;
                            }

                            waitListKey = choiceInstCd + "." + choiceBrCd + "." + StreamId + ".G1.WBJEE." + option + ".B";

                            if (SeatRankTypeMapping.ContainsKey(waitListKey) && candRanks.ContainsKey(SeatRankTypeMapping[waitListKey]))
                            {
                                choiceRankType = SeatRankTypeMapping[waitListKey];
                                rank = candRanks[choiceRankType];
                                procOptno = procOptno + 1;
                                choices = choices.Append(waitListKey + ":" + rank.ToString() + ",");
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
                rdrChoice.Close();
                if (procOptno > 0)
                {
                    dtAllChoices.Rows.Add(new object[] { drCand[DBParam_RollNo].ToString(), 0, procOptno, choices.ToString() });
                    choices.Clear();
                }
            }
            connChoice.Close();
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "truncate table XT_VirtualChoice");
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString) { DestinationTableName = "XT_VirtualChoice", BulkCopyTimeout = 350 };
            bulkCopy.WriteToServer(dtAllChoices);
            return new ActionOutput(ActionStatus.Success, "Completed:Choice created for" + dtAllChoices.Rows.Count + " candidates");
        }



        protected override string GetAllotmentSequence(string rollno, string category, string subcategoryList, string gender, string domicile, string additionalInfo, string symbolList, string rankData)
        {
            List<KeyValuePair<string, int>> optionList = new List<KeyValuePair<string, int>>();
            StringBuilder catSubCatOptions = new StringBuilder();
            catSubCatOptions.Clear();

            if (domicile.Equals("35"))
            {
                if (subcategoryList.ToUpper().Contains("TF:Yes".ToUpper()))
                    catSubCatOptions.Append("HS.NO.TF,");
            }

            catSubCatOptions.Append("AI.OP.NO,");

            if (domicile.Equals("35"))
            {
                catSubCatOptions.Append("HS.OP.NO,");
                if (subcategoryList.ToUpper().Contains("PH:Yes".ToUpper()))
                {
                    catSubCatOptions.Append("HS.OP.PH,");
                }

                if (category != "GN")
                {
                    if (domicile.Equals("35"))
                    {
                        catSubCatOptions.Append("HS." + category + ".NO,");
                    }

                    if (subcategoryList.ToUpper().Contains("PH:Yes".ToUpper()))
                    {
                        catSubCatOptions.Append("HS." + category + ".PH,");
                    }
                }

            }


            if (catSubCatOptions.Length > 0)
            {
                return catSubCatOptions.ToString().Substring(0, catSubCatOptions.Length - 1);
            }
            else
                return "";

        }




        private string getSubjectCode(string subject)
        {
            string code = "N";
            //P:006,C:007,M:008,B:010,Bt:053,Cs:022,Ca:068,E:002
            switch (subject)
            {
                case "P":
                    code = "006"; break;
                case "C":
                    code = "007"; break;
                case "M":
                    code = "008"; break;
                case "B":
                    code = "010"; break;
                case "Bt":
                    code = "053"; break;
                case "Cs":
                    code = "022"; break;
                case "Ca":
                    code = "068"; break;
                case "E":
                    code = "002"; break;
                case "Tv":
                    code = "197"; break;
                default:
                    code = "N"; break;
            }

            return code;

        }

        private string isEligible(string rollno, string instituteId, string programId)
        {


            string isEligibleForOpen = "Y";
            string isEligibleForReserved = "Y";

            if (programId[6].Equals("T"))
                return "YY";

            //Jadavpur(104637)+ other than Marine(11645)
            if (instituteId.Equals("104637") && !programId.Substring(0, 5).Equals("11645"))
            {
                var percentage_M = CalculateAverage(new string[] { "M" }, rollno);
                var percentage_PCM = CalculateAverage(new string[] { "P", "C", "M" }, rollno);
                if (percentage_M < 60 || percentage_PCM < 60)
                    isEligibleForOpen = "N";

                if (percentage_M < 45 || percentage_PCM < 45)
                    isEligibleForReserved = "N";
            }
            //CU(104640) , program other than Jute(11644)+marine(11645)
            else if (instituteId.Equals("104640") && !(new List<string> { "11644", "11645" }).Contains(programId.Substring(0, 5)))
            {
                var percentage_PCM = CalculateAverage(new string[] { "P", "C", "M" }, rollno);
                if (percentage_PCM < 60)
                    isEligibleForOpen = "N";

                if (percentage_PCM < 55)
                    isEligibleForReserved = "N";
            }
            //CU(104640) , Jute(11644)
            else if (instituteId.Equals("104640") && "11644".Equals(programId.Substring(0, 5)))
            {
                var betterOfSubject = GetBetterOf(new string[] { "C", "B", "Bt" }, rollno);
                if (betterOfSubject.Equals(string.Empty))
                {
                    return "NN";
                }

                var percentage_PCT = CalculateAverage(new string[] { "P", "M", betterOfSubject }, rollno);
                if (percentage_PCT < 60)
                    isEligibleForOpen = "N";

                if (percentage_PCT < 55)
                    isEligibleForReserved = "N";
            }

            //JIS(104632) ,  other than pharma(11633)/marine (11645)
            else if (instituteId.Equals("104632") && !(new List<string> { "11633", "11645" }).Contains(programId.Substring(0, 5)))
            {
                var betterOfSubject = GetBetterOf(new string[] { "C", "B", "Bt", "Cs", "Ca", "Tv" }, rollno);
                if (betterOfSubject.Equals(string.Empty))
                {
                    return "NN";
                }
                var percentage_PCT = CalculateAverage(new string[] { "P", "M", betterOfSubject }, rollno);
                var class12percentage = GetClass12Percentage(rollno);
                if (percentage_PCT < 60 || class12percentage < 60)
                    isEligibleForOpen = "N";

                if (percentage_PCT < 55 || class12percentage < 55)
                    isEligibleForReserved = "N";
            }
            //WBUAH(104643)/BCKV(104635) ,  Dairy Technology(10918)
            else if ((instituteId.Equals("104643") || instituteId.Equals("104635")) && !(new List<string> { "11633", "11645" }).Contains(programId.Substring(0, 5)))
            {
                var percentage_PCME = CalculateAverage(new string[] { "P", "C", "M", "E" }, rollno);
                if (percentage_PCME < 50)
                    isEligibleForOpen = "N";

                if (percentage_PCME < 40)
                    isEligibleForReserved = "N";
            }
            //All ,  Pharma(11633)
            else if ("11633".Equals(programId.Substring(0, 5)))
            {
                var betterOfSubject = GetBetterOf(new string[] { "M", "B" }, rollno);
                if (betterOfSubject.Equals(string.Empty))
                {
                    return "NN";
                }
                var percentage_PCT = CalculateAverage(new string[] { "P", "C", betterOfSubject }, rollno);
                if (percentage_PCT < 45)
                    isEligibleForOpen = "N";

                if (percentage_PCT < 40)
                    isEligibleForReserved = "N";
            }
            //All ,  other than Pharma(11633)/Marine (11645)
            else if (!(new List<string> { "11633", "11645" }).Contains(programId.Substring(0, 5)))
            {
                var betterOfSubject = GetBetterOf(new string[] { "C", "B", "Bt", "Cs", "Ca", "Tv" }, rollno);
                if (betterOfSubject.Equals(string.Empty))
                {
                    return "NN";
                }
                var percentage_PCT = CalculateAverage(new string[] { "P", "M", betterOfSubject }, rollno);
                if (percentage_PCT < 45)
                    isEligibleForOpen = "N";

                if (percentage_PCT < 40)
                    isEligibleForReserved = "N";
            }
            return isEligibleForOpen + isEligibleForReserved;
        }


        private double GetClass12Percentage(string rollno)
        {
            if (qualificationMarksDetails.ContainsKey(rollno + ".04"))
                return qualificationMarksDetails[rollno + ".04"].percentageMarks;
            else
                return 0;
        }


        protected Dictionary<string, QualificationMarksDetail> qualificationMarksDetails;
        protected void LoadQualificationMarksDetails()
        {
            qualificationMarksDetails = new Dictionary<string, QualificationMarksDetail>();
            string qry = "select boardId,rollNo,qualificationId, passStatus,obtainedMarks,maximumMarks,percentageMarks from App_QualificationMarksDetail where qualificationId='04' and passStatus='01'";
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, qry).Tables[0];
            foreach (DataRow dr in dtOptions.Rows)
            {
                qualificationMarksDetails.Add(dr["rollNo"].ToString() + "." + dr["qualificationId"].ToString(),
                       new QualificationMarksDetail()
                       {
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["obtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["maximumMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }
        }


        protected Dictionary<string, QualificationMarksDetail> qualificationSubjectMarksDetails;
        protected void LoadQualificationSubjectMarksDetails()
        {
            qualificationSubjectMarksDetails = new Dictionary<string, QualificationMarksDetail>();
            string qry = "select boardId,rollNo,qualificationId, subjectId, passStatus,totalObtainedMarks,totalMaxMarks,percentageMarks from App_QualificationSubjectMarksDetail where qualificationId='04' and passStatus='01' and subjectId<>'ZZ' order by rollNo,qualificationId,subjectId";
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, qry).Tables[0];

            foreach (DataRow dr in dtOptions.Rows)
            {
                qualificationSubjectMarksDetails.Add(dr["rollNo"].ToString() + "." + dr["qualificationId"].ToString() + "." + dr["subjectId"].ToString(),
                       new QualificationMarksDetail()
                       {
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["totalObtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["totalMaxMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }
        }

        private string GetBetterOf(string[] subjectList, string candRollNo)
        {
            double maxPercentage = 0;
            string subjectCode = string.Empty;
            string key = string.Empty;
            foreach (string s in subjectList)
            {
                key = candRollNo + ".04." + getSubjectCode(s);
                if (qualificationSubjectMarksDetails.ContainsKey(key))
                {
                    if (qualificationSubjectMarksDetails[key].percentageMarks > maxPercentage)
                    {
                        subjectCode = s;
                        maxPercentage = qualificationSubjectMarksDetails[key].percentageMarks;
                    }
                }
            }
            return subjectCode;
        }

        private double CalculateAverage(string[] subjectList, string candRollNo)
        {
            double totalObtainedMarks = 0;
            double totalMaxMarks = 0;
            string key = string.Empty;

            foreach (string s in subjectList)
            {
                key = candRollNo + ".04." + getSubjectCode(s);
                if (qualificationSubjectMarksDetails.ContainsKey(key))
                {
                    totalObtainedMarks += qualificationSubjectMarksDetails[key].totalObtainedMarks;
                    totalMaxMarks += qualificationSubjectMarksDetails[key].totalMaximumMarks;
                }
                else
                    return 0;
            }

            if (totalMaxMarks > 0)
            {
                return (totalObtainedMarks * 100) / totalMaxMarks;
            }
            else
                return 0;
        }
    }
    //public class QualificationMarksDetail /*Commented as the same class is already present in JELET*/
    //{
    //    public string passStatus;
    //    public double totalObtainedMarks;
    //    public double totalMaximumMarks;
    //    public double percentageMarks;
    //}
}
