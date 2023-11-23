using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BAL
{
    public class UPSEECUETUGAllocation2022 : ComCouns21Allocation
    {
        public UPSEECUETUGAllocation2022() : base()
        {
            // board Id 
            boardId = "130052221";

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat_UPSEECTUG";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate_UPSEECTUG";

            //Procedure to prepare Previous allotment or seat gaurnatee candidates
            DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment_UPSEECTUG";

            //Procedure to dereserve seats
            DBProc_DereserveSeat = "XApp_CC_DereserveSeat_UPSEECTUG";

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
            bool isRural = false;
            string choiceInstCd = string.Empty;
            string choiceBrCd = string.Empty;
            string choiceStreamId = string.Empty;
            string choiceGroupId = string.Empty;
            string choiceSeatTypeId = string.Empty;
            string choiceGenderId = string.Empty;
            string choiceRankType = string.Empty;
            string choiceQuotaId = string.Empty;
            string waitListKey = string.Empty;

            //string[] frozenSeats = { "801506.120631N.08.G1.UPSEE.HS.OP.GL.B"
            //,"801506.120631N.08.G1.UPSEE.HS.SC.GL.B"
            //,"801506.120631N.08.G1.UPSEE.HS.BC.NO.B"
            //,"801506.120631N.08.G1.UPSEE.HS.OP.NO.B"
            //,"801506.120631N.08.G1.UPSEE.HS.SC.NO.B"};
            string[] frozenSeats = { };
            //string[] frozenStreams = { "08","09","10"};
            string[] frozenStreams = { };
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
                isRural = candAdditionalInfo.Split(',')[1].Split('=')[1].Equals("Yes", StringComparison.InvariantCultureIgnoreCase) ? true : false;
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
                        choiceStreamId = StreamInfoMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGroupId = GroupMaster[choiceInstCd + "." + choiceBrCd];
                        choiceSeatTypeId = SeatTypeMaster[choiceInstCd + "." + choiceBrCd];
                        var genderOptions = GenderMaster[choiceInstCd + "." + choiceBrCd].Split(',').ToList<string>();
                        if (!candGender.Equals("02") && genderOptions.Contains("F"))
                        {
                            genderOptions.Remove("F");
                        }

                        if (choiceBrCd.EndsWith("T") && !candSubcategory.ToUpper().Contains("TF:YES"))
                        {
                            continue;
                        }



                        foreach (string option in catSubCatOptions.Split(','))
                        {
                            if (choiceBrCd.EndsWith("T") && !option.Equals("HS.OP.TF"))
                            {
                                continue;
                            }
                            else if (!choiceBrCd.EndsWith("T") && option.Equals("HS.OP.TF"))
                            {
                                continue;
                            }

                            foreach (string optionGenderId in genderOptions)
                            {
                                choiceGenderId = optionGenderId;
                                waitListKey = choiceInstCd + "." + choiceBrCd + "." + choiceStreamId + "." + choiceGroupId + "." + choiceSeatTypeId + "." + option + "." + choiceGenderId;

                                if (frozenSeats.Contains(waitListKey))
                                {
                                    if (isRetained == 0)
                                        continue;
                                    AllotmentDetail obj = objAllAllotmentDetails[candRollNo];
                                    if (obj.Instcd + "." + obj.Brcd + "." + obj.Sequence != waitListKey)
                                        continue;
                                }

                                if (frozenStreams.Contains(choiceStreamId))
                                {
                                    if (isRetained == 0)
                                        continue;
                                    AllotmentDetail obj = objAllAllotmentDetails[candRollNo];
                                    if (obj.Instcd + "." + obj.Brcd + "." + obj.Sequence != waitListKey)
                                        continue;
                                }

                                if (SeatRankTypeMapping.ContainsKey(waitListKey) && candRanks.ContainsKey(SeatRankTypeMapping[waitListKey]))
                                {
                                    if ("04,08,15,16".Contains(choiceStreamId))
                                    {
                                        choiceRankType = isRural ? "02" : "01";
                                    }
                                    else
                                        choiceRankType = "01";
                                    rank = candRanks[choiceStreamId + "." + choiceRankType];
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

        protected override string GetAllotmentSequence(string rollno, string category, string subcategoryList, string gender, string domicile, string additionalInfo, string symbolList, string rankData)
        {
            List<KeyValuePair<string, int>> optionList = new List<KeyValuePair<string, int>>();
            StringBuilder catSubCatOptions = new StringBuilder();
            catSubCatOptions.Clear();
            string stateId = additionalInfo.Split(',')[0].Split('=')[1];

            if (domicile.Equals("98"))
            {
                if (!stateId.Equals("98"))
                {
                    catSubCatOptions.Append("AI.OP.NO,HS.OP.NO");
                }
                else
                {
                    catSubCatOptions.Append("AI.OP.NO");
                }
                return catSubCatOptions.ToString();
            }
            else
            {
                if (subcategoryList.ToUpper().Contains("TF:YES"))
                {
                    catSubCatOptions.Append("HS.OP.TF,");
                }

                catSubCatOptions.Append("AI.OP.NO,");
                if (gender.Equals("02"))
                {
                    catSubCatOptions.Append("HS.OP.GL,");
                    if ("SC,ST,BC".Contains(category))
                        catSubCatOptions.Append("HS." + category + ".GL,");
                }

                catSubCatOptions.Append("HS.OP.NO,");

                foreach (string subcat in subcategoryList.Split(','))
                {
                    if ("PH:YES,AF:YES,FF:YES".ToUpper().Contains(subcat.ToUpper()))
                    {
                        catSubCatOptions.Append("HS.OP." + subcat.Substring(0, 2) + ",");
                    }
                }

                if ("SC,ST,BC,EW".Contains(category))
                {
                    catSubCatOptions.Append("HS." + category + ".NO,");

                    foreach (string sub in subcategoryList.Split(','))
                    {
                        if ("PH:YES,AF:YES,FF:YES".ToUpper().Contains(sub.ToUpper()) && category != "EW")
                        {
                            catSubCatOptions.Append("HS." + category + "." + sub.Substring(0, 2) + ",");
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
