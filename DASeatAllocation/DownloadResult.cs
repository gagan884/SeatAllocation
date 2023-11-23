using AppFramework;
using BAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DASeatAllocation
{
    public partial class DownloadResult : Form
    {
        public DownloadResult()
        {
            InitializeComponent();
            lblHeader.Text = AppConfiguration.Header;
            lblSubheader.Text = AppConfiguration.SubHeader;
            lblRoundNo.Text = lblRoundNo.Text + ", Round No: " + AppConfiguration.RoundNo.ToString();
        }

        Form result;
        private void btnEligibleCandidate_Click(object sender, EventArgs e)
        {
            try
            {
                IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                DataTable dt = objDownload.GetEligibleCandiate(AppConfiguration.RoundNo);

                if (dt != null)
                {
                    if (rbl_CSV.Checked)
                        ExportCSV(ref dt, "EligibleCandidate");
                    else if (rbl_Excel.Checked)
                        ExportSpreadSheet(ref dt, "EligibleCandidate");
                    else
                    {
                        DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                        gv.DataSource = dt;
                        result = new Form() { Text = "Eligible Candidate", AutoSize = true, AutoScroll = true };
                        result.Controls.Add(gv);
                        result.Show();
                    }
                }
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
        }

        private void btnSeatBeforeProcessing_Click(object sender, EventArgs e)
        {
            try
            {
                IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                DataTable dt = objDownload.PSeatBeforeProcessing(AppConfiguration.RoundNo);

                if (dt != null)
                {
                    if (rbl_CSV.Checked)
                        ExportCSV(ref dt, "SeatBeforeProcessing");
                    else if (rbl_Excel.Checked)
                        ExportSpreadSheet(ref dt, "SeatBeforeProcessing");
                    else
                    {
                        DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                        gv.DataSource = dt;
                        result = new Form() { Text = "Seat Before Processing", AutoSize = true, AutoScroll = true };
                        result.Controls.Add(gv);
                        result.Show();
                    }
                }
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
        }

        private void btnChoice_Click(object sender, EventArgs e)
        {
            try
            { 
            IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
            DataTable dt = objDownload.GetChoice(AppConfiguration.RoundNo);

            if (dt != null)
            {
                if (rbl_CSV.Checked)
                    ExportCSV(ref dt, "Choice");
                else if (rbl_Excel.Checked)
                    ExportSpreadSheet(ref dt, "Choice");
                else
                {
                    DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                    gv.DataSource = dt;
                    result = new Form() { Text = "Choice", AutoSize = true, AutoScroll = true };
                    result.Controls.Add(gv);
                    result.Show();
                }
            }
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
        }

        private void btnAllotment_Click(object sender, EventArgs e)
        {
            try
            {
                IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                DataTable dt = objDownload.GetAllotment(AppConfiguration.RoundNo);

                if (dt != null)
                {
                    if (rbl_CSV.Checked)
                        ExportCSV(ref dt, "Allotment");
                    else if (rbl_Excel.Checked)
                        ExportSpreadSheet(ref dt, "Allotment");
                    else
                    {
                        DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                        gv.DataSource = dt;
                        result = new Form() { Text = "Allotment", AutoSize = true, AutoScroll = true };
                        result.Controls.Add(gv);
                        result.Show();
                    }
                }            
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
}

        private void btnAllotmentSummary_Click(object sender, EventArgs e)
        {
            try
            { 
            IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
            DataTable dt = objDownload.GetAllotmentSummary(AppConfiguration.RoundNo);

            if (dt != null)
            {
                if (rbl_CSV.Checked)
                    ExportCSV(ref dt, "AllotmentSummary");
                else if (rbl_Excel.Checked)
                    ExportSpreadSheet(ref dt, "AllotmentSummary");
                else
                {
                    DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                    gv.DataSource = dt;
                    result = new Form() { Text = "Allotment Summary", AutoSize = true, AutoScroll = true };
                    result.Controls.Add(gv);
                    result.Show();
                }
            }
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
        }

        private void btnSeatAfterProcessing_Click(object sender, EventArgs e)
        {
            try
            {
                IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                DataTable dt = objDownload.PSeatAfterProcessing(AppConfiguration.RoundNo);

                if (dt != null)
                {
                    if (rbl_CSV.Checked)
                        ExportCSV(ref dt, "SeatAfterProcessing");
                    else if (rbl_Excel.Checked)
                        ExportSpreadSheet(ref dt, "SeatAfterProcessing");
                    else
                    {
                        DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                        gv.DataSource = dt;
                        result = new Form() { Text = "Seats After Processing", AutoSize = true, AutoScroll = true };
                        result.Controls.Add(gv);
                        result.Show();
                    }
                }
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
        }

        private void btnORCR_Click(object sender, EventArgs e)
        {
            try
            {
                IDownload objDownload = ObjectFactory.GetDownloadObject(AppConfiguration.BoardId, AppConfiguration.RoundNo);
                DataTable dt = objDownload.GetORCR(AppConfiguration.RoundNo);

                if (dt != null)
                {
                    if (rbl_CSV.Checked)
                        ExportCSV(ref dt, "ORCR");
                    else if (rbl_Excel.Checked)
                        ExportSpreadSheet(ref dt, "ORCR");                   
                    else
                    {
                        DataGridView gv = new DataGridView() { AutoSize = true, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, EnableHeadersVisualStyles = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells, AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells };
                        gv.DataSource = dt;
                        result = new Form() { Text = "Seats After Processing", AutoSize = true, AutoScroll = true };
                        result.Controls.Add(gv);
                        result.Show();
                    }
                }
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show("Not implemented");
            }
        }

        protected void ExportCSV(ref DataTable dt, string filename)
        {            
            StringBuilder csv = new StringBuilder();
            int FieldCount = dt.Columns.Count;
            int i = 0;
            foreach (DataColumn columns in dt.Columns)
            {
                i++;
                csv.Append("\""+columns.ColumnName + "\"");
                if (i < FieldCount)
                {
                    csv.Append(",");
                }
            }
            csv.AppendLine("");

            foreach (DataRow row in dt.Rows)
            {
                i = 0;
                foreach (DataColumn column in dt.Columns)
                {
                    i++;
                    csv.Append("\""+row[column.ColumnName].ToString()+"\"");
                    if (i < FieldCount)
                    {
                        csv.Append(",");
                    }
                }
                csv.AppendLine("");
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV File|*.csv|Text File|*.txt";
            saveFileDialog1.Title = "Save csv file";
            saveFileDialog1.FileName = filename + ".csv";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog1.OpenFile();
                    StreamWriter sr = new StreamWriter(fs);
                    sr.Write(csv);
                    sr.Flush();
                    fs.Close();
                }
            }
        }


        

        protected void ExportSpreadSheet(ref DataTable dt, string filename)
        {

            StringBuilder content = new StringBuilder();

            content.Append("<table><tr>");
            foreach (DataColumn columns in dt.Columns)
            {
                content.Append("<th>" + columns.ColumnName + "</th>");
            }
            content.Append("</tr>");
            content.AppendLine("");
            foreach (DataRow row in dt.Rows)
            {
               
                content.AppendLine("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    content.Append("<td>" + row[column.ColumnName].ToString() + "</td>");
                }
                content.AppendLine("</tr>");
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel file|*.xls";
            saveFileDialog1.Title = "Save csv file";
            saveFileDialog1.FileName = filename + ".xls";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog1.OpenFile();
                    StreamWriter sr = new StreamWriter(fs);
                    sr.Write(content.ToString());
                    sr.Flush();
                    fs.Close();
                }
            }
        }

        protected void ExportToDocument(ref DataTable dt, string filename)
        {

            StringBuilder content = new StringBuilder();

            content.Append("<table><tr>");
            foreach (DataColumn columns in dt.Columns)
            {
                content.Append("<th>" + columns.ColumnName + "</th>");
            }
            content.Append("</tr>");
            content.AppendLine("");
            foreach (DataRow row in dt.Rows)
            {

                content.AppendLine("<tr>");
                foreach (DataColumn column in dt.Columns)
                {
                    content.Append("<td>" + row[column.ColumnName].ToString() + "</td>");
                }
                content.AppendLine("</tr>");
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Word file|*.doc";
            saveFileDialog1.Title = "Save document file";
            saveFileDialog1.FileName = filename + ".doc";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs =
                       (System.IO.FileStream)saveFileDialog1.OpenFile();
                    StreamWriter sr = new StreamWriter(fs);
                    sr.Write(content.ToString());
                    sr.Flush();
                    fs.Close();
                }
            }
        }


    }
}
