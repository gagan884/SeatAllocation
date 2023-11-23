using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;

namespace BAL
{
    public class JECAAllocation2022 : ComCouns21Allocation
    {
        public JECAAllocation2022() : base()
        {
            // board Id 
            boardId = "134162221";

            //Procedure to prepare seat
            DBProc_PrepareSeat = "XApp_CC_PrepareSeat";

            //Procedure to prepare Eligible Candidate
            DBProc_PrepareEligibleCandidate = "XApp_CC_PrepareEligibleCandidate_JECA";

            //Procedure to prepare Previous allotment or seat gaurnatee candidates
            DBProc_PreparePreviousAllotment = "XApp_CC_PreparePreviousAllotment_JECA";

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
            LoadSeatDetails();//For Rank Type
            LoadStreamInfo(); //Stream Id
            LoadSeatTypeInfo();//For Seat Type
            LoadGenderInfo(); //For Gender Id
            LoadGroupInfo(); //For Group Id
            //LoadQuotaInfo(); //For Quota 
            LoadRestrictions();
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
            string choiceGroupId = string.Empty;
            string choiceSeatTypeId = string.Empty;
            string choiceGenderId = string.Empty;
            string choiceRankType = string.Empty;
            string choiceQuotaId = string.Empty;
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
                        choiceStreamId = StreamInfoMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGroupId = GroupMaster[choiceInstCd + "." + choiceBrCd];
                        choiceSeatTypeId = SeatTypeMaster[choiceInstCd + "." + choiceBrCd];
                        choiceGenderId = GenderMaster[choiceInstCd + "." + choiceBrCd];
                        //choiceQuotaId = QuotaMaster[choiceInstCd + "." + choiceBrCd];

                        string eligibilityType = isEligible(candRollNo, GetRestrictionCategory(choiceInstCd));

                        foreach (string option in catSubCatOptions.Split(','))
                        {

                            if ((option == "AI.OP.NO" || option == "HS.OP.NO") && eligibilityType[0] != 'Y')
                            {
                                SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text,
                                   string.Format("insert into XApp_InvalidChoiceOptions values('{0}', {1}, '{2}', '{3}', '{4}','{5}')",
                                   candRollNo, choiceOptNo, choiceInstCd, choiceBrCd, option, eligibilityType.Substring(3)
                                   ));
                                continue;
                            }
                            else if (eligibilityType[2] != 'Y')
                            {
                                SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text,
                                   string.Format("insert into XApp_InvalidChoiceOptions values('{0}', {1}, '{2}', '{3}', '{4}','{5}')",
                                   candRollNo, choiceOptNo, choiceInstCd, choiceBrCd, option, eligibilityType.Substring(3)
                                   ));
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
            return new ActionOutput(ActionStatus.Success, "completed:" + dtAllChoices.Rows.Count);
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
            catSubCatOptions.Append("AI.OP.NO,");
            if (domicile.Equals("35"))
            {
                catSubCatOptions.Append("HS.OP.NO,");

                if (subcategoryList.Contains("PH:YES"))
                {
                    catSubCatOptions.Append("HS.OP.PH,");
                }

                if (category != "GN")
                {
                    catSubCatOptions.Append("HS." + category + ".NO,");

                    if (subcategoryList.Contains("PH:YES"))
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


        protected Dictionary<string, QualificationMarksDetails> UGQualificationDetails;
        protected Dictionary<string, QualificationMarksDetails> UGMathematicsDetails;
        protected Dictionary<string, QualificationMarksDetails> XIIQualificationDetails;
        protected Dictionary<string, QualificationMarksDetails> XIIMathematicsDetails;
        protected Dictionary<string, QualificationMarksDetails> XQualificationDetails;
        protected Dictionary<string, QualificationMarksDetails> XMathematicsDetails;
        protected void LoadQualificationSubjectMarksDetails()
        {
            UGQualificationDetails = new Dictionary<string, QualificationMarksDetails>();
            string UGqry = "select rollno,qualificationId,passStatus,obtainedMarks,maximumMarks,percentageMarks from App_QualificationMarksDetail where qualificationId in ('51','54','55','58','59','60','61','65')";
            DataTable UGdtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, UGqry).Tables[0];

            foreach (DataRow dr in UGdtOptions.Rows)
            {
                UGQualificationDetails.Add(dr["rollNo"].ToString(),
                       new QualificationMarksDetails()
                       {
                           courseId = dr["qualificationId"].ToString(),
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["obtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["maximumMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }

            UGMathematicsDetails = new Dictionary<string, QualificationMarksDetails>();
            string UGMathqry = "select rollno,qualificationId, passStatus,totalObtainedMarks,totalMaxMarks,percentageMarks from App_QualificationSubjectMarksDetail where subjectId='008' and qualificationId in ('51','54','55','58','59','60','61','65')";
            DataTable UGMathdtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, UGMathqry).Tables[0];

            foreach (DataRow dr in UGMathdtOptions.Rows)
            {
                UGMathematicsDetails.Add(dr["rollNo"].ToString(),
                       new QualificationMarksDetails()
                       {
                           courseId = dr["qualificationId"].ToString(),
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["totalObtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["totalMaxMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }

            //Class XII
            string XIIqry = "select rollno,qualificationId,passStatus,obtainedMarks,maximumMarks,percentageMarks from App_QualificationMarksDetail where qualificationId in ('04')";
            DataTable XIIdtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, XIIqry).Tables[0];

            XIIQualificationDetails = new Dictionary<string, QualificationMarksDetails>();
            foreach (DataRow dr in XIIdtOptions.Rows)
            {
                XIIQualificationDetails.Add(dr["rollNo"].ToString(),
                       new QualificationMarksDetails()
                       {
                           courseId = dr["qualificationId"].ToString(),
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["obtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["maximumMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }

            string XIIMathqry = "select rollno,qualificationId, passStatus,totalObtainedMarks,totalMaxMarks,percentageMarks from App_QualificationSubjectMarksDetail where subjectId='008' and qualificationId='04'";
            DataTable XIIMathdtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, XIIMathqry).Tables[0];

            XIIMathematicsDetails = new Dictionary<string, QualificationMarksDetails>();
            foreach (DataRow dr in XIIMathdtOptions.Rows)
            {
                XIIMathematicsDetails.Add(dr["rollNo"].ToString(),
                       new QualificationMarksDetails()
                       {
                           courseId = dr["qualificationId"].ToString(),
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["totalObtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["totalMaxMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }

            //Class Xth
            string Xqry = "select rollno,qualificationId,passStatus,obtainedMarks,maximumMarks,percentageMarks from App_QualificationMarksDetail where qualificationId in ('03')";
            DataTable XdtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, Xqry).Tables[0];

            XQualificationDetails = new Dictionary<string, QualificationMarksDetails>();
            foreach (DataRow dr in XdtOptions.Rows)
            {
                XQualificationDetails.Add(dr["rollNo"].ToString(),
                       new QualificationMarksDetails()
                       {
                           courseId = dr["qualificationId"].ToString(),
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["obtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["maximumMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }

            string XMathqry = "select rollno,qualificationId, passStatus,totalObtainedMarks,totalMaxMarks,percentageMarks from App_QualificationSubjectMarksDetail where subjectId='008' and qualificationId='03'";
            DataTable XMathdtOptions = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, XMathqry).Tables[0];
            XMathematicsDetails = new Dictionary<string, QualificationMarksDetails>();
            foreach (DataRow dr in XMathdtOptions.Rows)
            {
                XMathematicsDetails.Add(dr["rollNo"].ToString(),
                       new QualificationMarksDetails()
                       {
                           courseId = dr["qualificationId"].ToString(),
                           passStatus = dr["passStatus"].ToString(),
                           totalObtainedMarks = Convert.ToDouble(dr["totalObtainedMarks"]),
                           totalMaximumMarks = Convert.ToDouble(dr["totalMaxMarks"]),
                           percentageMarks = Convert.ToDouble(dr["percentageMarks"])
                       }
                       );
            }


            //select rollno,qualificationId, passStatus,totalObtainedMarks,totalMaxMarks,percentageMarks from App_QualificationSubjectMarksDetail where subjectId='008' and qualificationId in ('51','54','55','58','59','60','61','65')



        }


        //03	Class 10th or Equivalent
        //04	Class 12th or Equivalent
        //51	BE/BTech(Computer Sc./IT)
        //54	BE/BTech(Others)
        //55	BSc(Hon's)-Computer Sc.
        //58	BSc (Hon's)-Others
        //59	BSc (Major)-Computer Application
        //60	BSc/BA/B Com-General
        //61	BA/B Com (Hons)
        //65	BCA

        public Dictionary<string, Restriction> restrictions;
        public void LoadRestrictions()
        {
            restrictions = new Dictionary<string, Restriction>()
            {
                //Jadavpur
                {"104637",new Restriction{ agg_UG_isApplicable="Y",pas_UG_isApplicable="Y",agg_UG_open=60,agg_UG_res=45,
                                     agg_XII_isApplicable="Y",pas_XII_isApplicable="Y",agg_XII_open=60,agg_XII_res=45,
                                     agg_X_isApplicable="Y",pas_X_isApplicable="Y",agg_X_open=60,agg_X_res=45,
                                     mat_agg_UG_isApplicable="N",mat_pas_UG_isApplicable="Y",mat_agg_UG_open=0,mat_agg_UG_res=0,
                                     mat_agg_XII_isApplicable="Y",mat_pas_XII_isApplicable="Y",mat_agg_XII_open=60,mat_agg_XII_res=45,
                                     mat_agg_X_isApplicable="Y",mat_pas_X_isApplicable="Y",mat_agg_X_open=60,mat_agg_X_res=45} },
                //Calcutta University
                {"105379",new Restriction{ agg_UG_isApplicable="Y",pas_UG_isApplicable="Y",agg_UG_open=60,agg_UG_res=55,
                                     agg_XII_isApplicable="Y",pas_XII_isApplicable="Y",agg_XII_open=60,agg_XII_res=55,
                                     agg_X_isApplicable="Y",pas_X_isApplicable="Y",agg_X_open=60,agg_X_res=55,
                                     mat_agg_UG_isApplicable="Y",mat_pas_UG_isApplicable="Y",mat_agg_UG_open=60,mat_agg_UG_res=55,
                                     mat_agg_XII_isApplicable="Y",mat_pas_XII_isApplicable="Y",mat_agg_XII_open=60,mat_agg_XII_res=55,
                                     mat_agg_X_isApplicable="Y",mat_pas_X_isApplicable="Y",mat_agg_X_open=60,mat_agg_X_res=55} },
                //Kalyani University
                {"105380",new Restriction{ agg_UG_isApplicable="Y",pas_UG_isApplicable="Y",agg_UG_open=60,agg_UG_res=45,
                                     agg_XII_isApplicable="Y",pas_XII_isApplicable="Y",agg_XII_open=60,agg_XII_res=45,
                                     agg_X_isApplicable="Y",pas_X_isApplicable="Y",agg_X_open=60,agg_X_res=45,
                                     mat_agg_UG_isApplicable="N",mat_pas_UG_isApplicable="Y",mat_agg_UG_open=0,mat_agg_UG_res=0,
                                     mat_agg_XII_isApplicable="N",mat_pas_XII_isApplicable="Y",mat_agg_XII_open=0,mat_agg_XII_res=0,
                                     mat_agg_X_isApplicable="N",mat_pas_X_isApplicable="Y",mat_agg_X_open=0,mat_agg_X_res=0} },
                //North Bengal University  
                {"105377",new Restriction{  agg_UG_isApplicable="Y",pas_UG_isApplicable="Y",agg_UG_open=60,agg_UG_res=50,
                                     agg_XII_isApplicable="Y",pas_XII_isApplicable="Y",agg_XII_open=60,agg_XII_res=50,
                                     agg_X_isApplicable="N",pas_X_isApplicable="N",agg_X_open=0,agg_X_res=0,
                                     mat_agg_UG_isApplicable="N",mat_pas_UG_isApplicable="Y",mat_agg_UG_open=0,mat_agg_UG_res=0,
                                     mat_agg_XII_isApplicable="N",mat_pas_XII_isApplicable="Y",mat_agg_XII_open=0,mat_agg_XII_res=0,
                                     mat_agg_X_isApplicable="N",mat_pas_X_isApplicable="N",mat_agg_X_open=0,mat_agg_X_res=0} },
                //Vidyasagar University  
                {"105381",new Restriction{  agg_UG_isApplicable="Y",pas_UG_isApplicable="Y",agg_UG_open=50,agg_UG_res=45,
                                     agg_XII_isApplicable="Y",pas_XII_isApplicable="Y",agg_XII_open=50,agg_XII_res=45,
                                     agg_X_isApplicable="N",pas_X_isApplicable="N",agg_X_open=0,agg_X_res=0,
                                     mat_agg_UG_isApplicable="Y",mat_pas_UG_isApplicable="Y",mat_agg_UG_open=50,mat_agg_UG_res=45,
                                     mat_agg_XII_isApplicable="Y",mat_pas_XII_isApplicable="Y",mat_agg_XII_open=50,mat_agg_XII_res=45,
                                     mat_agg_X_isApplicable="N",mat_pas_X_isApplicable="N",mat_agg_X_open=0,mat_agg_X_res=0} },
                //Others
                {"999999",new Restriction{ agg_UG_isApplicable="Y",pas_UG_isApplicable="Y",agg_UG_open=50,agg_UG_res=45,
                                     agg_XII_isApplicable="Y",pas_XII_isApplicable="Y",agg_XII_open=50,agg_XII_res=45,
                                     agg_X_isApplicable="N",pas_X_isApplicable="N",agg_X_open=0,agg_X_res=0,
                                     mat_agg_UG_isApplicable="Y",mat_pas_UG_isApplicable="Y",mat_agg_UG_open=50,mat_agg_UG_res=45,
                                     mat_agg_XII_isApplicable="Y",mat_pas_XII_isApplicable="Y",mat_agg_XII_open=50,mat_agg_XII_res=45,
                                     mat_agg_X_isApplicable="N",mat_pas_X_isApplicable="N",mat_agg_X_open=0,mat_agg_X_res=0} }

            };
        }

        public string isEligible(string rollno, string RestrictionCategory)
        {
            string isEligibleForOpen = "Y";
            string isEligibleForReserved = "Y";
            string invalidreason = "";
            Restriction restriction;
            QualificationMarksDetails ugQualification, XIIQualification, XQualification, ugMathQualification, XIIMathQualification, XMathQualification;

            if (restrictions.ContainsKey(RestrictionCategory))
            {
                restriction = restrictions[RestrictionCategory];
            }
            else
                throw new Exception("Invalid restriction category");


            ugQualification = (UGQualificationDetails.ContainsKey(rollno)) ? UGQualificationDetails[rollno] : null;
            XIIQualification = (XIIQualificationDetails.ContainsKey(rollno)) ? XIIQualificationDetails[rollno] : null;
            XQualification = (XQualificationDetails.ContainsKey(rollno)) ? XQualificationDetails[rollno] : null;
            ugMathQualification = (UGMathematicsDetails.ContainsKey(rollno)) ? UGMathematicsDetails[rollno] : null;
            XIIMathQualification = (XIIMathematicsDetails.ContainsKey(rollno)) ? XIIMathematicsDetails[rollno] : null;
            XMathQualification = (XMathematicsDetails.ContainsKey(rollno)) ? XMathematicsDetails[rollno] : null;


            if (RestrictionCategory != "999999")
            {
                if (restriction.pas_UG_isApplicable == "Y" && (ugQualification == null || ugQualification.passStatus != "01"))
                    return "N,N,ug.f";

                if (restriction.agg_UG_isApplicable == "Y")
                {
                    if (ugQualification == null)
                        return "N,N,ug.f";

                    if (restriction.agg_UG_open > ugQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "ug.p";
                    }

                    if (restriction.agg_UG_res > ugQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "ug.p";
                    }
                }

                if (restriction.mat_pas_UG_isApplicable == "Y" && (ugMathQualification == null || ugMathQualification.passStatus != "01"))
                    return "N,N,ug.mat.f";

                if (restriction.mat_agg_UG_isApplicable == "Y")
                {
                    if (ugMathQualification == null || ugMathQualification.passStatus != "01")
                        return "N,N,ug.mat.f";

                    if (restriction.mat_agg_UG_open > ugMathQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "ug.mat.p";
                    }

                    if (restriction.mat_agg_UG_res > ugMathQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "ug.mat.p";
                    }
                }

                //XII
                if (restriction.pas_XII_isApplicable == "Y" && (XIIQualification == null || XIIQualification.passStatus != "01"))
                    return "N,N,XII.f";

                if (restriction.agg_XII_isApplicable == "Y")
                {
                    if (XIIQualification == null)
                        return "N,N,XII.f";

                    if (restriction.agg_XII_open > XIIQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "XII.p";
                    }

                    if (restriction.agg_XII_res > XIIQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "XII.p";
                    }

                }

                if (restriction.mat_pas_XII_isApplicable == "Y" && (XIIMathQualification == null || XIIMathQualification.passStatus != "01"))
                    return "N,N,XII.mat.f";

                if (restriction.mat_agg_XII_isApplicable == "Y")
                {
                    if (XIIMathQualification == null)
                        return "N,N,XII.mat.f";

                    if (restriction.mat_agg_XII_open > XIIMathQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "XII.mat.p";
                    }

                    if (restriction.mat_agg_XII_res > XIIMathQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "XII.mat.p";
                    }
                }

                //X
                if (restriction.pas_X_isApplicable == "Y" && (XQualification == null || XQualification.passStatus != "01"))
                    return "N,N,X.f";

                if (restriction.agg_X_isApplicable == "Y")
                {
                    if (XQualification == null)
                        return "N,N,X.f";

                    if (restriction.agg_X_open > XQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "X.p";
                    }

                    if (restriction.agg_X_res > XQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "X.p";
                    }
                }

                if (restriction.mat_pas_X_isApplicable == "Y" && (XMathQualification == null || XMathQualification.passStatus != "01"))
                    return "N,N,X.mat.f";

                if (restriction.mat_agg_X_isApplicable == "Y")
                {
                    if (XMathQualification == null)
                        return "N,N,X.mat.p";

                    if (restriction.mat_agg_X_open > XMathQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "X.mat.p";
                    }

                    if (restriction.mat_agg_X_res > XMathQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "X.mat.p";
                    }
                }
            }
            else
            {
                if (restriction.pas_UG_isApplicable == "Y" && (ugQualification == null || ugQualification.passStatus != "01"))
                    return "N,N,ug.f";

                if (restriction.agg_UG_isApplicable == "Y")
                {
                    if (ugQualification == null)
                        return "N,N,ug.f";

                    if (restriction.agg_UG_open > ugQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "ug.p";
                    }

                    if (restriction.agg_UG_res > ugQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "ug.p";
                    }
                }



                //XII
                if (restriction.pas_XII_isApplicable == "Y" && (XIIQualification == null || XIIQualification.passStatus != "01"))
                    return "N,N,XII.f";

                if (restriction.agg_XII_isApplicable == "Y")
                {
                    if (XIIQualification == null)
                        return "N,N,XII.f";

                    if (restriction.agg_XII_open > XIIQualification.percentageMarks)
                    {
                        isEligibleForOpen = "N";
                        invalidreason = "XII.p";
                    }

                    if (restriction.agg_XII_res > XIIQualification.percentageMarks)
                    {
                        isEligibleForReserved = "N";
                        invalidreason = "XII.p";
                    }
                }


                if ((ugMathQualification == null || ugMathQualification.passStatus != "01")
                    && (XIIMathQualification == null || XIIMathQualification.passStatus != "01"))
                    return "N,N,UG-XII.f";


                if ((ugMathQualification == null || restriction.mat_agg_UG_open > ugMathQualification.percentageMarks)
                    &&
                    (XIIMathQualification == null || restriction.mat_agg_XII_open > XIIMathQualification.percentageMarks)
                    )
                {
                    isEligibleForOpen = "N";
                    invalidreason = "UG-XII.p";
                }

                if ((ugMathQualification == null || restriction.mat_agg_UG_res > ugMathQualification.percentageMarks)
                   &&
                   (XIIMathQualification == null || restriction.mat_agg_XII_res > XIIMathQualification.percentageMarks)
                   )
                {
                    isEligibleForReserved = "N";
                    invalidreason = "UG-XII.p";
                }
            }

            return isEligibleForOpen + "," + isEligibleForReserved + "," + invalidreason;

        }


        public string GetRestrictionCategory(string instcd)
        {
            if ("'104637','105379','105380','105377','105381'".Contains(instcd))
                return instcd;
            else
                return "999999";
        }
    }

    public class QualificationMarksDetails
    {
        public string courseId;
        public string passStatus;
        public double totalObtainedMarks;
        public double totalMaximumMarks;
        public double percentageMarks;
    }
    public class Restriction
    {
        public string agg_UG_isApplicable;
        public string pas_UG_isApplicable;
        public double agg_UG_open;
        public double agg_UG_res;
        public string agg_XII_isApplicable;
        public string pas_XII_isApplicable;
        public double agg_XII_open;
        public double agg_XII_res;
        public string agg_X_isApplicable;
        public string pas_X_isApplicable;
        public double agg_X_open;
        public double agg_X_res;

        public string mat_agg_UG_isApplicable;
        public string mat_pas_UG_isApplicable;
        public double mat_agg_UG_open;
        public double mat_agg_UG_res;
        public string mat_agg_XII_isApplicable;
        public string mat_pas_XII_isApplicable;
        public double mat_agg_XII_open;
        public double mat_agg_XII_res;
        public string mat_agg_X_isApplicable;
        public string mat_pas_X_isApplicable;
        public double mat_agg_X_open;
        public double mat_agg_X_res;
    }
}
