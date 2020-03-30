namespace UnitTypeCore.Forms
{
	partial class UnitTypeManagerConfigForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UnitTypeManagerConfigForm));
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new System.Windows.Forms.Panel();
			this.txtBoxHeader = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnAddEnum = new System.Windows.Forms.Button();
			this.toolTipHeader = new System.Windows.Forms.ToolTip(this.components);
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.btnFunctions = new System.Windows.Forms.Button();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			this.menuStrip1.Visible = false;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabPage2);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(800, 450);
			this.tabControl.TabIndex = 1;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.propertyGrid);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(792, 400);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Properties";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.txtBoxHeader);
			this.tabPage2.Controls.Add(this.panel1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(792, 424);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Header FIle";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point(3, 3);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(786, 394);
			this.propertyGrid.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Controls.Add(this.flowLayoutPanel1);
			this.panel1.Controls.Add(this.panel2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(186, 418);
			this.panel1.TabIndex = 0;
			// 
			// txtBoxHeader
			// 
			this.txtBoxHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtBoxHeader.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtBoxHeader.Location = new System.Drawing.Point(189, 3);
			this.txtBoxHeader.Multiline = true;
			this.txtBoxHeader.Name = "txtBoxHeader";
			this.txtBoxHeader.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtBoxHeader.Size = new System.Drawing.Size(600, 418);
			this.txtBoxHeader.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(184, 22);
			this.label1.TabIndex = 0;
			this.label1.Text = "TAGS";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel2
			// 
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel2.Location = new System.Drawing.Point(0, 22);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(184, 2);
			this.panel2.TabIndex = 1;
			// 
			// btnAddEnum
			// 
			this.btnAddEnum.Location = new System.Drawing.Point(3, 3);
			this.btnAddEnum.Name = "btnAddEnum";
			this.btnAddEnum.Size = new System.Drawing.Size(175, 23);
			this.btnAddEnum.TabIndex = 3;
			this.btnAddEnum.Text = "[ENUM]";
			this.toolTipHeader.SetToolTip(this.btnAddEnum, "This is where the code for the enum will appear\r\n\r\n");
			this.btnAddEnum.UseVisualStyleBackColor = true;
			this.btnAddEnum.Click += new System.EventHandler(this.btnAddEnum_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flowLayoutPanel1.Controls.Add(this.btnAddEnum);
			this.flowLayoutPanel1.Controls.Add(this.btnFunctions);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 24);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(184, 392);
			this.flowLayoutPanel1.TabIndex = 4;
			// 
			// btnFunctions
			// 
			this.btnFunctions.Location = new System.Drawing.Point(3, 32);
			this.btnFunctions.Name = "btnFunctions";
			this.btnFunctions.Size = new System.Drawing.Size(175, 23);
			this.btnFunctions.TabIndex = 4;
			this.btnFunctions.Text = "[FUNCTIONS]";
			this.toolTipHeader.SetToolTip(this.btnFunctions, "These are all the functions that will use the enum. It\'s a different tag in case " +
        "you want to put the helper functions in a namespace.");
			this.btnFunctions.UseVisualStyleBackColor = true;
			this.btnFunctions.Click += new System.EventHandler(this.btnFunctions_Click);
			// 
			// UnitTypeManagerConfigForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.menuStrip1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "UnitTypeManagerConfigForm";
			this.ShowInTaskbar = false;
			this.Text = "Unit Type Editor Config ";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UnitTypeManagerConfigForm_FormClosing);
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox txtBoxHeader;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Button btnAddEnum;
		private System.Windows.Forms.ToolTip toolTipHeader;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnFunctions;
	}
}