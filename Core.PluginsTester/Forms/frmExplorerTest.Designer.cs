namespace Core.PluginsTester.Forms
{
    partial class frmExplorerTest
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
            this.SuspendLayout();
            // 
            // explorer1
            // 
            this.explorer1.Location = new System.Drawing.Point(12, 12);
            this.explorer1.Name = "explorer1";
            this.explorer1.Size = new System.Drawing.Size(776, 426);
            this.explorer1.SizeAttrib = 60;
            this.explorer1.SizeDate = 60;
            this.explorer1.SizeName = 260;
            this.explorer1.SizeSize = 60;
            this.explorer1.StartupFolder = Explorator.SpecialFolders.MyDocuments;
            this.explorer1.StartupOther = null;
            this.explorer1.TabIndex = 0;
            // 
            // frmExplorerTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.explorer1);
            this.Name = "frmExplorerTest";
            this.Text = "frmExplorerTest";
            this.Load += new System.EventHandler(this.frmExplorerTest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Explorator.Explorer explorer1;
    }
}