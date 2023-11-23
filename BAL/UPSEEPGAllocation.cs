using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BAL
{
    public class UPSEEUGAllocation : ComCouns21Allocation
    {
        public UPSEEUGAllocation() : base()
        {
            boardId = "130052121";
        }
        public override ActionOutput PrepareEligibleCandidate()
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "XApp_CC_PrepareEligibleCandidate_UPSEEUG", sqlParameters);
            int eligibleCandidateCount = Convert.ToInt32(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select isnull(count(*),0) from XT_EligibleCandidate").Tables[0].Rows[0][0]);
            return new ActionOutput(ActionStatus.Success, "Completed:" + eligibleCandidateCount.ToString());
        }

        public override ActionOutput PrepareSeat()
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "XApp_CC_PrepareSeat_UPSEEUG", sqlParameters);
            int totalSeat = Convert.ToInt32(SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select isnull(sum(tseat),0) from XT_Seat").Tables[0].Rows[0][0]);
            return new ActionOutput(ActionStatus.Success, "Completed:" + totalSeat.ToString());
        }

        public override ActionOutput PreparePreviousAllotment()
        {
            SqlParameter[] sqlParameters = new SqlParameter[2];
            sqlParameters[0] = new SqlParameter("@" + DBParam_BoardId, boardId);
            sqlParameters[1] = new SqlParameter("@" + DBParam_RoundNo, roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "XApp_CC_PreparePreviousAllotment_UPSEEUG", sqlParameters);
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
            string[] femaleInstitutes = { "800117", "800849" };
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
                        choiceStreamId = StreamInfoMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGroupId = GroupMaster[choiceInstCd + "." + choiceBrCd];
                        choiceSeatTypeId = SeatTypeMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGenderId = GenderMaster[choiceInstCd + "." + choiceBrCd];

                        if (femaleInstitutes.Contains(choiceInstCd) && candGender.Equals("01"))
                        {
                            continue;
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

                            waitListKey = choiceInstCd + "." + choiceBrCd + "." + choiceStreamId + "." + choiceGroupId + "." + choiceSeatTypeId + "." + option + "." + choiceGenderId;

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
            string stateId = additionalInfo.Split('=')[1];

            if (domicile.Equals("98"))
            {
                if (stateId.Equals("33"))
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
