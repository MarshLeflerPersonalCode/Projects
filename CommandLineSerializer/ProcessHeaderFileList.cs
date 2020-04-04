using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineSerializer
{
	public class ProcessHeaderFileList
	{
		private List<HeaderFile> m_HeadersToProcess = new List<HeaderFile>();
		private List<HeaderFile> m_HeadersProcessed = new List<HeaderFile>();
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

	}
}
