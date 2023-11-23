using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BAL
{
    public class JENPASUGAllocation2022 : ComCouns21Allocation
    {
        public JENPASUGAllocation2022() : base()
        {
            // board Id 
            boardId = "134132221";

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate_JENPASUG";

            //Procedure to prepare Previous allotment or seat gaurnatee candidates
            DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment_JENPASUG";

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
            string choiceStreamId = string.Empty;
            string choiceGroupId = string.Empty;
            string choiceSeatTypeId = string.Empty;
            string choiceGenderId = string.Empty;
            string choiceRankType = string.Empty;
            string choiceQuotaId = string.Empty;
            string waitListKey = string.Empty;
            string[] CatPrograms_Nursing = { "12057NN" };//Nursing
            string[] CatPrograms_BHA = { "12276NN" };//Bachelor of Hospital Administration
            string[] CatPrograms_Others = { "12054NN", "12055NN", "12056NN", "12058NN", "12059NN", "12060NN", "12782NN" };//Others


            string[] PwdPrograms_Nursing = { "12057NN" }; //Nursing
            string[] PwdPrograms_Others = { "12054NN", "12060NN", "12056NN", "12058NN", "12059NN", "12276NN", "12782NN" };//Others
            string[] PwdPrograms_BPT = { "12055NN" };//Bachelor of Physiotherapy BPT /*BPT Code changed by shivam on 05AUg2022*/
            double candCatPerc_Nursing = -1, candCatPerc_BHA = -1, candCatPerc_Others = -1;
            string isPwD_Nursing = string.Empty, isPwD_BPT = string.Empty, isPwD_Others = string.Empty;
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

                CalculateCandPercentage(drCand["AdditionalInfo"].ToString(), ref candCatPerc_Nursing, ref candCatPerc_BHA, ref candCatPerc_Others);
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, string.Format("insert into App_CandCatPerc values('{0}',{1},{2},{3})", candRollNo, candCatPerc_Nursing, candCatPerc_BHA, candCatPerc_Others));
                isPwD_Nursing = (candSubcategory.ToUpper().Contains("P1:YES")) ? "Y" : "N";
                isPwD_BPT = (candSubcategory.ToUpper().Contains("P2:YES")) ? "Y" : "N";
                isPwD_Others = (candSubcategory.ToUpper().Contains("P3:YES")) ? "Y" : "N";
                candSymbolData = drCand["symbol"].ToString();

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
                        choiceGenderId = GenderMaster[choiceInstCd + "." + choiceBrCd];
                        choiceQuotaId = QuotaMaster[choiceInstCd + "." + choiceBrCd];

                        //if (choiceBrCd == "12056NN" && candRollNo == "1041101042")
                        //{
                        //;
                        //}

                        if (choiceGenderId.Equals("F") && candGender.Equals("01"))
                        {
                            continue;
                        }

                        foreach (string option in catSubCatOptions.Split(','))
                        {
                            if (option.Equals("OP.NO") || option.Equals("EW.NO"))
                            {
                                if (CatPrograms_Nursing.Contains(choiceBrCd) && candCatPerc_Nursing < 45)
                                    continue;
                                else if (CatPrograms_BHA.Contains(choiceBrCd) && candCatPerc_BHA < 50)
                                    continue;
                                else if (CatPrograms_Others.Contains(choiceBrCd) && candCatPerc_Others < 45)
                                    continue;
                            }
                            else
                            {
                                if (CatPrograms_Nursing.Contains(choiceBrCd) && candCatPerc_Nursing < 40)
                                    continue;
                                else if (CatPrograms_BHA.Contains(choiceBrCd) && candCatPerc_BHA < 45)
                                    continue;
                                else if (CatPrograms_Others.Contains(choiceBrCd) && candCatPerc_Others < 40)
                                    continue;
                            }

                            if (option.Substring(3, 2).Equals("PH"))
                            {
                                if (PwdPrograms_Nursing.Contains(choiceBrCd) && isPwD_Nursing.Equals("N"))
                                    continue;
                                else if (PwdPrograms_Others.Contains(choiceBrCd) && isPwD_Others.Equals("N"))
                                    continue;
                                else if (PwdPrograms_BPT.Contains(choiceBrCd) && isPwD_BPT.Equals("N"))
                                    continue;
                            }


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
            return new ActionOutput(ActionStatus.Success, "Completed:Choice created for " + dtAllChoices.Rows.Count + " candidates");
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

            if (domicile.Equals("35"))
            {
                catSubCatOptions.Append("OP.NO,");
                if (subcategoryList.Contains("PH:YES"))
                {
                    catSubCatOptions.Append("OP.PH,");
                }

                if (category != "GN")
                {
                    catSubCatOptions.Append(category + ".NO,");

                    if (subcategoryList.Contains("PH:YES"))
                    {
                        catSubCatOptions.Append(category + ".PH,");
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


        private void CalculateCandPercentage(string strSubjectMarks, ref double candCatPerc_Nursing, ref double candCatPerc_BHA, ref double candCatPerc_Others)
        {
            candCatPerc_Nursing = 0;
            candCatPerc_BHA = 0;
            candCatPerc_Others = 0;

            Dictionary<string, double[]> dctSubjectMarks = new Dictionary<string, double[]>();
            string[] _subjectmarks;
            double totalObtainedMarks = 0;
            double totalMaxMarks = 0;
            foreach (string str in strSubjectMarks.Split(','))
            {
                _subjectmarks = str.Split(':');
                if (!string.IsNullOrWhiteSpace(_subjectmarks[1]))
                {
                    dctSubjectMarks.Add(_subjectmarks[0], new double[] { Convert.ToDouble(_subjectmarks[1]), Convert.ToDouble(_subjectmarks[2]) });
                }
            }

            if (dctSubjectMarks.ContainsKey("006") && dctSubjectMarks.ContainsKey("007") && dctSubjectMarks.ContainsKey("010") && dctSubjectMarks.ContainsKey("002"))
            {
                totalObtainedMarks = dctSubjectMarks["006"][0] + dctSubjectMarks["007"][0] + dctSubjectMarks["010"][0];
                totalMaxMarks = dctSubjectMarks["006"][1] + dctSubjectMarks["007"][1] + dctSubjectMarks["010"][1];
                candCatPerc_Nursing = totalObtainedMarks * 100 / totalMaxMarks;
            }

            if (dctSubjectMarks.ContainsKey("006") && dctSubjectMarks.ContainsKey("007") && dctSubjectMarks.ContainsKey("010") && dctSubjectMarks.ContainsKey("002"))
            {
                totalObtainedMarks = dctSubjectMarks["006"][0] + dctSubjectMarks["007"][0] + dctSubjectMarks["010"][0] + dctSubjectMarks["002"][0];
                totalMaxMarks = dctSubjectMarks["006"][1] + dctSubjectMarks["007"][1] + dctSubjectMarks["010"][1] + dctSubjectMarks["002"][1];
                candCatPerc_Others = totalObtainedMarks * 100 / totalMaxMarks;
            }

            if (dctSubjectMarks.ContainsKey("04"))
            {
                totalObtainedMarks = dctSubjectMarks["04"][0];
                totalMaxMarks = dctSubjectMarks["04"][1];
                candCatPerc_BHA = totalObtainedMarks * 100 / totalMaxMarks;
            }
        }
    }
}
