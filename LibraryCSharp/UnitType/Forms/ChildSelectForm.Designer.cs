namespace Library.UnitType.Forms
{
	partial class ChildSelectForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChildSelectForm));
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtBoxFilter = new System.Windows.Forms.TextBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnOkay = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lstBoxUnitTypes = new System.Windows.Forms.ListBox();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Margin = new System.Windows.Forms.Padding(5);
			this.panel1.Name = "panel1";
			this.panel1.Padding = new System.Windows.Forms.Padding(5);
			this.panel1.Size = new System.Drawing.Size(311, 68);
			this.panel1.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtBoxFilter);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(5, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(301, 47);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter";
			// 
			// txtBoxFilter
			// 
			this.txtBoxFilter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtBoxFilter.Location = new System.Drawing.Point(3, 16);
			this.txtBoxFilter.Name = "txtBoxFilter";
			this.txtBoxFilter.Size = new System.Drawing.Size(295, 20);
			this.txtBoxFilter.TabIndex = 0;
			this.txtBoxFilter.TextChanged += new System.EventHandler(this.txtBoxFilter_TextChanged);
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.Control;
			this.panel2.Controls.Add(this.btnCancel);
			this.panel2.Controls.Add(this.btnOkay);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 513);
			this.panel2.Margin = new System.Windows.Forms.Padding(5);
			this.panel2.Name = "panel2";
			this.panel2.Padding = new System.Windows.Forms.Padding(5);
			this.panel2.Size = new System.Drawing.Size(311, 36);
			this.panel2.TabIndex = 2;
			// 
			// btnOkay
			// 
			this.btnOkay.Enabled = false;
			this.btnOkay.Location = new System.Drawing.Point(199, 8);
			this.btnOkay.Name = "btnOkay";
			this.btnOkay.Size = new System.Drawing.Size(104, 23);
			this.btnOkay.TabIndex = 0;
			this.btnOkay.Text = "OK";
			this.btnOkay.UseVisualStyleBackColor = true;
			this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(8, 8);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(104, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "CANCEL";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// lstBoxUnitTypes
			// 
			this.lstBoxUnitTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstBoxUnitTypes.FormattingEnabled = true;
			this.lstBoxUnitTypes.Location = new System.Drawing.Point(0, 68);
			this.lstBoxUnitTypes.Name = "lstBoxUnitTypes";
			this.lstBoxUnitTypes.Size = new System.Drawing.Size(311, 445);
			this.lstBoxUnitTypes.TabIndex = 3;
			this.lstBoxUnitTypes.SelectedIndexChanged += new System.EventHandler(this.lstBoxUnitTypes_SelectedIndexChanged);
			// 
			// ChildSelectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(311, 549);
			this.Controls.Add(this.lstBoxUnitTypes);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChildSelectForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Choose Children";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChildSelectForm_FormClosing);
			this.Shown += new System.EventHandler(this.ChildSelectForm_Shown);
			this.panel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox txtBoxFilter;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOkay;
		private System.Windows.Forms.ListBox lstBoxUnitTypes;
	}
}