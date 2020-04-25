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
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
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
        private string m_strNamespace = "Dynamic";//.ClassCreator";
		private Dictionary<string, Type> m_ClassesCompiled = new Dictionary<string, Type>();
		private Dictionary<ClassStructure, string> m_ClassesAsCode = new Dictionary<ClassStructure, string>();
        private Dictionary<string, ClassStructure> m_ClassStructuresByName = new Dictionary<string, ClassStructure>();
        private Dictionary<EnumList, string> m_EnumsAsCode = new Dictionary<EnumList, string>();
        private Dictionary<string, string> m_TypeConverters = new Dictionary<string, string>();
        private string m_strTypeConverters = "";
        private string m_strDatabaseFolder = ".\\Databases\\";
        private string m_strListsFolder = ".\\Lists\\";
        private List<ClassCreatorContentFolders> m_ContentFolders = new List<ClassCreatorContentFolders>();
        public ClassCreatorManager()
		{

            m_strInitialContentsOfMasterFile = _createInitialFile();
			variableDefinitionHandler = new VariableDefinitionHandler("VariableDefinitions.json");

		}

        public ClassParserManager getClassParser() { return m_ClassParser; }
        public void setDirectories(string strDatabaseFolder, string strListFolder, List<ClassCreatorContentFolders> mContentFolders)
        {
            m_strDatabaseFolder = strDatabaseFolder;
            m_strListsFolder = strListFolder;
            m_ContentFolders = mContentFolders;
        }
        public void showVariableDefinitionEditor(Form mParent)
        {
            VariableDefinitionEditor mEditor = new VariableDefinitionEditor(this);
            mEditor.ShowDialog(mParent);
        }

        //shouldn't include the period at the end. Should be something like "Dynamic" or "Dynamic.Class". Default is "Dynamic"
        public void setNamespace(string strNamespace) { m_strNamespace = strNamespace; }

		public VariableDefinitionHandler variableDefinitionHandler { get; set; }

		public object createNewClass(string strClassName)
		{
			try
			{
				if(m_ClassesCompiled.ContainsKey(strClassName) == false)
				{
					log("Error - no class with name: " + strClassName + " was created in the classes created.");
					return null;
				}				
				object mObjectCreated = Activator.CreateInstance(m_ClassesCompiled[strClassName]);
				ClassInstance mInstance = mObjectCreated as ClassInstance;
				if (mInstance != null)
				{
                    mInstance.m_ClassStructure = m_ClassStructuresByName[strClassName];
                    mInstance.m_strClassName = strClassName;
                    mInstance.m_bIsDirty = false;
				}

				return mObjectCreated;

			}
			catch (Exception e)
			{
				log("Error in creating class: " + strClassName + ". Exception thrown was: " + e.Message);
			}
			return null;
		}

        public Type getClassType(string strClassName)
        {
            if(m_ClassesCompiled.ContainsKey(strClassName))
            {
                return m_ClassesCompiled[strClassName];
            }
            return null;
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

        private void _assignTypeConverters()
        {
            foreach (ClassStructure mClass in m_ClassParser.getProjectWrapper().classStructures.Values)
            {
                if (mClass.isSerialized)
                {
                    foreach( ClassVariable mVariable in mClass.variables)
                    {
                        if( mVariable.variableProperties.ContainsKey("LIST"))
                        {
                            string strListName = mVariable.variableProperties["LIST"].ToUpper();
                            if( m_TypeConverters.ContainsKey(strListName))
                            {
                                mVariable.typeConverter = m_TypeConverters[strListName];
                            }
                            else
                            {
                                log("ERROR - type converter: " + strListName + " was specified but wasn't found in the dynamic mastercode.cs file.");
                            }
                        }
                    }
                }
            }
        }

		private void _processClassesAndCompile()
		{
            m_strTypeConverters = TypeConverterWriter.createTypeConverters(m_TypeConverters, m_ContentFolders, m_strDatabaseFolder, m_strListsFolder, m_ClassParser.getProjectWrapper());
            _assignTypeConverters();
            foreach (EnumList mEnum in m_ClassParser.getProjectWrapper().enums.Values)
			{
				string strEnum = EnumWriter.writeEnum(mEnum, m_ClassParser.getProjectWrapper());
				m_EnumsAsCode[mEnum] = strEnum;
			}
			foreach (ClassStructure mClass in m_ClassParser.getProjectWrapper().classStructures.Values)
			{
				if (mClass.isSerialized)
				{
					string strClass = ClassWriter.writeClass(this, mClass, m_ClassParser.getProjectWrapper());
					m_ClassesAsCode[mClass] = strClass;
                    m_ClassStructuresByName[mClass.name] = mClass;
				}
			}

			_buildMasterFile();
			_compileCode();			
			m_ClassesCompiled.Clear();
            if (m_CompileResults != null &&
                m_CompileResults.Errors.HasErrors == false)
            {
                foreach (Type mType in m_CompileResults.CompiledAssembly.GetTypes())
                {
                    m_ClassesCompiled[mType.Name] = mType;
                }


                AppDomain.CurrentDomain.Load(m_CompileResults.CompiledAssembly.GetName());
            }
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
                    m_StringWriter.WriteLine(_getHelperFunctions());
                    m_StringWriter.WriteLine(m_strTypeConverters);
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

		private LogFile m_LogFile = null;
		public LogFile logFile 
		{ 
			get 
			{ 
				return m_LogFile; 
			} 
			set 
			{ 
				m_LogFile = value; 
				if(m_LogFile != null)
				{
					m_LogFile.log("registered Class Creator Manager.");					
				}
				variableDefinitionHandler.logFile = logFile;
				
			} 
		}
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
                        try
                        {
                            parms.ReferencedAssemblies.Add(mAssembly.Location);
                        }
                        catch { }
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
using Library.Database;
using Library.UnitType;
using Library;
namespace " + m_strNamespace + @"
{
<CLASSES>
} //end of namespace
";
		}

        private string _getHelperFunctions()
        {
            return @"
        public class HelperFunctions
        {
            static public string makeRelativePath(string strStartingDirectory, string strAbsolutePath)
		    {
			    try
			    {
				    string strFolderRelativeTo = strStartingDirectory;//AppDomain.CurrentDomain.BaseDirectory;
				    string strFullFilePath = strAbsolutePath;
				    Uri pathUri = new Uri(strFullFilePath);
				    // Folders must end in a slash
				    if (!strFolderRelativeTo.EndsWith(Path.DirectorySeparatorChar.ToString()))
				    {
					    strFolderRelativeTo += Path.DirectorySeparatorChar;
				    }
				    Uri folderUri = new Uri(strFolderRelativeTo);
				    return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
			    }
			    catch
			    {

			    }
			    return strAbsolutePath;
		    }
        } //end helper functions
";
        }




    }//end of class

    public class ClassCreatorContentFolders
    {
        public ClassCreatorContentFolders()
        {
            codeName = "";
            path = "";
        }
        [DisplayName("Name"), Description("This is the name you will use in code to bring up the file load dialog. FilePath=\"Content\" or FolderPath=\"Content\" for example")]
        public string codeName { get; set; }
        [DisplayName("Path"), Description("This is the relative path to the folder you want(or full)")]
        public string path { get; set; }

    }//end of class

}//end of namespace
