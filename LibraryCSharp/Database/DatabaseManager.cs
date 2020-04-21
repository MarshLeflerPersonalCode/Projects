using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.ClassCreator;
using Library.IO;
using System.Threading;
using System.Windows.Forms;

namespace Library.Database
{
    public class DatabaseManager
    {
        private static DatabaseManager g_GlobalDatabaseManager = null;
        private string m_strDirectory = ".\\Databases\\";
        private Mutex m_Mutex = new Mutex();
        private List<Thread> m_Threads = new List<Thread>();
        private Database m_LastDatabase = null;
        public DatabaseManager()
        {
            databases = new List<Database>();
            classCreators = new List<ClassCreatorManager>();
            g_GlobalDatabaseManager = this;
        }
        public static DatabaseManager getDatabaseManager() { return g_GlobalDatabaseManager; }
        public List<Database> databases { get; set; }
        public string getDatabaseDirectory() { return m_strDirectory; }
        //when creating classes if we create dynamic classes we to use the class creator manager
        public List<ClassCreatorManager> classCreators { get; set; }

        public bool createDatabase(string strDatabaseName)
        {
            strDatabaseName = strDatabaseName.Trim();
            foreach (Database mDatabase in databases)
            {
                if( strDatabaseName.ToUpper() == mDatabase.databaseName.ToUpper())
                {
                    log("ERROR - in creating database " + strDatabaseName + ", name is already taken.");
                    return false;
                }
            }
            string strFolderSafeName = strDatabaseName.Replace(" ", "_");
            string strFolderPath = Path.Combine(getDatabaseDirectory(), strFolderSafeName);
            DirectoryInfo mInfo = Directory.CreateDirectory(strFolderPath);
            if( mInfo == null ||
                mInfo.Exists == false )
            {
                log("ERROR - in attempting to create database folder: " + strFolderPath + ".");
                return false;
            }

            Database mNewDatabase = new Database(this);
            mNewDatabase.getConfig().databaseName = strDatabaseName;
            databases.Add(mNewDatabase);
            mNewDatabase.saveDatabase(false);
            return true;

        }

        public void loadConfigsFromDirectory(string strDirectory)
        {
            m_strDirectory = strDirectory;
            m_Mutex.WaitOne();
            try
            {
                databases.Clear();
                string[] mFiles = Directory.GetFiles(strDirectory, "database.json", SearchOption.AllDirectories);
                foreach (string strFile in mFiles)
                {
                    databases.Add(new Database(this));
                }
                for (int iIndex = 0; iIndex < databases.Count; iIndex++)
                {
                    Database mDatabase = databases[iIndex];
                    string strFile = mFiles[iIndex];
                    m_Threads.Add(new Thread(() => _loadDatabase(mDatabase, strFile)));                    
                }
                for (int iIndex = 0; iIndex < m_Threads.Count; iIndex++)
                {
                    m_Threads[iIndex].Start();
                }
                
            }
            catch(Exception e)
            {
                log("Error - attempting to create databases on thread. " + Environment.NewLine + "Exception was: " + e.Message);
            }
            m_Mutex.ReleaseMutex();
        }

        public bool getDatabasesLoaded()
        {
            
            m_Mutex.WaitOne();
            for (int iIndex = 0; iIndex < databases.Count; iIndex++)
            {
                if(databases[iIndex].isLoaded == false )
                {
                    m_Mutex.ReleaseMutex();
                    return false;
                }
            }
            m_Threads.Clear();
            m_Mutex.ReleaseMutex();
            return true;
            
        }

        private void _loadDatabase(Database mDatabase, string strPathAndFile)
        {
            try
            {
                if (mDatabase.loadDatabase(strPathAndFile))
                {
                    log("Successful loaded database " + databases.Last().databaseName + ". Number of entries: " + databases.Last().getEntries().Count.ToString());
                    return;
                }
                else
                {
                    log("Error - unable to load database from file: " + strPathAndFile);
                }
            }
            catch (Exception e)
            {
                log("Error - unable to load database from file: " + strPathAndFile + Environment.NewLine + "Exception was: " + e.Message);
            }
            MessageBox.Show("ERROR IN LOADING DATABASE" + Environment.NewLine + strPathAndFile, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //sets up using the log file to log messages
        public LogFile logFile{get;set;}
        public bool log(string strLogMessage)
        {
            if( logFile != null )
            {
                logFile.log(strLogMessage);
                return true;
            }
            return false;
        }

        public Database getDatabase(string strDatabase)
        {
            if(m_LastDatabase != null &&
                m_LastDatabase.databaseName == strDatabase )
            {
                return m_LastDatabase;
            }
            
            foreach(Database mDatabase in databases)
            {
                if( mDatabase.databaseName == strDatabase )
                {
                    m_LastDatabase = mDatabase;
                    return mDatabase;
                }
            }
            return null;
        }

        public Type getDatabaseEntryType(string strDatabase)
        {
            Database mDatabase = getDatabase(strDatabase);
            if (mDatabase == null ||
              mDatabase.databaseEntryClass == null ||
              mDatabase.databaseEntryClass == "")
            {
                log("ERROR - no database or database entry class supplied to create database entry.");
                return null;
            }
            

            Type mType = Type.GetType(mDatabase.databaseEntryClass);
            if (mType != null)
            {
                return mType;
            }


            foreach (ClassCreatorManager mManager in classCreators)
            {
                mType = mManager.getClassType(mDatabase.databaseEntryClass);
                if (mType != null)
                {
                    return mType;
                }
            }
            log("Error - unable to find class type: " + strDatabase);
            return null;
        }

        public void showConfigDialog(Form mParentForm)
        {
            DatabaseConfigForm mConfigForm = new DatabaseConfigForm(this);
            mConfigForm.ShowDialog(mParentForm);
        }
       


    }
}
