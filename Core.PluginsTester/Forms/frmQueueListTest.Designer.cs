namespace Core.PluginsTester.Forms
{
    partial class frmQueueListTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.explorer1 = new Explorator.Explorer();
            this.queueList1 = new Explorator.QueueList();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 409);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(275, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(293, 409);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 29);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // explorer1
            // 
            this.explorer1.IsPcExplorer = true;
            this.explorer1.Location = new System.Drawing.Point(12, 12);
            this.explorer1.Name = "explorer1";
            this.explorer1.OtherExplorer = null;
            this.explorer1.Size = new System.Drawing.Size(775, 156);
            this.explorer1.SizeAttrib = 60;
            this.explorer1.SizeDate = 60;
            this.explorer1.SizeName = 60;
            this.explorer1.SizeSize = 60;
            this.explorer1.StartupFolder = Explorator.SpecialFolders.MyDocuments;
            this.explorer1.StartupOther = null;
            this.explorer1.TabIndex = 3;
            // 
            // queueList1
            // 
            this.queueList1.Location = new System.Drawing.Point(12, 182);
            this.queueList1.Name = "queueList1";
            this.queueList1.Size = new System.Drawing.Size(776, 221);
            this.queueList1.SizeDestination = 150;
            this.queueList1.SizeName = 150;
            this.queueList1.SizeProgress = 185;
            this.queueList1.SizeSize = 75;
            this.queueList1.SizeSource = 150;
            this.queueList1.SizeSpeed = 75;
            this.queueList1.TabIndex = 0;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(477, 409);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(178, 29);
            this.button3.TabIndex = 4;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // frmQueueListTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.explorer1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.queueList1);
            this.Name = "frmQueueListTest";
            this.Text = "frmQueueListTest";
            this.Load += new System.EventHandler(this.frmQueueListTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Explorator.QueueList queueList1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private Explorator.Explorer explorer1;
        private System.Windows.Forms.Button button3;
    }
}