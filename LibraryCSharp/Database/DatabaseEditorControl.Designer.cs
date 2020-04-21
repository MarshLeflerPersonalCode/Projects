namespace Library.Database
{
    partial class DatabaseEditorControl
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
            this.m_StatListView = new System.Windows.Forms.ListView();
            this.NameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.GuidHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtBoxFilterListView = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.statObjectViewer = new CustomControls.ObjectViewer();
            this.timerForFilter = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
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
            this.splitContainer1.Size = new System.Drawing.Size(806, 620);
            this.splitContainer1.SplitterDistance = 252;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 1;
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
            this.m_StatListView.Size = new System.Drawing.Size(250, 568);
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
            this.panel1.Size = new System.Drawing.Size(250, 50);
            this.panel1.TabIndex = 0;
            // 
            // txtBoxFilterListView
            // 
            this.txtBoxFilterListView.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtBoxFilterListView.Location = new System.Drawing.Point(10, 23);
            this.txtBoxFilterListView.Name = "txtBoxFilterListView";
            this.txtBoxFilterListView.Size = new System.Drawing.Size(228, 20);
            this.txtBoxFilterListView.TabIndex = 7;
            this.txtBoxFilterListView.TextChanged += new System.EventHandler(this.txtBoxFilterListView_TextChanged);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(10, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(228, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Filter";
            // 
            // statObjectViewer
            // 
            this.statObjectViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statObjectViewer.Location = new System.Drawing.Point(0, 0);
            this.statObjectViewer.Name = "statObjectViewer";
            this.statObjectViewer.Size = new System.Drawing.Size(544, 618);
            this.statObjectViewer.TabIndex = 0;
            this.statObjectViewer.PropertyValueChanged += new CustomControls.ObjectViewer.PropertyValueChangedHandler(this.statObjectViewer_PropertyValueChanged);
            // 
            // timerForFilter
            // 
            this.timerForFilter.Tick += new System.EventHandler(this.timerForFilter_Tick);
            // 
            // DatabaseEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "DatabaseEditorControl";
            this.Size = new System.Drawing.Size(806, 620);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView m_StatListView;
        private System.Windows.Forms.ColumnHeader NameHeader;
        private System.Windows.Forms.ColumnHeader GuidHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtBoxFilterListView;
        private System.Windows.Forms.Label label3;
        private CustomControls.ObjectViewer statObjectViewer;
        private System.Windows.Forms.Timer timerForFilter;
    }
}
