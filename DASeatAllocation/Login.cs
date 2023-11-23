using AppFramework;
using BAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DASeatAllocation
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            //txtServerDB.Text = "10.25.97.205";
            //txtDBName.Text = "JAC_27072017_0800_R3";
            //txtUserIdDB.Text = "sa";
            //txtPwDDB.Text = "0205@nic";
            //txtUserId.Text = "JACAdmin";
            //txtPwD.Text = "Test@1234";

        }

        private void Login_Load(object sender, EventArgs e)
        {
            gbDatabase.Enabled = true;
            gbLogin.Enabled = false;

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUserId.Text.Trim()) || String.IsNullOrEmpty(txtPwD.Text.Trim()))
            {
                MessageBox.Show("Kindly enter login credentials");
                return;
            }

            IUserInterface obj = ObjectFactory.GetUIObject(cmbBoardId.SelectedItem.ToString(), Convert.ToInt32(cmbRound.SelectedItem));
            ActionOutput res = obj.Validate(cmbBoardId.SelectedItem.ToString(),txtUserId.Text.Trim(), txtPwD.Text.Trim(), Convert.ToInt32(cmbRound.SelectedItem));

            if (res.resultType!=ActionStatus.Success){
                MessageBox.Show("Kindly enter login credentials");
                return;
            }
            else if (res.resultType==ActionStatus.Success)
            {
                AppConfiguration.BoardId = cmbBoardId.SelectedItem.ToString();
                AppConfiguration.UserId = txtUserId.Text.Trim();
                AppConfiguration.Role = res.data.ToString();
                AppConfiguration.RoundNo = Convert.ToInt32(cmbRound.SelectedItem);
                DAL.boardId = AppConfiguration.BoardId;
                DAL.roundNo = AppConfiguration.RoundNo;
                ICommon objCommon = ObjectFactory.GetCommonObject();
                IUserInterface objUI = ObjectFactory.GetUIObject(DAL.boardId, DAL.roundNo);
                AppConfiguration.Header = objUI.GetHeader();
                AppConfiguration.SubHeader = objUI.GetSubheader();
                this.Close();
            }

        }

        //private void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    if (String.IsNullOrEmpty(txtUserId.Text.Trim()) || String.IsNullOrEmpty(txtPwD.Text.Trim()))
        //    {
        //        MessageBox.Show("Kindly enter login credentials");
        //        return;
        //    }

        //    DataTable dt = objDAL.GetDataTableUsingCommand("Select boardId,userId,pwd,maxRoundNo,userRole from dbo.XT_Administrator where boardId='" + cmbBoardId.SelectedItem.ToString() + "' and userId='"+txtUserId.Text.Trim()+"' and pwd='"+txtPwD.Text.Trim()+"'");
        //    if (dt == null || dt.Rows.Count == 0)
        //    {
        //        MessageBox.Show("Kindly enter login credentials");
        //        return;
        //    }
        //    else
        //    {

        //        AppConfiguration.BoardId= cmbBoardId.SelectedItem.ToString();
        //        AppConfiguration.UserId = txtUserId.Text.Trim();
        //        AppConfiguration.Role = dt.Rows[0]["userRole"].ToString();
        //        AppConfiguration.RoundNo= Convert.ToInt32(cmbRound.SelectedItem);
        //        DAL.boardId = AppConfiguration.BoardId;
        //        DAL.roundNo = AppConfiguration.RoundNo;
        //        ICommon objCommon = ObjectFactory.GetCommonObject();
        //        IUserInterface objUI = ObjectFactory.GetUIObject(DAL.boardId, DAL.roundNo);
        //        AppConfiguration.Header = objUI.GetHeader();
        //        AppConfiguration.SubHeader = objUI.GetSubheader();


        //        this.Close();  
        //    }
        //}

        DAL objDAL;
        private void btnSetDB_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtServerDB.Text.Trim()) || string.IsNullOrEmpty(txtDBName.Text.Trim()) || string.IsNullOrEmpty(txtPwDDB.Text.Trim()))
            {
                MessageBox.Show("Kindly enter database credentials");
                return;
            }
            string connString = "Data Source = " + txtServerDB.Text.Trim() + "; database=" + txtDBName.Text.Trim() + ";uid=" + txtUserIdDB.Text.Trim() + ";pwd=" + txtPwDDB.Text.Trim();
            SqlConnection cn = null;
            try
            {
                cn = new SqlConnection(connString);
                cn.Open();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid details,Kindly enter valid database credentials");
                return;
            }
            finally
            {
                if (cn != null && cn.State == ConnectionState.Open)
                    cn.Close();
            }
            DAL.connString = connString;
            gbLogin.Enabled = true;
            gbDatabase.Enabled = false;

            objDAL = new DAL();
            DataTable dt = objDAL.GetDataTableUsingCommand("Select 1 from sys.tables where name='XT_Administrator'");
            if (dt == null || dt.Rows.Count == 0)
            {
                DisbaleLoginPanel("Login cradentials table does not exist in database!.");
                return;
            }


            dt = objDAL.GetDataTableUsingCommand("Select distinct boardId from dbo.XT_Administrator");
            if (dt == null || dt.Rows.Count == 0)
            {
                DisbaleLoginPanel("Login cradentials are not exist in database!.");
                return;
            }

            cmbBoardId.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                cmbBoardId.Items.Add(dr["boardId"].ToString());
            }
            cmbBoardId.SelectedIndex = 0;

            if (cmbBoardId.Items.Count == 1)
                LoadBoardDetails(cmbBoardId.SelectedItem.ToString());


        }

        int maxRoundNo;
        string pwd;
        private void LoadBoardDetails(string boardId)
        {
            DataRow dr = objDAL.GetDataTableUsingCommand("Select boardId,pwd,maxRoundNo from dbo.XT_Administrator where boardId='" + boardId + "'").Rows[0];
            maxRoundNo = Convert.ToInt32(dr["maxRoundNo"]);
            pwd = dr["pwd"].ToString();

            cmbRound.Items.Clear();
            for (int i = 1; i <= maxRoundNo; i++)
            {
                cmbRound.Items.Add(i);
            }
            cmbRound.SelectedIndex = 0;
        }

        private void DisbaleLoginPanel(string msg = "")
        {
            if (msg.Length > 0)
                MessageBox.Show(msg);
            gbLogin.Enabled = false;
            gbDatabase.Enabled = true;
            DAL.connString = null;

        }

        private void cmbBoardId_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBoardDetails(cmbBoardId.SelectedItem.ToString());
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            gbDatabase.Enabled = true;
            gbLogin.Enabled = false;
        }
    }
}
