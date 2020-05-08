using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.IO;
using Library.ClassParser;
using CommandLineSerializer.codeWriters;
namespace CommandLineSerializer
{
	[Serializable]
	public class HeaderFile
	{
		private LogFile m_LogFile = null;
		private SerializerController m_Controller = null;
		private StringWriter m_StringWriter = null;
		private List<CodeWriter> m_CodeWriters = new List<CodeWriter>();
        private List<ClassStructure> m_AdditionalHeadersNeeded = new List<ClassStructure>();
		public void initialize(SerializerController mController, string strHeaderFile, string strExportedFile)
		{
			m_Controller = mController;
			headerFile = strHeaderFile;
			headerFileWriteTime = File.GetLastWriteTime(headerFile);
			exportedHeaderFile = strExportedFile;
			exportedHeaderFileWriteTime = File.GetLastWriteTime(exportedHeaderFile);
			if (exportedHeaderFileWriteTime.Year < 2000)
			{
				exportedHeaderFileWriteTime = new DateTime(0);
			}
			//it's important we add these in specific orders - primary variables should always be last
			m_CodeWriters.Add(new TArrayWriter());
			m_CodeWriters.Add(new ClassOrStructWriter());
			m_CodeWriters.Add(new EnumWriter());
			m_CodeWriters.Add(new PrimaryVariableTypeWriter());

		}

		private void log(string strMessage)
		{
			m_Controller.log(strMessage);
		}
        public void addClassRef(ClassStructure mClassStructure)
        {
            if (m_AdditionalHeadersNeeded.Contains(mClassStructure) == false)
            {
                m_AdditionalHeadersNeeded.Add(mClassStructure);
            }
        }
		public List<CodeWriter> getCodeWriters() { return m_CodeWriters; }
		public Dictionary<string, ESERIALIZE_DATA_TYPE> getTypeDefs() { return m_Controller.getTypeDefs(); }
		public ProjectWrapper getProjectWrapper() { return m_Controller.getProjectWrapper(); }
		public EnumList getEnumList(string strEnumName) { return getProjectWrapper().getEnumListByName(strEnumName); }
		public ClassStructure getClassStructure(string strClassStructureName) { return getProjectWrapper().getClassStructByName(strClassStructureName); }
		public string headerFile { get; set; }
		public DateTime headerFileWriteTime { get; set; }
		public string exportedHeaderFile { get; set; }
		public DateTime exportedHeaderFileWriteTime { get; set; }
		public LogFile getLogFile() { return m_LogFile; }
		public void setLogFile(LogFile mLogFile) { m_LogFile = mLogFile; }
		
		private int m_iBrackets = 0;
		private string m_strPadding = "";
		public void addLine(string strLine)
		{
			addLine(strLine, true);
		}
		public void addLine(string strLine, bool bIsMacroLine)
		{
			if (m_StringWriter == null)
			{
				m_StringWriter = new StringWriter();
			}


			int iBracketAdd = 0;
			for (int iBracketIndex = 0; iBracketIndex < strLine.Length; iBracketIndex++)
			{
				if(strLine[iBracketIndex] == '{')
				{
					iBracketAdd++;
				}
				else if(strLine[iBracketIndex] == '}')
				{
					iBracketAdd--;
				}
			}
			bool bWroteLine = false;
			if(iBracketAdd != 0 )
			{
				m_iBrackets += iBracketAdd;
				if( iBracketAdd > 0 )
				{
					if (bIsMacroLine)
					{
						m_StringWriter.WriteLine(m_strPadding + strLine + "\\");    //we have to add because it needs to be a macro.
					}
					else
					{
						m_StringWriter.WriteLine(strLine);
					}
					bWroteLine = true;
				}
				if( m_iBrackets > 0 )
				{
					m_strPadding = "".PadRight(m_iBrackets * 5, ' ');
				}
				else
				{
					m_strPadding = "";
				}
				
			}
			
			if(bWroteLine == false)
			{
				if (bIsMacroLine)
				{
					m_StringWriter.WriteLine(m_strPadding + strLine + "\\");    //we have to add because it needs to be a macro.
				}
				else
				{
					m_StringWriter.WriteLine(strLine);
				}
			}

		}

		public bool getNeedsToProcessHeader()
		{
			if (exportedHeaderFileWriteTime == null ||
				exportedHeaderFileWriteTime < headerFileWriteTime)
			{
				return true;
			}
			return false;
		}

		private string getClassName(string strLineParsing)
		{

			if (strLineParsing == "" ||
				strLineParsing[strLineParsing.Length - 1] == ';' ||
				strLineParsing.Contains(',') ||
				strLineParsing.Contains("("))
			{
				return "";
			}
			strLineParsing = strLineParsing.Trim();
			if (strLineParsing.StartsWith("class ") ||
				strLineParsing.StartsWith("struct "))
			{
				strLineParsing = strLineParsing.Replace("class ", "");
				strLineParsing = strLineParsing.Replace("struct ", "");
				strLineParsing = strLineParsing.Trim();
				int iEndOfClassName = strLineParsing.IndexOf(" ");
				if (iEndOfClassName < 0)
				{
					iEndOfClassName = strLineParsing.Length;
				}
				string strClassName = strLineParsing.Substring(0, iEndOfClassName);
				return strClassName;
			}
			return "";
		}

		public void processHeader()
		{

			string[] mLines = File.ReadAllLines(headerFile);
			bool bSerializedHeader = false;
			string strClassOrStructName = "";
			//need to find a way to make this settable from external

            List<string> mHeaderFilesFound = new List<string>();
            mHeaderFilesFound.Add(Path.GetFileName(headerFile));
            for (int iLineIndex = 0; iLineIndex < mLines.Length; iLineIndex++)
			{
				string strLine = mLines[iLineIndex];
				if (bSerializedHeader == false)
				{
					if (iLineIndex > 50)
					{
						return;
					}
                    string strHeaderFile = strLine.Trim();

                    if (strHeaderFile.StartsWith("#include") &&
                        strHeaderFile.Contains("\""))
                    {
                        strHeaderFile = strHeaderFile.Remove(0, 8); //#include
                        strHeaderFile = strHeaderFile.Replace("\"", "");
                        strHeaderFile = strHeaderFile.Trim();

                        try
                        {
                            mHeaderFilesFound.Add(Path.GetFileName(strHeaderFile));
                            if (strHeaderFile.Contains(".serialize.inc"))
                            {
                                bSerializedHeader = true;
                            }
                        }
                        catch { }
                    }
                    continue;
				}
				string strClassName = getClassName(strLine);
				if (strClassName != "")
				{
					strClassOrStructName = strClassName;

				}

				if (strLine.Contains("KCSERIALIZE_CODE") && strLine.Contains("//") == false)
				{
					if (strLine.Contains("KCSERIALIZE_CODE_MANUAL") == false)
					{
                        if (mHeaderFilesFound.Contains("KCDataGroup") == false)
                        {
                            addLine("#include \"Systems/DataGroup/KCDataGroup.h\"", false);
                        }
                        addLine("#include \"EnumsByName.h\"", false);
                        addLine("#include \"ClassCreation.h\"", false);
                        //addLine("<additional_headers>", false);
                        _createSerializeFile(strClassOrStructName, iLineIndex);
					}
				}
			}

			if (m_StringWriter != null &&
                bSerializedHeader)
			{
                string strFileToWrite = m_StringWriter.ToString();
                /*ProjectWrapper mProjectWrapper = m_Controller.getProjectWrapper();
                List<string> mHeadersAdded = new List<string>();
                string strAdditionalHeaders = "";
                foreach (ClassStructure mClass in m_AdditionalHeadersNeeded)
                {
                    string strFileForClass = Path.GetFileName( mClass.file );
                    if( mHeaderFilesFound.Contains(strFileForClass) == false )
                    {
                        mHeaderFilesFound.Add(strFileForClass);
                        strAdditionalHeaders = strAdditionalHeaders + "#include \"" + mClass.file + "\"" + Environment.NewLine;
                    }
                    string strSerializationCreationFile = "ClassCreation_" + mClass.name + ".h";
                    if (mHeaderFilesFound.Contains(strSerializationCreationFile) == false)
                    {
                        mHeaderFilesFound.Add(strSerializationCreationFile);
                        strAdditionalHeaders = strAdditionalHeaders + "#include \"" + strSerializationCreationFile + "\"" + Environment.NewLine;
                    }

                }
                strFileToWrite = strFileToWrite.Replace("<additional_headers>", strAdditionalHeaders);*/
                File.WriteAllText(exportedHeaderFile, strFileToWrite);
				if (File.Exists(exportedHeaderFile))
				{
					exportedHeaderFileWriteTime = File.GetLastWriteTime(exportedHeaderFile);
					log("Wrote header: " + exportedHeaderFile);
				}
				else
				{
					log("ERROR - unable to write header file: " + exportedHeaderFile);
				}
			}


		}

		private void _createSerializeFile(string strClassName, int iLineNumber)
		{
			ProjectWrapper mProjectWrapper = m_Controller.getProjectWrapper();
			ClassStructure mClass = mProjectWrapper.getClassStructByName(strClassName);
			if (mClass == null)
			{
				log("ERROR - unable to find class :" + strClassName + " in file: " + headerFile);
				return;
			}
			foreach (ClassVariable mVariable in mClass.variables)
			{
				if (mVariable.isUE4Variable)
				{
					if (_validateVariable(mVariable, true) == false)
					{
						mVariable.isUE4Variable = false;
					}
				}
			}
			int iPadSizeLeft = (100 - strClassName.Length) / 2;
			string strComment = strClassName.PadLeft(strClassName.Length + iPadSizeLeft, '/');
			strComment = strComment.PadRight(strComment.Length + iPadSizeLeft, '/');
			string strLineComment = "".PadRight(strComment.Length, '/');
			addLine(strLineComment, false);
			addLine(strComment, false);
			addLine("", false);

			_writeMacroDefineCode(iLineNumber);
            _writeSerializationFunctions(mClass);
            _writeByteWriterCode(mClass);
			_writeByteReaderCode(mClass);
			_writeDataGroupWriteCode(mClass);
			_writeDataGroupReadCode(mClass);

			addLine("", false);
			addLine(strComment, false);
			addLine(strLineComment, false);				

		}

		private void _writeMacroDefineCode(int iLineNumber)
		{
			string strMacroName = "KCSERIALIZEOBJECT_" + (iLineNumber + 1).ToString();
			addLine("#ifdef " + strMacroName, false);
			addLine("#undef " + strMacroName, false);
			addLine("#endif", false);
			addLine("#define " + strMacroName + " ");
		}
        private void _writeSerializationFunctions(ClassStructure mClass)
        {
            addLine("virtual KCString getClassName(){ return \"" + mClass.name + "\";}");            
        }

        private void _writeByteWriterCode(ClassStructure mClass)
		{
			addLine("virtual bool serialize(KCByteWriter &mByteWriter)");
			addLine("{");
			_addInheritance(mClass, "serialize(mByteWriter);");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				foreach (CodeWriter mCodeWriter in m_CodeWriters)
				{
					if (mCodeWriter.attemptByteWriterCode(this, mVariable))
					{
						break;
					}
				}
			}
			addLine("return true;");
			addLine("}");
		}
		private void _writeByteReaderCode(ClassStructure mClass)
		{
			addLine("virtual bool deserialize(KCByteReader &mByteReader)");
			addLine("{");
			_addInheritance(mClass, "deserialize(mByteReader);");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				foreach (CodeWriter mCodeWriter in m_CodeWriters)
				{
					if (mCodeWriter.attemptByteReaderCode(this, mVariable))
					{
						break;
					}
				}
			}
			addLine("return true;");
			addLine("}");
		}
		private void _writeDataGroupWriteCode(ClassStructure mClass)
		{
			addLine("virtual bool serialize(KCDataGroup &mDataGroup, const KCString &strGroupName)");
			addLine("{");
			addLine("if (mDataGroup.getGroupName().isEmpty()) { mDataGroup.setGroupName(strGroupName); }");
			_addInheritance(mClass, "serialize(mDataGroup, strGroupName);");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				foreach (CodeWriter mCodeWriter in m_CodeWriters)
				{
					if (mCodeWriter.attemptDataGroupWriteCode(this, mVariable, "mDataGroup"))
					{
						break;
					}
				}
			}
			addLine("return true;");
			addLine("}");
			addLine("virtual bool serialize(KCDataGroup &mDataGroup)");
			addLine("{");
			addLine("return serialize(mDataGroup, \"" + mClass.name + "\");");
			addLine("}");
		}
		private void _writeDataGroupReadCode(ClassStructure mClass)
		{

			addLine("virtual bool deserialize(const KCDataGroup &mDataGroup)");
			addLine("{");
			_addInheritance(mClass, "deserialize(mDataGroup);");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				foreach (CodeWriter mCodeWriter in m_CodeWriters)
				{
					if (mCodeWriter.attemptDataGroupReadCode(this, mVariable, "mDataGroup"))
					{
						break;
					}
				}
			}
			addLine("return true;");
			addLine("}", false);	//this ends the macro
		}
		


		private bool _validateVariable(ClassVariable mVariable, bool bLog)
		{
			string strVariableLowerCase = mVariable.variableType.ToLower();
			if(m_Controller.getTypeDefs().ContainsKey(strVariableLowerCase))
			{
				return true;
			}

			EnumList mEnum = m_Controller.getProjectWrapper().getEnumListByName(mVariable.variableType);
			if (mEnum != null)
			{
				return true;
			}
			string strVariableType = mVariable.variableType;
			if (strVariableType.Contains("<"))
			{
				strVariableType = strVariableType.Substring(0, strVariableType.IndexOf("<"));
                if (strVariableType.Contains("TArray")) //HACK!
                {
                    return true;
                }
            }
			ClassStructure mClassStruct = m_Controller.getProjectWrapper().getClassStructByName(strVariableType);
			if (mClassStruct != null)
			{
				return true;
			}
			if (bLog)
			{
				log("ERROR - in attempting to serialize variable: " + mVariable.variableType + "  " + mVariable.variableName);
			}

			return false;
		}

		private bool _isSerialized(ClassStructure mClass)
		{
			if( mClass == null )
			{
				return false;
			}
			if( mClass.isSerialized)
			{ 
				return true;
			}
			foreach (string strInheritClass in mClass.classStructuresInheritingFrom)
			{
				ClassStructure mParent = getClassStructure(strInheritClass);
				if (mParent == null)
				{
					//this might be okay. We could be inheriting from a class we don't know about
					continue;
				}
				if( _isSerialized(mParent) )
				{
					return true;
				}
			}
			return false;
		}

		private void _addInheritance(ClassStructure mClass, string strFunctionSuffix)
		{
			foreach( string strInheritClass in mClass.classStructuresInheritingFrom)
			{
				ClassStructure mParent = getClassStructure(strInheritClass);
				if(mParent == null)
				{
					//this might be okay. We could be inheriting from a class we don't know about
					continue;
				}
				if(_isSerialized(mParent))
				{
					addLine(mParent.name + "::" + strFunctionSuffix);
				}
			}
		}


	}


}
