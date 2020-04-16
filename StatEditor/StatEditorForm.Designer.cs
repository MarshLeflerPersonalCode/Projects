namespace StatEditor
{
	partial class StatEditorForm
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this.m_StatListView = new System.Windows.Forms.ListView();
			this.TypeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.AppliciableHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GraphHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.timerProcessClasses = new System.Windows.Forms.Timer(this.components);
			this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statObjectViewer = new CustomControls.ObjectViewer();
			this.menuStrip1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(850, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 651);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(850, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 24);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(850, 627);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.splitContainer1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(842, 601);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Stats";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(842, 601);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Configuration";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.m_StatListView);
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.statObjectViewer);
			this.splitContainer1.Size = new System.Drawing.Size(836, 595);
			this.splitContainer1.SplitterDistance = 262;
			this.splitContainer1.SplitterWidth = 8;
			this.splitContainer1.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(260, 66);
			this.panel1.TabIndex = 0;
			// 
			// m_StatListView
			// 
			this.m_StatListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.TypeHeader,
            this.NameHeader,
            this.AppliciableHeader,
            this.GraphHeader});
			this.m_StatListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_StatListView.FullRowSelect = true;
			this.m_StatListView.GridLines = true;
			this.m_StatListView.HideSelection = false;
			this.m_StatListView.Location = new System.Drawing.Point(0, 66);
			this.m_StatListView.MultiSelect = false;
			this.m_StatListView.Name = "m_StatListView";
			this.m_StatListView.Size = new System.Drawing.Size(260, 527);
			this.m_StatListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.m_StatListView.TabIndex = 1;
			this.m_StatListView.UseCompatibleStateImageBehavior = false;
			this.m_StatListView.View = System.Windows.Forms.View.Details;
			this.m_StatListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_StatListView_MouseDown);
			// 
			// TypeHeader
			// 
			this.TypeHeader.Tag = "Type";
			this.TypeHeader.Text = "Type";
			this.TypeHeader.Width = 40;
			// 
			// NameHeader
			// 
			this.NameHeader.Tag = "Name";
			this.NameHeader.Text = "Name";
			this.NameHeader.Width = 150;
			// 
			// AppliciableHeader
			// 
			this.AppliciableHeader.Text = "Appliciable";
			// 
			// GraphHeader
			// 
			this.GraphHeader.Text = "Graph";
			// 
			// timerProcessClasses
			// 
			this.timerProcessClasses.Interval = 10;
			this.timerProcessClasses.Tick += new System.EventHandler(this.timerProcessClasses_Tick);
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Name = "testToolStripMenuItem";
			this.testToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
			this.testToolStripMenuItem.Text = "test";
			this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
			// 
			// statObjectViewer
			// 
			this.statObjectViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.statObjectViewer.Location = new System.Drawing.Point(0, 0);
			this.statObjectViewer.Name = "statObjectViewer";
			this.statObjectViewer.Size = new System.Drawing.Size(564, 593);
			this.statObjectViewer.TabIndex = 0;
			// 
			// StatEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(850, 673);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "StatEditorForm";
			this.Text = "Stat Editor";
			this.Shown += new System.EventHandler(this.StatEditorForm_Shown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView m_StatListView;
		private System.Windows.Forms.ColumnHeader TypeHeader;
		private System.Windows.Forms.ColumnHeader NameHeader;
		private System.Windows.Forms.ColumnHeader AppliciableHeader;
		private System.Windows.Forms.ColumnHeader GraphHeader;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Timer timerProcessClasses;
		private CustomControls.ObjectViewer statObjectViewer;
		private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
	}
}

