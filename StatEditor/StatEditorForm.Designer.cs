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
            this.unitTypeConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabs = new System.Windows.Forms.TabControl();
            this.timerProcessClasses = new System.Windows.Forms.Timer(this.components);
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
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
            this.variableTypesToolStripMenuItem,
            this.unitTypeConfigToolStripMenuItem});
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.configurationToolStripMenuItem.Text = "&Configuration";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.toolsToolStripMenuItem.Text = "&Editor";
            this.toolsToolStripMenuItem.Click += new System.EventHandler(this.toolsToolStripMenuItem_Click);
            // 
            // databasesToolStripMenuItem
            // 
            this.databasesToolStripMenuItem.Name = "databasesToolStripMenuItem";
            this.databasesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.databasesToolStripMenuItem.Text = "&Databases";
            this.databasesToolStripMenuItem.Click += new System.EventHandler(this.databasesToolStripMenuItem_Click);
            // 
            // variableTypesToolStripMenuItem
            // 
            this.variableTypesToolStripMenuItem.Name = "variableTypesToolStripMenuItem";
            this.variableTypesToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.variableTypesToolStripMenuItem.Text = "&Variable Types";
            this.variableTypesToolStripMenuItem.Click += new System.EventHandler(this.variableTypesToolStripMenuItem_Click);
            // 
            // unitTypeConfigToolStripMenuItem
            // 
            this.unitTypeConfigToolStripMenuItem.Name = "unitTypeConfigToolStripMenuItem";
            this.unitTypeConfigToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.unitTypeConfigToolStripMenuItem.Text = "&Unit Type Config";
            this.unitTypeConfigToolStripMenuItem.Click += new System.EventHandler(this.unitTypeConfigToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 651);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(850, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabs
            // 
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 24);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(850, 627);
            this.tabs.TabIndex = 2;
            // 
            // timerProcessClasses
            // 
            this.timerProcessClasses.Interval = 5;
            this.timerProcessClasses.Tick += new System.EventHandler(this.timerProcessClasses_Tick);
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // StatEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 673);
            this.Controls.Add(this.tabs);
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
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.Timer timerProcessClasses;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databasesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem variableTypesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyNamectrlcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyGuidctrlgToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unitTypeConfigToolStripMenuItem;
        private System.Windows.Forms.Timer updateTimer;
    }
}

