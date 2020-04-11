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
		private List<EnumList> m_EnumList = new List<EnumList>();
		private List<EnumList> m_EnumListForThreading = new List<EnumList>();
		private List<ClassStructure> m_ClassStructures = new List<ClassStructure>();
		private List<ClassStructure> m_EmptyListForThreading = new List<ClassStructure>();
		private Dictionary<string,string> m_DefinesList = new Dictionary<string, string>();
		private Dictionary<string, string> m_DefinesListForThreading = new Dictionary<string, string>();
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
		public List<EnumList> enumLists { get { return (getDoneParsing()) ? m_EnumList : m_EnumListForThreading; } }

		public Dictionary<string, string> defines { get { return (getDoneParsing()) ? m_DefinesList : m_DefinesListForThreading; } }

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
				if (mParser.parse(errorsParsing))
				{
					foreach (ClassStructure mStruct in mParser.classStructures)
					{
						m_ClassStructures.Add(mStruct);
					}
					foreach(EnumList mList in mParser.enumLists)
					{
						m_EnumList.Add(mList);
					}
					foreach (KeyValuePair<string,string> mData in mParser.defines)
					{
						m_DefinesList[mData.Key] = mData.Value;
					}
					
				}
			}
			m_bDoneParsing = true;
			m_Thread = null;
		}
		

	}
}
