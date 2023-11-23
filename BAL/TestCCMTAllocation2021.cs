using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace BAL
{
    public class TestCCMTAllocation2021 : AbstractAllocation
    {
        protected string DBProc_CheckOutputTablesClean = "XApp_CC_AreOutputTablesClean";
        protected string DBProc_CleanOutputTables = "XApp_CC_CleanOutputTables";
        protected string DBProc_PrepareSeat = "XApp_CC_PrepareSeat";
        protected string DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate";
        protected string DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment";
        protected string DBProc_GetCandidateChoice = "XApp_CC_GetCandidateChoice";
        protected string DBProc_GetSeatRankTypeMapping = "XApp_CC_GetSeatRankTypeMapping";
        protected string DBProc_GetStreamMapping = "XApp_CC_InstituteProgramStreamMaster";
        protected string DBProc_GetSeatTypeMapping = "XApp_CC_InstituteProgramSeatTypeMaster";
        protected string DBProc_GetGroupMapping = "XApp_CC_InstituteProgramGroupMaster";
        protected string DBProc_GetQuotaMapping = "XApp_CC_InstituteProgramQuotaMaster";
        protected string DBProc_GetGenderMapping = "XApp_CC_InstituteProgramGenderMaster";
        protected string DBProc_GetAllotmentSequence = "XApp_CC_GetAllotmentSequence";
        protected string DBProc_GetEligibleCandidate = "XP_GetEligibleCandidate";
        protected string DBProc_UpdateApplication = "XApp_CC_UpdateApplication";
        protected string DBProc_GetGetSymbolCatSubcatOptions = "XApp_CC_GetSymbolCatSubcatOptions";
        protected string AllotmentSequnceMode = "Code";//DB Code
        protected string DBProc_GetCandidateAllChoices = "XApp_CC_GetCandidateChoiceAll";
        protected string DBProc_DereserveSeat = "XApp_CC_DereserveSeat";


        protected const string DBParam_BoardId = "boardId";
        protected const string DBParam_RoundNo = "roundNo";
        protected const string DBParam_RollNo = "rollNo";
        protected const string DBParam_GateCd = "GateCd"; /*Added by Shivam on 27May2022*/
        protected const string DBParam_QDegreeType = "QDegreeType"; /*Added by Shivam on 27May2022*/
        protected const string DBParam_Degree = "Degree"; /*Added by Shivam on 27May2022*/
        protected const string DBParam_CandCategory = "category";
        protected const string DBParam_CandSubcategory = "subcategory";
        protected const string DBParam_CandGender = "gender";
        protected const string DBParam_CandDomicile = "domicile";
        protected const string DBParam_CandAddionalInfo = "additionalInfo";
        protected const string DBParam_CandSymbol = "symbol";
        protected const string DBParam_CandRank = "rank";
        protected const string DBParam_CandWiilingness = "willingness";
        protected const string DBParam_InstituteProgramId = "instituteProgram";
        protected const string DBParam_WLKey = "wlKey";
        protected const string DBParam_RankType = "rankType";
        protected const string DBParam_result = "result";
        protected const string DBParam_allotmentSequence = "allotmentSequence";
        protected const string DBParam_allotmentTable = "dtAllotment";
        protected const string DBParam_allotmentSummaryTable = "dtAllotmentSummary";
        protected const string DBParam_CatSubcatOptionsKey = "CandCatSubcat";
        protected const string DBParam_CatSubcatOptionsvalue = "availableCatSubCatOption";
        protected const string DBParam_isSeatConversionAllowed = "isSeatConversionAllowed";
        protected const string DBParam_isIterationrequired = "isIterationrequired";
        protected string boardId;
        protected string GateCd;
        protected string QDegreeType;
        protected string Degree;
        protected string connectionString;

        public TestCCMTAllocation2021() : base()
        {

            // board Id 
            boardId = "105012121"; /*BoardId changed and refered from CCMT2021 counselling db*/

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate_CCMT";

            //Procedure to prepare Previous allotment or seat gaurnatee candidates
            DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment";

            //Procedure to dereserve seats
            DBProc_DereserveSeat = "XApp_CC_DereserveSeat";

            //Procedure to update the allotment,allotment summary and other output table
            //after completion of allocation
            DBProc_UpdateApplication = "XApp_CC_UpdateApplication";

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
            DBProc_GetCandidateChoice = "XApp_CC_GetCandidateChoice_CCMT";            
            DBProc_GetGetSymbolCatSubcatOptions = "XApp_CC_GetSymbolCatSubcatOptions";
            AllotmentSequnceMode = "Code";//"DB"/"Code"
            DBProc_GetCandidateAllChoices = "XApp_CC_GetCandidateChoiceAll";

            connectionString = ObjectFactory.GetCommonObject().GetConnectionString();
        }
        public override ActionOutput PrepareSeat()
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_PrepareSeat, sqlParameters);
            int totalSeat = Convert.ToInt32(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select isnull(sum(tseat),0) from XT_Seat").Tables[0].Rows[0][0]);
            return new ActionOutput(ActionStatus.Success, "Completed:" + totalSeat.ToString());
        }

        public override ActionOutput PrepareEligibleCandidate()
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_PrepareEligibleCandidate, sqlParameters);
            int eligibleCandidateCount = Convert.ToInt32(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select isnull(count(*),0) from XT_EligibleCandidate").Tables[0].Rows[0][0]);
            return new ActionOutput(ActionStatus.Success, "Completed:" + eligibleCandidateCount.ToString());
        }

        public override ActionOutput PreparePreviousAllotment()
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_PreparePreviousAllotment, sqlParameters);
            int previousAllotment = Convert.ToInt32(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select isnull(count(*),0) from XT_PreviousAllotment").Tables[0].Rows[0][0]);
            return new ActionOutput(ActionStatus.Success, "Completed:" + previousAllotment.ToString());
        }

        public override ActionOutput CreateVirtualChoice()
        {
            LoadPreviousAllotment();
            LoadSeatDetails();
            LoadStreamInfo();
            LoadSeatTypeInfo();
            LoadGenderInfo();
            LoadGroupInfo();
            LoadQuotaInfo();
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
            cmdChoice.Parameters.Add("@" + DBParam_RoundNo, SqlDbType.Int);
            cmdChoice.Parameters.Add("@" + DBParam_GateCd, SqlDbType.VarChar);
            cmdChoice.Parameters.Add("@" + DBParam_QDegreeType, SqlDbType.Int);
            cmdChoice.Parameters.Add("@" + DBParam_Degree, SqlDbType.VarChar);

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
            string choiceGroupId = string.Empty;
            string choiceSeatTypeId = string.Empty;
            string choiceGenderId = string.Empty;
            string choiceRankType = string.Empty;
            string choiceQuotaId = string.Empty;
            string waitListKey = string.Empty;
            string candidateGate = string.Empty;
            string candidateDegree = string.Empty;
            string candidateDegreeType = string.Empty;
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
                candidateGate = drCand["AdditionalInfo"].ToString().Split('|')[1].Split('=')[1];
                candidateDegree = drCand["AdditionalInfo"].ToString().Split('|')[2].Split('=')[1];
                candidateDegreeType = drCand["AdditionalInfo"].ToString().Split('|')[0].Split('=')[1];
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
                    //catSubCatOptions = GetAllotmentSequenceFromDB(candRollNo, candCategory, candSubcategory, candGender, candDomicile, candAdditionalInfo, candSymbolData, candRankData);
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
                cmdChoice.Parameters["@" + DBParam_GateCd].SqlValue = candidateGate;
                cmdChoice.Parameters["@" + DBParam_Degree].SqlValue = candidateDegree;
                cmdChoice.Parameters["@" + DBParam_QDegreeType].SqlValue = candidateDegreeType;
                cmdChoice.Parameters["@" + DBParam_RoundNo].SqlValue = roundNo;
                rdrChoice = cmdChoice.ExecuteReader();
                while ((rdrChoice.Read()))
                {
                    choiceInstCd = rdrChoice["instcd"].ToString().Trim();
                    choiceBrCd = rdrChoice["brcd"].ToString().Trim();
                    choiceOptNo = Convert.ToInt32(rdrChoice["optno"]);
                    choiceGroupId = rdrChoice["GroupCd"].ToString().Trim();

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
                        choiceStreamId = StreamInfoMaster[choiceInstCd + "." + choiceBrCd];
                        choiceSeatTypeId = SeatTypeMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGenderId = GenderMaster[choiceInstCd + "." + choiceBrCd];
                        choiceQuotaId = QuotaMaster[choiceInstCd + "." + choiceBrCd];

                        foreach (string option in catSubCatOptions.Split(','))
                        {
                            waitListKey = choiceInstCd + "." + choiceBrCd + "." + choiceStreamId + "." + choiceGroupId + "." + choiceSeatTypeId + "." + choiceQuotaId + "." + option + "." + choiceGenderId;

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
            return new ActionOutput(ActionStatus.Success, "completed:" + dtAllChoices.Rows.Count);
        }

        protected Dictionary<string, string> SeatRankTypeMapping = null;
        protected void LoadSeatDetails()
        {
            SeatRankTypeMapping = new Dictionary<string, string>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            DataTable dtSeats = SqlHelper.ExecuteDataset(connectionString, DBProc_GetSeatRankTypeMapping, sqlParameters).Tables[0];
            foreach (DataRow dr in dtSeats.Rows)
            {
                //if (dr["wlKey"].ToString() == "700057.UN04.PU.G1.NA.AS.BC.NO.B")
                //{
                //    continue;
                //}
                SeatRankTypeMapping.Add(dr["wlKey"].ToString(), dr["rankTypeId"].ToString());
            }
        }

        protected Dictionary<string, string> StreamInfoMaster;
        protected void LoadStreamInfo()
        {
            StreamInfoMaster = new Dictionary<string, string>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, DBProc_GetStreamMapping, sqlParameters).Tables[0];
            foreach (DataRow dr in dtOptions.Rows)
            {
                StreamInfoMaster.Add(dr["instituteProgram"].ToString(), dr["StreamId"].ToString());
            }
        }

        protected Dictionary<string, string> SeatTypeMaster;
        protected void LoadSeatTypeInfo()
        {
            SeatTypeMaster = new Dictionary<string, string>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, DBProc_GetSeatTypeMapping, sqlParameters).Tables[0];
            foreach (DataRow dr in dtOptions.Rows)
            {
                SeatTypeMaster.Add(dr["instituteProgram"].ToString(), dr["SeatTypeId"].ToString());
            }
        }


        protected Dictionary<string, string> GroupMaster;
        protected void LoadGroupInfo()
        {
            GroupMaster = new Dictionary<string, string>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, DBProc_GetGroupMapping, sqlParameters).Tables[0];
            foreach (DataRow dr in dtOptions.Rows)
            {
                GroupMaster.Add(dr["instituteProgram"].ToString(), dr["GroupId"].ToString());
            }
        }

        protected Dictionary<string, string> QuotaMaster;
        protected void LoadQuotaInfo()
        {
            QuotaMaster = new Dictionary<string, string>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, DBProc_GetQuotaMapping, sqlParameters).Tables[0];
            foreach (DataRow dr in dtOptions.Rows)
            {
                QuotaMaster.Add(dr["instituteProgram"].ToString(), dr["QuotaId"].ToString());
            }
        }


        protected Dictionary<string, string> GenderMaster;
        protected void LoadGenderInfo()
        {
            GenderMaster = new Dictionary<string, string>();
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            DataTable dtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, DBProc_GetGenderMapping, sqlParameters).Tables[0];
            foreach (DataRow dr in dtOptions.Rows)
            {
                GenderMaster.Add(dr["instituteProgram"].ToString(), dr["GenderId"].ToString());
            }
        }







        string options = string.Empty;

        private Dictionary<string, int> SurrendredCandidates;

        public override bool DereserveSeat(int iterationSeq)
        {
            SqlParameter[] sqlParameters = new SqlParameter[4];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            sqlParameters[2] = new SqlParameter("@" + DBParam_isIterationrequired, SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output };
            int status = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_DereserveSeat, sqlParameters);
            if (sqlParameters[2].Value.Equals("N"))
                return false;
            else
                return true;
        }

        public override DataTable GetOriginalChoice(string rollno)
        {
            SqlParameter[] sqlParameters = new SqlParameter[3];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            sqlParameters[2] = new SqlParameter("@" + DBParam_RollNo, rollno);
            DataSet ds = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, DBProc_GetCandidateAllChoices, sqlParameters);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        public override ActionOutput UpdateApplicationAfterAllocation()
        {
            LoadOutputTables();
            SqlConnection connUpdateApp = new SqlConnection(connectionString);
            connUpdateApp.Open();
            SqlCommand cmd = new SqlCommand(DBProc_UpdateApplication, connUpdateApp) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@" + DBParam_RoundNo, roundNo);
            cmd.Parameters.AddWithValue("@" + DBParam_BoardId, boardId);
            cmd.Parameters.AddWithValue("@" + DBParam_allotmentTable, dtAllotmentTypeTable);
            cmd.Parameters.AddWithValue("@" + DBParam_allotmentSummaryTable, dtAllotmentSummaryTypeTable);

            cmd.ExecuteNonQuery();
            connUpdateApp.Close();
            return new ActionOutput(ActionStatus.Success);
        }

        DataTable dtAllotmentTypeTable = null;
        DataTable dtAllotmentSummaryTypeTable = null;
        private void LoadOutputTables()
        {
            dtAllotmentTypeTable = new DataTable();
            dtAllotmentTypeTable.Columns.Add("boardId");
            dtAllotmentTypeTable.Columns.Add("roundNo");
            dtAllotmentTypeTable.Columns.Add("rollNo");
            dtAllotmentTypeTable.Columns.Add("ranktypeId");
            dtAllotmentTypeTable.Columns.Add("rank");
            dtAllotmentTypeTable.Columns.Add("instituteId");
            dtAllotmentTypeTable.Columns.Add("programId");
            dtAllotmentTypeTable.Columns.Add("streamId");
            dtAllotmentTypeTable.Columns.Add("groupId");
            dtAllotmentTypeTable.Columns.Add("seatTypeId");
            dtAllotmentTypeTable.Columns.Add("allottedCat");
            dtAllotmentTypeTable.Columns.Add("allottedQuota");
            dtAllotmentTypeTable.Columns.Add("seatGenderId");

            dtAllotmentSummaryTypeTable = new DataTable();
            dtAllotmentSummaryTypeTable.Columns.Add("boardId");
            dtAllotmentSummaryTypeTable.Columns.Add("roundNo");
            dtAllotmentSummaryTypeTable.Columns.Add("instituteId");
            dtAllotmentSummaryTypeTable.Columns.Add("programId");
            dtAllotmentSummaryTypeTable.Columns.Add("streamId");
            dtAllotmentSummaryTypeTable.Columns.Add("groupId");
            dtAllotmentSummaryTypeTable.Columns.Add("seatType");
            dtAllotmentSummaryTypeTable.Columns.Add("quotaId");
            dtAllotmentSummaryTypeTable.Columns.Add("categoryId");
            dtAllotmentSummaryTypeTable.Columns.Add("subcategoryId");
            dtAllotmentSummaryTypeTable.Columns.Add("genderId");
            dtAllotmentSummaryTypeTable.Columns.Add("rankTypeId");
            dtAllotmentSummaryTypeTable.Columns.Add("InItSeats");
            dtAllotmentSummaryTypeTable.Columns.Add("NewSeats");
            dtAllotmentSummaryTypeTable.Columns.Add("Allotted");
            dtAllotmentSummaryTypeTable.Columns.Add("OpeningRank");
            dtAllotmentSummaryTypeTable.Columns.Add("ClosingRank");
            dtAllotmentSummaryTypeTable.Columns.Add("OpeningRank_NewCand");
            dtAllotmentSummaryTypeTable.Columns.Add("ClosingRank_NewCand");
            dtAllotmentSummaryTypeTable.Columns.Add("DereserveFrom");
            dtAllotmentSummaryTypeTable.Columns.Add("DereserveTo");

            DataTable dtAllotment = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select * from XT_Allotment").Tables[0];
            foreach (DataRow dr in dtAllotment.Rows)
            {
                string[] WLElements = dr["sequence"].ToString().Split('.');
                string wlKey = dr["instcd"].ToString() + "." + dr["brcd"].ToString() + "." + dr["sequence"].ToString();
                if (!SeatRankTypeMapping.ContainsKey(wlKey))
                {
                    throw new Exception(string.Format("Invalid key {0}. Ranlk type not found for this key", wlKey));
                }
                dtAllotmentTypeTable.Rows.Add(new object[] { boardId, roundNo, dr["RollNo"].ToString(), SeatRankTypeMapping[wlKey].Substring(3), dr["rank"].ToString(), dr["instcd"].ToString(), dr["brcd"].ToString(), WLElements[0], WLElements[1], WLElements[2], WLElements[4] + WLElements[5], WLElements[3], WLElements[6] });

            }

            DataTable dtAllotmentSummary = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select * from XT_AllotmentSummary").Tables[0];
            foreach (DataRow dr in dtAllotmentSummary.Rows)
            {
                string[] WLElements = dr["sequence"].ToString().Split('.');
                string wlKey = dr["instcd"].ToString() + "." + dr["brcd"].ToString() + "." + dr["sequence"].ToString();
                if (!SeatRankTypeMapping.ContainsKey(wlKey))
                {
                    throw new Exception(string.Format("Invalid key {0}. Ranlk type not found for this key", wlKey));
                }
                try
                {
                    dtAllotmentSummaryTypeTable.Rows.Add(new object[] { boardId, roundNo, dr["instcd"].ToString(), dr["brcd"].ToString(), WLElements[0], WLElements[1], WLElements[2], WLElements[3], WLElements[4], WLElements[5], WLElements[6],SeatRankTypeMapping[wlKey].Substring(3)
                    ,dr["InItSeats"].ToString(),dr["NewSeats"].ToString(),dr["Allotted"].ToString(),dr["OpeningRank"].ToString(),dr["ClosingRank"].ToString(),dr["OpeningRank_NewCand"].ToString(),dr["ClosingRank_NewCand"].ToString(),dr["DereserveFrom"].ToString(),dr["DereserveTo"].ToString() });

                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
        }

        protected string GetAllotmentSequence(string rollno, string category, string subcategoryList, string gender, string domicile, string additionalInfo, string symbolList, string rankData)
        {
            bool isEligible = false;
            subcategoryList = subcategoryList.ToUpper();
            category = category.ToUpper();

            foreach (string s in symbolList.Split(','))
            {
                if (!string.IsNullOrEmpty(s) && s.Split('=')[1] != "N")
                {
                    isEligible = true;
                    break;
                }
            }
            if (!isEligible)
                return string.Empty;

            StringBuilder catSubCatOptions = new StringBuilder();
            catSubCatOptions.Clear();
            string symbol = symbolList[3].ToString();
            if (symbol == "*")
            {
                catSubCatOptions.Append("OP.NO,"); 

                if (subcategoryList.ToUpper() == "PH:YES")
                    catSubCatOptions.Append("OP.PH,");

                if (category != "GN")
                {
                    catSubCatOptions.Append(category + ".NO,");
                    if (subcategoryList.ToUpper() == "PH:YES")
                        catSubCatOptions.Append(category + ".PH,");
                }
            }

            else if ((symbol == "=") && (category == "BC" || category == "EW"))
            {
                if (subcategoryList.ToUpper() == "PH:YES")
                    catSubCatOptions.Append("OP.PH,");

                catSubCatOptions.Append(category + ".NO,");

                if (subcategoryList.ToUpper() == "PH:YES")
                    catSubCatOptions.Append(category + ".PH,");
            }

            else if ((symbol == "+") && (category == "SC" || category == "ST"))
            {
                if (subcategoryList.ToUpper() == "PH:YES")
                    catSubCatOptions.Append("OP.PH,");

                catSubCatOptions.Append(category + ".NO,");

                if (subcategoryList.ToUpper() == "PH:YES")
                    catSubCatOptions.Append(category + ".PH,");
            }
            else if (symbol == "$" && category == "BC" && subcategoryList.ToUpper() == "PH:YES")
            {
                catSubCatOptions.Append("OP.PH,BC.PH,");
            }
            else if (symbol == "%" && (category == "EW" || category == "GN") && subcategoryList.ToUpper() == "PH:YES")
            {
                catSubCatOptions.Append("OP.PH,");
                if (category == "EW")
                    catSubCatOptions.Append(category + ".PH,");
            }


            if (catSubCatOptions.Length > 0)
            {
                return catSubCatOptions.ToString().Substring(0, catSubCatOptions.Length - 1);
            }
            else
                return "";
        }

    }
}
