using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using Library.DataGroup;

namespace CommandLineSerializer
{
	[Serializable]
	public class SerializerConfigFile
	{
		
		public int m_iValue = 13;
		public SerializerConfigFile() 
		{
			//headerFiles = new List<HeaderFile>();
			//configFile = "CommandLineSerializer.cfg";

		}

		public void initialize(SerializerController mSerializerController, string strConfigFile)
		{
			headerFiles = new List<HeaderFile>();
			configFile = strConfigFile;
		}
		private SerializerController serializerController { get; }

		private string configFile { get; set; }
		public List<HeaderFile> headerFiles { get; set; }
	
		public void addHeaderFile(HeaderFile mFile)
		{
			if (headerFiles.Contains(mFile) == false)
			{
				headerFiles.Add(mFile);
			}
		}
		public void clearHeaderFiles()
		{
			headerFiles.Clear();
		}


		private void _log(string strLogMessage)
		{
			if(serializerController != null)
			{
				serializerController.log(strLogMessage);
			}
		}

		public bool save(string strDirectory)
		{
			string strFullPath = Path.Combine(strDirectory, configFile);
			DataGroup mDataGroup = new DataGroup(this);
			string strErrorMessage = mDataGroup.saveToFile(strFullPath);
			if(strErrorMessage == "")
			{
				_log("Saved serializer config file correctly. At location: " + strFullPath);
			}
			else
			{
				_log("ERROR - Unable to save serializer config file. Reason: " + strFullPath);
			}
			return true;
		}

		public bool getHeaderFileNeedsToRecompile(HeaderFile mHeaderFile)
		{
			if(mHeaderFile == null) { return false; }
			if(mHeaderFile.getNeedsToProcessHeader()) { return true; }
			foreach(HeaderFile mFile in headerFiles)
			{
				if( mFile.headerFile == mHeaderFile.headerFile)
				{
					return (mFile.headerFileWriteTime != mHeaderFile.headerFileWriteTime) ? true : false;
				}
			}
			return true;	//not found
		}
	}
}
