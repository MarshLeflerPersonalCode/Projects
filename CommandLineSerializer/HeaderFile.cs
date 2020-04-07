using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.IO;
using Library.ClassParser;
namespace CommandLineSerializer
{
	[Serializable]
	public class HeaderFile
	{
		private LogFile m_LogFile = null;
		private SerializerController m_Controller = null;
		public void initialize(SerializerController mController, string strHeaderFile, string strExportedFile)
		{
			m_Controller = mController;
			headerFile = strHeaderFile;
			headerFileWriteTime = File.GetLastWriteTime(headerFile);
			exportedHeaderFile = strExportedFile;
			exportedHeaderFileWriteTime = File.GetLastWriteTime(exportedHeaderFile);
			if(exportedHeaderFileWriteTime.Year < 2000 )
			{
				exportedHeaderFileWriteTime = new DateTime(0);
			}
		}

		public string headerFile { get; set; }
		public DateTime headerFileWriteTime { get; set; }
		public string exportedHeaderFile { get; set; }
		public DateTime exportedHeaderFileWriteTime { get; set; }
		public LogFile getLogFile() { return m_LogFile; }
		public void setLogFile(LogFile mLogFile) { m_LogFile = mLogFile; }

		public bool getNeedsToProcessHeader()
		{
			if( exportedHeaderFileWriteTime == null ||
				exportedHeaderFileWriteTime < headerFileWriteTime )
			{
				return true;
			}
			return false;
		}

		public void processHeader()
		{
			
			string[] mLines = File.ReadAllLines(headerFile);
			bool bSerializedHeader = false;
			for(int iLineIndex = 0; iLineIndex < mLines.Length; iLineIndex++)
			{
				string strLine = mLines[iLineIndex];
				if( bSerializedHeader == false)
				{
					if(strLine.Trim().StartsWith("#include") &&
					   strLine.Contains("serialized.h"))
					{
						bSerializedHeader = true;
					}
					continue;
				}
			}

		}
	}
}
