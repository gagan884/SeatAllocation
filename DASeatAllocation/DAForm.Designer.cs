namespace DASeatAllocation
{
    partial class DAForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DAForm));
            this.btnInIt = new System.Windows.Forms.Button();
            this.btnVirtualChoice = new System.Windows.Forms.Button();
            this.btnAllotment = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblSubheader = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAllotmentSummary = new System.Windows.Forms.Button();
            this.tblAllotmentOverview = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.flpExport = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPDF = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.wbAllotmentOverview = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tblAllotmentOverview.SuspendLayout();
            this.flpExport.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnInIt
            // 
            this.btnInIt.BackColor = System.Drawing.Color.CadetBlue;
            this.btnInIt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnInIt.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInIt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInIt.ForeColor = System.Drawing.Color.Azure;
            this.btnInIt.Location = new System.Drawing.Point(3, 3);
            this.btnInIt.Name = "btnInIt";
            this.btnInIt.Size = new System.Drawing.Size(361, 47);
            this.btnInIt.TabIndex = 0;
            this.btnInIt.Text = "Initialize for Seat Allocation";
            this.btnInIt.UseVisualStyleBackColor = false;
            this.btnInIt.Click += new System.EventHandler(this.btnInIt_Click);
            // 
            // btnVirtualChoice
            // 
            this.btnVirtualChoice.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.btnVirtualChoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnVirtualChoice.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVirtualChoice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVirtualChoice.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnVirtualChoice.Location = new System.Drawing.Point(3, 56);
            this.btnVirtualChoice.Name = "btnVirtualChoice";
            this.btnVirtualChoice.Size = new System.Drawing.Size(361, 47);
            this.btnVirtualChoice.TabIndex = 1;
            this.btnVirtualChoice.Text = "Create Virtual Choice";
            this.btnVirtualChoice.UseVisualStyleBackColor = false;
            this.btnVirtualChoice.Click += new System.EventHandler(this.btnVirtualChoice_Click);
            // 
            // btnAllotment
            // 
            this.btnAllotment.BackColor = System.Drawing.Color.CornflowerBlue;
            this.btnAllotment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAllotment.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAllotment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllotment.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnAllotment.Location = new System.Drawing.Point(3, 109);
            this.btnAllotment.Name = "btnAllotment";
            this.btnAllotment.Size = new System.Drawing.Size(361, 47);
            this.btnAllotment.TabIndex = 2;
            this.btnAllotment.Text = "Seat Allocation";
            this.btnAllotment.UseVisualStyleBackColor = false;
            this.btnAllotment.Click += new System.EventHandler(this.btnAllotment_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.ForeColor = System.Drawing.Color.Maroon;
            this.lblMessage.Location = new System.Drawing.Point(127, 340);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(367, 19);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "----------------------------------------------------";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.lblHeader, 3);
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(3, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(611, 37);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "lblHeader";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubheader
            // 
            this.lblSubheader.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.lblSubheader, 3);
            this.lblSubheader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubheader.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubheader.ForeColor = System.Drawing.Color.Bisque;
            this.lblSubheader.Location = new System.Drawing.Point(3, 37);
            this.lblSubheader.Name = "lblSubheader";
            this.lblSubheader.Size = new System.Drawing.Size(611, 37);
            this.lblSubheader.TabIndex = 8;
            this.lblSubheader.Text = "lblSubheader";
            this.lblSubheader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tableLayoutPanel3.SetColumnSpan(this.lblTitle, 3);
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.LemonChiffon;
            this.lblTitle.Location = new System.Drawing.Point(3, 74);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(611, 20);
            this.lblTitle.TabIndex = 9;
            this.lblTitle.Text = "Seat Allocation";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tblAllotmentOverview, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(623, 562);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(617, 94);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.492569F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 91.50743F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel3.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.lblTitle, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.lblSubheader, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(617, 94);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnAllotmentSummary, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnInIt, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnVirtualChoice, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnAllotment, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(127, 123);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(367, 214);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // btnAllotmentSummary
            // 
            this.btnAllotmentSummary.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnAllotmentSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAllotmentSummary.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAllotmentSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllotmentSummary.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnAllotmentSummary.Location = new System.Drawing.Point(3, 162);
            this.btnAllotmentSummary.Name = "btnAllotmentSummary";
            this.btnAllotmentSummary.Size = new System.Drawing.Size(361, 49);
            this.btnAllotmentSummary.TabIndex = 3;
            this.btnAllotmentSummary.Text = "View Allotment Overview";
            this.btnAllotmentSummary.UseVisualStyleBackColor = false;
            this.btnAllotmentSummary.Click += new System.EventHandler(this.btnAllotmentOverview_Click);
            // 
            // tblAllotmentOverview
            // 
            this.tblAllotmentOverview.ColumnCount = 2;
            this.tblAllotmentOverview.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tblAllotmentOverview.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tblAllotmentOverview.Controls.Add(this.label1, 0, 0);
            this.tblAllotmentOverview.Controls.Add(this.flpExport, 1, 0);
            this.tblAllotmentOverview.Controls.Add(this.wbAllotmentOverview, 0, 1);
            this.tblAllotmentOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblAllotmentOverview.Location = new System.Drawing.Point(127, 362);
            this.tblAllotmentOverview.Name = "tblAllotmentOverview";
            this.tblAllotmentOverview.RowCount = 2;
            this.tblAllotmentOverview.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tblAllotmentOverview.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblAllotmentOverview.Size = new System.Drawing.Size(367, 197);
            this.tblAllotmentOverview.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Allotment Overview";
            // 
            // flpExport
            // 
            this.flpExport.Controls.Add(this.btnPDF);
            this.flpExport.Controls.Add(this.btnPrint);
            this.flpExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpExport.Location = new System.Drawing.Point(259, 3);
            this.flpExport.Name = "flpExport";
            this.flpExport.Size = new System.Drawing.Size(105, 54);
            this.flpExport.TabIndex = 1;
            // 
            // btnPDF
            // 
            this.btnPDF.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPDF.BackgroundImage")));
            this.btnPDF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPDF.Location = new System.Drawing.Point(3, 3);
            this.btnPDF.Name = "btnPDF";
            this.btnPDF.Size = new System.Drawing.Size(40, 40);
            this.btnPDF.TabIndex = 43;
            this.btnPDF.UseVisualStyleBackColor = true;
            this.btnPDF.Click += new System.EventHandler(this.btnPDF_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPrint.BackgroundImage")));
            this.btnPrint.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPrint.Location = new System.Drawing.Point(49, 3);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(40, 40);
            this.btnPrint.TabIndex = 44;
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // wbAllotmentOverview
            // 
            this.tblAllotmentOverview.SetColumnSpan(this.wbAllotmentOverview, 2);
            this.wbAllotmentOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbAllotmentOverview.Location = new System.Drawing.Point(3, 63);
            this.wbAllotmentOverview.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbAllotmentOverview.Name = "wbAllotmentOverview";
            this.wbAllotmentOverview.Size = new System.Drawing.Size(361, 131);
            this.wbAllotmentOverview.TabIndex = 2;
            // 
            // DAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 562);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "DAForm";
            this.Text = "DA based seat allocation";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DAForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tblAllotmentOverview.ResumeLayout(false);
            this.tblAllotmentOverview.PerformLayout();
            this.flpExport.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInIt;
        private System.Windows.Forms.Button btnVirtualChoice;
        private System.Windows.Forms.Button btnAllotment;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblSubheader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnAllotmentSummary;
        private System.Windows.Forms.TableLayoutPanel tblAllotmentOverview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flpExport;
        private System.Windows.Forms.Button btnPDF;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.WebBrowser wbAllotmentOverview;
    }
}

