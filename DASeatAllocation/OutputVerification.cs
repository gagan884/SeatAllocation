using AppFramework;
using BAL;
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace DASeatAllocation
{
    public partial class OutputVerification : Form
    {
        IAllocation obj = ObjectFactory.GetAllocationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
        public OutputVerification()
        {
            InitializeComponent();
            lblHeader.Text = AppConfiguration.Header;
            lblSubheader.Text = AppConfiguration.SubHeader;
            lblRoundNo.Text = lblRoundNo.Text + ", Round No: " + AppConfiguration.RoundNo.ToString();
        }

        private void btnTotSeatVsAllotSeat_Click(object sender, EventArgs e)
        {
            try
            {
                IValidation objValidation = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                ActionOutput res = objValidation.TotalSeatsWithAllotted(AppConfiguration.RoundNo);
                ((Button)sender).BackColor = AppConfiguration.GetStatusColor(res.resultType);
                wbMessages.DocumentText += "<h3 style='padding-bottom:5px; margin-bottom:2px;'> Total seats Vs Allotted seats : <span style='color:" + AppConfiguration.GetStatusColor(res.resultType).Name + ";'>" + AppConfiguration.GetStatusTitle(res.resultType) + "</span> </h3> <i>  " + res.message + "</i>";

            }
            catch (Exception ex)
            {
                wbMessages.DocumentText += "<br/> Error occured: " + ex.Message;
            }


            //IValidation objValidation = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
            //CustomeResponse res = objValidation.ChoiceWithSeatMatrix(AppConfiguration.RoundNo);

            //wbMessages.DocumentText += "<span style='color:" + GetStatusColor(res.Code).Name + "'> <br/><b>Filled Choice : </b>" + GetStatusTitle(res.Code) + " " + res.Message + "</span>";


        }

        private void btnPrevCand_Click(object sender, EventArgs e)
        {
            try
            {
                IValidation objValidation = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                ActionOutput res = objValidation.OldRetainCandidateAllocation(AppConfiguration.RoundNo);
                ((Button)sender).BackColor = AppConfiguration.GetStatusColor(res.resultType);
                wbMessages.DocumentText += "<h3 style='padding-bottom:5px; margin-bottom:2px;'> Previous retained candidates : <span style='color:" + AppConfiguration.GetStatusColor(res.resultType).Name + ";'>" + AppConfiguration.GetStatusTitle(res.resultType) + "</span> </h3> <i>  " + res.message + "</i>";
            }
            catch (Exception ex)
            {
                wbMessages.DocumentText += "<br/> Error occured: " + ex.Message;
            }
        }

        private void btnDereserve_Click(object sender, EventArgs e)
        {
            try
            {
                IValidation objValidation = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                ActionOutput res = objValidation.Dereservation(AppConfiguration.RoundNo);
                ((Button)sender).BackColor = AppConfiguration.GetStatusColor(res.resultType);


                string text = "<h3 style='padding-bottom:5px; margin-bottom:2px;'> Dereservation validation : <span style='color:" + AppConfiguration.GetStatusColor(res.resultType).Name + ";'>" + AppConfiguration.GetStatusTitle(res.resultType) + @"</span> </h3>
                 Rules are :
                 <ul>
                 <li>No of seats dereserved from must be less than or equeal to ( [total seats]- [allotted seats])</li>
                 <li>Allotted seats must be less than or equal to ([total seats] + [dereserved from seats] - [dereserved to seats]).</li>
                 <li>Overall Institute/program wise seats must be intact despite dereservation.</li>
                 <li>New seats in sebsequent iterations must be equeal to (Old seats +[Dereserved from]- [Dereserved from] )).</li></ul>";

                text += "<i>" + res.message + "</i>";

                wbMessages.DocumentText += text;
            }
            catch (Exception ex)
            {
                wbMessages.DocumentText += "<br/> Error occured: " + ex.Message;
            }
        }

        private void btnRankViolation_Click(object sender, EventArgs e)
        {
            try
            {
                IValidation objValidation = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                DataTable dt = objValidation.RankViolation(AppConfiguration.RoundNo);
                ActionStatus status = ActionStatus.Success;
                StringBuilder text = new StringBuilder();
                text.Append(AppFramework.DocumentStyleSheet.DefaultStyle);
                if (dt != null && dt.Rows.Count > 0)
                {
                    status = ActionStatus.Failed;
                    text.Append("<table>");
                    text.Append("<tr><th>RollNo</th><th>ProcOptNo</th><th>Instcd</th><th>Brcd</th><th>Sequence</th><th>CandRank</th><th>ClosingRank</th><th>InItSeats</th><th>NewSeats</th><th>Allotted</th><th>DereserveFrom</th><th>Reason</th></tr>");
                    foreach (DataRow dr in dt.Rows)
                    {
                        text.Append("<tr>");
                        for (int i = 0; i < 12; i++)
                        {
                            text.Append("<td>" + (dr[i] != null ? dr[i].ToString() : "") + "</td>");
                        }
                        text.Append("</tr>");
                    }
                    text.Append("</table>");
                }
                else
                    status = ActionStatus.Success;

                ((Button)sender).BackColor = AppConfiguration.GetStatusColor(status);
                string RuleText = "<h3 style='padding-bottom:5px; margin-bottom:2px;'> Rank Violation : <span style='color:" + AppConfiguration.GetStatusColor(status).Name + ";'>" + AppConfiguration.GetStatusTitle(status) + @"</span> </h3>
                 Rules are :
                 <ul>
                 <li>Seat Was vacant but not allotted</li>
                 <li>Closing rank of new candidates  in the program is worse than non-allotted candidate.</li>
                 <li>Non Allotted candidate was there but seat dereserved.</li>
                 <li>New allotment is worse than previous allotment for old seat confirmed candidate.</li></ul>";

                if (status.Equals("PS"))
                {
                    RuleText += "<i> Rank violation NOT found. </i>";
                }
                else
                {
                    RuleText += "<i> Rank violation found. </i>";
                    Form frm = new Form() { WindowState = FormWindowState.Maximized };
                    WebBrowser wb = new WebBrowser();
                    wb.DocumentText = text.ToString();
                    wb.Dock = DockStyle.Fill;
                    frm.Controls.Add(wb);
                    frm.Show();
                }
                wbMessages.DocumentText += RuleText;
            }
            catch (Exception ex)
            {
                lblRankViolation.Text = ex.Message;
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }



        private void btnPDF_Click(object sender, EventArgs e)
        {
            string strFileName = AppConfiguration.Header + "-Output Verification" + ".pdf";
            AppConfiguration.SavePDF(strFileName, GetTextToSave());
        }

        private string GetTextToSave()
        {
            string Header = "<h1 style='text-align:center;'>" + AppConfiguration.Header + "</h1>";
            string documentTitle = "<h3 style='text-align:center;'>" + lblRoundNo.Text + "</h3>";
            string roundNoDate = "<table  style='border:0;width:100%'> <tr> <td style='text-align:left;'> Round No " + AppConfiguration.RoundNo + "</td> <td style='text-align:right;'> Date:" + System.DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr></table>";
            return Header + documentTitle + roundNoDate + "<hr/>" + wbMessages.DocumentText;
        }


        private string GetCandidateDetails(string rollno)
        {
            StringBuilder text = new StringBuilder();
            IAllocation obj = ObjectFactory.GetAllocationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
            obj.SetRoundNo(AppConfiguration.RoundNo);


            DataSet ds = obj.GetCandidateDetails(txtRollNo.Text.Trim());
            if (ds.Tables.Count > 0)
            {
                text.Append("<h3><center>Candidate Allotment Summary</center></h3>");
                text.Append("<br/>");
                text.Append("<h4>Profile Details</h4>");
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    text.Append("<table>");
                    text.Append("<tr><th>Rollno</th><th>Category</th><th>Subcategory</th><th>Gender</th><th>Domicile</th><th>AdditionalInfo</th><th>Symbol</th><th>Rank</th><th>Willingness</th></tr>");
                    text.Append("<tr>");
                    for (int i = 0; i < 9; i++)
                    {
                        text.Append("<td>" + (ds.Tables[0].Rows[0][i] != null ? ds.Tables[0].Rows[0][i].ToString() : "") + "</td>");
                    }
                    text.Append("</tr></table>");
                }
                else
                {
                    text.Append("<p>No record found<p>");
                    return text.ToString();
                }

                text.Append("<h4>Previous Round allotment Details</h4>");
                if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                {
                    text.Append("<table>");
                    text.Append("<tr><th>Instcd</th><th>Brcd</th><th>Sequence</th><th>Optno</th><th>isRetained</th></tr>");
                    text.Append("<tr>");
                    for (int i = 1; i < 6; i++)
                    {
                        text.Append("<td>" + (ds.Tables[1].Rows[0][i] != null ? ds.Tables[1].Rows[0][i].ToString() : "") + "</td>");
                    }
                    text.Append("</tr></table>");
                }
                else
                {
                    text.Append("<p>No record found<p>");
                }

                text.Append("<h4>Current Round allotment Details</h4>");
                if (ds.Tables[2] != null && ds.Tables[2].Rows.Count > 0)
                {
                    text.Append("<table>");
                    text.Append("<tr><th>Instcd</th><th>Brcd</th><th>Sequence</th><th>Optno</th><th>isRetained</th></tr>");
                    text.Append("<tr>");
                    for (int i = 1; i < 5; i++)
                    {
                        text.Append("<td>" + (ds.Tables[2].Rows[0][i] != null ? ds.Tables[2].Rows[0][i].ToString() : "") + "</td>");
                    }
                    text.Append("</tr></table>");
                }
                else
                {
                    text.Append("<p>No record found<p>");
                }

                text.Append("<br/>");

                StringBuilder orginalChoice = new StringBuilder();
                orginalChoice.Append("<h4>Original Choice</h4>");
                using (DataTable dt = obj.GetOriginalChoice(txtRollNo.Text.Trim()))
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        orginalChoice.Append("<table style='background - color:bisque; '>");
                        int ColumnCount = dt.Columns.Count;
                        orginalChoice.Append("<tr>");
                        foreach (DataColumn dr in dt.Columns)
                        {
                            orginalChoice.Append("<th>" + dr.ColumnName + "</th>");
                        }
                        orginalChoice.Append("</tr>");
                        foreach (DataRow dr in dt.Rows)
                        {
                            orginalChoice.Append("<tr>");
                            for (int i = 0; i < ColumnCount; i++)
                            {
                                orginalChoice.Append("<td>" + (dr[i] != null ? dr[i].ToString() : "") + "</td>");
                            }
                            orginalChoice.Append("</tr>");
                        }

                        //orginalChoice.Append("<tr><th>Optno</th><th>Instcd</th><th>Brcd</th><th>RoundNo</th><th>Validity</th></tr>");
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    orginalChoice.Append("<tr>");
                        //    for (int i = 1; i < 6; i++)
                        //    {
                        //        orginalChoice.Append("<td>" + (dr[i] != null ? dr[i].ToString() : "") + "</td>");
                        //    }
                        //    orginalChoice.Append("</tr>");
                        //}
                        //orginalChoice.Append("</table>");

                        //orginalChoice.Append("<tr><th>Optno</th><th>Instcd</th><th>Brcd</th><th>Validity</th></tr>");
                        //foreach (DataRow dr in dt.Rows)
                        //{
                        //    orginalChoice.Append("<tr>");
                        //    for (int i = 0; i < 4; i++)
                        //    {
                        //        orginalChoice.Append("<td>" + (dr[i] != null ? dr[i].ToString() : "") + "</td>");
                        //    }
                        //    orginalChoice.Append("</tr>");
                        //}
                        orginalChoice.Append("</table>");

                    }
                    else
                    {
                        orginalChoice.Append("<span>No record found</span>");
                    }
                }

                StringBuilder virtualChoice = new StringBuilder();
                virtualChoice.Append("<h4>Virtual Choices</h4>");
                using (DataTable dt = obj.GetVirtualChoice(txtRollNo.Text.Trim()))
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        virtualChoice.Append("<table  style='background - color: darksalmon; '>");
                        virtualChoice.Append("<tr><th>Instcd</th><th>Brcd</th><th>Sequence</th><th>ProcOptno</th><th>Rank</th></tr>");
                        foreach (DataRow dr in dt.Rows)
                        {
                            virtualChoice.Append("<tr>");
                            for (int i = 0; i < 5; i++)
                            {
                                virtualChoice.Append("<td>" + (dr[i] != null ? dr[i].ToString() : "") + "</td>");
                            }
                            virtualChoice.Append("</tr>");
                        }
                        virtualChoice.Append("</table>");

                    }
                    else
                    {
                        virtualChoice.Append("<span>No record found</span>");
                    }
                }

                text.Append("<table  border='0'><tr style='vertical-align:top;'><td>" + orginalChoice.ToString() + "</td><td>" + virtualChoice.ToString() + "</td></tr></table>");

                text.Append("<br/>");

                IValidation obj1 = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                using (DataTable dt = obj1.ValidateVChoiceWithAllotmentSummary(AppConfiguration.RoundNo, txtRollNo.Text.Trim()))
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        text.Append("<h4>Virtual choice Comparision with allotment summary</h4>");
                        text.Append("<Table>");
                        text.Append("<tr><th>ProcOptno</th><th>instcd</th><th>Brcd</th><th>Sequence</th><th>CandRank</th><th>ClosingRank_NewCand</th><th>InitSeat</th><th>New Seat</th><th>Allotted</th><th>Dereserve From</th><th>Closing Rank</th><th>Reason</th></tr>");
                        foreach (DataRow dr in dt.Rows)
                        {
                            text.Append("<tr>");
                            for (int i = 1; i < 13; i++)
                            {
                                text.Append("<td>" + (dr[i] != null ? dr[i].ToString() : "") + "</td>");
                            }
                            text.Append("</tr>");
                        }
                    }

                }



            }

            return text.ToString();
        }


        private void btnViewCandidate_Click(object sender, EventArgs e)
        {
            Form frm = new Form() { WindowState = FormWindowState.Maximized };
            WebBrowser wb = new WebBrowser();
            StringBuilder text = new StringBuilder();
            text.Append(AppFramework.DocumentStyleSheet.DefaultStyle);
            text.Append(GetCandidateDetails(txtRollNo.Text.Trim()));
            wb.DocumentText = text.ToString();
            wb.Dock = DockStyle.Fill;
            frm.Controls.Add(wb);
            frm.Show();


        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.DocumentText = GetTextToSave();
            wb.Print();
        }

        private void btnBusinessRulesCheck_Click(object sender, EventArgs e)
        {
            
        }

        //private void btnProfile_Click(object sender, EventArgs e)
        //{
        //    IAllocation obj = ObjectFactory.GetAllocationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
        //    obj.SetRoundNo(AppConfiguration.RoundNo);

        //    DataSet ds = obj.GetCandidateDetails(txtRollNo.Text.Trim());
        //    if (ds.Tables.Count > 0)
        //    {
        //        if (ds.Tables[0] != null)
        //        {
        //            dgvCand.DataSource = ds.Tables[0];

        //        }

        //        if (ds.Tables[1] != null)
        //        {
        //            dgvPreAllotment.DataSource = ds.Tables[1];
        //        }

        //        if (ds.Tables[2] != null)
        //        {
        //            dgvCurrentAllotment.DataSource = ds.Tables[2];
        //        }

        //        using (DataTable dt = obj.GetOriginalChoice(txtRollNo.Text.Trim()))
        //        {
        //            DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
        //            gv.DataSource = dt;
        //            originalChoiceForm = new Form() { Text = "Original Choice choice", Anchor = AnchorStyles.Left, Dock = DockStyle.Left, AutoSize = true, AutoScroll = true };
        //            originalChoiceForm.Controls.Add(gv);
        //            originalChoiceForm.Show();
        //        }

        //        using (DataTable dt = obj.GetVirtualChoice(txtRollNo.Text.Trim()))
        //        {
        //            DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
        //            gv.DataSource = dt;
        //            if (virtualChoiceForm == null || virtualChoiceForm.IsDisposed == true)
        //            {

        //                virtualChoiceForm = new Form() { Text = "Virtual choice", AutoSize = true, AutoScroll = true };
        //                virtualChoiceForm.Controls.Add(gv);
        //                virtualChoiceForm.Show();
        //            }
        //        }
        //    }
        //}

        //private void btnChoice_Click(object sender, EventArgs e)
        //{
        //    IValidation obj = ObjectFactory.GetValidationObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
        //    using (DataTable dt = obj.ValidateVChoiceWithAllotmentSummary(AppConfiguration.RoundNo, txtRollNo.Text.Trim()))
        //    {
        //        if (choiceComparisionForm == null || choiceComparisionForm.IsDisposed == true)
        //        {
        //            DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
        //            gv.DataSource = dt;
        //            choiceComparisionForm = new Form() { Text = "Compare Choice with Allotment Summary", AutoSize = true, AutoScroll = true };
        //            choiceComparisionForm.Controls.Add(gv);
        //            choiceComparisionForm.Show();
        //        }

        //    }
        //}


    }
}
