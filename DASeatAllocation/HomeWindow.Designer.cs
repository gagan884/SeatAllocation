namespace DASeatAllocation
{
    partial class HomeWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInputVerification = new System.Windows.Forms.Button();
            this.btnSeatAllocation = new System.Windows.Forms.Button();
            this.btnOutputValidation = new System.Windows.Forms.Button();
            this.btnDownloadResult = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblSubheader = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblRoundNo = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInputVerification
            // 
            this.btnInputVerification.BackColor = System.Drawing.Color.LightSeaGreen;
            this.btnInputVerification.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInputVerification.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInputVerification.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnInputVerification.Location = new System.Drawing.Point(128, 147);
            this.btnInputVerification.Name = "btnInputVerification";
            this.btnInputVerification.Size = new System.Drawing.Size(369, 41);
            this.btnInputVerification.TabIndex = 0;
            this.btnInputVerification.Text = "Input Verification";
            this.btnInputVerification.UseVisualStyleBackColor = false;
            this.btnInputVerification.Visible = false;
            this.btnInputVerification.Click += new System.EventHandler(this.btnInputVerification_Click);
            // 
            // btnSeatAllocation
            // 
            this.btnSeatAllocation.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnSeatAllocation.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSeatAllocation.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeatAllocation.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSeatAllocation.Location = new System.Drawing.Point(128, 194);
            this.btnSeatAllocation.Name = "btnSeatAllocation";
            this.btnSeatAllocation.Size = new System.Drawing.Size(369, 38);
            this.btnSeatAllocation.TabIndex = 1;
            this.btnSeatAllocation.Text = "Seat Allocation";
            this.btnSeatAllocation.UseVisualStyleBackColor = false;
            this.btnSeatAllocation.Click += new System.EventHandler(this.btnSeatAllocation_Click);
            // 
            // btnOutputValidation
            // 
            this.btnOutputValidation.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOutputValidation.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOutputValidation.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOutputValidation.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnOutputValidation.Location = new System.Drawing.Point(128, 238);
            this.btnOutputValidation.Name = "btnOutputValidation";
            this.btnOutputValidation.Size = new System.Drawing.Size(369, 41);
            this.btnOutputValidation.TabIndex = 2;
            this.btnOutputValidation.Text = "Output Validation";
            this.btnOutputValidation.UseVisualStyleBackColor = false;
            this.btnOutputValidation.Click += new System.EventHandler(this.btnOutputValidation_Click);
            // 
            // btnDownloadResult
            // 
            this.btnDownloadResult.BackColor = System.Drawing.Color.RosyBrown;
            this.btnDownloadResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDownloadResult.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownloadResult.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDownloadResult.Location = new System.Drawing.Point(128, 285);
            this.btnDownloadResult.Name = "btnDownloadResult";
            this.btnDownloadResult.Size = new System.Drawing.Size(369, 39);
            this.btnDownloadResult.TabIndex = 3;
            this.btnDownloadResult.Text = "Download result";
            this.btnDownloadResult.UseVisualStyleBackColor = false;
            this.btnDownloadResult.Click += new System.EventHandler(this.btnDownloadResult_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.MistyRose;
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(128, 330);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(369, 40);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblSubheader
            // 
            this.lblSubheader.AutoSize = true;
            this.lblSubheader.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tableLayoutPanel2.SetColumnSpan(this.lblSubheader, 3);
            this.lblSubheader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubheader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubheader.ForeColor = System.Drawing.Color.Bisque;
            this.lblSubheader.Location = new System.Drawing.Point(0, 46);
            this.lblSubheader.Margin = new System.Windows.Forms.Padding(0);
            this.lblSubheader.Name = "lblSubheader";
            this.lblSubheader.Size = new System.Drawing.Size(619, 30);
            this.lblSubheader.TabIndex = 10;
            this.lblSubheader.Text = "lblSubheader";
            this.lblSubheader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tableLayoutPanel2.SetColumnSpan(this.lblHeader, 3);
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblHeader.Font = new System.Drawing.Font("Arial Rounded MT Bold", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.Lavender;
            this.lblHeader.Location = new System.Drawing.Point(0, 0);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(619, 46);
            this.lblHeader.TabIndex = 9;
            this.lblHeader.Text = "lblHeader";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblRoundNo
            // 
            this.lblRoundNo.AutoSize = true;
            this.lblRoundNo.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tableLayoutPanel2.SetColumnSpan(this.lblRoundNo, 3);
            this.lblRoundNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRoundNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRoundNo.ForeColor = System.Drawing.Color.LemonChiffon;
            this.lblRoundNo.Location = new System.Drawing.Point(0, 76);
            this.lblRoundNo.Margin = new System.Windows.Forms.Padding(0);
            this.lblRoundNo.Name = "lblRoundNo";
            this.lblRoundNo.Size = new System.Drawing.Size(619, 21);
            this.lblRoundNo.TabIndex = 11;
            this.lblRoundNo.Text = "Round No";
            this.lblRoundNo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnOutputValidation, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnDownloadResult, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnExit, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnInputVerification, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSeatAllocation, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(625, 406);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(619, 97);
            this.panel1.TabIndex = 12;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 7.859532F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 92.14046F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.tableLayoutPanel2.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblSubheader, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblRoundNo, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.29412F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.70588F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(619, 97);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // HomeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 586);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "HomeWindow";
            this.Text = "HomeWindow";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInputVerification;
        private System.Windows.Forms.Button btnSeatAllocation;
        private System.Windows.Forms.Button btnOutputValidation;
        private System.Windows.Forms.Button btnDownloadResult;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblSubheader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblRoundNo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel1;
    }
}