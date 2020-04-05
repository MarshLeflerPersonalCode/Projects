using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace CommandLineSerializer
{
	public class ProcessHeaderFileList
	{
		private static int m_iThreadsCreated = 0;
		private bool m_bDoneProcessingHeaders = false;
		private List<HeaderFile> m_HeadersToProcess = new List<HeaderFile>();
		private List<HeaderFile> m_HeadersProcessed = new List<HeaderFile>();
		private Thread m_Thread = null;		
		public ProcessHeaderFileList()
		{

		}
		public void addHeaderFileToProcess(HeaderFile mHeader)
		{
			m_HeadersToProcess.Add(mHeader);
		}

		public int getNumberOfHeadersRemainingToProcess()
		{
			return m_HeadersToProcess.Count;
		}

		//returns if it's done processing the headers
		public bool isDoneProcessingHeaders { get { return m_bDoneProcessingHeaders && (m_Thread == null); } }

		//returns the headers processed
		public List<HeaderFile> getHeadersProcessed() { return m_HeadersProcessed; }

		public void startProcessingHeaders()
		{
			if( m_HeadersToProcess.Count == 0 )
			{
				m_bDoneProcessingHeaders = true;
				return;
			}
			try
			{
				m_Thread = new Thread(new ThreadStart(_processHeaders));
				m_Thread.Name = String.Format("Thread{0}", m_iThreadsCreated++);			
				m_Thread.Start();
			}
			catch
			{
				m_Thread = null;
			}
		}

		private void _processHeaders()
		{
			while(m_HeadersToProcess.Count() > 0 )
			{
				HeaderFile mHeader = m_HeadersToProcess[m_HeadersToProcess.Count - 1];
				m_HeadersToProcess.RemoveAt(m_HeadersToProcess.Count - 1);
				mHeader.processHeader();
				m_HeadersProcessed.Add(mHeader);
			}
			m_bDoneProcessingHeaders = true;
			m_Thread = null;
		}

	}
}
