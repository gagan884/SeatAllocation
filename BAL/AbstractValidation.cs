using System;
using AppFramework;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BAL
{
    public abstract class AbstractValidation : IValidation
    {
       
        public virtual ActionOutput TotalSeatsWithAllotted(int roundNo)
        {
            string rule = "Institute/program/sequence wise Available seats vs allotted seats";
            DataTable dt = new DAL().GetDataTableUsingCommand("select * from XT_AllotmentSummary where NewSeats<Allotted");
            if (dt != null && dt.Rows.Count > 0)
                return new ActionOutput(ActionStatus.Failed, "Failed: " +rule, dt,DataType.DataTable);
            else
                return new ActionOutput(ActionStatus.Success, "Passed: " +rule);
        }

        public virtual ActionOutput OldRetainCandidateAllocation(int roundNo)
        {
            string rule = "Checking old retained candidates got seat in new allotment also.";
            DataTable dt = new DAL().GetDataTableUsingCommand("select * from XT_PreviousAllotment  where RollNo not in (select rollno from XT_Allotment)");
            if (dt != null && dt.Rows.Count > 0)
                return new ActionOutput(ActionStatus.Failed, "Failed: " + rule, dt,DataType.DataTable);
            else
                return new ActionOutput(ActionStatus.Success, "Passed: " + rule);
        }

        public virtual ActionOutput Dereservation(int roundNo)
        {
           
            try
            {               
                SqlConnection connChoice = new SqlConnection(ObjectFactory.GetCommonObject().GetConnectionString());
                SqlCommand cmdChoice = new SqlCommand("XP_ValidateDereserve", connChoice);
                cmdChoice.CommandType = CommandType.StoredProcedure;
                cmdChoice.Parameters.Add("@status", SqlDbType.Char,1);
                connChoice.Open();
                cmdChoice.Parameters[0].Direction = ParameterDirection.Output;
                cmdChoice.ExecuteNonQuery();
                connChoice.Close();               
                switch (cmdChoice.Parameters[0].Value.ToString())
                {
                    case "F":
                        return new ActionOutput(ActionStatus.Failed, "Failed: Dereservation is not valid. For exact reason update stored procedure for dereservation validation");
                    case "S":
                        return new ActionOutput(ActionStatus.Success, "Passed: No issue found but update stored procedure for dereservation validation");
                    case "0":
                        return new ActionOutput(ActionStatus.Success, "Passed: No issue found.");
                    case "1":
                        return new ActionOutput(ActionStatus.Failed, "Failed: No of seats dereserved from must be less than or equeal to ( [total seats]- [allotted seats])");
                    case "2":
                        return new ActionOutput(ActionStatus.Failed, "Failed: Allotted seats are greater than total seats or allotted seats are greater than ([total seats] + [dereserved from seats] - [dereserved to seats]).");
                    case "3":
                        return new ActionOutput(ActionStatus.Failed, "Failed: Overall Institute/program wise seats must be intact despite dereservation).");
                    case "4":
                        return new ActionOutput(ActionStatus.Failed, "Failed: New seats in next iteration must be equeal to (Old seats +[Dereserved from]- [Dereserved from] )).");
                    default:
                        return new ActionOutput(ActionStatus.Failed, "Invalid response.");
                }
            }
            catch (Exception ex)
            {
                return new ActionOutput(ActionStatus.Failed, "Error occured: "+ex.Message);
            }
            
        }



        public virtual DataTable RankViolation(int roundNo)
        {
            DataTable dtAllotmentSummary = new DAL().GetDataTableUsingCommand("select Instcd+'.'+Brcd+'.'+Sequence wl,ClosingRank,ClosingRank_NewCand, InItSeats, NewSeats, Allotted, DereserveFrom from XT_AllotmentSummary");

            int initSeats = 1, newSeats = 2, allotted = 3, dereservedFrom = 4, ClosingRankNewCand = 5;


            Dictionary<string, double[]> allotmentSummary = new Dictionary<string, double[]>();
            foreach (DataRow dr in dtAllotmentSummary.Rows)
            {
                allotmentSummary.Add(dr["wl"].ToString(), new double[] { Convert.ToInt32(dr["ClosingRank"]), Convert.ToInt32(dr["InItSeats"]), Convert.ToInt32(dr["NewSeats"]), Convert.ToInt32(dr["Allotted"]), Convert.ToInt32(dr["DereserveFrom"]), Convert.ToInt32(dr["ClosingRank_NewCand"]), });
            }

            DataTable dtCandidateAllotment = new DAL().GetDataTableUsingCommand("select Rollno,Instcd +'.'+Brcd+'.'+Sequence wl from XT_Allotment");
            Dictionary<string, string> candidateAllotment = new Dictionary<string, string>();
            foreach (DataRow dr in dtCandidateAllotment.Rows)
            {
                candidateAllotment.Add(dr["rollno"].ToString(), dr["wl"].ToString());
            }

            DataTable dtValidation = new DataTable();
            dtValidation.Columns.Add("rollno");
            dtValidation.Columns.Add("procOptno");
            dtValidation.Columns.Add("Instcd");
            dtValidation.Columns.Add("Brcd");
            dtValidation.Columns.Add("Sequence");
            dtValidation.Columns.Add("CandRank");
            dtValidation.Columns.Add("ClosingRank");
            dtValidation.Columns.Add("InItSeats");
            dtValidation.Columns.Add("NewSeats");
            dtValidation.Columns.Add("Allotted");
            dtValidation.Columns.Add("DereserveFrom");
            dtValidation.Columns.Add("reason");


            SqlConnection cn = new SqlConnection(DAL.connString);
            SqlCommand cmdChoices = new SqlCommand("select rollno,VChoiceWithRank from XT_VirtualChoice", cn);


            cn.Open();
            SqlDataReader reader = cmdChoices.ExecuteReader();
            string choices, waitList, allottedWaitList;
            int prOptNo = 0;
            while (reader.Read())
            {
                choices = reader["VChoiceWithRank"].ToString();

                if (choices.Length == 0)
                    throw new Exception();

                if (candidateAllotment.ContainsKey(reader["rollno"].ToString()))
                    allottedWaitList = candidateAllotment[reader["rollno"].ToString()];
                else
                    allottedWaitList = null;

                prOptNo = 0;
                foreach (string ch in choices.Substring(0, choices.Length - 1).Split(','))
                {
                    prOptNo++;
                    waitList = ch.Split(':')[0];

                    if (!allotmentSummary.ContainsKey(waitList))
                    {
                        continue;
                    }

                    //if (allotmentSummary[waitList][newSeats] == 0)
                    //    continue;

                    if (allottedWaitList != null && waitList == allottedWaitList)//Allotted
                        break;

                    //Not Allotted waitList               

                    //Seat is vacant but not allotted
                    if (allotmentSummary[waitList][initSeats] > allotmentSummary[waitList][allotted] || allotmentSummary[waitList][newSeats] > allotmentSummary[waitList][allotted] || allotmentSummary[waitList][dereservedFrom] > 0)
                    {
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Split('.')[2], Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), "Seat Vacancy Error" });
                        continue;
                    }

                    //Closing Rank Error
                    if (allotmentSummary[waitList][ClosingRankNewCand] > Convert.ToDouble(ch.Split(':')[1]))
                    {
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Split('.')[2], Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), "Closing Rank Error" });
                        continue;
                    }
                }
            }

            reader.Close();

            new DAL().ExecuteCommand("truncate table XT_Validation");

            if (dtValidation.Rows.Count > 0)
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(cn) { DestinationTableName = "XT_Validation", BulkCopyTimeout = 500 };
                bulkCopy.WriteToServer(dtValidation);
            }
            cn.Close();

            if (dtValidation.Rows.Count > 0)
                return dtValidation;
            else
                return null;
        }

        public virtual DataTable ValidateVChoiceWithAllotmentSummary(int roundno, string rollno)
        {
            DataTable dtAllotmentSummary = new DAL().GetDataTableUsingCommand("select Instcd+'.'+Brcd+'.'+Sequence wl,ClosingRank , InItSeats, NewSeats, Allotted, DereserveFrom, ClosingRank_NewCand from XT_AllotmentSummary");
            int initSeats = 1, newSeats = 2, allotted = 3, dereservedFrom = 4, ClosingRankNewCand = 5;

            Dictionary<string, double[]> allotmentSummary = new Dictionary<string, double[]>();
            foreach (DataRow dr in dtAllotmentSummary.Rows)
            {
                allotmentSummary.Add(dr["wl"].ToString(), new double[] { Convert.ToInt32(dr["ClosingRank"]), Convert.ToInt32(dr["InItSeats"]), Convert.ToInt32(dr["NewSeats"]), Convert.ToInt32(dr["Allotted"]), Convert.ToInt32(dr["DereserveFrom"]), Convert.ToInt32(dr["ClosingRank_NewCand"]), });
            }

            DataTable dtCandidateAllotment = new DAL().GetDataTableUsingCommand("select Rollno,Instcd +'.'+Brcd+'.'+Sequence wl from XT_Allotment");
            Dictionary<string, string> candidateAllotment = new Dictionary<string, string>();
            foreach (DataRow dr in dtCandidateAllotment.Rows)
            {
                candidateAllotment.Add(dr["rollno"].ToString(), dr["wl"].ToString());
            }

            DataTable dtValidation = new DataTable();
            dtValidation.Columns.Add("rollno");
            dtValidation.Columns.Add("procOptno");
            dtValidation.Columns.Add("Instcd");
            dtValidation.Columns.Add("Brcd");
            dtValidation.Columns.Add("Sequence");
            dtValidation.Columns.Add("CandRank");
            dtValidation.Columns.Add("ClosingRank_NewCand");
            dtValidation.Columns.Add("InItSeats");
            dtValidation.Columns.Add("NewSeats");
            dtValidation.Columns.Add("Allotted");
            dtValidation.Columns.Add("DereserveFrom");
            dtValidation.Columns.Add("ClosingRank");
            dtValidation.Columns.Add("reason");


            SqlConnection cn = new SqlConnection(DAL.connString);
            SqlCommand cmdChoices = new SqlCommand("select rollno,VChoiceWithRank from XT_VirtualChoice where rollno=@rollno", cn);
            cmdChoices.Parameters.AddWithValue("@rollno", rollno);


            cn.Open();
            SqlDataReader reader = cmdChoices.ExecuteReader();
            string choices, waitList, allottedWaitList;
            int prOptNo = 0;
            while (reader.Read())
            {
                choices = reader["VChoiceWithRank"].ToString();

                if (choices.Length == 0)
                    throw new Exception();

                if (candidateAllotment.ContainsKey(reader["rollno"].ToString()))
                    allottedWaitList = candidateAllotment[reader["rollno"].ToString()];
                else
                    allottedWaitList = null;

                prOptNo = 0;
                foreach (string ch in choices.Substring(0, choices.Length - 1).Split(','))
                {
                    prOptNo++;
                    waitList = ch.Split(':')[0];

                    //No Seat in Seat Matrix
                    if (!allotmentSummary.ContainsKey(waitList) || allotmentSummary[waitList][2] == 0)
                    {
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length+ waitList.Split('.')[1].Length+2), Convert.ToDouble(ch.Split(':')[1]), "--", "--", "--", "--", "--", "--", "No Seat in Seat Matrix", });
                        continue;
                    }

                    if (allottedWaitList != null && waitList == allottedWaitList)//Allotted
                    {
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length + waitList.Split('.')[1].Length + 2), Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), allotmentSummary[waitList][0].ToString(), "Allotted", });
                        break;
                    }

                    //Seat is vacant but not allotted
                    if (allotmentSummary[waitList][initSeats] > allotmentSummary[waitList][allotted] || allotmentSummary[waitList][newSeats] > allotmentSummary[waitList][allotted] || allotmentSummary[waitList][dereservedFrom] > 0)
                    {
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length + waitList.Split('.')[1].Length + 2), Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), allotmentSummary[waitList][0].ToString(), "Seat Vacancy Error" });
                        continue;
                    }

                    //Closing Rank Error
                    if (allotmentSummary[waitList][ClosingRankNewCand] > Convert.ToDouble(ch.Split(':')[1]))
                    {
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length + waitList.Split('.')[1].Length + 2), Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), allotmentSummary[waitList][0].ToString(), "Closing Rank Error" });
                        continue;
                    }


                    //Not Allotted waitList               

                    if (allotmentSummary[waitList][newSeats] < allotmentSummary[waitList][allotted])
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length + waitList.Split('.')[1].Length + 2), Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), allotmentSummary[waitList][0].ToString(), "Seat is vacanct", });
                    else if (allotmentSummary[waitList][ClosingRankNewCand] >= Convert.ToDouble(ch.Split(':')[1]))
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length + waitList.Split('.')[1].Length + 2), Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), allotmentSummary[waitList][0].ToString(), "Rank Violation", });
                    else
                        dtValidation.Rows.Add(new object[] { reader["rollno"].ToString(), prOptNo, waitList.Split('.')[0], waitList.Split('.')[1], waitList.Substring(waitList.Split('.')[0].Length + waitList.Split('.')[1].Length + 2), Convert.ToDouble(ch.Split(':')[1]), allotmentSummary[waitList][ClosingRankNewCand].ToString(), allotmentSummary[waitList][initSeats].ToString(), allotmentSummary[waitList][newSeats].ToString(), allotmentSummary[waitList][allotted].ToString(), allotmentSummary[waitList][dereservedFrom].ToString(), allotmentSummary[waitList][0].ToString(), "--", });

                }

            }
            reader.Close();
            cn.Close();

            if (dtValidation.Rows.Count > 0)
            {
                return dtValidation;
            }
            else
                return null;
        }

       
    }
}
