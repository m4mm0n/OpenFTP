namespace Core.PluginsTester.Forms
{
    partial class frmPercentageBarTest
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
            this.percentageBar1 = new Explorator.PercentageBar();
            this.SuspendLayout();
            // 
            // percentageBar1
            // 
            this.percentageBar1.BackColor = System.Drawing.Color.Transparent;
            this.percentageBar1.BackgroundColor = System.Drawing.Color.Red;
            this.percentageBar1.BorderColor = System.Drawing.Color.Red;
            this.percentageBar1.CurrentBytes = ((long)(5000));
            this.percentageBar1.ForeColor = System.Drawing.Color.White;
            this.percentageBar1.GradiantColor = System.Drawing.Color.Black;
            this.percentageBar1.GradiantPosition = Explorator.PercentageBar.GradiantArea.Center;
            this.percentageBar1.Image = null;
            this.percentageBar1.Location = new System.Drawing.Point(115, 131);
            this.percentageBar1.Maximum = 100F;
            this.percentageBar1.MaximumBytes = ((long)(10000000));
            this.percentageBar1.Minimum = 0F;
            this.percentageBar1.Name = "percentageBar1";
            this.percentageBar1.ProgressColor = System.Drawing.Color.Yellow;
            this.percentageBar1.ShowPercentage = true;
            this.percentageBar1.ShowText = true;
            this.percentageBar1.Size = new System.Drawing.Size(184, 22);
            this.percentageBar1.Value = 50F;
            // 
            // frmPercentageBarTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.percentageBar1);
            this.Name = "frmPercentageBarTest";
            this.Text = "frmPercentageBarTest";
            this.ResumeLayout(false);

        }

        #endregion

        private Explorator.PercentageBar percentageBar1;
    }
}