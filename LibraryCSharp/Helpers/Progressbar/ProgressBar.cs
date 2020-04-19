using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Library
{
	public partial class ProgressBar : Form
	{
		private Stopwatch m_StopWatch = null;
		private Mutex m_Mutex = new Mutex();
		private double m_fEstimatedTime = 3.0;
		
		private string m_strMessageToShow = "";
		public ProgressBar()
		{
			InitializeComponent();
			m_LabelForDisplay.Text = "";
			m_StopWatch = Stopwatch.StartNew();
		}
        public double getTime() { return m_StopWatch.Elapsed.TotalSeconds; }
        public void setEstimated(double fEstimated)
		{
			m_fEstimatedTime = fEstimated;
		}

		public void setMessage(string strMessage)
		{
			m_Mutex.WaitOne();
			m_strMessageToShow = strMessage;
			m_Mutex.ReleaseMutex();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			m_Mutex.WaitOne();
			m_LabelForDisplay.Text = m_strMessageToShow;
			m_Mutex.ReleaseMutex();
			double fPercent = Math.Min(1.0, m_StopWatch.Elapsed.TotalSeconds / m_fEstimatedTime);
			m_ProgressBar.Value = Math.Max(0, (int)(fPercent * (double)m_ProgressBar.Maximum) );
		}
	}
}
