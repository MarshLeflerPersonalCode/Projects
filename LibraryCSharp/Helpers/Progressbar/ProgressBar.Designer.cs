namespace Library
{
    partial class ProgressBar
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.m_LabelForDisplay = new System.Windows.Forms.Label();
            this.m_ProgressBar = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.m_LabelForDisplay);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(489, 20);
            this.panel1.TabIndex = 0;
            // 
            // m_LabelForDisplay
            // 
            this.m_LabelForDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_LabelForDisplay.Location = new System.Drawing.Point(0, 0);
            this.m_LabelForDisplay.Name = "m_LabelForDisplay";
            this.m_LabelForDisplay.Size = new System.Drawing.Size(489, 20);
            this.m_LabelForDisplay.TabIndex = 0;
            this.m_LabelForDisplay.Text = "label1";
            // 
            // m_ProgressBar
            // 
            this.m_ProgressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_ProgressBar.Location = new System.Drawing.Point(0, 20);
            this.m_ProgressBar.Maximum = 1000;
            this.m_ProgressBar.Name = "m_ProgressBar";
            this.m_ProgressBar.Size = new System.Drawing.Size(489, 40);
            this.m_ProgressBar.Step = 1;
            this.m_ProgressBar.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 61);
            this.ControlBox = false;
            this.Controls.Add(this.m_ProgressBar);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressBar";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label m_LabelForDisplay;
        private System.Windows.Forms.ProgressBar m_ProgressBar;
        private System.Windows.Forms.Timer timer1;
    }
}