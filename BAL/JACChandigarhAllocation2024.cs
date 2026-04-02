using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace BAL
{
    public class JACChandigarhAllocation2024 : ComCouns21Allocation
    {
        public JACChandigarhAllocation2024() : base()
        {
            // board Id 
            boardId = "113012421";

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat_JAC";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate_JAC";

            //Procedure to prepare Previous allotment or seat gaurnatee candidates
            DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment_JAC";

            //Procedure to dereserve seats
            DBProc_DereserveSeat = "XApp_CC_DereserveSeat_JAC";

            //Procedure to update the allotment,allotment summary and other output table
            //after completion of allocation
            DBProc_UpdateApplication = "XApp_CC_UpdateApplication_JAC";

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
            LoadStreamInfo();
            LoadSeatDetails();
            SeatRankTypeMapping.Add("104528.00000NN.03.G1.JAC.AI.NO.K2.B", "03.01");
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
                                //isValidChoice = (allottedOptno == choiceOptNo && objAllAllotmentDetails[candRollNo].Instcd.Trim() == choiceInstCd && objAllAllotmentDetails[candRollNo].Brcd.Trim() == choiceBrCd); ; ;
                                isValidChoice = (allottedOptno == choiceOptNo);
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
                        string GroupId = "G1";
                        string seatTypeId = "JAC";

                        foreach (string option in catSubCatOptions.Split(','))
                        {
                            waitListKey = choiceInstCd + "." + choiceBrCd + "." + choiceStreamId + "." + GroupId + "." + seatTypeId + "." + option + ".B";
                            if (choiceInstCd.Equals("104528") && option.Equals("AI.NO.K2"))
                            {
                                procOptno = procOptno + 1;
                                choices = choices.Append("104528.00000NN." + choiceStreamId + ".G1.JAC." + option + ".B:" + candRanks["03.01"].ToString() + ",");
                            }
                            else if (SeatRankTypeMapping.ContainsKey(waitListKey) && candRanks.ContainsKey(SeatRankTypeMapping[waitListKey]))
                            {
                                choiceRankType = SeatRankTypeMapping[waitListKey];
                                rank = candRanks[choiceRankType];
                                procOptno = procOptno + 1;
                                choices = choices.Append(waitListKey + ":" + rank.ToString() + ",");
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


        string[] SubCatList = { "PH", "FF", "RV", "K1", "K2", "SG", "RA", "BA", "CA", "AP", "TP", "KA", "D1", "D2", "S1", "S2" };
        //protected override string GetAllotmentSequence(string rollno, string category, string subcategoryList, string gender, string domicile, string additionalInfo, string symbolList, string rankData)
        //{
        //    List<KeyValuePair<string, int>> optionList = new List<KeyValuePair<string, int>>();
        //    StringBuilder catSubCatOptions = new StringBuilder();
        //    catSubCatOptions.Clear();
        //    subcategoryList = subcategoryList.ToUpper();
        //    if (subcategoryList.Contains("E1:YES")) //EW
        //    {
        //        catSubCatOptions.Append("AI.NO.E1,");
        //        if (domicile == "06")
        //            catSubCatOptions.Append("HS.NO.E1,");
        //        else
        //            catSubCatOptions.Append("OS.NO.E1,");

        //    }

        //    if (subcategoryList.Contains("E2:YES")) //EW
        //    {
        //        catSubCatOptions.Append("AI.NO.E2,");
        //        if (domicile == "06")
        //            catSubCatOptions.Append("HS.NO.E2,");
        //        else
        //            catSubCatOptions.Append("OS.NO.E2,");
        //    }

        //    catSubCatOptions.Append("AI.OP.NO,");
        //    if (domicile == "06")
        //        catSubCatOptions.Append("HS.OP.NO,");
        //    else
        //        catSubCatOptions.Append("OS.OP.NO,");

        //    if (category != "GN")
        //    {
        //        catSubCatOptions.Append("AI." + category + ".NO,");
        //        if (domicile == "06")
        //            catSubCatOptions.Append("HS." + category + ".NO,");
        //        else
        //            catSubCatOptions.Append("OS." + category + ".NO,");
        //    }

        //    for (int i = 0; i < 16; i++)
        //    {
        //        if (subcategoryList.ToUpper().Contains(SubCatList[i].ToUpper() + ":YES"))
        //        {
        //            catSubCatOptions.Append("AI.NO." + SubCatList[i] + ",");
        //            if (domicile == "06")
        //                catSubCatOptions.Append("HS.NO." + SubCatList[i] + ",");
        //            else
        //                catSubCatOptions.Append("OS.NO." + SubCatList[i] + ",");
        //        }
        //    }


        //    if (catSubCatOptions.Length > 0)
        //    {
        //        return catSubCatOptions.ToString().Substring(0, catSubCatOptions.Length - 1);
        //    }
        //    else
        //        return "";

        //}

        protected override string GetAllotmentSequence(string rollno, string category, string subcategoryList, string gender, string domicile, string additionalInfo, string symbolList, string rankData)
        {
            List<KeyValuePair<string, int>> optionList = new List<KeyValuePair<string, int>>();
            StringBuilder catSubCatOptions = new StringBuilder();
            catSubCatOptions.Clear();
            subcategoryList = subcategoryList.ToUpper();
            if (subcategoryList.Contains("E1:YES")) //EW
            {
                catSubCatOptions.Append("AI.NO.E1,");
                if (domicile == "28")
                    catSubCatOptions.Append("HS.NO.E1,");
                else
                    catSubCatOptions.Append("OS.NO.E1,");

            }

            if (subcategoryList.Contains("E2:YES")) //EW
            {
                catSubCatOptions.Append("AI.NO.E2,");
                if (domicile == "28")
                    catSubCatOptions.Append("HS.NO.E2,");
                else
                    catSubCatOptions.Append("OS.NO.E2,");
            }

            catSubCatOptions.Append("AI.OP.NO,");
            if (domicile == "28")
                catSubCatOptions.Append("HS.OP.NO,");
            else
                catSubCatOptions.Append("OS.OP.NO,");

            if (category != "GN")
            {
                catSubCatOptions.Append("AI." + category + ".NO,");
                if (domicile == "28")
                    catSubCatOptions.Append("HS." + category + ".NO,");
                else
                    catSubCatOptions.Append("OS." + category + ".NO,");
            }

            for (int i = 0; i < 16; i++)
            {
                if (subcategoryList.ToUpper().Contains(SubCatList[i].ToUpper() + ":YES"))
                {
                    catSubCatOptions.Append("AI.NO." + SubCatList[i] + ",");
                    if (domicile == "28")
                        catSubCatOptions.Append("HS.NO." + SubCatList[i] + ",");
                    else
                        catSubCatOptions.Append("OS.NO." + SubCatList[i] + ",");
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
