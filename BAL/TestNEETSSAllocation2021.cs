using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace BAL
{
    public class TestNEETSSAllocation2021 : ComCouns21Allocation
    {
        public TestNEETSSAllocation2021() : base()
        {
            // board Id 
            boardId = "140042121"; /*BoardId referred from Production server*/

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate";

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
            DBProc_GetCandidateChoice = "XApp_CC_GetCandidateChoice";            
            DBProc_GetGetSymbolCatSubcatOptions = "XApp_CC_GetSymbolCatSubcatOptions";
            AllotmentSequnceMode = "Code";//"DB"/"Code"
            DBProc_GetCandidateAllChoices = "XApp_CC_GetCandidateChoiceAll";
        }


        string options = string.Empty;
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
            string choiceQuotaId = string.Empty;
            string choiceStreamId = string.Empty;
            string choiceRankType = string.Empty;
            string waitListKey = string.Empty;
            string choiceGroupId = string.Empty;
            string choiceSeatTypeId = string.Empty;
            string choiceGenderId = string.Empty;
            foreach (DataRow drCand in dtCand.Rows)
            {
                procOptno = 0;
                choices.Clear();
                candRollNo = drCand[DBParam_RollNo].ToString();
                candCategory = drCand[DBParam_CandCategory].ToString();
                if (candRollNo == "2055200066"
                    )
                {
                    ;
                }
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
                    choiceQuotaId = rdrChoice["quotaId"].ToString().Trim();

                    string catSubCatOptions = string.Empty;
                    if (AllotmentSequnceMode.Equals("DB"))
                    {
                        catSubCatOptions = GetAllotmentSequenceFromDB(candRollNo, candCategory, candSubcategory, candGender, candDomicile, candAdditionalInfo, candSymbolData, candRankData);
                    }
                    else
                    {
                        catSubCatOptions = this.GetAllotmentOptionCode(candCategory, candSubcategory, candSymbolData, choiceQuotaId);
                    }

                    if (string.IsNullOrEmpty(catSubCatOptions))
                    {
                        continue;
                    }

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
                                isValidChoice = (allottedOptno == choiceOptNo && objAllAllotmentDetails[candRollNo].Instcd.Trim() == choiceInstCd && objAllAllotmentDetails[candRollNo].Brcd.Trim() == choiceBrCd && objAllAllotmentDetails[candRollNo].Sequence.Substring(9, 2) == choiceQuotaId);
                                break;
                            default:
                                throw new Exception("Invalid willingness");
                        }
                    }
                    if (isValidChoice)
                    {
                        choiceStreamId = StreamInfoMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGroupId = GroupMaster[choiceInstCd + "." + choiceBrCd];
                        choiceSeatTypeId = SeatTypeMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGenderId = GenderMaster[choiceInstCd + "." + choiceBrCd];

                        if (choiceGenderId == "F" && candGender != "02") continue;
                        if (objAllAllotmentDetails.ContainsKey(candRollNo) && objAllAllotmentDetails[candRollNo].Instcd.Trim() == choiceInstCd && objAllAllotmentDetails[candRollNo].Brcd.Trim() == choiceBrCd && objAllAllotmentDetails[candRollNo].Sequence.Substring(9, 2) == choiceQuotaId)
                        {

                            waitListKey = choiceInstCd + "." + choiceBrCd + "." + objAllAllotmentDetails[candRollNo].Sequence; //choiceInstCd + "." + choiceBrCd + ".94.G1.NA." +choiceQuotaId+"."+objAllAllotmentDetails[candRollNo].Sequence.Substring(12,5)+".B";
                            choiceRankType = SeatRankTypeMapping[waitListKey];
                            rank = candRanks[choiceRankType];
                            choices = choices.Append(waitListKey + ":" + rank.ToString() + ",");
                            procOptno = procOptno + 1;
                            continue;
                        }
                        else
                        {
                            foreach (string option in catSubCatOptions.Split(','))
                            {

                                // waitListKey = choiceInstCd + "." + choiceBrCd + ".94.G1.NA." +choiceQuotaId+"."+option+".B";
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


        public string GetAllotmentOptionCode(string cat, string subcat, string symbol, string quotaid)
        {
            this.options = string.Empty;
            if (symbol.Equals("94=*"))
            {
                this.options = string.Concat(this.options, "OP.NO,");
                if (subcat.ToUpper().Equals("PH:YES"))
                {
                    this.options = string.Concat(this.options, "OP.PH,");
                }
                if (!cat.ToUpper().Equals("GN"))
                {
                    this.options = string.Concat(this.options, cat, ".NO,");
                    if (subcat.ToUpper().Equals("PH:YES"))
                    {
                        this.options = string.Concat(this.options, cat, ".PH,");
                    }
                }
            }
            else if (symbol != "94=*")
            {
                if (quotaid == "PS" || quotaid == "NR" || quotaid == "MM" || quotaid == "JM")
                {
                    this.options = string.Concat(this.options, "OP.NO,");
                }

                if (symbol.Equals("94=="))
                {

                    this.options = string.Concat(this.options, cat, ".NO,");
                    if (subcat.ToUpper().Equals("PH:YES"))
                    {
                        this.options = string.Concat("OP.PH,", this.options);
                        this.options = string.Concat(this.options, cat, ".PH,");
                    }

                }
                else if (symbol.Equals("94=$"))
                {
                    if (cat != "GN")
                    {
                        this.options = string.Concat(this.options, cat, ".NO,");
                        if (subcat.ToUpper().Equals("PH:YES"))
                        {
                            this.options = string.Concat(this.options, cat, ".PH,");
                        }
                    }
                }
            }


            if (this.options.Length <= 0)
            {
                return string.Empty;
            }
            return this.options.Substring(0, this.options.Length - 1);
        }

        protected override string GetAllotmentSequence(string rollno, string category, string subcategoryList, string gender, string domicile, string additionalInfo, string symbolList, string rankData)
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
            if ("01,02".Contains(gender))
            {
                if (subcategoryList.ToUpper().Contains("TF:YES".ToUpper()))
                {
                    catSubCatOptions.Append("OP.TF,");
                }

                if (gender == "02")
                {
                    catSubCatOptions.Append("OP.GL,");

                    if ("SC,ST,BC,EW".Contains(category))
                    {
                        catSubCatOptions.Append(category + ".GL,");
                    }
                }

                catSubCatOptions.Append("OP.NO,");



                foreach (string sub in subcategoryList.Split(','))
                {
                    if ("PH:YES,AF:YES,FF:YES".ToUpper().Contains(sub.ToUpper()))
                    {
                        catSubCatOptions.Append("OP." + sub.Substring(0, 2) + ",");
                    }
                }


                if ("SC,ST,BC,EW".Contains(category))
                {
                    catSubCatOptions.Append(category + ".NO,");

                    foreach (string sub in subcategoryList.Split(','))
                    {
                        if ("PH:YES,AF:YES,FF:YES".ToUpper().Contains(sub.ToUpper()))
                        {
                            catSubCatOptions.Append(category + "." + sub.Substring(0, 2) + ",");
                        }
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

    }
}
