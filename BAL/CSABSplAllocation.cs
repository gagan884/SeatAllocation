using System;
using System.Data;
using AppFramework;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BAL
{
    public class CSABSplAllocation : AbstractAllocation, IUserInterface
    {
        protected readonly string DBProc_CheckOutputTablesClean="XApp_AreOutputTablesClean_CSABSpecial";
        protected readonly string DBProc_CleanOutputTables = "XApp_CleanOutputTables_CSABSpecial";
        protected readonly string DBProc_PrepareSeat = "XApp_PrepareSeat_CSABSpecial";
        protected readonly string DBProc_PrepareEligibleCandidate = "XApp_PrepareEligibleCandidate_CSABSpecial";
        protected readonly string DBProc_PreparePreviousAllotment = "XApp_PreparePreviousAllotment_CSABSpecial";
        protected readonly string DBProc_GetCandidateChoice = "XApp_GetCandidateChoice_CSABSpecial";
        protected readonly string DBProc_GetEligibleCandidate = "XP_GetEligibleCandidate";
        protected readonly string DBProc_UpdateApplication = "XApp_UpdateApplication_CSABSpecial";
        protected readonly string DBProc_GetCandidateAllChoices = "XApp_GetCandidateChoiceAll_CSABSpecial";
        protected readonly string DBProc_DereserveSeat = "XApp_DereserveSeat_CSABSpecial";
        
        protected const string DBParam_BoardId = "boardId";
        protected const string DBParam_RoundNo = "roundNo";
        protected const string DBParam_RollNo = "rollNo";
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
        protected string boardId ;
        protected string connectionString ;

        public CSABSplAllocation() : base() {
            boardId = "134172121";
            connectionString = ObjectFactory.GetCommonObject().GetConnectionString();
        }

        

        #region InputPreparation    
        public override ActionOutput PrepareSeat()
        {
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@roundno", roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_PrepareSeat, sqlParameters);
            return new ActionOutput(ActionStatus.Success);
        }

        public override ActionOutput PrepareEligibleCandidate()
        {
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@roundno", roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_PrepareEligibleCandidate, sqlParameters);
            return new ActionOutput(ActionStatus.Success);
        }

        public override ActionOutput PreparePreviousAllotment()
        {
            SqlParameter[] sqlParameters = new SqlParameter[1];
            sqlParameters[0] = new SqlParameter("@roundno", roundNo);
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, DBProc_PreparePreviousAllotment, sqlParameters);
            return new ActionOutput(ActionStatus.Success);
        }

        #endregion InputPreparation
        string options = string.Empty;

        public override ActionOutput CreateVirtualChoice()
        {
            LoadPreviousAllotment();
            DataTable dtAllChoices = new DataTable();
            dtAllChoices.Columns.Add("rollNo");
            dtAllChoices.Columns.Add("totalChoices");
            dtAllChoices.Columns.Add("totalVChoices");
            dtAllChoices.Columns.Add("virtualChoices");
            int procOptno = 0;
            bool isValidChoice = false;
            int rankType = -1;
            string gender;
            string seatType = string.Empty;
            string choices = string.Empty;
            int isRetained = 0;
            int Optno = 0;
            string ArchitectureBrcd = "5101,5102";
            string PlanningBrcd = "4501";

            //Load Home Institute Directory
            DataTable dtHomeInstitute = SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select distinct instcd+hstcd code from HomeInstitute").Tables[0];
            string[] HomeInstitute = new string[dtHomeInstitute.Rows.Count];
            int i = 0;
            foreach (DataRow x in dtHomeInstitute.Rows)
            {
                HomeInstitute[i++] = x["code"].ToString();
            }


            SqlConnection connChoice = new SqlConnection(connectionString);
            SqlCommand cmdChoice = new SqlCommand(DBProc_GetCandidateChoice, connChoice);
            cmdChoice.CommandType = CommandType.StoredProcedure;
            cmdChoice.Parameters.Add("@RollNo", SqlDbType.VarChar);
            SqlDataReader rdrChoice = default(SqlDataReader);
            connChoice.Open();

            DataTable dtCand = SqlHelper.ExecuteDataset(connectionString, DBProc_GetEligibleCandidate).Tables[0];
            Dictionary<string, string> dtGetDomicile = new Dictionary<string, string>();
            foreach (DataRow dr in SqlHelper.ExecuteDataset(connectionString, CommandType.Text, "select Instcd+Brcd+Sequence [key], StateList quota from XT_Seat").Tables[0].Rows)
            {
                dtGetDomicile.Add(dr["key"].ToString(), dr["quota"].ToString());
            }

            int restrictedOPtNo;
            foreach (DataRow drCand in dtCand.Rows)
            {

                procOptno = 0;
                choices = string.Empty;
                gender = drCand["gender"].ToString();
                if (drCand["Willingness"].ToString() == "RF")
                {
                    restrictedOPtNo = (int)objSql.GetDataTableUsingCommand("select * from Allotment where roundno=" + (roundNo - 1) + " and rollno='" + drCand["Rollno"].ToString() + "'").Rows[0]["optno"];
                }
                else
                    restrictedOPtNo = 0;


                cmdChoice.Parameters["@RollNo"].SqlValue = drCand["Rollno"].ToString();

                rdrChoice = cmdChoice.ExecuteReader();
                while ((rdrChoice.Read()))
                {
                    isRetained = 0;
                    Optno = 0;
                    if (objAllAllotmentDetails.ContainsKey(drCand["Rollno"].ToString()) && objAllAllotmentDetails[drCand["Rollno"].ToString()].IsRetained != 0)
                    {
                        isRetained = objAllAllotmentDetails[drCand["Rollno"].ToString()].IsRetained;
                        Optno = objAllAllotmentDetails[drCand["Rollno"].ToString()].OptNo;
                    }

                    if (isRetained == 0)
                    {
                        if (drCand["Willingness"].ToString() != "RF")
                            isValidChoice = true;
                        else
                        {
                            if (Convert.ToInt32(rdrChoice["optno"]) < restrictedOPtNo)
                                isValidChoice = true;
                            else
                                isValidChoice = false;
                        }
                    }
                    else
                    {
                        switch (drCand["Willingness"].ToString())
                        {
                            case "FL":
                                isValidChoice = (Optno >= Convert.ToInt32(rdrChoice["optno"]));
                                break;
                            case "SL":
                                isValidChoice = (Optno >= Convert.ToInt32(rdrChoice["optno"]) && objAllAllotmentDetails[drCand["Rollno"].ToString()].Instcd.Trim() == rdrChoice["instcd"].ToString().Trim());
                                break;
                            case "FR":
                                isValidChoice = (Optno == Convert.ToInt32(rdrChoice["optno"]) && objAllAllotmentDetails[drCand["Rollno"].ToString()].Instcd.Trim() == rdrChoice["instcd"].ToString().Trim() && objAllAllotmentDetails[drCand["Rollno"].ToString()].Brcd.Trim() == rdrChoice["brcd"].ToString().Trim());
                                break;
                            default:
                                throw new Exception("Invalid willingness -" + drCand["Rollno"].ToString());
                        }

                    }
                    if (isValidChoice)
                    {
                        if (PlanningBrcd.Equals(rdrChoice["Brcd"].ToString()))
                        {
                            rankType = 2;
                        }
                        else if (ArchitectureBrcd.Contains(rdrChoice["Brcd"].ToString()))
                        {
                            rankType = 1;
                        }
                        else
                        {
                            rankType = 0;
                        }


                        if (("401,402,426,435").Contains(rdrChoice["instcd"].ToString()))
                            seatType = "AH";
                        else if (rdrChoice["instcd"].ToString()[0] == '2' || "422,427".Contains(rdrChoice["instcd"].ToString()))
                            seatType = "OH";
                        else
                            seatType = "AI";


                        options = GetAllotmentOptionCode(drCand["Category"].ToString(), drCand["Subcategory"].ToString(), drCand["Symbol"].ToString()[2 * rankType].ToString(), drCand["AdditionalInfo"].ToString().Split('=')[1]);
                        if (string.IsNullOrEmpty(options))
                            continue;

                        foreach (string option in options.Split(';'))
                        {
                            switch (seatType)
                            {
                                case "AH":
                                    if (gender.Equals("2"))
                                    {
                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "AI" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                    }
                                    procOptno = procOptno + 1;
                                    choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "AI" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");

                                    if (HomeInstitute.Contains(rdrChoice["Instcd"].ToString() + drCand["Domicile"].ToString()))
                                    {
                                        if (gender.Equals("2"))
                                        {
                                            procOptno = procOptno + 1;
                                            choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "HS" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                        }

                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "HS" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                    }
                                    break;
                                case "OH":
                                    if (gender.Equals("2"))
                                    {
                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "AI" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                    }
                                    procOptno = procOptno + 1;
                                    choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "AI" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");

                                    if (rdrChoice["Instcd"].ToString() == "225" && drCand["Domicile"].ToString() == "JK")
                                    {
                                        if (gender.Equals("2"))
                                        {
                                            procOptno = procOptno + 1;
                                            choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "JK" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                        }
                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "JK" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");

                                    }
                                    else if (rdrChoice["Instcd"].ToString() == "225" && drCand["Domicile"].ToString() == "LA")
                                    {
                                        if (gender.Equals("2"))
                                        {
                                            procOptno = procOptno + 1;
                                            choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "LA" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                        }
                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "LA" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");

                                    }
                                    else if (rdrChoice["Instcd"].ToString() == "209" && drCand["Domicile"].ToString() == "GO")
                                    {
                                        if (gender.Equals("2"))
                                        {
                                            procOptno = procOptno + 1;
                                            choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "GO" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                        }
                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "GO" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                    }
                                    else
                                    {
                                        if (HomeInstitute.Contains(rdrChoice["Instcd"].ToString() + drCand["Domicile"].ToString()))
                                        {
                                            if (gender.Equals("2"))
                                            {
                                                procOptno = procOptno + 1;
                                                choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "HS" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                            }
                                            procOptno = procOptno + 1;
                                            choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "HS" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                        }
                                        else
                                        {
                                            if (gender.Equals("2"))
                                            {
                                                procOptno = procOptno + 1;
                                                choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "OS" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                            }
                                            procOptno = procOptno + 1;
                                            choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "OS" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                        }
                                    }
                                    break;
                                case "AI":
                                    if (gender.Equals("2"))
                                    {
                                        procOptno = procOptno + 1;
                                        choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "AI" + option + "F:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                    }
                                    procOptno = procOptno + 1;
                                    choices = choices + (rdrChoice["Instcd"].ToString() + "." + rdrChoice["Brcd"].ToString() + "." + "AI" + option + "B:" + Convert.ToDouble(drCand["Rank"].ToString().Split(',')[rankType]) + ",");
                                    break;
                            }
                        }
                    }
                }
                rdrChoice.Close();
                if (procOptno > 0)
                {
                    dtAllChoices.Rows.Add(new object[] { drCand["rollNo"].ToString(), 0, procOptno, choices.ToString() });
                    choices = String.Empty;
                }
            }
            connChoice.Close();
            SqlHelper.ExecuteNonQuery(connectionString, CommandType.Text, "truncate table XT_VirtualChoice");
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connectionString) { DestinationTableName = "XT_VirtualChoice", BulkCopyTimeout = 350 };
            bulkCopy.WriteToServer(dtAllChoices);
            return new ActionOutput(ActionStatus.Success, "Completed:Choice created for " + dtAllChoices.Rows.Count + " candidates");

        }

        public string GetAllotmentOptionCode(string cat, string subcat, string symbol, string nationality)
        {
            options = string.Empty;
            if (nationality == "2" || nationality == "3")
            {
                if (symbol.Equals("*"))
                {
                    options += "OPNO;";
                }
            }
            else if (nationality == "1")
            {
                if (symbol.Equals("*"))
                {
                    options += "OPNO;";
                    if (subcat == "1")
                        options += "OPPH;";
                    if (cat != "GN")
                    {
                        options += cat + "NO;";
                        if (subcat == "1")
                            options += cat + "PH;";
                    }
                }
                else if ((symbol == "+" && (cat == "SC" || cat == "ST")) || (symbol == "=" && cat == "BC"))
                {
                    if (subcat == "1")
                        options += "OPPH;";
                    options += cat + "NO;";
                    if (subcat == "1")
                        options += cat + "PH;";
                }
                else if (symbol == "%" && subcat == "1")
                {
                    options += "OPPH;";
                }
                else if (symbol == "$" && cat == "BC" && subcat == "1")
                {
                    options += "OPPH;";
                    options += "BCPH;";
                }
            }
            return options.Length > 0 ? options.Substring(0, options.Length - 1) : string.Empty;
        }

        public override bool DereserveSeat(int iterationSeq)
        {
            SqlConnection connDereserve = new SqlConnection(connectionString);
            connDereserve.Open();
            SqlCommand cmd = new SqlCommand(DBProc_DereserveSeat, connDereserve) { CommandType = CommandType.StoredProcedure };
            SqlParameter parIsIterationRequired = new SqlParameter("@isIterationrequired", SqlDbType.VarChar, 10) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(parIsIterationRequired);
            cmd.ExecuteNonQuery();
            connDereserve.Close();
            return (parIsIterationRequired.Value.ToString() == "Y" ? true : false);
        }

        public override DataTable GetOriginalChoice(string rollno)
        {
            SqlConnection connChoice = new SqlConnection(connectionString);
            SqlCommand cmdChoice = new SqlCommand(DBProc_GetCandidateChoice, connChoice);
            cmdChoice.CommandType = CommandType.StoredProcedure;
            cmdChoice.Parameters.AddWithValue("@RollNo", rollno);
            DataSet ds = new DataSet();
            new SqlDataAdapter(cmdChoice).Fill(ds);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];
            else return null;
        }

        public override ActionOutput UpdateApplicationAfterAllocation()
        {
            SqlConnection connUpdateApp = new SqlConnection(connectionString);
            connUpdateApp.Open();
            SqlCommand cmd = new SqlCommand(DBProc_UpdateApplication, connUpdateApp) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@roundno", roundNo);
            cmd.ExecuteNonQuery();
            connUpdateApp.Close();
            return new ActionOutput(ActionStatus.Success);
        }

        public  string GetHeader()
        {
            return "Common Counselling Application";
        }

        public  string GetSubheader()
        {
            return "Seat Allocation";
        }       

        public ActionOutput Validate(string boardId, string userId, string password, int roundno)
        {
            return new ActionOutput(ActionStatus.Success, "Valid", "admin", DataType.StringValue);
        }
    }
}
