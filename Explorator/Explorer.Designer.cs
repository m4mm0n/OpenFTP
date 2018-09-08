namespace Explorator
{
    partial class Explorer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvDirectoryList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxQuickView = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxTransfer = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxQueue = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxAdvancedTransfers = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxTransferAs = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxQueueAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxRename = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxMakeDir = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.ctxCopyNames = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxChangePerm = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvDirectoryList
            // 
            this.lvDirectoryList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.lvDirectoryList.ContextMenuStrip = this.contextMenuStrip1;
            this.lvDirectoryList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDirectoryList.Location = new System.Drawing.Point(0, 0);
            this.lvDirectoryList.Name = "lvDirectoryList";
            this.lvDirectoryList.Size = new System.Drawing.Size(588, 423);
            this.lvDirectoryList.TabIndex = 0;
            this.lvDirectoryList.UseCompatibleStateImageBehavior = false;
            this.lvDirectoryList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Size";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Date";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Attrib";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxChangePerm,
            this.ctxQuickView,
            this.toolStripMenuItem1,
            this.ctxTransfer,
            this.ctxQueue,
            this.ctxAdvancedTransfers,
            this.toolStripMenuItem2,
            this.ctxRename,
            this.ctxDelete,
            this.toolStripMenuItem3,
            this.ctxMakeDir,
            this.toolStripMenuItem4,
            this.ctxCopyNames});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(198, 248);
            // 
            // ctxQuickView
            // 
            this.ctxQuickView.Name = "ctxQuickView";
            this.ctxQuickView.Size = new System.Drawing.Size(197, 22);
            this.ctxQuickView.Text = "Preview";
            this.ctxQuickView.Click += new System.EventHandler(this.quickViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 6);
            // 
            // ctxTransfer
            // 
            this.ctxTransfer.Name = "ctxTransfer";
            this.ctxTransfer.Size = new System.Drawing.Size(197, 22);
            this.ctxTransfer.Text = "Transfer Selected";
            this.ctxTransfer.Click += new System.EventHandler(this.ctxTransfer_Click);
            // 
            // ctxQueue
            // 
            this.ctxQueue.Name = "ctxQueue";
            this.ctxQueue.Size = new System.Drawing.Size(197, 22);
            this.ctxQueue.Text = "Add Selected To Queue";
            this.ctxQueue.Click += new System.EventHandler(this.ctxQueue_Click);
            // 
            // ctxAdvancedTransfers
            // 
            this.ctxAdvancedTransfers.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxTransferAs,
            this.ctxQueueAs});
            this.ctxAdvancedTransfers.Name = "ctxAdvancedTransfers";
            this.ctxAdvancedTransfers.Size = new System.Drawing.Size(197, 22);
            this.ctxAdvancedTransfers.Text = "Advanced Transfer";
            // 
            // ctxTransferAs
            // 
            this.ctxTransferAs.Name = "ctxTransferAs";
            this.ctxTransferAs.Size = new System.Drawing.Size(185, 22);
            this.ctxTransferAs.Text = "Transfer Selected As..";
            this.ctxTransferAs.Click += new System.EventHandler(this.ctxTransferAs_Click);
            // 
            // ctxQueueAs
            // 
            this.ctxQueueAs.Name = "ctxQueueAs";
            this.ctxQueueAs.Size = new System.Drawing.Size(185, 22);
            this.ctxQueueAs.Text = "Queue Selected As..";
            this.ctxQueueAs.Click += new System.EventHandler(this.ctxQueueAs_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(194, 6);
            // 
            // ctxRename
            // 
            this.ctxRename.Name = "ctxRename";
            this.ctxRename.Size = new System.Drawing.Size(197, 22);
            this.ctxRename.Text = "Rename";
            this.ctxRename.Click += new System.EventHandler(this.ctxRename_Click);
            // 
            // ctxDelete
            // 
            this.ctxDelete.Name = "ctxDelete";
            this.ctxDelete.Size = new System.Drawing.Size(197, 22);
            this.ctxDelete.Text = "Delete";
            this.ctxDelete.Click += new System.EventHandler(this.ctxDelete_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(194, 6);
            // 
            // ctxMakeDir
            // 
            this.ctxMakeDir.Name = "ctxMakeDir";
            this.ctxMakeDir.Size = new System.Drawing.Size(197, 22);
            this.ctxMakeDir.Text = "Make Directory";
            this.ctxMakeDir.Click += new System.EventHandler(this.ctxMakeDir_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(194, 6);
            // 
            // ctxCopyNames
            // 
            this.ctxCopyNames.Name = "ctxCopyNames";
            this.ctxCopyNames.Size = new System.Drawing.Size(197, 22);
            this.ctxCopyNames.Text = "Copy Selected Names";
            this.ctxCopyNames.Click += new System.EventHandler(this.ctxCopyNames_Click);
            // 
            // ctxChangePerm
            // 
            this.ctxChangePerm.Name = "ctxChangePerm";
            this.ctxChangePerm.Size = new System.Drawing.Size(197, 22);
            this.ctxChangePerm.Text = "Change Permissions";
            // 
            // Explorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvDirectoryList);
            this.Name = "Explorer";
            this.Size = new System.Drawing.Size(588, 423);
            this.Load += new System.EventHandler(this.Explorer_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvDirectoryList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ctxQuickView;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ctxTransfer;
        private System.Windows.Forms.ToolStripMenuItem ctxQueue;
        private System.Windows.Forms.ToolStripMenuItem ctxAdvancedTransfers;
        private System.Windows.Forms.ToolStripMenuItem ctxTransferAs;
        private System.Windows.Forms.ToolStripMenuItem ctxQueueAs;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ctxRename;
        private System.Windows.Forms.ToolStripMenuItem ctxDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem ctxMakeDir;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem ctxCopyNames;
        private System.Windows.Forms.ToolStripMenuItem ctxChangePerm;
    }
}
