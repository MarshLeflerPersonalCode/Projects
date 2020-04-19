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
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyNamectrlcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyGuidctrlgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.variableTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.m_StatListView = new System.Windows.Forms.ListView();
            this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GuidHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.timerProcessClasses = new System.Windows.Forms.Timer(this.components);
            this.statObjectViewer = new CustomControls.ObjectViewer();
            this.txtBoxFilterListView = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timerForFilter = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.configurationToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(850, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyNamectrlcToolStripMenuItem,
            this.copyGuidctrlgToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // copyNamectrlcToolStripMenuItem
            // 
            this.copyNamectrlcToolStripMenuItem.Name = "copyNamectrlcToolStripMenuItem";
            this.copyNamectrlcToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.copyNamectrlcToolStripMenuItem.Text = "&Copy Name(ctrl+c)";
            // 
            // copyGuidctrlgToolStripMenuItem
            // 
            this.copyGuidctrlgToolStripMenuItem.Name = "copyGuidctrlgToolStripMenuItem";
            this.copyGuidctrlgToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.copyGuidctrlgToolStripMenuItem.Text = "Copy &Guid(ctrl+g)";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem,
            this.databasesToolStripMenuItem,
            this.variableTypesToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // databasesToolStripMenuItem
            // 
            this.databasesToolStripMenuItem.Name = "databasesToolStripMenuItem";
            this.databasesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.databasesToolStripMenuItem.Text = "&Databases";
            this.databasesToolStripMenuItem.Click += new System.EventHandler(this.databasesToolStripMenuItem_Click);
            // 
            // variableTypesToolStripMenuItem
            // 
            this.variableTypesToolStripMenuItem.Name = "variableTypesToolStripMenuItem";
            this.variableTypesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.variableTypesToolStripMenuItem.Text = "Variable Types";
            this.variableTypesToolStripMenuItem.Click += new System.EventHandler(this.variableTypesToolStripMenuItem_Click);
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
            // m_StatListView
            // 
            this.m_StatListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameHeader,
            this.GuidHeader});
            this.m_StatListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_StatListView.FullRowSelect = true;
            this.m_StatListView.GridLines = true;
            this.m_StatListView.HideSelection = false;
            this.m_StatListView.Location = new System.Drawing.Point(0, 50);
            this.m_StatListView.MultiSelect = false;
            this.m_StatListView.Name = "m_StatListView";
            this.m_StatListView.Size = new System.Drawing.Size(260, 543);
            this.m_StatListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.m_StatListView.TabIndex = 1;
            this.m_StatListView.UseCompatibleStateImageBehavior = false;
            this.m_StatListView.View = System.Windows.Forms.View.Details;
            this.m_StatListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.m_StatListView_ColumnClick);
            this.m_StatListView.SelectedIndexChanged += new System.EventHandler(this.m_StatListView_SelectedIndexChanged);
            this.m_StatListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_StatListView_KeyDown);
            this.m_StatListView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_StatListView_MouseDown);
            // 
            // NameHeader
            // 
            this.NameHeader.Tag = "Name";
            this.NameHeader.Text = "Name";
            this.NameHeader.Width = 150;
            // 
            // GuidHeader
            // 
            this.GuidHeader.Text = "Guid";
            this.GuidHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.GuidHeader.Width = 80;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtBoxFilterListView);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(260, 50);
            this.panel1.TabIndex = 0;
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
            // timerProcessClasses
            // 
            this.timerProcessClasses.Interval = 5;
            this.timerProcessClasses.Tick += new System.EventHandler(this.timerProcessClasses_Tick);
            // 
            // statObjectViewer
            // 
            this.statObjectViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statObjectViewer.Location = new System.Drawing.Point(0, 0);
            this.statObjectViewer.Name = "statObjectViewer";
            this.statObjectViewer.Size = new System.Drawing.Size(564, 593);
            this.statObjectViewer.TabIndex = 0;
            this.statObjectViewer.PropertyValueChanged += new CustomControls.ObjectViewer.PropertyValueChangedHandler(this.statObjectViewer_PropertyValueChanged);
            // 
            // txtBoxFilterListView
            // 
            this.txtBoxFilterListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtBoxFilterListView.Location = new System.Drawing.Point(10, 23);
            this.txtBoxFilterListView.Name = "txtBoxFilterListView";
            this.txtBoxFilterListView.Size = new System.Drawing.Size(238, 20);
            this.txtBoxFilterListView.TabIndex = 7;
            this.txtBoxFilterListView.TextChanged += new System.EventHandler(this.txtBoxFilterListView_TextChanged);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(238, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Filter";
            // 
            // timerForFilter
            // 
            this.timerForFilter.Tick += new System.EventHandler(this.timerForFilter_Tick);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatEditorForm_FormClosing);
            this.Shown += new System.EventHandler(this.StatEditorForm_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
		private System.Windows.Forms.ColumnHeader NameHeader;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Timer timerProcessClasses;
		private CustomControls.ObjectViewer statObjectViewer;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databasesToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader GuidHeader;
        private System.Windows.Forms.ToolStripMenuItem variableTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyNamectrlcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyGuidctrlgToolStripMenuItem;
        private System.Windows.Forms.TextBox txtBoxFilterListView;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timerForFilter;
    }
}

