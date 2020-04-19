namespace CustomControls
{
	partial class ObjectViewer
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ListViewSplitter = new System.Windows.Forms.SplitContainer();
            this.objectListView = new System.Windows.Forms.ListView();
            this.columnArrayHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnObjectType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.arrayListView = new System.Windows.Forms.ListView();
            this.columnArrayIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnArrayType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnArrayCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnNewArrayItem = new System.Windows.Forms.Button();
            this.btnDeleteArrayItem = new System.Windows.Forms.Button();
            this.btnDownArrayItem = new System.Windows.Forms.Button();
            this.btnUpArrayItem = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.objectPropertyPanel = new System.Windows.Forms.Panel();
            this.panelPropertyGrid = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtBoxFilterPropertyGrid = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.objectFilteredPropertyGrid = new CustomControls.FilteredPropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ListViewSplitter)).BeginInit();
            this.ListViewSplitter.Panel1.SuspendLayout();
            this.ListViewSplitter.Panel2.SuspendLayout();
            this.ListViewSplitter.SuspendLayout();
            this.panel1.SuspendLayout();
            this.objectPropertyPanel.SuspendLayout();
            this.panelPropertyGrid.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ListViewSplitter);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.objectPropertyPanel);
            this.splitContainer1.Size = new System.Drawing.Size(800, 700);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 0;
            // 
            // ListViewSplitter
            // 
            this.ListViewSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ListViewSplitter.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.ListViewSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ListViewSplitter.Location = new System.Drawing.Point(0, 0);
            this.ListViewSplitter.Name = "ListViewSplitter";
            this.ListViewSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ListViewSplitter.Panel1
            // 
            this.ListViewSplitter.Panel1.Controls.Add(this.objectListView);
            this.ListViewSplitter.Panel1.Controls.Add(this.label1);
            // 
            // ListViewSplitter.Panel2
            // 
            this.ListViewSplitter.Panel2.Controls.Add(this.arrayListView);
            this.ListViewSplitter.Panel2.Controls.Add(this.panel1);
            this.ListViewSplitter.Panel2.Controls.Add(this.label2);
            this.ListViewSplitter.Panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.ListViewSplitter.Size = new System.Drawing.Size(350, 700);
            this.ListViewSplitter.SplitterDistance = 266;
            this.ListViewSplitter.SplitterWidth = 8;
            this.ListViewSplitter.TabIndex = 1;
            // 
            // objectListView
            // 
            this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnArrayHeader,
            this.columnName,
            this.columnObjectType});
            this.objectListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.objectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectListView.FullRowSelect = true;
            this.objectListView.GridLines = true;
            this.objectListView.HideSelection = false;
            this.objectListView.Location = new System.Drawing.Point(0, 20);
            this.objectListView.MultiSelect = false;
            this.objectListView.Name = "objectListView";
            this.objectListView.Size = new System.Drawing.Size(348, 244);
            this.objectListView.TabIndex = 2;
            this.objectListView.UseCompatibleStateImageBehavior = false;
            this.objectListView.View = System.Windows.Forms.View.Details;
            this.objectListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.objectListView_ItemSelectionChanged);
            // 
            // columnArrayHeader
            // 
            this.columnArrayHeader.Text = "*";
            this.columnArrayHeader.Width = 25;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 200;
            // 
            // columnObjectType
            // 
            this.columnObjectType.Text = "Type";
            this.columnObjectType.Width = 150;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "OBJECTS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // arrayListView
            // 
            this.arrayListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnArrayIndex,
            this.columnArrayType,
            this.columnArrayCount});
            this.arrayListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.arrayListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arrayListView.FullRowSelect = true;
            this.arrayListView.GridLines = true;
            this.arrayListView.HideSelection = false;
            this.arrayListView.Location = new System.Drawing.Point(0, 43);
            this.arrayListView.Name = "arrayListView";
            this.arrayListView.Size = new System.Drawing.Size(348, 381);
            this.arrayListView.TabIndex = 3;
            this.arrayListView.UseCompatibleStateImageBehavior = false;
            this.arrayListView.View = System.Windows.Forms.View.Details;
            this.arrayListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.arrayListView_ItemSelectionChanged);
            // 
            // columnArrayIndex
            // 
            this.columnArrayIndex.Text = "Index";
            this.columnArrayIndex.Width = 45;
            // 
            // columnArrayType
            // 
            this.columnArrayType.Text = "Type";
            this.columnArrayType.Width = 200;
            // 
            // columnArrayCount
            // 
            this.columnArrayCount.Text = "Count";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnNewArrayItem);
            this.panel1.Controls.Add(this.btnDeleteArrayItem);
            this.panel1.Controls.Add(this.btnDownArrayItem);
            this.panel1.Controls.Add(this.btnUpArrayItem);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(348, 23);
            this.panel1.TabIndex = 4;
            // 
            // btnNewArrayItem
            // 
            this.btnNewArrayItem.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNewArrayItem.Location = new System.Drawing.Point(84, 3);
            this.btnNewArrayItem.Name = "btnNewArrayItem";
            this.btnNewArrayItem.Size = new System.Drawing.Size(70, 17);
            this.btnNewArrayItem.TabIndex = 3;
            this.btnNewArrayItem.Text = "new";
            this.toolTip1.SetToolTip(this.btnNewArrayItem, "Add new item to array");
            this.btnNewArrayItem.UseVisualStyleBackColor = true;
            this.btnNewArrayItem.Click += new System.EventHandler(this.btnNewArrayItem_Click);
            // 
            // btnDeleteArrayItem
            // 
            this.btnDeleteArrayItem.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDeleteArrayItem.Location = new System.Drawing.Point(3, 3);
            this.btnDeleteArrayItem.Name = "btnDeleteArrayItem";
            this.btnDeleteArrayItem.Size = new System.Drawing.Size(70, 17);
            this.btnDeleteArrayItem.TabIndex = 2;
            this.btnDeleteArrayItem.Text = "delete";
            this.toolTip1.SetToolTip(this.btnDeleteArrayItem, "Delete selected item in array");
            this.btnDeleteArrayItem.UseVisualStyleBackColor = true;
            this.btnDeleteArrayItem.Click += new System.EventHandler(this.btnDeleteArrayItem_Click);
            // 
            // btnDownArrayItem
            // 
            this.btnDownArrayItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDownArrayItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownArrayItem.Image = global::CustomControls.Properties.Resources.DownArrow16x16;
            this.btnDownArrayItem.Location = new System.Drawing.Point(319, 3);
            this.btnDownArrayItem.Name = "btnDownArrayItem";
            this.btnDownArrayItem.Size = new System.Drawing.Size(17, 17);
            this.btnDownArrayItem.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnDownArrayItem, "Move an selected item in array down");
            this.btnDownArrayItem.UseVisualStyleBackColor = true;
            this.btnDownArrayItem.Click += new System.EventHandler(this.btnDownArrayItem_Click);
            // 
            // btnUpArrayItem
            // 
            this.btnUpArrayItem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpArrayItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpArrayItem.Image = global::CustomControls.Properties.Resources.UpArrow16x16;
            this.btnUpArrayItem.Location = new System.Drawing.Point(295, 3);
            this.btnUpArrayItem.Name = "btnUpArrayItem";
            this.btnUpArrayItem.Size = new System.Drawing.Size(17, 17);
            this.btnUpArrayItem.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnUpArrayItem, "Move selected item up in array");
            this.btnUpArrayItem.UseVisualStyleBackColor = true;
            this.btnUpArrayItem.Click += new System.EventHandler(this.btnUpArrayItem_Click);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(348, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "ARRAY ITEMS";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // objectPropertyPanel
            // 
            this.objectPropertyPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.objectPropertyPanel.Controls.Add(this.panelPropertyGrid);
            this.objectPropertyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectPropertyPanel.Location = new System.Drawing.Point(0, 0);
            this.objectPropertyPanel.Name = "objectPropertyPanel";
            this.objectPropertyPanel.Size = new System.Drawing.Size(440, 698);
            this.objectPropertyPanel.TabIndex = 1;
            // 
            // panelPropertyGrid
            // 
            this.panelPropertyGrid.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panelPropertyGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPropertyGrid.Controls.Add(this.objectFilteredPropertyGrid);
            this.panelPropertyGrid.Controls.Add(this.panel2);
            this.panelPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.panelPropertyGrid.Name = "panelPropertyGrid";
            this.panelPropertyGrid.Size = new System.Drawing.Size(438, 696);
            this.panelPropertyGrid.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txtBoxFilterPropertyGrid);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10);
            this.panel2.Size = new System.Drawing.Size(436, 50);
            this.panel2.TabIndex = 3;
            // 
            // txtBoxFilterPropertyGrid
            // 
            this.txtBoxFilterPropertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtBoxFilterPropertyGrid.Location = new System.Drawing.Point(10, 23);
            this.txtBoxFilterPropertyGrid.Name = "txtBoxFilterPropertyGrid";
            this.txtBoxFilterPropertyGrid.Size = new System.Drawing.Size(414, 20);
            this.txtBoxFilterPropertyGrid.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(414, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Filter";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 10;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // objectFilteredPropertyGrid
            // 
            this.objectFilteredPropertyGrid.BrowsableProperties = null;
            this.objectFilteredPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectFilteredPropertyGrid.FilterIsCaseSensitive = false;
            this.objectFilteredPropertyGrid.FilterString = "";
            this.objectFilteredPropertyGrid.HiddenAttributes = null;
            this.objectFilteredPropertyGrid.HiddenProperties = null;
            this.objectFilteredPropertyGrid.Location = new System.Drawing.Point(0, 50);
            this.objectFilteredPropertyGrid.Name = "objectFilteredPropertyGrid";
            this.objectFilteredPropertyGrid.showArrays = true;
            this.objectFilteredPropertyGrid.showEnumerators = true;
            this.objectFilteredPropertyGrid.showObjects = true;
            this.objectFilteredPropertyGrid.Size = new System.Drawing.Size(436, 644);
            this.objectFilteredPropertyGrid.TabIndex = 2;
            this.objectFilteredPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.objectFilteredPropertyGrid_PropertyValueChanged);
            // 
            // ObjectViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ObjectViewer";
            this.Size = new System.Drawing.Size(800, 700);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ListViewSplitter.Panel1.ResumeLayout(false);
            this.ListViewSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ListViewSplitter)).EndInit();
            this.ListViewSplitter.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.objectPropertyPanel.ResumeLayout(false);
            this.panelPropertyGrid.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer ListViewSplitter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private FilteredPropertyGrid objectFilteredPropertyGrid;
		private System.Windows.Forms.Panel objectPropertyPanel;
		private System.Windows.Forms.ListView objectListView;
		private System.Windows.Forms.ColumnHeader columnName;
		private System.Windows.Forms.ColumnHeader columnObjectType;
		private System.Windows.Forms.ListView arrayListView;
		private System.Windows.Forms.ColumnHeader columnArrayType;
		private System.Windows.Forms.Panel panelPropertyGrid;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox txtBoxFilterPropertyGrid;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Timer timerUpdate;
		private System.Windows.Forms.ColumnHeader columnArrayHeader;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btnUpArrayItem;
		private System.Windows.Forms.Button btnNewArrayItem;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnDeleteArrayItem;
		private System.Windows.Forms.Button btnDownArrayItem;
		private System.Windows.Forms.ColumnHeader columnArrayIndex;
		private System.Windows.Forms.ColumnHeader columnArrayCount;
	}
}
