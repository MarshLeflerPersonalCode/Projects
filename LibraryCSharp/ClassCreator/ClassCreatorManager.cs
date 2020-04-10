using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using Library.ClassParser;
using Library.IO;
using Library.ClassCreator.Writers;
using System.Threading;
namespace Library.ClassCreator
{

	//this class uses the class parser to dynamically create classes on the fly. 
	//We can parse any code we want from other sources as long as its properties 
	//are defined the classParser. We can then create representations of them in
	//C# with this class.
	//
	//Every class extends ClassInstance.cs. ClassInstance has a lot of helper functions for
	//getting and setting properties/objects
	//
	//
	public class ClassCreatorManager
	{
		private ClassParserManager m_ClassParser = null;
		private StringWriter m_StringWriter = null;
		private Thread m_Thread = null;
		private CompilerResults m_CompileResults = null;
		private bool m_bDoneCompiling = false;
		private string m_strExportedDLL = "";
		private string m_strInitialContentsOfMasterFile = "";
		private Dictionary<ClassStructure, string> m_ClassesAsCode = new Dictionary<ClassStructure, string>();
		private Dictionary<EnumList, string> m_EnumsAsCode = new Dictionary<EnumList, string>();
		public ClassCreatorManager()
		{
			m_strInitialContentsOfMasterFile = _createInitialFile();
		}

		public void setInitialContentsOfMasterFile(string strInitialContentsOfMasterFile) { m_strInitialContentsOfMasterFile = strInitialContentsOfMasterFile; }

		public void intialize(ClassParserManager mClassParser, string strExportedDLLName)
		{
			m_ClassParser = mClassParser;
			m_strExportedDLL = strExportedDLLName;
			try
			{
				m_Thread = new Thread(new ThreadStart(_processClassesAndCompile));
				m_Thread.Name = "Compiling";
				m_Thread.Start();
			}
			catch
			{
				m_Thread = null;
				m_bDoneCompiling = true;
			}
		}
		private void _processClassesAndCompile()
		{ 
			
			foreach (EnumList mEnum in m_ClassParser.getProjectWrapper().enums.Values)
			{
				string strEnum = EnumWriter.writeEnum(mEnum, m_ClassParser.getProjectWrapper());
				m_EnumsAsCode[mEnum] = strEnum;
			}
			foreach (ClassStructure mClass in m_ClassParser.getProjectWrapper().classStructures.Values)
			{
				if (mClass.isSerialized)
				{
					string strClass = ClassWriter.writeClass(mClass, m_ClassParser.getProjectWrapper());
					m_ClassesAsCode[mClass] = strClass;
				}
			}

			_buildMasterFile();
			_compileCode();
			m_bDoneCompiling = true;
			m_Thread = null;
		}

		public bool isDoneParsingAndCompiling()
		{
			return m_bDoneCompiling;
		}
		public CompilerResults getCompileErrors() { return m_CompileResults; }

		public string getErrorsAsString()
		{
			if( m_CompileResults == null )
			{
				return "";
			}
			string strErrors = "";
			foreach (CompilerError mError in m_CompileResults.Errors)
			{
				string strError = mError.ErrorText + ": Line Number:" + mError.Line.ToString() + "\n";
				strErrors = strErrors + strError + "\n";

			}
			return strErrors;
		}

		private void _buildMasterFile()
		{
			m_StringWriter = new StringWriter();
			string[] mLines = m_strInitialContentsOfMasterFile.Split('\n');
			string strCommentLine = "".PadRight(100, '/');
			foreach (string strLine in mLines)
			{
				if(strLine.Contains("<CLASSES>"))
				{
					foreach (KeyValuePair<EnumList, string > mData in m_EnumsAsCode)
					{
						m_StringWriter.WriteLine(strCommentLine);						
						string strTitleComment = mData.Key.enumName.PadRight(50, '/').PadLeft(100 , '/');						
						m_StringWriter.WriteLine(strTitleComment);
						m_StringWriter.WriteLine(mData.Value);
						m_StringWriter.WriteLine(strTitleComment);
						m_StringWriter.WriteLine(strCommentLine);
						m_StringWriter.WriteLine();

					}
					foreach (KeyValuePair<ClassStructure, string> mData in m_ClassesAsCode)
					{
						m_StringWriter.WriteLine(strCommentLine);
						string strTitleComment = mData.Key.name.PadRight(50, '/').PadLeft(100, '/');
						m_StringWriter.WriteLine(strTitleComment);
						m_StringWriter.WriteLine(mData.Value);
						m_StringWriter.WriteLine(strTitleComment);
						m_StringWriter.WriteLine(strCommentLine);
						m_StringWriter.WriteLine();
					}
				}
				else
				{
					m_StringWriter.WriteLine( strLine.Trim() );
				}
			}
			

			
		}


		public LogFile logFile { get; set; }
		public void log(string strLogMessage)
		{
			if(logFile != null)
			{
				logFile.log(strLogMessage);
			}
		}



		private void _compileCode()
		{
			if(m_StringWriter == null)
			{
				log("ERROR - no code to compile.");
				m_bDoneCompiling = true;
				return;
			}
			string strMasterFile = m_StringWriter.ToString();
			

			if (strMasterFile == null ||
				strMasterFile == "")
			{
				log("ERROR - no code to compile.");
				m_bDoneCompiling = true;
				return;
			}

			string strDLLName = m_strExportedDLL;
			int iDllLockTest = 1;
			try
			{
				File.WriteAllText("mastercode.cs", strMasterFile);

				do
				{					
					CompilerParameters parms = new CompilerParameters();
					parms.GenerateExecutable = false;
					parms.GenerateInMemory = true;
					parms.IncludeDebugInformation = true;
					
					parms.OutputAssembly = strDLLName;
					parms.CompilerOptions = " /unsafe";
					AppDomain currentDomain = AppDomain.CurrentDomain;
					foreach (Assembly mAssembly in currentDomain.GetAssemblies())
					{
						parms.ReferencedAssemblies.Add(mAssembly.Location);
					}
					string strExecutableAssembly = Assembly.GetExecutingAssembly().Location;
					//parms.ReferencedAssemblies.Add(strExecutableAssembly);
					CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
					m_CompileResults = provider.CompileAssemblyFromSource(parms, strMasterFile);
					//in an attempt to not make our editors lock we create multiple dll's! FUN!
					if (m_CompileResults.Errors.Count == 1)
					{
						string strErrorLine = m_CompileResults.Errors[0].ErrorText;
						if (strErrorLine.Contains(strDLLName))
						{
							strDLLName = m_strExportedDLL + iDllLockTest.ToString() + ".dll";
							iDllLockTest++;
							continue;
						}
					}
					if (m_CompileResults.Errors.Count > 0)
					{
						string strErrors = getErrorsAsString();
						log("Compile Error: " + Environment.NewLine + strErrors);						
						log("ERROR - and error occurred while compiling. Check file mastercode.cs");
					
					}
					m_bDoneCompiling = true;
					return;
				} while (true);

			}
			catch (Exception e)
			{
				log("ERROR - in compiling source code. Error is: " + e.Message);				
			}
			m_bDoneCompiling = true;
		}



		private string _createInitialFile()
		{
			return @"
using System;                                 
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Library.ClassParser;
using Library.ClassCreator;
using Library.IO;
namespace Library.ClassCreator
{
<CLASSES>
} //end of namespace
";
		}

	}//end of class
}//end of namespace
