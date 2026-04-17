using AppFramework;
using BAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DASeatAllocation
{
    public partial class DAForm : Form
    {
        int roundno = -1;
        string boardId = string.Empty;
        IAllocation objAllocation = null;
        ICommon objCommon = null;
        ActionOutput objResponse;
        public DAForm()
        {
            InitializeComponent();
            objCommon = ObjectFactory.GetCommonObject();
            //IUserInterface objUI = ObjectFactory.GetUIObject(boardId, DAL.roundNo);

            boardId = AppConfiguration.BoardId;
            lblHeader.Text = AppConfiguration.Header;
            lblSubheader.Text = AppConfiguration.SubHeader;
            tblAllotmentOverview.Visible = false;
            lblTitle.Text = "Seat Allocation for Round No. " + AppConfiguration.RoundNo.ToString();
            try
            {
                roundno = AppConfiguration.RoundNo;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Invalid roundno.";
                return;
            }
            objAllocation = ObjectFactory.GetAllocationObject(boardId, roundno);
            //txtRoundNo.Text = AppConfiguration.RoundNo.ToString();
            //txtRoundNo.Enabled = false;
        }

        private void DAForm_Load(object sender, EventArgs e)
        {

        }

        private void btnInIt_Click(object sender, EventArgs e)
        {
             //Get roundno
            //try
            //{
            //    roundno = Convert.ToInt16(txtRoundNo.Text);
            //}
            //catch (Exception ex)
            //{
            //    lblMessage.Text = "Invalid roundno.";
            //    return;
            //}
            
                    
            

            objAllocation.InItAllocation(roundno);

            objAllocation.PrepareEligibleCandidate();
            objResponse = objAllocation.PrepareSeat();
            objResponse = objAllocation.PreparePreviousAllotment();


            if (objResponse.resultType==ActionStatus.Success)
                lblMessage.Text = "Data connection created.\nEligible Candidate, Seat and Previous Allotment details preapared.";
            else
                lblMessage.Text = "Error:" + objResponse.message;
        }

        private void btnVirtualChoice_Click(object sender, EventArgs e)
        {
            objResponse = objAllocation.VirtualCreationNew();
             //objResponse = objAllocation.CreateVirtualChoice();
            if (objResponse.resultType == ActionStatus.Success)
                lblMessage.Text = "Virtual choice created";
            else
                lblMessage.Text = "Error:" + objResponse.message;
        }

        private void btnAllotment_Click(object sender, EventArgs e)
        {
            objResponse = objAllocation.AllotSeatNew();  
            objResponse = objAllocation.PrepareAllotmentSummary();
            objResponse = objAllocation.UpdateApplicationAfterAllocation();
            objAllocation.UnloadAllocation();
            lblMessage.Text = "Allocation Completed";
           
        }

        private void btnAllotmentOverview_Click(object sender, EventArgs e)
        {
            wbAllotmentOverview.DocumentText = objAllocation.GetAllotmentOverview(roundno).data.ToString();
            tblAllotmentOverview.Visible = true;
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            string strFileName = AppConfiguration.Header + "-Allotment Overview" + ".pdf";
            AppConfiguration.SavePDF(strFileName, GetTextToSave());
        }

        private string GetTextToSave()
        {
            string Header = "<h1 style='text-align:center;'>" + AppConfiguration.Header + "</h1>";
            string documentTitle = "<h3 style='text-align:center;'>Allotment Overview</h3>";
            string roundNoDate = "<table  style='border:0;width:100%'> <tr> <td style='text-align:left;'> Round No " + AppConfiguration.RoundNo + "</td> <td style='text-align:right;'> Date:" + System.DateTime.Now.ToString("dd/MM/yyyy") + "</td></tr></table>";            
            return Header + documentTitle + roundNoDate + "<hr/>" + wbAllotmentOverview.DocumentText;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.DocumentText = GetTextToSave();
            wb.Print();
        }
    }
}
