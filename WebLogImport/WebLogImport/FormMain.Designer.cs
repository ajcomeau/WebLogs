namespace WebLogImport
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdBrowse = new System.Windows.Forms.Button();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.dgFile = new System.Windows.Forms.DataGridView();
            this.cmdSave = new System.Windows.Forms.Button();
            this.textDomain = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rbDirectory = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ckAddToList = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgFile)).BeginInit();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFileName
            // 
            this.txtFileName.Location = new System.Drawing.Point(131, 42);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(361, 20);
            this.txtFileName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Import File:";
            // 
            // cmdBrowse
            // 
            this.cmdBrowse.Location = new System.Drawing.Point(131, 69);
            this.cmdBrowse.Name = "cmdBrowse";
            this.cmdBrowse.Size = new System.Drawing.Size(75, 23);
            this.cmdBrowse.TabIndex = 2;
            this.cmdBrowse.Text = "Browse";
            this.toolTips.SetToolTip(this.cmdBrowse, "Browse for file or directory.");
            this.cmdBrowse.UseVisualStyleBackColor = true;
            this.cmdBrowse.Click += new System.EventHandler(this.cmdBrowse_Click);
            // 
            // cmdLoad
            // 
            this.cmdLoad.Location = new System.Drawing.Point(213, 69);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(75, 23);
            this.cmdLoad.TabIndex = 3;
            this.cmdLoad.Text = "Load";
            this.toolTips.SetToolTip(this.cmdLoad, "Load the specified file(s) into the datagrid for previewing.");
            this.cmdLoad.UseVisualStyleBackColor = true;
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // dgFile
            // 
            this.dgFile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFile.Location = new System.Drawing.Point(12, 113);
            this.dgFile.Name = "dgFile";
            this.dgFile.Size = new System.Drawing.Size(837, 346);
            this.dgFile.TabIndex = 4;
            // 
            // cmdSave
            // 
            this.cmdSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSave.Location = new System.Drawing.Point(721, 465);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(128, 23);
            this.cmdSave.TabIndex = 5;
            this.cmdSave.Text = "Save to Database";
            this.toolTips.SetToolTip(this.cmdSave, "Save all records showing in the datagrid to a SQL Server database.");
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // textDomain
            // 
            this.textDomain.Location = new System.Drawing.Point(607, 43);
            this.textDomain.MaxLength = 25;
            this.textDomain.Name = "textDomain";
            this.textDomain.Size = new System.Drawing.Size(188, 20);
            this.textDomain.TabIndex = 6;
            this.toolTips.SetToolTip(this.textDomain, "Use this field to add a domain name or other identifier to the data when storing " +
        "records for multiple sites.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(524, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Domain Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Import Directory:";
            // 
            // rbFile
            // 
            this.rbFile.AutoSize = true;
            this.rbFile.Checked = true;
            this.rbFile.Location = new System.Drawing.Point(13, 7);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(14, 13);
            this.rbFile.TabIndex = 11;
            this.rbFile.TabStop = true;
            this.toolTips.SetToolTip(this.rbFile, "Import single log file or all *.log files in a specific directory.");
            this.rbFile.UseVisualStyleBackColor = true;
            this.rbFile.CheckedChanged += new System.EventHandler(this.rbFile_CheckedChanged);
            // 
            // rbDirectory
            // 
            this.rbDirectory.AutoSize = true;
            this.rbDirectory.Location = new System.Drawing.Point(13, 33);
            this.rbDirectory.Name = "rbDirectory";
            this.rbDirectory.Size = new System.Drawing.Size(14, 13);
            this.rbDirectory.TabIndex = 12;
            this.rbDirectory.UseVisualStyleBackColor = true;
            this.rbDirectory.CheckedChanged += new System.EventHandler(this.rbDirectory_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbFile);
            this.panel1.Controls.Add(this.rbDirectory);
            this.panel1.Location = new System.Drawing.Point(2, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(33, 62);
            this.panel1.TabIndex = 13;
            // 
            // ckAddToList
            // 
            this.ckAddToList.AutoSize = true;
            this.ckAddToList.Location = new System.Drawing.Point(350, 73);
            this.ckAddToList.Name = "ckAddToList";
            this.ckAddToList.Size = new System.Drawing.Size(146, 17);
            this.ckAddToList.TabIndex = 15;
            this.ckAddToList.Text = "Add records to current list";
            this.toolTips.SetToolTip(this.ckAddToList, "Add the contents of the new file to the records already displayed in the datagrid" +
        ".");
            this.ckAddToList.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 502);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(861, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel1
            // 
            this.statusLabel1.Name = "statusLabel1";
            this.statusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 524);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.ckAddToList);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textDomain);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.dgFile);
            this.Controls.Add(this.cmdLoad);
            this.Controls.Add(this.cmdBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFileName);
            this.Name = "FormMain";
            this.Text = "Web Log Preview & Save";
            ((System.ComponentModel.ISupportInitialize)(this.dgFile)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cmdBrowse;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.DataGridView dgFile;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.TextBox textDomain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbFile;
        private System.Windows.Forms.RadioButton rbDirectory;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox ckAddToList;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel1;
        private System.Windows.Forms.ToolTip toolTips;
    }
}

