using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace CommandLineSerializer
{
	[Serializable]
	public class HeaderFile
	{

		public void initialize(string strHeaderFile, string strExportedFile)
		{
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
		}
	}
}
