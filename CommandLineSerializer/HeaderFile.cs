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
		private StringWriter m_StringWriter = null;
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

		private void log(string strMessage)
		{
			m_Controller.log(strMessage);
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

		private string getClassName(string strLineParsing)
		{
		
			if(strLineParsing == "" ||
				strLineParsing[ strLineParsing.Length - 1 ] == ';' ||
				strLineParsing.Contains(',') ||
				strLineParsing.Contains("(") )
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
				if( iEndOfClassName < 0)
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
			for(int iLineIndex = 0; iLineIndex < mLines.Length; iLineIndex++)
			{
				string strLine = mLines[iLineIndex];
				if( bSerializedHeader == false)
				{
					if(iLineIndex > 50 )
					{
						return;
					}
					if(strLine.Trim().StartsWith("#include") &&
					   strLine.Contains(".serialize.inc"))
					{
						bSerializedHeader = true;
					}
					continue;
				}
				string strClassName = getClassName(strLine);
				if(strClassName != "")
				{
					strClassOrStructName = strClassName;
					
				}
				
				if ( strLine.Contains("KCSERIALIZE_CODE") && strLine.Contains("//") == false)
				{
					_createSerializeFile(strClassOrStructName, iLineIndex);
				}
			}

			if( m_StringWriter != null )
			{
				File.WriteAllText(exportedHeaderFile, m_StringWriter.ToString());
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
			if(m_StringWriter == null )
			{
				m_StringWriter = new StringWriter();
			}
			ProjectWrapper mProjectWrapper = m_Controller.getProjectWrapper();
			ClassStructure mClass = mProjectWrapper.getClassStructByName(strClassName);
			if( mClass == null )
			{
				log("ERROR - unable to find class :" + strClassName + " in file: " + headerFile);
				return;
			}
			int iPadSizeLeft = (100 - strClassName.Length) / 2;
			string strComment = strClassName.PadLeft(iPadSizeLeft, '/');
			strComment = strComment.PadRight(strComment.Length + iPadSizeLeft, '/');
			string strLineComment = "".PadRight(strComment.Length, '/');

			m_StringWriter.WriteLine(strLineComment);
			m_StringWriter.WriteLine(strComment);
			m_StringWriter.WriteLine("");
			string strMacroName = "KCSERIALIZEOBJECT_" + (iLineNumber + 1).ToString();
			m_StringWriter.WriteLine("#ifdef " + strMacroName);
			m_StringWriter.WriteLine("#undef " + strMacroName);
			m_StringWriter.WriteLine("#endif");			
			m_StringWriter.WriteLine("#define " + strMacroName + " bool serialize(KCByteWriter &mByteWriter)\\");
			m_StringWriter.WriteLine("{\\");
			foreach( ClassVariable mVariable in mClass.variables)
			{
				if( mVariable.isUE4Variable)
				{
					if (_attemptToWriteObject(mVariable, m_StringWriter, "serialize(mByteWriter);") == false &&
						_attemptToWriteEnum(mVariable, m_StringWriter, "mByteWriter << (<EnumType>)<VarName>;") == false )
					{
						m_StringWriter.WriteLine("     mByteWriter << " + mVariable.variableName + ";\\");
					}
				}
			}
			m_StringWriter.WriteLine("     return true;\\");
			m_StringWriter.WriteLine("}\\");
			m_StringWriter.WriteLine("bool deserialize(KCByteReader &mByteReader)\\");
			m_StringWriter.WriteLine("{\\");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				if (mVariable.isUE4Variable)
				{
					if (_attemptToWriteObject(mVariable, m_StringWriter, "deserialize(mByteReader);") == false &&
						_attemptToWriteEnum(mVariable, m_StringWriter, "<EnumType> m_<VarName>((<EnumType>)<VarName>); mByteReader << m_<VarName>; <VarName> = (<EnumName>)m_<VarName>;") == false )
					{						
						m_StringWriter.WriteLine("     mByteReader << " + mVariable.variableName + ";\\");
					}
				}
			}
			m_StringWriter.WriteLine("     return true;\\");
			m_StringWriter.WriteLine("}\\");

			//////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////////data group////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			m_StringWriter.WriteLine("bool serialize(KCDataGroup &mDataGroup, const KCString &strGroupName)\\");
			m_StringWriter.WriteLine("{\\");
			m_StringWriter.WriteLine("     if (mDataGroup.getGroupName().isEmpty()) { mDataGroup.setGroupName(strGroupName); }\\");
			//m_StringWriter.WriteLine("mDataGroup.setProperty("m_strTest", m_strTest);\\");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				if (mVariable.isUE4Variable)
				{
					if (_attemptToWriteObject(mVariable, m_StringWriter, "serialize(mDataGroup, \"" + mVariable.variableName + "\");") == false &&
						_attemptToWriteEnum(mVariable, m_StringWriter, "mDataGroup.setProperty(\"<VarName>\", (<EnumType>)<VarName>);") == false)
					{
						m_StringWriter.WriteLine("     mDataGroup.setProperty(\"" + mVariable.variableName + "\", " + mVariable.variableName + ");\\");
					}
				}
			}
			m_StringWriter.WriteLine("     return true;\\");
			m_StringWriter.WriteLine("}\\");
			m_StringWriter.WriteLine("bool serialize(KCDataGroup &mDataGroup)\\");
			m_StringWriter.WriteLine("{\\");
			m_StringWriter.WriteLine("return serialize(mDataGroup, \"" + strClassName + "\");\\");
			m_StringWriter.WriteLine("}\\");		
			m_StringWriter.WriteLine("bool deserialize(KCDataGroup &mDataGroup)\\");
			m_StringWriter.WriteLine("{\\");
			foreach (ClassVariable mVariable in mClass.variables)
			{
				if (mVariable.isUE4Variable)
				{
					if (_attemptToWriteObject(mVariable, m_StringWriter, "deserialize(mDataGroup);") == false &&
						_attemptToWriteEnum(mVariable, m_StringWriter, "<VarName> = (<EnumName>)mDataGroup.getProperty(\"<VarName>\", (<EnumType>)<VarName>);") == false)
					{
						m_StringWriter.WriteLine("     " + mVariable.variableName + " = mDataGroup.getProperty(\"" + mVariable.variableName + "\", " + mVariable.variableName + ");\\");
					}
				}
			}
			m_StringWriter.WriteLine("     return true;\\");
			m_StringWriter.WriteLine("}");
			m_StringWriter.WriteLine("");


			m_StringWriter.WriteLine(strComment);
			m_StringWriter.WriteLine(strLineComment);
			m_StringWriter.WriteLine("");
		}

		private bool _attemptToWriteObject(ClassVariable mVariable, StringWriter mStringWriter, string strSuffix)
		{
			ClassStructure mClassStruct = m_Controller.getProjectWrapper().getClassStructByName(mVariable.variableType);
			if( mClassStruct != null )
			{
				if( mVariable.isPointer)
				{
					mStringWriter.WriteLine("     if(" + mVariable.variableName + " != nullptr){" + mVariable.variableName.Trim() + "->" + strSuffix + "}\\");
				}
				else
				{
					mStringWriter.WriteLine("     " + mVariable.variableName.Trim() + "." + strSuffix + "\\");
				}
				return true;
			}

			return false;
		}
		private bool _attemptToWriteEnum(ClassVariable mVariable, StringWriter mStringWriter, string strStringToWrite)
		{
			EnumList mEnum = m_Controller.getProjectWrapper().getEnumListByName(mVariable.variableType);
			if (mEnum != null)
			{
				strStringToWrite = strStringToWrite.Replace("<EnumName>", mEnum.enumName);
				strStringToWrite = strStringToWrite.Replace("<VarName>", mVariable.variableName);
				string strType = mEnum.type;
				if( strType == null ||
					strType == "" )
				{
					strType = "int32";
				}
				strStringToWrite = strStringToWrite.Replace("<EnumType>", strType);
				mStringWriter.WriteLine("     " + strStringToWrite + "\\");
				return true;
			}
			return false;

		}
	}


}
