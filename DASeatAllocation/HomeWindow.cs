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
    public partial class HomeWindow : Form
    {
        ICommon objCommon;
        public HomeWindow()
        {
            InitializeComponent();
          
            lblHeader.Text = AppConfiguration.Header ;
            lblSubheader.Text = AppConfiguration.SubHeader;
            lblRoundNo.Text = "Round No: "+ AppConfiguration.RoundNo.ToString();

            if (AppConfiguration.Role == "verify")
            {
                btnSeatAllocation.Enabled = false;
            }




        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnInputVerification_Click(object sender, EventArgs e)
        {
            new InputVerification().Show();
        }

        private void btnSeatAllocation_Click(object sender, EventArgs e)
        {
            //Application.Run(new DAForm());
            new DAForm().Show();

        }

        private void btnOutputValidation_Click(object sender, EventArgs e)
        {
            new OutputVerification().Show();
        }

        private void btnDownloadResult_Click(object sender, EventArgs e)
        {
            new DownloadResult().Show();
        }
    }
}
