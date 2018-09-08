namespace Core.PluginsTester.Forms
{
    partial class frmFtpConnectUploadDownloadTest
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
            this.explorer1 = new Explorator.Explorer();
            this.explorer2 = new Explorator.Explorer();
            this.queueList1 = new Explorator.QueueList();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // explorer1
            // 
            this.explorer1.IsPcExplorer = true;
            this.explorer1.Location = new System.Drawing.Point(384, 12);
            this.explorer1.Name = "explorer1";
            this.explorer1.OtherExplorer = null;
            this.explorer1.Size = new System.Drawing.Size(404, 227);
            this.explorer1.SizeAttrib = 60;
            this.explorer1.SizeDate = 60;
            this.explorer1.SizeName = 60;
            this.explorer1.SizeSize = 60;
            this.explorer1.StartupFolder = Explorator.SpecialFolders.MyDocuments;
            this.explorer1.StartupOther = null;
            this.explorer1.TabIndex = 0;
            // 
            // explorer2
            // 
            this.explorer2.IsPcExplorer = true;
            this.explorer2.Location = new System.Drawing.Point(12, 12);
            this.explorer2.Name = "explorer2";
            this.explorer2.OtherExplorer = null;
            this.explorer2.Size = new System.Drawing.Size(366, 227);
            this.explorer2.SizeAttrib = 60;
            this.explorer2.SizeDate = 60;
            this.explorer2.SizeName = 60;
            this.explorer2.SizeSize = 60;
            this.explorer2.StartupFolder = Explorator.SpecialFolders.MyDocuments;
            this.explorer2.StartupOther = null;
            this.explorer2.TabIndex = 1;
            // 
            // queueList1
            // 
            this.queueList1.Location = new System.Drawing.Point(12, 245);
            this.queueList1.Name = "queueList1";
            this.queueList1.Size = new System.Drawing.Size(775, 108);
            this.queueList1.SizeDestination = 150;
            this.queueList1.SizeName = 150;
            this.queueList1.SizeProgress = 185;
            this.queueList1.SizeSize = 75;
            this.queueList1.SizeSource = 150;
            this.queueList1.SizeSpeed = 75;
            this.queueList1.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 440);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(93, 440);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Transfer";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 359);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(776, 69);
            this.listBox1.TabIndex = 5;
            // 
            // frmFtpConnectUploadDownloadTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 475);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.queueList1);
            this.Controls.Add(this.explorer2);
            this.Controls.Add(this.explorer1);
            this.Name = "frmFtpConnectUploadDownloadTest";
            this.Text = "frmFtpConnectUploadDownloadTest";
            this.Load += new System.EventHandler(this.frmFtpConnectUploadDownloadTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Explorator.Explorer explorer1;
        private Explorator.Explorer explorer2;
        private Explorator.QueueList queueList1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
    }
}