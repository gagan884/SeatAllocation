using AppFramework;
using BAL;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
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

//using iTextSharp.tool.xml;

namespace DASeatAllocation
{
    public partial class InputVerification : Form
    {
        DataTable verificationSummary;
        public InputVerification()
        {
            InitializeComponent();
            lblHeader.Text = AppConfiguration.Header;
            lblSubheader.Text = AppConfiguration.SubHeader;
            lblRoundNo.Text = lblRoundNo.Text + ", Round No: " + AppConfiguration.RoundNo.ToString();

            verificationSummary = new DataTable();
            verificationSummary.Columns.Add("type");
            verificationSummary.Columns.Add("status");
            verificationSummary.Columns.Add("message");
            wbMessages.DocumentText = "";
        }

        private void btnSeatMatrixTotalCheck_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSeatMatrixTotalVsRetainedCheck_Click(object sender, EventArgs e)
        {
           
        }

        private void btnSeatMatrixPrimaryKeyCheck_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCandidateUniqueRollNo_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCandidateDistinctValueCheck_Click(object sender, EventArgs e)
        {
            
        }

        private void btnCandidateBRuleVsDomainValues_Click(object sender, EventArgs e)
        {
            
        }

        private void btnChoiceMatchSeatMatrix_Click(object sender, EventArgs e)
        {
            
        }

        private void btnChoiceBRules_Click(object sender, EventArgs e)
        {
            
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.DocumentText = GetTextToSave();
            wb.Print();
        }


        private string GetTextToSave()
        {
            string Header = "<h1 style='text-align:center;'>" + AppConfiguration.Header + "</h1>";
            string documentTitle = "<h3 style='text-align:center;'>Input Data Verification</h3>";
            string roundNoDate = "<table  style='border:0;width:100%'> <tr> <td style='text-align:left;'> Round No " + AppConfiguration.RoundNo + "</td> <td style='text-align:right;'> Date:" + System.DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr></table>";
            //string Footer = "<br/><br/><h3 style='text-align:right;'</h3>";
            return Header + documentTitle + roundNoDate + "<hr/>" + wbMessages.DocumentText;


            //string test = Header + documentTitle + wbMessages.DocumentText;
            //return Header + documentTitle;
        }

        //private void btnPDF_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string strFileName = AppConfiguration.Header + "-InputVerification" + ".pdf";
        //        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        //        saveFileDialog1.Filter = "PDF File|*.pdf";
        //        saveFileDialog1.Title = "Save PDF file";
        //        saveFileDialog1.FileName = strFileName;
        //        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        //        {
        //            if (saveFileDialog1.FileName != "")
        //            {
        //                string text = GetTextToSave();
        //                System.IO.FileStream fs =
        //                   (System.IO.FileStream)saveFileDialog1.OpenFile();
        //                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
        //                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
        //                pdfDoc.Open();

        //                StringReader sr = new StringReader(text);
        //                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
        //                pdfDoc.Close();
        //                fs.Close();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Unable to generate PDF. Error Message: " + ex.Message);
        //    }
        //}

        private void btnPDF_Click(object sender, EventArgs e)
        {
            try
            {
                string strFileName = AppConfiguration.Header + "-InputVerification" + ".pdf";
                AppConfiguration.SavePDF(strFileName, GetTextToSave());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to generate PDF. Error Message: " + ex.Message);
            }
        }
    }
}
