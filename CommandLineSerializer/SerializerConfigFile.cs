using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using Library;
using Library.Utilities;
using Library.IO;


namespace CommandLineSerializer
{
    [Serializable]
	public class SerializerConfigFile
	{



		//[System.Xml.Serialization.XmlIgnore]
		
        public SerializerConfigFile() 
		{
        
		}

		
		public void initialize(SerializerController mSerializerController, string strConfigFile)
		{
			headerFiles = new List<HeaderFile>();            
            configFile = strConfigFile;
			serializerController = mSerializerController;
        

        }
		private SerializerController serializerController { get; set; }

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
            string strErrorMessage = "";
            try
            {
                if(SerializationToJson.serializeObjectToFile(this, strFullPath, ref strErrorMessage) == false )
                {
                    _log("ERROR - in serializing " + this.GetType().Name + " into json. Error was: " + Environment.NewLine + strErrorMessage);
                }
            }
            catch(Exception e)
            {
                _log("ERROR - in serializing " + strDirectory + " into json. Error was: " + e.Message);
            }
			return true;
		}

        public static SerializerConfigFile createConfigFile(string strDirectory, ref string strErrors)
        {
            Type mType = Type.GetType("CommandLineSerializer.SerializerConfigFile");
            try
            {

                strErrors = "";
                SerializerConfigFile mConfigFile = SerializationToJson.deserializeObjectFromFile(mType, strDirectory, ref strErrors) as SerializerConfigFile;
                if( mConfigFile == null )
                {
                    strErrors = "ERROR - in deserializing SerializerConfigFile into json. Error was: " + Environment.NewLine + strErrors;
                    return null;
                }
                return mConfigFile;
            }
            catch(Exception e)
            {
                strErrors = "ERROR - in deserializing SerializerConfigFile into json. Error was: " + Environment.NewLine + e.Message;
            }
            return null;
                
            
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
