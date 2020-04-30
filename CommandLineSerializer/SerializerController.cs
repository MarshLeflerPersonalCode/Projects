using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.CommandLine;
using Library.IO;
using Library;
using System.Threading;
using System.Timers;
using Library.ClassParser;
using System.Diagnostics;

namespace CommandLineSerializer
{
	public enum ESERIALIZE_DATA_TYPE
	{
		NUMBER,
		STRING,
		COUNT
	};
	public class SerializerController
	{
		private LogFile m_LogFile = null;
		private List<HeaderFile> m_HeaderFiles = new List<HeaderFile>();
		private List<ProcessHeaderFileList> m_Processes = new List<ProcessHeaderFileList>();
		private SerializerConfigFile m_ConfigFile = new SerializerConfigFile();
		private ClassParserManager m_ClassParserManager = new ClassParserManager();
		private Stopwatch m_StopWatch = null;
		private Dictionary<string, ESERIALIZE_DATA_TYPE> m_TypeDefs = new Dictionary<string, ESERIALIZE_DATA_TYPE>();
		public SerializerController(string[] args)
		{
			m_StopWatch = Stopwatch.StartNew();
			_createCommandLineArguments(args);
			_createLogFile();
			_attemptToConfigureTypeDefs();

			if (_createIntermediateDirectory() == false)
			{
				_showTimeExecuting();
				log("Closing down");
				Environment.Exit(0);
				return;
			}
			if( _runClean())
			{
				_showTimeExecuting();
				log("Closing down");
				Environment.Exit(0);
				return;

			}
			_loadConfigFile();
			if (_findHeaders() == false)
			{
				log("ERROR - Closing down because unable to find headers");
				_showTimeExecuting();
				return;
			}
			_buildClassStructures();
			if(_waitForClassStructuresToParse() == false )
			{
				log("ERROR - Closing down because there were errors parsing the classes/structs.");
				_showTimeExecuting();
				return;
			}
			_startProcessingHeaders();
			_waitForHeadersToDefineObjects();
			if( _checkForErrors())
			{

			}
			_waitForHeadersToWriteHeaders();
			_waitForHeadersToProcess();
            _writeEnumFile();
			_saveConfigFile();
			m_LogFile.flushLog();
			_showTimeExecuting();

		}

		//creates the command line arguments
		private void _createCommandLineArguments(string[] args)
		{
			commandLineArguments = new CommandLineArguments();
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Help", "-?" }, "Prints out all the commands"));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Pause", "-P" }, "Pauses at the end waiting for a key stroke"));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Debug", "-D" }, "Shows all the values of the commands processed"));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-DisplayOff", "-DO" }, "Will make it so that logging info won't be printed to the screen."));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-LogFile", "-LF" }, "The log file. Must include the file name.", ""));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-ForceRecompile", "-FR" }, "Forces all headers to recompile."));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-SourceDir", "-SD" }, "the directory where the header files will be parsed", ""));
            string strIntermediatePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strIntermediatePath = strIntermediatePath.Substring(0, strIntermediatePath.Length - Path.GetFileName(strIntermediatePath).Length);
            strIntermediatePath = Path.Combine(strIntermediatePath, "Intermediate");

            commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-IntermediateDir", "-ID" }, "the intermediate directory where the source files will be generated. The project must point there.", strIntermediatePath));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-MaxThreads", "-MT" }, "Max threads allowed to process headers. Uses Max - 1", Environment.ProcessorCount - 1));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Clean", "-c" }, "Cleans all the configs for a full recompile and then exits."));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-Verbose", "-V" }, "Writes all the details to the log."));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-TypeDefs", "-TD" }, "Type Definitions need to be passed in as a string,  with each value specifying if its a number or string. e.g. 'KCString=string, KCName=string, KCGuid=number'", ""));
			commandLineArguments.addCommandLineOption(new CommandLineOption(new string[] { "-LoadTypeDef", "-LTD" }, "Specifies a specific file to load for all the type defs. Each line should define a new type with an equal specifying if it's a number or string: e.g. KCString=string, KCGuid=number", ""));
			commandLineArguments.parseArguments(args);
			if (commandLineArguments.getCommandValueAsString("-LogFile") == "")
			{
				commandLineArguments.setCommand("-LogFile", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CommandLineSerializer.log"));
			}

			if (commandLineArguments.getCommandValueAsBool("-?"))
			{
				Console.WriteLine(commandLineArguments.getCommandsAsAstring());
			}
			if (commandLineArguments.getCommandValueAsBool("-Debug"))
			{
				Console.WriteLine(commandLineArguments.getCommandsAsDisplayString());
			}

			
			

		}

		public Dictionary<string, ESERIALIZE_DATA_TYPE> getTypeDefs() { return m_TypeDefs; }

		private void _addTypeDef(string strName, ESERIALIZE_DATA_TYPE eType)
		{
			m_TypeDefs[strName.ToLower()] = eType;
		}
		private void _parseTypeDefs(string strLine)
		{
			strLine = strLine.Replace(" ", "");
			strLine.Trim();
			if (strLine == "")
			{
				return;
			}
			int iEqualSign = strLine.IndexOf('=');
			if( iEqualSign < 0 )
			{
				log("ERROR - attempting to parse type def. Line is missing equal. e.g. variable=number or variable=string");
				return;
			}
			string strVariableName = strLine.Substring(0, iEqualSign).Trim();
			if (strVariableName.Length == 0)
			{
				log("ERROR - attempting to parse type def. Line is missing type. e.g. variable=number or variable=string");
				return;
			}
			iEqualSign++;
			string strVariableType = strLine.Substring(iEqualSign, strLine.Length - iEqualSign).Trim();
			if (strVariableType.Length == 0)
			{
				log("ERROR - attempting to parse type def. Line is missing value. e.g. variable=number or variable=string");
				return;
			}
			//we need to check to see if there are commas now
			int iCommaIndex = strVariableType.IndexOf(',');
			if( iCommaIndex > 0 )
			{
				iCommaIndex++;
				string strKeepParsing = strVariableType.Substring(iCommaIndex, strVariableType.Length - iCommaIndex);
				strVariableType = strVariableType.Substring(0, iCommaIndex - 1).Trim();
				_parseTypeDefs(strKeepParsing);
			}			
			if (strVariableType.Length == 0)
			{
				log("ERROR - attempting to parse type def. Line is missing value. e.g. variable=number or variable=string");
				return;
			}
			if (strVariableType.ToLower().StartsWith("s"))
			{
				_addTypeDef(strVariableName, ESERIALIZE_DATA_TYPE.STRING);
			}
			else
			{
				_addTypeDef(strVariableName, ESERIALIZE_DATA_TYPE.NUMBER);				
			}

		}
		private void _attemptToConfigureTypeDefs()
		{
			_addTypeDef("string", ESERIALIZE_DATA_TYPE.STRING);
			_addTypeDef("fstring", ESERIALIZE_DATA_TYPE.STRING);
			_addTypeDef("fname", ESERIALIZE_DATA_TYPE.STRING);
			_addTypeDef("kcstring", ESERIALIZE_DATA_TYPE.STRING);
			_addTypeDef("kcname", ESERIALIZE_DATA_TYPE.STRING);
			_addTypeDef("std::string", ESERIALIZE_DATA_TYPE.STRING);
			_addTypeDef("char", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("int", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("int32", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("int8", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("int16", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("int64", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("uint", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("uint32", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("uint8", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("uint16", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("uint64", ESERIALIZE_DATA_TYPE.NUMBER);
			_addTypeDef("float", ESERIALIZE_DATA_TYPE.NUMBER);
            _addTypeDef("bool", ESERIALIZE_DATA_TYPE.NUMBER);
            _addTypeDef("double", ESERIALIZE_DATA_TYPE.NUMBER);
			if (commandLineArguments.getCommandValueAsString("-TypeDefs") != "")
			{
				_parseTypeDefs(commandLineArguments.getCommandValueAsString("-TypeDefs"));
			}
			if (commandLineArguments.getCommandValueAsString("-LoadTypeDef") == "")
			{
				return;
			}
			if(File.Exists(commandLineArguments.getCommandValueAsString("-LoadTypeDef")) == false )
			{
				log("ERROR - file not found. Unable to load type defs at location: " + commandLineArguments.getCommandValueAsString("-LoadTypeDef"));
				return;
			}
			string[] mTypeDefs = File.ReadAllLines(commandLineArguments.getCommandValueAsString("-LoadTypeDef"));
			if(mTypeDefs == null || mTypeDefs.Length == 0)
			{
				log("ERROR - type def file found but contents were empty. location loaded from: " + commandLineArguments.getCommandValueAsString("-LoadTypeDef"));
				return;
			}
			foreach(string strLine in mTypeDefs)
			{
				_parseTypeDefs(strLine);
			}

		}

		private void _showTimeExecuting()
		{
			if( m_StopWatch != null)
			{
				int iCountOfFilesParsed = (m_ClassParserManager != null) ? m_ClassParserManager.filesNeedingUpdate.Count : 0;
				string strTimeExecuting = "Executing Command Line Serializer. Found " + m_ClassParserManager.getNumberOfFilesParsed().ToString() + " files to parse, " + iCountOfFilesParsed.ToString() + " changes/headers parsed. " + getThreadsToUse().ToString() + " threads took :" + m_StopWatch.Elapsed.TotalSeconds.ToString() + " seconds.";
				if (m_LogFile != null &&
					m_LogFile.logOnlyErrors == false)
				{
					log(strTimeExecuting);
				}
				else
				{
					Console.WriteLine(strTimeExecuting);
				}
			}
		}

		private bool _runClean()
		{
			if (commandLineArguments.getCommandValueAsBool("-Clean"))
			{
				log("Cleaning solution.");
				string strPathToIntermediateDir = commandLineArguments.getCommandValueAsString("-IntermediateDir");
                Directory.CreateDirectory(strPathToIntermediateDir);
				if (Directory.Exists(strPathToIntermediateDir) == false)
				{
					log("Unable to clean solution. IntermediateDir not set correct. Current value is = " + strPathToIntermediateDir);
				}
				else
				{
					string[] mFiles = Directory.GetFiles(strPathToIntermediateDir, "*.cfg*");
					foreach (string strFile in mFiles)
					{
						log("CLEANING/DELETING - " + strFile);
						File.Delete(strFile);
					}
				}				
				return true;

			}
			return false;
		}
		
		public bool shouldPauseOnExit { get { return (commandLineArguments != null)?commandLineArguments.getCommandValueAsBool("-Pause"):false; } }

		public int getThreadsToUse() { return Math.Max(1, commandLineArguments.getCommandValueAsInt("-MaxThreads")); }

		public bool getDoingFullRecompile() { return commandLineArguments.getCommandValueAsBool("-ForceRecompile"); }
		private bool _createIntermediateDirectory()
		{
			string strPathToIntermediateDir = commandLineArguments.getCommandValueAsString("-IntermediateDir");
			if (Directory.Exists(strPathToIntermediateDir) == false)
			{
				Directory.CreateDirectory(strPathToIntermediateDir);
				if (Directory.Exists(strPathToIntermediateDir) == false)
				{
					log("Unable to create directory: " + strPathToIntermediateDir);
					return false;
				}
			}
			return true;
		}
		private void _loadConfigFile()
		{
			

			string strFullPath = Path.Combine(commandLineArguments.getCommandValueAsString("-IntermediateDir"), "CommandLineSerializer.json");

            string strErrors = "";
			m_ConfigFile = SerializerConfigFile.createConfigFile(strFullPath, ref strErrors);
			if(m_ConfigFile == null)
			{
                if(strErrors != "")
                {
                    log(strErrors);
                }
                log("Creating new config file for command line parser.");
				m_ConfigFile = new SerializerConfigFile();
			}
			m_ConfigFile.initialize( this, "CommandLineSerializer.json");
			
		}

		private void _saveConfigFile()
		{
			m_ConfigFile.clearHeaderFiles();
			foreach(HeaderFile mFile in m_HeaderFiles)
			{
				m_ConfigFile.addHeaderFile(mFile);
			}
			m_ConfigFile.save(commandLineArguments.getCommandValueAsString("-IntermediateDir"));
		}

	

		
		//returns the command line arguments
		public CommandLineArguments commandLineArguments { get; set; }

		//returns if it should be showing output to the screen
		public bool showDebugInfo { get; set;}

		//returns if it should be logging output
		public bool logOutput { get; set; }

		private void _createLogFile()
		{
			////attempt to log and say if the console will print or not
			m_LogFile = new LogFile(commandLineArguments.getCommandValueAsString("-LogFile"), 100, !commandLineArguments.getCommandValueAsBool("-DisplayOff"));
			if (m_LogFile.getValidLogFile())
			{
				m_LogFile.logOnlyErrors = !commandLineArguments.getCommandValueAsBool("-Verbose");
				log("Log file created at: " + commandLineArguments.getCommandValueAsString("-LogFile"));
			}
			else
			{
				log("Unable to create log file at: " + commandLineArguments.getCommandValueAsString("-LogFile"));
			}
			log("Console output is turned : " + ((!commandLineArguments.getCommandValueAsBool("-DisplayOff"))?"On":"Off"));
		}

		//logs the line and prints to the debug window
		public void log(string strLine) 
		{
			if (m_LogFile != null)
			{
				m_LogFile.log(strLine);
			}
		}

        public LogFile getLogFile() { return m_LogFile; }

        public bool _findHeaders()
		{
			string strPathToIntermediateDir = commandLineArguments.getCommandValueAsString("-IntermediateDir");

			string[] mFiles = Directory.GetFiles(commandLineArguments.getCommandValueAsString("-SourceDir"), "*.h", SearchOption.AllDirectories);
			foreach( string strFile in mFiles)
			{
				HeaderFile mHeaderFile = new HeaderFile();
				mHeaderFile.setLogFile(m_LogFile);
				string strFileName = Path.GetFileName(strFile);

				string strExportedHeaderFile = Path.Combine(strPathToIntermediateDir, Path.GetFileNameWithoutExtension(strFile) + ".serialize.inc");
				mHeaderFile.initialize(this, strFile, strExportedHeaderFile);
				m_HeaderFiles.Add(mHeaderFile);
			}
			if(m_HeaderFiles.Count == 0 )
			{
				log("No header files found in directory: " + commandLineArguments.getCommandValueAsString("-SourceDir"));
				return false;
			}
			log( m_HeaderFiles.Count.ToString() + " header files found in directory: " + commandLineArguments.getCommandValueAsString("-SourceDir"));
			return true;
		}

		public HeaderFile getHeaderFile(string strFile)
		{
			foreach( HeaderFile mHeader in m_HeaderFiles)
			{
				if( mHeader.headerFile == strFile)
				{
					return mHeader;
				}
			}
			return null;
		}

		private void _startProcessingHeaders()
		{
			if(m_ClassParserManager == null )
			{
				log("Error - Unable to process headers. Class Parser is null.");
				return;
			}
			if(m_ClassParserManager.filesNeedingUpdate.Count == 0 )
			{
				log("All headers are up to date.");
				return;
			}

			int iThreads = getThreadsToUse();

			for (int iProcessIndex = 0; iProcessIndex < iThreads; iProcessIndex++)
			{
				m_Processes.Add(new ProcessHeaderFileList());
			}
			int iCurrentProcessBucket = 0;
			bool bForce = getDoingFullRecompile();
			foreach (string strFile in m_ClassParserManager.filesNeedingUpdate)
			{
				HeaderFile mHeaderFile = getHeaderFile(strFile);
				if( mHeaderFile == null )
				{
					log("Error - Header file appears to be missing: " + strFile);
					return;
				}			
				if (bForce == false &&
					m_ConfigFile.getHeaderFileNeedsToRecompile(mHeaderFile) == false)
				{
					log(mHeaderFile.headerFile + " is up to date.");
				}
				else
				{
					log(mHeaderFile.headerFile + " recompiling header.");
				}

				m_Processes[iCurrentProcessBucket].addHeaderFileToProcess(mHeaderFile);
				iCurrentProcessBucket++;
				if (iCurrentProcessBucket >= iThreads)
				{
					iCurrentProcessBucket = 0;
				}
			}
			log("Processing headers on " + iThreads.ToString() + " processors.");
			for (int iProcessIndex = 0; iProcessIndex < m_Processes.Count; iProcessIndex++)
			{
				log("Thread_" + iProcessIndex.ToString() + ") Processing " + m_Processes[iProcessIndex].getNumberOfHeadersRemainingToProcess().ToString() + " headers.");
				m_Processes[iProcessIndex].startProcessingHeaders();
			}


		}

		private void _waitForHeadersToDefineObjects()
		{

		}

		private bool _checkForErrors()
		{
			for (int iProcessIndex = 0; iProcessIndex < m_Processes.Count; iProcessIndex++)
			{
				//todo: check for errors
				/*if( m_Processes[iProcessIndex].hasAnyErrors == true )
				{
					return true;
				}*/
			}
			return false;
		}

		private void _waitForHeadersToWriteHeaders()
		{

		}


		private void _waitForHeadersToProcess()
		{
			while(true)
			{
				bool bIsDone = true;
				for (int iProcessIndex = 0; iProcessIndex < m_Processes.Count; iProcessIndex++)
				{
					bIsDone = bIsDone & m_Processes[iProcessIndex].isDoneProcessingHeaders;
				}
				if( bIsDone)
				{
					break;
				}
				Thread.Sleep(5);
			};
		}

		private void _buildClassStructures()
		{
			m_ClassParserManager.threadsToUse = getThreadsToUse();
			m_ClassParserManager.logFile = m_LogFile;
			foreach (HeaderFile mFile in m_HeaderFiles)
			{
				m_ClassParserManager.addFileToParse(mFile.headerFile);
			}
			string strFullPath = Path.Combine(commandLineArguments.getCommandValueAsString("-IntermediateDir"), "classParser.cfg");
			m_ClassParserManager.compareToCachedData((getDoingFullRecompile())?"":strFullPath);
			m_ClassParserManager.buildClassStructures();
		}
		private bool _waitForClassStructuresToParse()
		{
			while (true)
			{
				if( m_ClassParserManager.getDoneParsingClassesOnThreads())
				{
					break;
				}
				Thread.Sleep(10);
			};


			List<string> mErrors = new List<string>();
			m_ClassParserManager.getErrors(mErrors);
			foreach(string strError in mErrors)
			{
				log("ERROR - " + strError);
			}
			if( mErrors.Count == 0 )
			{
				string strFullPath = Path.Combine(commandLineArguments.getCommandValueAsString("-IntermediateDir"), "classParser.cfg");
				m_ClassParserManager.saveCachedData(strFullPath);
			}
			return (mErrors.Count == 0) ? true : false;

		}

		public ProjectWrapper getProjectWrapper()
		{
			return m_ClassParserManager.getProjectWrapper();
		}

        public void _writeEnumFile()
        {
            int iCountOfFilesParsed = (m_ClassParserManager != null) ? m_ClassParserManager.filesNeedingUpdate.Count : 0;
            if( iCountOfFilesParsed == 0 )
            {
                return; //nothing changed.
            }
            string strPathToIntermediateDir = commandLineArguments.getCommandValueAsString("-IntermediateDir");
            string strEnumHeaderFile = Path.Combine(strPathToIntermediateDir, "EnumsByName.h");
            string strEnumCPPFile = Path.Combine(strPathToIntermediateDir, "EnumsByName.cpp");
            StringWriter mHeaderWriter = new StringWriter();
            mHeaderWriter.WriteLine("#pragma once" + Environment.NewLine + Environment.NewLine);
            mHeaderWriter.WriteLine("#include \"KCIncludes.h\"" + Environment.NewLine + Environment.NewLine);
            mHeaderWriter.WriteLine("class _SERIALIZER_");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("public:");
            mHeaderWriter.WriteLine("static int32 _getEnumValueByName(int32 iEnumID, const KCString &strEnumItemName)");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("     static std::unordered_map<int32, std::unordered_map<KCString, int32>> m_EnumLookUpByIDAndName;");
            mHeaderWriter.WriteLine("     if(m_EnumLookUpByIDAndName.size() > 0 )");
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          return m_EnumLookUpByIDAndName[iEnumID][strEnumItemName];");
            mHeaderWriter.WriteLine("     }");
            foreach (EnumList mEnumList in m_ClassParserManager.getProjectWrapper().enums.Values)
            {
                mHeaderWriter.WriteLine("     {");
                mHeaderWriter.WriteLine("          m_EnumLookUpByIDAndName[" + mEnumList.uniqueID + "] = std::unordered_map<KCString, int32>();");
                mHeaderWriter.WriteLine("          std::unordered_map<KCString, int32> &mNameToValue = m_EnumLookUpByIDAndName[" + mEnumList.uniqueID + "];");
                mHeaderWriter.Write("          ");
                int iIndex = 0;
                foreach (EnumItem mItem in mEnumList.enumItems)
                {
                    mHeaderWriter.Write("mNameToValue[\"" + mItem.name + "\"] = " + iIndex.ToString() + ";");
                    iIndex++;
                }
                mHeaderWriter.WriteLine("");
                mHeaderWriter.WriteLine("     }");

            }
            mHeaderWriter.WriteLine("     return m_EnumLookUpByIDAndName[iEnumID][strEnumItemName];");
            mHeaderWriter.WriteLine("}");



            mHeaderWriter.WriteLine("static const KCString & _getEnumItemNameByValue(int32 iEnumID, int32 iValue)");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("     static std::unordered_map<int32, std::unordered_map<int32, KCString>> m_EnumLookUpByIDAndValue;");
            mHeaderWriter.WriteLine("     if(m_EnumLookUpByIDAndValue.size() > 0 )");
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          return m_EnumLookUpByIDAndValue[iEnumID][iValue];");
            mHeaderWriter.WriteLine("     }");
            foreach (EnumList mEnumList in m_ClassParserManager.getProjectWrapper().enums.Values)
            {
                mHeaderWriter.WriteLine("     {");
                mHeaderWriter.WriteLine("          m_EnumLookUpByIDAndValue[" + mEnumList.uniqueID + "] = std::unordered_map<int32,KCString>();");
                mHeaderWriter.WriteLine("          std::unordered_map<int, KCString> &mValueToName = m_EnumLookUpByIDAndValue[" + mEnumList.uniqueID + "];");
                mHeaderWriter.Write("          ");
                int iIndex = 0;
                foreach (EnumItem mItem in mEnumList.enumItems)
                {
                    mHeaderWriter.Write("mValueToName[" + iIndex.ToString() + "] = \"" + mItem.name + "\";");
                    iIndex++;
                }
                mHeaderWriter.WriteLine("");
                mHeaderWriter.WriteLine("     }");

            }
            mHeaderWriter.WriteLine("     return m_EnumLookUpByIDAndValue[iEnumID][iValue];");
            mHeaderWriter.WriteLine("}");



            mHeaderWriter.WriteLine("static const TArray<KCString> & _getEnumItemNamesByEnumName(const KCString &strEnumName)");
            mHeaderWriter.WriteLine("{");
            mHeaderWriter.WriteLine("    static std::unordered_map<KCString, TArray<KCString>> m_EnumLookUpByName;");
            mHeaderWriter.WriteLine("     if(m_EnumLookUpByName.size() > 0 )");
            mHeaderWriter.WriteLine("     {");
            mHeaderWriter.WriteLine("          return m_EnumLookUpByName[strEnumName];");
            mHeaderWriter.WriteLine("     }");
            foreach (EnumList mEnumList in m_ClassParserManager.getProjectWrapper().enums.Values)
            {
                mHeaderWriter.WriteLine("     {");
                mHeaderWriter.WriteLine("          m_EnumLookUpByName[\"" + mEnumList.enumName + "\"] = TArray<KCString>();");                
                mHeaderWriter.WriteLine("          TArray<KCString> &mArray1 = m_EnumLookUpByName[\"" + mEnumList.enumName + "\"];");
                mHeaderWriter.WriteLine("          mArray1.Reserve(" + mEnumList.enumItems.Count + ");");
                mHeaderWriter.Write("          ");
                int iIndex = 0;
                foreach (EnumItem mItem in mEnumList.enumItems)
                {
                    mHeaderWriter.Write("mArray1.Add(\"" + mItem.name + "\");");
                    iIndex++;
                }
                mHeaderWriter.WriteLine("");
                mHeaderWriter.WriteLine("     }");

            }
            mHeaderWriter.WriteLine("     return m_EnumLookUpByName[strEnumName];");
            mHeaderWriter.WriteLine("}");           
            mHeaderWriter.WriteLine("}; //end _SERIALIZER_");
            string strFile = mHeaderWriter.ToString();
            try
            {


                
                string strOldFile = File.ReadAllText(strEnumHeaderFile);
                if( strFile == strOldFile)
                {
                    log("EnumsByName.h - didn't change. Not resaving.");
                    return;
                }
            }
            catch
            {

            }
            File.WriteAllText(strEnumHeaderFile, mHeaderWriter.ToString());
            log("resaving EnumsByName.h.");



        }

    } //end class
}//end namespace
