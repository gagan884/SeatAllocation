using AppFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;

namespace BAL
{
    public abstract class AbstractAllocation : IAllocation
    {
        protected AllocationSqlHelper objSql = null;
        protected int roundNo = -1;
        public virtual ActionOutput InItAllocation(int roundNo)
        {
            this.roundNo = roundNo;
            objSql = new AllocationSqlHelper();
            objSql.StartConnection();
            return new ActionOutput(ActionStatus.Success, "");
        }

        string boardId = string.Empty;

        #region InputPreparation     
        public abstract ActionOutput PrepareSeat();
        public abstract ActionOutput PrepareEligibleCandidate();
        public abstract ActionOutput PreparePreviousAllotment();
        #endregion InputPreparation

        #region VirtualChoice        
        public abstract ActionOutput CreateVirtualChoice();
        public virtual DataTable GetVirtualChoice(string rollno)
        {
            SqlConnection connChoice = new SqlConnection(ObjectFactory.GetCommonObject().GetConnectionString());
            SqlCommand cmdChoice = new SqlCommand("XP_GetVirtualChoice", connChoice);
            cmdChoice.CommandType = CommandType.StoredProcedure;
            cmdChoice.Parameters.AddWithValue("@RollNo", rollno);
            DataSet ds = new DataSet();
            new SqlDataAdapter(cmdChoice).Fill(ds);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["totalVChoice"]) > 0)
            {
                DataTable dtChoiceList = new DataTable();
                dtChoiceList.Columns.Add("Instcd");
                dtChoiceList.Columns.Add("Brcd");
                dtChoiceList.Columns.Add("Sequence");
                dtChoiceList.Columns.Add("ProcOptno");
                dtChoiceList.Columns.Add("Rank");
                string choice = "";
                double rank = 0;
                int ProcOptno = 1;

                string allChoices = ds.Tables[0].Rows[0]["VChoiceWithRank"].ToString();
                allChoices = allChoices.Substring(0, allChoices.Length - 1);
                foreach (string choiceWithRank in allChoices.Split(','))
                {
                    choice = choiceWithRank.Split(':')[0];
                    rank = Convert.ToDouble(choiceWithRank.Split(':')[1]);
                    string sequence = choice.Substring(choice.Split('.')[0].Length + choice.Split('.')[1].Length + 2);
                    dtChoiceList.Rows.Add(new object[] { choice.Split('.')[0], choice.Split('.')[1], sequence, ProcOptno++, rank });
                }
                return dtChoiceList;
            }
            else
                return null;
        }
        #endregion VirtualChoice

        #region SeatAllocation

        Dictionary<string, WaitList> WaitListArray = default(Dictionary<string, WaitList>);


        DataTable dtSeats;
        Dictionary<string, Int16> PIList = default(Dictionary<string, Int16>);
        Queue<string> Applicants = default(Queue<string>);
        public ActionOutput AllotSeat()
        {
            PreAllotmentProcessing();
            DataTable dtEligibleCandidates = objSql.GetDataTableUsingCommand("Select RollNo from XT_VirtualChoice");
            LoadPreviousAllotment();
            LoadVirtualChoice();
            bool isIterationRequired = true;
            string strVirtualChoice = "";
            int iterationSeq = 1;
            while (isIterationRequired)
            {
                //Seat Preparation
                dtSeats = objSql.GetDataTableUsingCommand("XP_GetProcessingSeat", CommandType.StoredProcedure);
                Seats = new Dictionary<string, int>();
                WaitListArray = new Dictionary<string, WaitList>(dtSeats.Rows.Count);
                foreach (DataRow dr in dtSeats.Rows)
                {
                    Seats.Add(dr["WLKey"].ToString(), Convert.ToInt32(dr["TSeat"].ToString()));
                    WaitListArray.Add(dr["WLKey"].ToString(), new WaitList());
                }

                //Applicant Queue preparation
                PIList = new Dictionary<string, short>(dtEligibleCandidates.Rows.Count);
                Applicants = new Queue<string>(dtEligibleCandidates.Rows.Count);
                foreach (DataRow dr in dtEligibleCandidates.Rows)
                {
                    Applicants.Enqueue(dr["RollNo"].ToString());
                    PIList.Add(dr["RollNo"].ToString(), 1);
                }

                //Reset temprary fields
                string candRollNo = "";
                WaitList objWaitList = default(WaitList);

                WaitListNode wlMaxCandNonRetain = default(WaitListNode);
                AllotmentDetail objApplicantAllotmentDetail = default(AllotmentDetail);
                WaitListNode wlCand = default(WaitListNode);
                int candRetainStatus = 0;
                string wlName = null;
                double rank = 0, maxRank = 0;

                while (Applicants.Count > 0)
                {
                    //Reset fields for each applicant
                    candRollNo = "";
                    strVirtualChoice = "";
                    wlName = null;
                    rank = 0;
                    objWaitList = null;
                    candRetainStatus = 0;
                    wlMaxCandNonRetain = null;
                    //objApplicantAllotmentDetail = null;
                    wlCand = null;

                   

                    //Load applicant details
                    candRollNo = Applicants.Dequeue();

                    if (candRollNo == "241250105193")
                    {
                        var s = candRollNo;
                    }

                    if (!AllChoices.ContainsKey(candRollNo) || AllChoices[candRollNo].VChoiceCount == 0)
                    {
                        RejectCandidate(candRollNo);
                        continue;
                    }

                    //strVirtualChoice = AllChoices[candRollNo].VChoiceWithRank.Split(',')[PIList[candRollNo] - 1];
                    //strVirtualChoice.Substring(0, strVirtualChoice.Length - 1);
                    //wlName = strVirtualChoice.Split(':')[0];
                    //rank = Convert.ToDouble(strVirtualChoice.Split(':')[1]);
                    wlName = AllChoices[candRollNo].VChoices[PIList[candRollNo] - 1].VChoice;
                    if (wlName == "100724.11075NN.79.G1.NCVT.HS.OP.B")
                    {
                        var s = candRollNo;
                    }
                    rank = AllChoices[candRollNo].VChoices[PIList[candRollNo] - 1].Rank;

                    if (!(Seats.ContainsKey(wlName)) || Seats[wlName] == 0)
                    {
                        RejectCandidate(candRollNo);
                        continue;
                    }




                    if ((WaitListArray.ContainsKey(wlName)))
                    {
                        objWaitList = WaitListArray[wlName];
                    }
                    else
                    {
                        RejectCandidate(candRollNo);
                        continue;
                    }



                    candRetainStatus = 0;
                    if (objAllAllotmentDetails != null && objAllAllotmentDetails.ContainsKey(candRollNo) && wlName == (objAllAllotmentDetails[candRollNo].Instcd + "." + objAllAllotmentDetails[candRollNo].Brcd + "." + objAllAllotmentDetails[candRollNo].Sequence))
                    {
                        candRetainStatus = objAllAllotmentDetails[candRollNo].IsRetained;
                    }


                    if (objWaitList.size() < Seats[wlName])
                    {
                        objWaitList.Enqueue(new WaitListNode(candRollNo, rank, candRetainStatus));
                        continue;
                    }


                    maxRank = 0;
                    if (!objWaitList.isEmpty())
                    {
                        maxRank = objWaitList.GetMaxRank();
                    }



                    if (candRetainStatus == 1)
                    {
                        if (maxRank > 0)
                        {
                            RemoveAndReject(ref objWaitList, wlName, maxRank);
                        }
                        objWaitList.Enqueue(new WaitListNode(candRollNo, rank, candRetainStatus));
                        continue;
                    }
                    else
                    {
                        if (maxRank > 0 && rank <= maxRank)
                        {
                            objWaitList.Enqueue(new WaitListNode(candRollNo, rank, candRetainStatus));
                            RemoveAndReject(ref objWaitList, wlName, rank > maxRank ? rank : maxRank);
                        }
                        else
                        {
                            RejectCandidate(candRollNo);
                        }
                    }
                }
                SaveIterationResult(iterationSeq);
                isIterationRequired = DereserveSeat(iterationSeq);
                iterationSeq++;
            }

            return new ActionOutput(ActionStatus.Success, "");
        }

        private void RejectCandidate(string candRollno)
        {
            PIList[candRollno] += 1;
            if (AllChoices[candRollno].VChoiceCount >= PIList[candRollno])
            {
                Applicants.Enqueue(candRollno);
            }
        }

        List<WaitListNode> SameRankCandidates = new List<WaitListNode>();
        private void RemoveAndReject(ref WaitList objWaitList, string wlName, double rank)
        {
            int filledSeats = objWaitList.size(); ;
            int SeatCapacity = Seats[wlName];

            SameRankCandidates.Clear();
            while (!objWaitList.isEmpty() && objWaitList.GetMaxRank() == rank)
                SameRankCandidates.Add(objWaitList.dequeue());


            if (filledSeats >= SeatCapacity)
            {
                foreach (WaitListNode x in SameRankCandidates)
                    RejectCandidate(x.rollNo);
            }
            else
            {
                foreach (WaitListNode x in SameRankCandidates)
                {
                    objWaitList.Enqueue(x);
                }
            }

        }

        public ActionOutput PreAllotmentProcessing()
        {
            objSql.ExecuteProcedure("XP_PreAllotmentProcessing");
            return new ActionOutput(ActionStatus.Success, "Completed");
        }

        public abstract bool DereserveSeat(int iterationSeq);

        public ActionOutput PrepareAllotmentSummary()
        {
            objSql.ExecuteCommand("truncate table XT_Allotment;truncate table XT_AllotmentSummary");
            string sequence = string.Empty;
            string[] wlKeyElements = null;
            DataTable dtWaitList = new DataTable();
            dtWaitList.Columns.Add("Instcd");
            dtWaitList.Columns.Add("Brcd");
            dtWaitList.Columns.Add("Sequence");
            dtWaitList.Columns.Add("InItSeats");
            dtWaitList.Columns.Add("NewSeats");
            dtWaitList.Columns.Add("Allotted");
            dtWaitList.Columns.Add("OpeningRank");
            dtWaitList.Columns.Add("ClosingRank");
            dtWaitList.Columns.Add("OpeningRank_NewCand");
            dtWaitList.Columns.Add("ClosingRank_NewCand");
            dtWaitList.Columns.Add("DereserveFrom");
            dtWaitList.Columns.Add("DereserveTo");

            //Save Allotment
            DataTable dtAllotment = new DataTable();
            dtAllotment.Columns.Add("RollNo");
            dtAllotment.Columns.Add("Instcd");
            dtAllotment.Columns.Add("Brcd");
            dtAllotment.Columns.Add("Sequence");
            dtAllotment.Columns.Add("Rank");
            foreach (var wl in WaitListArray)
            {
                wlKeyElements = wl.Key.Split('.');
                sequence = wl.Key.Substring(wlKeyElements[0].Length + wlKeyElements[1].Length + 2);
                dtWaitList.Rows.Add(new object[] { wlKeyElements[0], wlKeyElements[1], sequence, 0, Seats[wl.Key], wl.Value.size(), 0, 0, 0, 0, 0, 0 });
                foreach (WaitListNode cand in wl.Value)
                {
                    dtAllotment.Rows.Add(new object[] { cand.rollNo, wlKeyElements[0], wlKeyElements[1], sequence, cand.rank });
                }
            }
            objSql.SaveTableUsingBulkCopy(ref dtAllotment, "XT_Allotment", 500);
            objSql.SaveTableUsingBulkCopy(ref dtWaitList, "XT_AllotmentSummary", 500);
            objSql.ExecuteProcedure("XP_UpdateAllotmentSummary");
            return new ActionOutput(ActionStatus.Success, "Completed");
        }

        Dictionary<string, int> Seats = default(Dictionary<string, int>);
        public void LoadProcessingSeat()
        {
            Seats = new Dictionary<string, int>();
            foreach (DataRow dr in objSql.GetDataTableUsingCommand("XP_GetProcessingSeat", CommandType.StoredProcedure).Rows)
            {
                Seats.Add(dr["RollNo"].ToString(), Convert.ToInt32(dr["TSeat"].ToString()));
            }
        }

        public static Dictionary<String, VirtualChoices> AllChoices;
        public static Dictionary<string, string[]> VirtualChoice;
        public void LoadVirtualChoice()
        {
            AllChoices = new Dictionary<string, VirtualChoices>();
            SqlConnection connVChoice = new SqlConnection(ObjectFactory.GetCommonObject().GetConnectionString());
            SqlDataReader rdrChoice = default(SqlDataReader);
            SqlCommand cmdChoice = new SqlCommand("XP_GetAllVirtualChoices", connVChoice) { CommandType = CommandType.StoredProcedure };
            AllChoices = new Dictionary<string, VirtualChoices>();
            connVChoice.Open();
            rdrChoice = cmdChoice.ExecuteReader();
            while ((rdrChoice.Read()))
            {

                string[] choices = rdrChoice["VChoiceWithRank"].ToString().Split(',');
                VirtualChoice[] virtualChoices = new VirtualChoice[choices.Length];
                int i = 0;
                foreach (string choiceWithRank in choices)
                {
                    if (!string.IsNullOrEmpty(choiceWithRank))
                    {
                        virtualChoices[i] = new VirtualChoice(choiceWithRank);
                        i++;
                    }
                }

                AllChoices.Add(rdrChoice["rollNo"].ToString(), new VirtualChoices(rdrChoice["rollNo"].ToString(), rdrChoice["VChoiceWithRank"].ToString(), 0, Convert.ToInt32(rdrChoice["totalVChoice"]), virtualChoices));
            }
            connVChoice.Close();
        }


        public void LoadEligibleCandidate()
        {

        }


        public void SaveIterationResult(int iterationSeq)
        {
            string sequence = string.Empty;
            string[] wlKeyElements = null;
            //Save into Seat Dereserve
            DataTable dtDereserve = new DataTable();
            dtDereserve.Columns.Add("IterationNo");
            dtDereserve.Columns.Add("Instcd");
            dtDereserve.Columns.Add("Brcd");
            dtDereserve.Columns.Add("Sequence");
            dtDereserve.Columns.Add("Seats");
            dtDereserve.Columns.Add("Allotted");
            dtDereserve.Columns.Add("DereserveFrom");
            dtDereserve.Columns.Add("DereserveTo");

            //Save Allotment
            DataTable dtAllotment = new DataTable();
            dtAllotment.Columns.Add("IterationNo");
            dtAllotment.Columns.Add("RollNo");
            dtAllotment.Columns.Add("Instcd");
            dtAllotment.Columns.Add("Brcd");
            dtAllotment.Columns.Add("Sequence");
            dtAllotment.Columns.Add("Rank");

            foreach (var wl in WaitListArray)
            {
                
                wlKeyElements = wl.Key.Split('.');
                sequence = wl.Key.Substring(wlKeyElements[0].Length + wlKeyElements[1].Length + 2);
                dtDereserve.Rows.Add(new object[] { iterationSeq, wlKeyElements[0], wlKeyElements[1], sequence, Seats[wl.Key], wl.Value.size(), 0, 0 });
                foreach (WaitListNode cand in wl.Value)
                {
                    if (cand.rollNo == "241250100364")
                    {
                        var s = cand.rollNo;
                    }
                    dtAllotment.Rows.Add(new object[] { iterationSeq, cand.rollNo, wlKeyElements[0], wlKeyElements[1], sequence, cand.rank });
                }


            }

            objSql.SaveTableUsingBulkCopy(ref dtAllotment, "XT_Allotted", 500);
            dtAllotment.Clear();
            dtAllotment = null;

            objSql.SaveTableUsingBulkCopy(ref dtDereserve, "XT_Dereserve", 500);
            dtDereserve.Clear();
            dtDereserve = null;

            objSql.ExecuteCommand("update A Set A.tSeat=B.Seats,A.aSeat=B.Allotted,A.bSeat=B.Seats-B.Allotted From XT_PSeat A inner join XT_Dereserve B on A.Instcd=B.Instcd and a.Brcd=B.Brcd and A.Sequence=B.Sequence and B.IterationNo=(select max(IterationNo) from XT_Dereserve) ");
        }
        #endregion SeatAllocation

        protected Dictionary<string, AllotmentDetail> objAllAllotmentDetails = default(Dictionary<string, AllotmentDetail>);
        public void LoadPreviousAllotment()
        {
            string connectionString = ObjectFactory.GetCommonObject().GetConnectionString();
            objAllAllotmentDetails = new Dictionary<string, AllotmentDetail>();
            foreach (DataRow dr in SqlHelper.ExecuteDataset(connectionString, "XP_GetPreviousAllotment").Tables[0].Rows)
            {
                objAllAllotmentDetails.Add(dr["RollNo"].ToString(), new AllotmentDetail(dr["RollNo"].ToString(), dr["Instcd"].ToString(), dr["Brcd"].ToString(), dr["Sequence"].ToString(), Convert.ToInt32(dr["OptNo"].ToString()), Convert.ToInt32(dr["isRetained"].ToString())));
            }
        }

        public abstract ActionOutput UpdateApplicationAfterAllocation();

        public ActionOutput UnloadAllocation()
        {
            objSql.CloseConnection();
            return new ActionOutput(ActionStatus.Success, "");
        }
        public abstract DataTable GetOriginalChoice(string rollno);

        public virtual DataSet GetCandidateDetails(string rollno)
        {
            SqlConnection connCandidate = new SqlConnection(DAL.connString);
            SqlCommand cmdChoice = new SqlCommand("XP_GetCandidateDetails", connCandidate);
            cmdChoice.CommandType = CommandType.StoredProcedure;
            cmdChoice.Parameters.AddWithValue("@RollNo", rollno);
            DataSet ds = new DataSet();
            new SqlDataAdapter(cmdChoice).Fill(ds);
            return ds;

        }

        public void SetRoundNo(int roundno)
        {
            roundNo = Convert.ToInt32(roundno);
        }

        public virtual ActionOutput GetAllotmentOverview(int roundno)
        {
            StringBuilder txt = new StringBuilder();
            txt.Append(AppFramework.DocumentStyleSheet.DefaultStyle);

            //Eligible Candidate
            string elgCount = new DAL().GetDataTableUsingCommand("select count(*) from XT_EligibleCandidate").Rows[0][0].ToString();
            txt.Append("<h3>Eligible Candidates : " + elgCount + "</h3>");


            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><th colspan='2'>Category Wise </th></tr>");
            foreach (DataRow dr in new DAL().GetDataTableUsingCommand("select Category,count(*) tot from XT_EligibleCandidate group by category").Rows)
            {
                txt.Append("<tr><td>" + (dr[0] != null ? dr[0].ToString() : "NULL") + "</td><td style='text-align:right;'>" + dr[1].ToString() + "</td></tr>");
            }
            txt.Append("</table>");

            txt.Append("<br/>");

            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><th colspan='2'>Subcategory Wise </th></tr>");
            foreach (DataRow dr in new DAL().GetDataTableUsingCommand("select Subcategory,count(*) tot from XT_EligibleCandidate group by Subcategory").Rows)
            {
                txt.Append("<tr><td>" + (dr[0] != null ? dr[0].ToString() : "NULL") + "</td><td style='text-align:right;'>" + dr[1].ToString() + "</td></tr>");
            }
            txt.Append("</table>");

            txt.Append("<br/>");

            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><th colspan='2'>Willingness Wise </th></tr>");
            foreach (DataRow dr in new DAL().GetDataTableUsingCommand("select Willingness,count(*) tot from XT_EligibleCandidate group by Willingness").Rows)
            {
                txt.Append("<tr><td>" + (dr[0] != null ? dr[0].ToString() : "NULL") + "</td><td style='text-align:right;'>" + dr[1].ToString() + "</td></tr>");
            }
            txt.Append("</table>");


            txt.Append("<br/>");

            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><th colspan='2'>Symbol Wise </th></tr>");
            foreach (DataRow dr in new DAL().GetDataTableUsingCommand("select Symbol,count(*) tot from XT_EligibleCandidate group by Symbol").Rows)
            {
                txt.Append("<tr><td>" + (dr[0] != null ? dr[0].ToString() : "NULL") + "</td><td style='text-align:right;'>" + dr[1].ToString() + "</td></tr>");
            }
            txt.Append("</table>");

            txt.Append("<br/>");

            //Previous confirmed Allotment
            string PreAllotment = new DAL().GetDataTableUsingCommand("select count(*) from XT_PreviousAllotment").Rows[0][0].ToString();
            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><th colspan='2'>Previous confirmed Allotment : " + PreAllotment + "</th></tr>");
            foreach (DataRow dr in new DAL().GetDataTableUsingCommand("select Willingness,count(*) tot from XT_PreviousAllotment A inner join XT_EligibleCandidate B on A.Rollno=B.Rollno  group by B.Willingness").Rows)
            {
                txt.Append("<tr><td>" + (dr[0] != null ? dr[0].ToString() : "NULL") + "</td><td style='text-align:right;'>" + dr[1].ToString() + "</td></tr>");
            }
            txt.Append("</table>");
            //End of eligible candidates


            txt.Append("<br/>");

            //No of Candidates with vitual choices
            string vChoice = new DAL().GetDataTableUsingCommand("select count(*) from XT_VirtualChoice").Rows[0][0].ToString();
            txt.Append("<h3>No of Candidates with virtual choices:" + vChoice + "</h3>");


            //Overall Allotment
            DataRow drAllotment = new DAL().GetDataTableUsingCommand("select sum(initSeats) initialSeats,sum(NewSeats) FinalSeats,sum(allotted) Allotted,sum(NewSeats-allotted) Vacancy, sum(dereservefrom) DereservedFrom,sum(dereserveto) DereservedTo from XT_AllotmentSummary").Rows[0];
            txt.Append("<h4>Overall Allotment : " + drAllotment["Allotted"].ToString() + "</h4>");
            txt.Append(@"
            <table border='1px' style='width:98%;border-collapse:collapse;'>
                <tr><th>initial Seats</th><th>Final Seats</th><th>Allotted</th><th>Vacancy</th><th>Dereserved From</th><th>Dereserved To</th></tr>
                <tr>" +
                   "<td>" + drAllotment["initialSeats"].ToString() + "</td>" +
                   "<td>" + drAllotment["FinalSeats"].ToString() + "</td>" +
                   "<td>" + drAllotment["Allotted"].ToString() + "</td>" +
                   "<td>" + drAllotment["Vacancy"].ToString() + "</td>" +
                   "<td>" + drAllotment["DereservedFrom"].ToString() + "</td>" +
                   "<td>" + drAllotment["DereservedTo"].ToString() + "</td>" +
            "</tr></table>");

            txt.Append("<br/>");

            string newAllotment = new DAL().GetDataTableUsingCommand("select Count(*) newAllotment from XT_Allotment where rollno not in (select rollno from XT_PreviousAllotment)").Rows[0][0].ToString();
            string RetainedAllotment = new DAL().GetDataTableUsingCommand("Select count(*) NoChange from XT_Allotment A inner join XT_PreviousAllotment B on A.RollNo=B.RollNo where A.instcd=B.instcd and A.brcd=B.brcd and A.Sequence=B.Sequence").Rows[0][0].ToString();
            string choiceUpgradationAllotment = new DAL().GetDataTableUsingCommand("Select count(*) ChoiceUpgradation from XT_Allotment A inner join XT_PreviousAllotment B on A.RollNo=B.RollNo where A.instcd<>B.instcd or A.brcd<>B.brcd").Rows[0][0].ToString();
            string categoryUpgradationAllotment = new DAL().GetDataTableUsingCommand("Select count(*) CategoryUpgradation from XT_Allotment A inner join XT_PreviousAllotment B on A.RollNo=B.RollNo where A.instcd=B.instcd and A.brcd=B.brcd and  A.Sequence<>B.Sequence").Rows[0][0].ToString();



            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><td>Fresh Allotment</td><td style='text-align:right;'>" + newAllotment + "</td></tr>");
            txt.Append("<tr><td>Retained</td><td style='text-align:right;'>" + RetainedAllotment + "</td></tr>");
            txt.Append("<tr><td>Choice Upgradation</td><td style='text-align:right;'>" + choiceUpgradationAllotment + "</td></tr>");
            txt.Append("<tr><td>Category Upgradation</td><td style='text-align:right;'>" + categoryUpgradationAllotment + "</td></tr>");
            txt.Append("</table>");

            txt.Append("<br/>");

            txt.Append("<h4>Sequence Wise Allocation </h4>");
            txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
            txt.Append("<tr><th>Sequence</th><th>initial Seats</th><th>Final Seats</th><th>Offered Seats</th><th>Allotted</th><th>Vacancy</th><th>Dereserved From</th><th>Dereserved To</th></tr>");
            foreach (DataRow dr in new DAL().GetDataTableUsingCommand(@"select A.Sequence,sum(initSeats) initialSeats,sum(NewSeats) FinalSeats,sum(initSeats-isnull(oldAllotted,0)) Offered, sum(allotted) Allotted,sum(NewSeats-allotted) Vacancy, sum(dereservefrom) DereservedFrom,sum(dereserveto) DereservedTo 
from XT_AllotmentSummary A left outer join (select Instcd, Brcd, Sequence,count(*) oldAllotted from XT_PreviousAllotment group by Instcd, Brcd, Sequence) B
	on A.instcd=B.instcd and A.brcd=B.Brcd and A.Sequence=B.Sequence
group by A.Sequence order by A.Sequence").Rows)
            {
                txt.Append(@"<tr>" +
                   "<td>" + dr["Sequence"].ToString() + "</td>" +
                   "<td>" + dr["initialSeats"].ToString() + "</td>" +
                   "<td>" + dr["FinalSeats"].ToString() + "</td>" +
                   "<td>" + dr["offered"].ToString() + "</td>" +
                   "<td>" + dr["Allotted"].ToString() + "</td>" +
                   "<td>" + dr["Vacancy"].ToString() + "</td>" +
                   "<td>" + dr["DereservedFrom"].ToString() + "</td>" +
                   "<td>" + dr["DereservedTo"].ToString() + "</td>" +
            "</tr>");
            }
            txt.Append("</table>");


            //End of overall allotment
            txt.Append("<br/>");

            txt.Append("<div style='font-size:9px; width:95%;'> <Table>");
            //No of Iterations in Allotment
            string iteration = new DAL().GetDataTableUsingCommand("select isnull(max(iterationno),0) from XT_Allotted").Rows[0][0].ToString();
            txt.Append("<tr><td>No of Iterations in Allotment</td><td style='text-align:right;'>" + iteration + "</td></tr>");

            txt.Append("</Table></div>");

            if (Convert.ToInt32(iteration) > 1)
            {
                txt.Append("<br/>");
                txt.Append("<h4>Iteration wise Allotment Summary</h4>");
                txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
                txt.Append("<tr><th>iterationno</th><th>Seats</th><th>Allotted</th><th>Dereserved From</th><th>Dereserved To</th></tr>");
                foreach (DataRow dr in new DAL().GetDataTableUsingCommand(@"select iterationno,sum(seats) Seats,sum(allotted) allotted,sum(Dereservefrom) Dereservefrom,sum(DereserveTo) DereserveTo from Xt_Dereserve group by iterationno").Rows)
                {
                    txt.Append(@"<tr>" +
                       "<td>" + dr["iterationno"].ToString() + "</td>" +
                       "<td>" + dr["Seats"].ToString() + "</td>" +
                       "<td>" + dr["allotted"].ToString() + "</td>" +
                       "<td>" + dr["Dereservefrom"].ToString() + "</td>" +
                       "<td>" + dr["DereserveTo"].ToString() + "</td>" +
                "</tr>");
                }
                txt.Append("</table>");

                txt.Append("<br/>");
                txt.Append("<h4>Iteration/Sequence wise Conversion Details</h4>");
                txt.Append("<table border='1px' style='width:98%;border-collapse:collapse;'>");
                txt.Append("<tr><th>iterationno</th><th>Sequence</th><th>Dereserved From</th><th>Dereserved To</th></tr>");
                foreach (DataRow dr in new DAL().GetDataTableUsingCommand(@"select iterationno,Sequence,sum(DereserveFrom) DereserveFrom,sum(DereserveTo) DereserveTo from Xt_Dereserve where DereserveFrom>0 or DereserveTo>0  group by iterationno,Sequence order by iterationno,Sequence").Rows)
                {
                    txt.Append(@"<tr>" +
                       "<td>" + dr["iterationno"].ToString() + "</td>" +
                       "<td>" + dr["Sequence"].ToString() + "</td>" +
                       "<td>" + dr["DereserveFrom"].ToString() + "</td>" +
                       "<td>" + dr["DereserveTo"].ToString() + "</td>" +
                "</tr>");
                }
                txt.Append("</table>");
            }




            return new ActionOutput(ActionStatus.Success, "Completed", txt.ToString(), DataType.HTML);
        }
    }
}
