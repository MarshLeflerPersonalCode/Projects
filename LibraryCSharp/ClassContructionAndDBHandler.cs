using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using Library.ClassCreator;
using Library.ClassParser;
using Library.Database;
using Library.IO;
using Library.UnitType;
using Newtonsoft.Json;

namespace Library
{

    //This class handles loading and parsing of c++ classes into c# classes.
    //It then loads all the databases specified in the database command line.
    //Lastly it also helps by creating the log.
    public class ClassContructionAndDBHandler
    {
        enum ELOAD_STATE
        {
            Configure,
            WaitForClassStructuresToBeBuilt,
            CompileCSharpClasses,
            WaitForDatabasesToLoad,
            Done
        };
        private ProgressBar m_ProgressBar = null;
        private ClassConstructionAndDBHandlerConfig m_Config = null;
        private ELOAD_STATE m_eLoadState = ELOAD_STATE.Configure;
        private LogFile m_LogFile = null;
        private ClassCreatorManager m_ClassCreatorManager = new ClassCreatorManager();
        private ClassParserManager m_ClassParser = new ClassParserManager();
        private DatabaseManager m_DatabaseManager = new DatabaseManager();
        private UnitTypeManager m_UnitTypeManager = new UnitTypeManager();
        
        
        public ClassContructionAndDBHandler()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            m_LogFile = new LogFile("./StatEditor.log");
            m_ClassParser.logFile = m_LogFile;
            m_ClassCreatorManager.logFile = m_LogFile;
            m_DatabaseManager.logFile = m_LogFile;
            m_DatabaseManager.classCreators.Add(m_ClassCreatorManager);
        }

        public void showProgressBar(Form mParentForm)
        {
            m_ProgressBar = new ProgressBar();
            m_ProgressBar.setEstimated(99999);
            if (mParentForm != null)
            {
                m_ProgressBar.ShowDialog(mParentForm);
            }
            else
            {
                m_ProgressBar.ShowDialog();
            }

        }

        
        //returns if everything is loaded or not
        public bool isLoaded()
        {
            if( m_eLoadState != ELOAD_STATE.Done)
            {
                _onTimedEvent();
            }
            return m_eLoadState == ELOAD_STATE.Done;
        }


        public ClassCreatorManager classCreatorManager { get { return m_ClassCreatorManager; } }
        public ClassParserManager classParserManager { get { return m_ClassParser; } }
        public DatabaseManager databaseManager { get { return m_DatabaseManager; } }

        public LogFile getLogFile() { return m_LogFile;  }
        public UnitTypeManager unitTypeManager
        {
            get { return m_UnitTypeManager; }
        }

        //logs a message
        public void log(string strMessage)
        {
            m_LogFile.log(strMessage);
        }
      
        private void _onTimedEvent()
        {
            switch (m_eLoadState)
            {
                case ELOAD_STATE.Configure:
                {
                    _loadConfig();
                    _findClasses();
                    m_ClassParser.compareToCachedData("");
                    m_ClassParser.buildClassStructures();
                    m_eLoadState = ELOAD_STATE.WaitForClassStructuresToBeBuilt;
                }
                break;               
                case ELOAD_STATE.WaitForClassStructuresToBeBuilt:
                {
                    if (m_ClassParser.getDoneParsingClassesOnThreads())
                    {
                        m_ClassParser.saveCachedData("classparser.cache");
                        m_ClassCreatorManager.intialize(m_ClassParser, "classcompile.dll");
                        m_eLoadState = ELOAD_STATE.CompileCSharpClasses;
                    }
                }
                break;
                case ELOAD_STATE.CompileCSharpClasses:
                {
                    if (m_ClassCreatorManager.isDoneParsingAndCompiling())
                    {
                        m_eLoadState = ELOAD_STATE.WaitForDatabasesToLoad;
                        m_DatabaseManager.loadConfigsFromDirectory(m_Config.databaseFolder);
                        string strErrors = m_ClassCreatorManager.getErrorsAsString();
                        if (strErrors != null && strErrors.Length != 0)
                        {
                            MessageBox.Show( "Error in Compiling. Check Log for more details.\n\n" + strErrors, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                break;
                case ELOAD_STATE.WaitForDatabasesToLoad:
                    if( m_DatabaseManager.getDatabasesLoaded())
                    {
                        if (m_ProgressBar != null)
                        {
                            m_Config.estimatedLoadTime = (float)m_ProgressBar.getTime();
                            m_ProgressBar.Hide();
                            m_ProgressBar = null;
                        }
                        m_eLoadState = ELOAD_STATE.Done;
                    }
                    break;
                case ELOAD_STATE.Done:
                {
                    
                }
                break;
            }
        }

        public void _loadConfig()
        {
            string strError = "";
            m_Config = ClassConstructionAndDBHandlerConfig.create();
            
            if(m_Config == null)
            {
                m_Config = new ClassConstructionAndDBHandlerConfig();

                log("ERROR - unable to load toolsconfig.xml. Error was: " + strError + Environment.NewLine + "Creating new toolsconfig.xml with defaults.");
                m_Config.save();
            }
            m_ProgressBar.setEstimated(m_Config.estimatedLoadTime);
        }

        private void _findClasses()
        {
            try
            {
                string[] mFiles = Directory.GetFiles(m_Config.sourcePath, "*.h", SearchOption.AllDirectories);
                if (mFiles == null ||
                    mFiles.Count() == 0)
                {
                    log("ERROR - unable to find any source files at: " + m_Config.sourcePath + Environment.NewLine + "Full path is: " + Path.GetFullPath(m_Config.sourcePath));
                    return;
                }
                foreach (string mFile in mFiles)
                {
                    m_ClassParser.addFileToParse(mFile);
                }
                log("Processing " + mFiles.Length + " header files.");
            }
            catch
            {
                log("ERROR - unable to find any source files at: " + m_Config.sourcePath + Environment.NewLine + "Full path is: " + Path.GetFullPath(m_Config.sourcePath));
            }
        }

        public void showEditorConfigForm(Form mParent)
        {
            ClassConstructionAndDBHandlerForm mConfigForm = new ClassConstructionAndDBHandlerForm(m_Config);
            mConfigForm.ShowDialog(mParent);
        }

    }//end class


    public class ClassConstructionAndDBHandlerConfig
    {
        private string m_strFileNameAndPath = "ToolsConfig.json";
        public ClassConstructionAndDBHandlerConfig()
        {
            sourcePath = "..\\CoreClasses\\";
            databaseFolder = ".\\Databases\\";
            estimatedLoadTime = 1.0f;
        }
        [DisplayName("Source Path"), Description("The relative path to the source folder")]
        public string sourcePath { get; set; }
        [DisplayName("DB Folder"), Description("The folder in which all the databases will create unique folders to store their entries.")]
        public string databaseFolder { get; set; }
        [Browsable(false)]
        public float estimatedLoadTime { get; set; }


        public string getFileName() { return m_strFileNameAndPath; }
        public void setFileName(string strFileNameAndPath) { m_strFileNameAndPath = strFileNameAndPath; }

        public static ClassConstructionAndDBHandlerConfig create()
        {
            return create("ToolsConfig.json");
        }
        public static ClassConstructionAndDBHandlerConfig create(string strPath)
        {
            
            Type mType = Type.GetType("Library.ClassConstructionAndDBHandlerConfig");                        
            try
            {
                if (File.Exists(strPath))
                {
                    ClassConstructionAndDBHandlerConfig mConfig = JsonConvert.DeserializeObject(File.ReadAllText(strPath), mType) as ClassConstructionAndDBHandlerConfig;
                    if (mConfig != null)
                    {
                        mConfig.setFileName(strPath);
                        return mConfig;
                    }
                }
                
            }
            catch
            {
                
            }
            return null;
        }
        public bool save()
        {
            return save(getFileName());
        }

        public bool save(string strPath)
        {
            try
            {
                string strValue = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(strPath, strValue);
                if (File.Exists(strPath) == false)
                {
                    return false;
                }
                return true;
            }
            catch
            {               
            }
            return false;
        }
    }//end config file
}//end namespace
