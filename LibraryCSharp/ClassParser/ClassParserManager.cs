using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.ClassParser.Private;
using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using Library.IO;
using System.Timers;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Library.ClassParser
{
	//this class parses c++ header files and creates ClassDefinitions. This can be slow so it can also save the time stamps compared.
	//Steps 
	//1) add all the header files you want parsed.
	//2) call compareToCachedData(string strConfigFile)
	//3) calls buildClassStructures()
	//4) call getDoneParsingClasses()
	//4) call getClassBuildErrors()
	//5) save parsed data
	public class ClassParserManager
	{
		private Stopwatch m_StopWatch = null;
		private static string g_strCachedClassStructs = ".structs";
		private bool m_bProcessed = false;
		private ClassParserConfig m_Config = new ClassParserConfig();
		private ProjectWrapper m_ProjectWrapper = new ProjectWrapper();
		private List<string> m_FilesToUpdate = new List<string>();
		private List<ProcessClassToStructureThreaded> m_ClassStructuresProcessing = new List<ProcessClassToStructureThreaded>();
		
		public ClassParserManager()
		{
			threadsToUse = 4;
		}

		public LogFile logFile { get; set; }

		private void log(string strLog)
		{
			if(logFile != null )
			{
				logFile.log(strLog);
				return;
			}
			Console.WriteLine(strLog);
		}
		public int threadsToUse { get; set; }

		public bool addFileToParse(string strHeaderFile)
		{
			return m_Config.addFile(strHeaderFile);
		}

        public int getNumberOfFilesParsed() { return m_Config.files.Count; }

		public ProjectWrapper getProjectWrapper() { return m_ProjectWrapper; }

		

		

		public List<string> filesNeedingUpdate { get { return m_FilesToUpdate; } }

		//returns true if there are any files needing to be updated. Pass in empty string or null to ignore cached data and rebuild everything
		public bool compareToCachedData(string strPathAndFile)
		{
			m_FilesToUpdate.Clear();
			ClassParserConfig mOldClassParserConfig = new ClassParserConfig();
			if( strPathAndFile == null ||
				strPathAndFile == "" ||
				mOldClassParserConfig.load(strPathAndFile) == false )
			{
				log("No cached data specified or found. Doing full recompile of class structures.");
				foreach (FileStamp mFile in m_Config.files)
				{
					m_FilesToUpdate.Add(mFile.file);
				}
				return (filesNeedingUpdate.Count > 0 )?true:false;
			}
			loadClassStructs(strPathAndFile + g_strCachedClassStructs);
			ClassParserConfig mFilesNeedingUpdate = m_Config.getNewerFiles(mOldClassParserConfig);
			foreach (FileStamp mFile in mFilesNeedingUpdate.files)
			{
                if( mFile.file.Contains("EnumsByName"))
                {
                    continue;
                }
				m_FilesToUpdate.Add(mFile.file);
				m_ProjectWrapper.removeAllInsideFile(mFile.file);
			}
			if( m_FilesToUpdate.Count == 0 )
			{
				log("No class structures to update because files are latest parsed.");
			}
			else
			{
				log( filesNeedingUpdate.Count.ToString() +  " files to re-parse for class structures.");
			}
			return (filesNeedingUpdate.Count > 0) ? true : false;
		}

		public void buildClassStructures()
		{
			if( filesNeedingUpdate.Count == 0)
			{
				return;
			}
			int iThreadCount = Math.Max(1, threadsToUse);
			for(int iThreadIndex = 0; iThreadIndex < iThreadCount; iThreadIndex++)
			{
				m_ClassStructuresProcessing.Add(new ProcessClassToStructureThreaded());
			}
			int iThreadToUse = 0;
			log("Processing " + filesNeedingUpdate.Count.ToString() + " header files on " + iThreadCount.ToString() + " threads.");
			foreach (string strFile in filesNeedingUpdate)
			{
				m_ClassStructuresProcessing[iThreadToUse].headersToParse.Add(strFile);

				iThreadToUse++;
				iThreadToUse = (iThreadToUse >= iThreadCount) ? 0 : iThreadToUse;
			}

			m_StopWatch = Stopwatch.StartNew();
			foreach( ProcessClassToStructureThreaded mProcess in m_ClassStructuresProcessing)
			{
				mProcess.parse();
			}
		}

		public bool getDoneParsingClassesOnThreads()
		{
			if (filesNeedingUpdate.Count == 0)
			{
				return true;
			}
			if( m_ClassStructuresProcessing.Count == 0 )
			{
				return false; //haven't called buildClassStructures
			}
			foreach( ProcessClassToStructureThreaded mProcesser in m_ClassStructuresProcessing)
			{
				if( mProcesser.getDoneParsing() == false)
				{
					return false;
				}
			}
			if(m_bProcessed == false )
			{
				_processClassStructures();
			}
			return true;
		}

		private void _processClassStructures()
		{
			if (m_bProcessed == false)
			{
				log("Processed " + m_ClassStructuresProcessing.Count.ToString() + " classes/struts. Took " + m_StopWatch.Elapsed.TotalSeconds.ToString() + " seconds.");
				m_StopWatch.Stop();
				m_bProcessed = true;
                int iEnumUniqueID = 0;
				foreach (ProcessClassToStructureThreaded mProcesser in m_ClassStructuresProcessing)
				{
					foreach (ClassStructure mStructure in mProcesser.classStructures)
					{
						m_ProjectWrapper.addClassStructure(mStructure);

					}

					foreach (EnumList mEnumList in mProcesser.enumLists)
					{
                        mEnumList.uniqueID = iEnumUniqueID;
                        iEnumUniqueID++;
                        m_ProjectWrapper.addEnumList(mEnumList);

					}
					foreach (KeyValuePair<string,string> mDefine in mProcesser.defines)
					{
						m_ProjectWrapper.addDefine(mDefine.Key, mDefine.Value);

					}
				}				

			}
		}

		public void getErrors(List<string> mErrors)
		{		
			
			if(getDoneParsingClassesOnThreads() == false )
			{
				mErrors.Add("Not done parsing yet.");
				return;
			}
			foreach (ProcessClassToStructureThreaded mProcesser in m_ClassStructuresProcessing)
			{
				foreach (string strError in mProcesser.errorsParsing )
				{
					mErrors.Add(strError);
				}
			}
			return;
		}

		public bool saveCachedData(string strPathAndFile)
		{
			if( m_Config.save(strPathAndFile) &&
				saveClassStructs( strPathAndFile + g_strCachedClassStructs) )
			{
				log("Saved Class Parser Manager config at: " + strPathAndFile);
				log("Saved Class Structures at: " + strPathAndFile + g_strCachedClassStructs);
				return true;
			}
			log("ERROR - Failed to save Class Parser Manager config at: " + strPathAndFile);
			log("ERROR - Failed to Save Class Structures at: " + strPathAndFile + g_strCachedClassStructs);

			return false;
		}
		private bool loadClassStructs(string strPathAndFile)
		{
			try
			{
				
				
				if (File.Exists(strPathAndFile) == false)
				{
					log("ERROR - No Class Parser Manager config found at: " + strPathAndFile);
					return false; //nothing to load
				}
				Type mType = m_ProjectWrapper.GetType();
				try
				{
					object mRawObject = JsonConvert.DeserializeObject(File.ReadAllText(strPathAndFile), mType);
					if (mRawObject == null)
					{
						log("ERROR - unable to parse struct file: " + strPathAndFile);
						return false;
					}
					m_ProjectWrapper = (ProjectWrapper)mRawObject;

				}
				catch (Exception e)
				{
					log("ERROR - unable to parse file: " + strPathAndFile + Environment.NewLine + "Error is: " + e.Message + ((e.InnerException != null)?Environment.NewLine + e.InnerException.Message:"") );
					return false;
				}
				
				if (m_ProjectWrapper.classStructures.Count == 0 &&
					m_ProjectWrapper.enums.Count == 0 )
				{
					log("ERROR - No Class Structures or Enums found in file: " + strPathAndFile + strPathAndFile);
					return false;
				}
				log("Loaded " + m_ProjectWrapper.classStructures.Count.ToString() + "class structures and " + m_ProjectWrapper.enums.Count.ToString() + " enums from file: " + strPathAndFile);
				return true;
			}
			catch (Exception e)
			{
				if (e.InnerException != null)
				{
					log("ERROR - " + e.Message + Environment.NewLine + e.InnerException.Message);
				}
				else
				{
					log("ERROR - " + e.Message);
				}
				return false;
			}
		}

		private bool saveClassStructs(string strPathAndFile)
		{
			try
			{
				File.Delete(strPathAndFile);
				if( m_ProjectWrapper.classStructures.Count == 0 )
				{
					log("ERROR - No Class Structures to save.");
					return true; //nothing to save
				}
				
				string strValue = JsonConvert.SerializeObject(m_ProjectWrapper, Newtonsoft.Json.Formatting.Indented);							
				if( strValue == null ||
					strValue == "" )
				{
					log("ERROR - Unable to parse Class Structures into json.");
					return false;
				}
				File.WriteAllText(strPathAndFile, strValue);
				if( File.Exists(strPathAndFile) == false)
				{
					log("ERROR - No Class Structures to save.");
					return false;
				}
				return true;
			}
			catch (Exception e)
			{
				if (e.InnerException != null)
				{
					log( "ERROR - " + e.Message + Environment.NewLine + e.InnerException.Message );
				}
				else
				{
					log("ERROR - " + e.Message);
				}
				return false;
			}
		}
	}
}
