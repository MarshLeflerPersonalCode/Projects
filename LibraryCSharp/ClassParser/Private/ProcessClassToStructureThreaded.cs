using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Library.IO;
namespace Library.ClassParser.Private
{
	public class ProcessClassToStructureThreaded
	{
		private static int m_iThreadsCreated = 0;
		private bool m_bDoneParsing = false;
		private bool m_bParsedHeadersCalled = false;
		private Thread m_Thread = null;
		private List<ClassStructure> m_ClassStructures = new List<ClassStructure>();
		private List<ClassStructure> m_EmptyListForThreading = new List<ClassStructure>();
		public ProcessClassToStructureThreaded()
		{
			errorsParsing = new List<string>();
			headersToParse = new List<string>();
			
		}
		public bool getDoneParsing() { return m_bParsedHeadersCalled && m_bDoneParsing && (m_Thread == null)?true:false; }
		public LogFile logFile { get; set; }
		public List<string> errorsParsing { get;set;}
		public List<string> headersToParse { get; set; }
		public List<ClassStructure> classStructures { get { return (getDoneParsing()) ? m_ClassStructures : m_EmptyListForThreading; } }


		private void log(string strMessage )
		{
			if(logFile != null)
			{
				logFile.log(strMessage);
			}
		}
		
		public void parse()
		{
			m_bParsedHeadersCalled = true;
			if(headersToParse.Count == 0 )
			{
				m_bDoneParsing = true;
			}
			try
			{
				m_Thread = new Thread(new ThreadStart(_processHeaders));
				m_Thread.Name = String.Format("ProcessingHeadersThread{0}", m_iThreadsCreated++);
				m_Thread.Start();
			}
			catch
			{
				m_Thread = null;
				m_bDoneParsing = true;
			}
		}

		private void _processHeaders()
		{
			foreach(string strHeaderToParse in headersToParse)
			{
				ClassParser mParser = new ClassParser(strHeaderToParse);
				mParser.logFile = logFile;
				ClassStructure mStructure = mParser.parse( errorsParsing);
				m_ClassStructures.Add(mStructure);
			}
			m_bDoneParsing = true;
			m_Thread = null;
		}
		

	}
}
