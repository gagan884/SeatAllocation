using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DASeatAllocation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
            if (string.IsNullOrEmpty(AppConfiguration.Role))
                return;
            Application.Run(new HomeWindow());               
            
        }

        //static void Exit()
        //{
            
        //}
    }

    public static class AppConfiguration
    {
        public static string UserId { get; set; }
        public static string Role { get; set; }
        public static string Header { get; set; }
        public static string SubHeader { get; set; }
        public static string BoardId { get; set; }
        public static int RoundNo { get; set; }

        public static string GetStatusTitle(ActionStatus status)
        {
            switch (status)
            {
                case ActionStatus.Success:
                    return "Passed";
                case ActionStatus.Failed:
                    return "Failed";
                case ActionStatus.Error:
                    return "Error";
                default:
                    return "";
            }
        }


        public static Color GetStatusColor(ActionStatus status)
        {
            switch (status)
            {
                case ActionStatus.Success:
                    return Color.Green;
                case ActionStatus.Failed:
                    return Color.Red;
                case ActionStatus.Error:
                    return Color.OrangeRed;
                default:
                    return Color.LightGray;

            }
        }

        public static void SavePDF(string strFileName, string text)
        {
            try
            {               
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "PDF File|*.pdf";
                saveFileDialog1.Title = "Save PDF file";
                saveFileDialog1.FileName = strFileName;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        
                        System.IO.FileStream fs =
                           (System.IO.FileStream)saveFileDialog1.OpenFile();
                        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);                       
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, fs);
                        pdfDoc.Open();

                        StringReader sr = new StringReader(text);
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                        pdfDoc.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to generate PDF. Error Message: " + ex.Message);
            }
        }

       

    }

   

    


}
