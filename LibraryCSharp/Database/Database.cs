using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Library.ClassCreator;
using Library.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml;

namespace Library.Database
{
    
	public enum EERROR_ADDING
	{
		NO_ERRORS,
		OBJECT_WAS_NULL,
		NAME_ALREADY_TAKEN,
		FILE_NAME_ALREADY_TAKEN,
		GUID_ALREADY_TAKEN,
        NO_GUIDS_AVAILABLE,
        EXCEPTION_THROWN,
        CLASS_TYPE_NOT_FOUND
	};
    public class Database : IClassInstanceCallbacks
    {
        bool m_bIgnorePropertySetCallback = false;
        DatabaseManager m_DatabaseManager = null;
        DatabaseConfig m_DatabaseConfig = new DatabaseConfig();
        
        List<ClassInstance> m_Instances = new List<ClassInstance>();
		Dictionary<int, ClassInstance> m_InstancesByGuid = new Dictionary<int, ClassInstance>();
		Dictionary<string, ClassInstance> m_InstancesByName = new Dictionary<string, ClassInstance>();
		Dictionary<string, ClassInstance> m_InstancesByFileName = new Dictionary<string, ClassInstance>();
        
		public Database(DatabaseManager mDatabaseManager)
		{
            m_DatabaseManager = mDatabaseManager;
            isLoaded = false;
		}
        public bool isLoaded { get; set; }
        //the class representing this database
        public string databaseEntryClass { get { return m_DatabaseConfig.databaseEntryClass; } set { m_DatabaseConfig.databaseEntryClass = value; } }
        //the database name.
        public string databaseName { get { return m_DatabaseConfig.databaseName; } set { m_DatabaseConfig.databaseName = value; } }
        //Each Database saves a guid. That guid will have this mask applied to it. For instance 0x‭8000000‬ is the 27th bit as a 1(0x0000‭1000000000000000000000000000‬). This allows for each guid to know what database they are in and can make them unique against other databases entries with the same mask.
        public int uniqueMask { get { return m_DatabaseConfig.uniqueMask; } set { m_DatabaseConfig.uniqueMask = value; } }
        //This is the mask in which the guid is actually generated in. Should be a number that represents something like 0xFFFF(65535)
        public int guidMask { get { return m_DatabaseConfig.guidMask; } set { m_DatabaseConfig.guidMask = value; } }
        public void _notifyOfPropertySetOnClassInstance(ClassInstance mInstance, string strProperty)
        {
            if(m_bIgnorePropertySetCallback )
            {
                return;
            }
            switch(strProperty)
            {
                case "m_DatabaseGuid":
                {
                    foreach(KeyValuePair<int, ClassInstance> mData in m_InstancesByGuid)
                    {
                        if( mData.Value == mInstance)
                        {
                            m_InstancesByGuid.Remove(mData.Key);
                            m_InstancesByGuid[getEntryGuid(mInstance)] = mInstance;
                        }
                    }
                }
                    break;
                case "m_strName":
                {
                    foreach (KeyValuePair<string, ClassInstance> mData in m_InstancesByName)
                    {
                        if (mData.Value == mInstance)
                        {
                            m_InstancesByName.Remove(mData.Key);
                            m_InstancesByName[getEntryName(mInstance)] = mInstance;
                        }
                    }
                }
                break;
                case "m_strFileName":
                {
                    foreach (KeyValuePair<string, ClassInstance> mData in m_InstancesByFileName)
                    {
                        if (mData.Value == mInstance)
                        {
                            m_InstancesByFileName.Remove(mData.Key);
                            m_InstancesByFileName[getEntryFileName(mInstance)] = mInstance;
                        }
                    }
                }
                break;
            }
        }


        public void removeEntry(ClassInstance mEntry)
        {
            if( mEntry == null)
            {
                return;
            }
            m_Instances.Remove(mEntry);
            m_InstancesByFileName.Remove(getEntryFileName(mEntry));
            m_InstancesByName.Remove(getEntryName(mEntry));
            m_InstancesByGuid.Remove(getEntryGuid(mEntry));
            mEntry.m_CallBacks.Remove(this);
        }

        public string getEntryFileName(ClassInstance mEntry)
        {

            string strFileName = mEntry.getPropertyValue("m_strFileName", "");
            if (strFileName != "")
            {
                return strFileName;
            }
            strFileName = getEntryName(mEntry);
            if( strFileName == "")
            {
                return strFileName;
            }
            return strFileName + ".xml";
        }
        private void _setEntryFileName(ClassInstance mEntry, string strFileName)
        {
            if( strFileName.EndsWith(".xml") == false )
            {
                strFileName = strFileName + ".xml";
            }
            m_bIgnorePropertySetCallback = true;
            mEntry.setProperty("m_strFileName", strFileName);
            m_bIgnorePropertySetCallback = false;
        }
        public string getEntryName(ClassInstance mEntry)
        {
            return mEntry.getPropertyValue("m_strName", "");
        }
        private void _setEntryName(ClassInstance mEntry, string strName)
        {
            m_bIgnorePropertySetCallback = true;
            mEntry.setProperty("m_strName", strName);
            m_bIgnorePropertySetCallback = false;
        }
        public int getEntryGuid(ClassInstance mEntry)
        {
            return mEntry.getPropertyValueInt("m_DatabaseGuid", -1);
        }
        private void _setEntryGuid(ClassInstance mEntry, int iGuid)
        {
            m_bIgnorePropertySetCallback = true;
            iGuid = iGuid & m_DatabaseConfig.uniqueMask; //we always put the guid mask and the unique mask on.
            iGuid = iGuid | m_DatabaseConfig.guidMask;  //we always put the guid mask and the unique mask on.
            mEntry.setProperty("m_DatabaseGuid", iGuid);
            m_bIgnorePropertySetCallback = false;
        }


        private bool _createUniqueGuidForEntry(ClassInstance mEntry)
        {
            int iTries = 1000;
            int iMask = m_DatabaseConfig.guidMask | m_DatabaseConfig.uniqueMask;
            Random mRandom = new Random();
            while(iTries > 0 )
            {
                int iGuid = mRandom.Next() & m_DatabaseConfig.uniqueMask;                
                iGuid = iGuid | m_DatabaseConfig.guidMask;
                ClassInstance mEntryWithGuid = getEntryByGuid(iGuid);
                if( mEntryWithGuid == null )
                {
                    _setEntryGuid(mEntry, iGuid);
                    return true;
                }
            };
            log("WARNING - guid creation took more than 1000 random attempts to find a unique guid.");
            int iStartIndex = mRandom.Next() & m_DatabaseConfig.uniqueMask;
            for ( int iGuidAttempt = iStartIndex; iGuidAttempt < m_DatabaseConfig.uniqueMask; iGuidAttempt++)
            {
                ClassInstance mEntryWithGuid = getEntryByGuid(iGuidAttempt);
                if (mEntryWithGuid == null)
                {
                    _setEntryGuid(mEntry, iGuidAttempt);
                    return true;
                }
            }
            for (int iGuidAttempt = 0; iGuidAttempt < iStartIndex; iGuidAttempt++)
            {
                ClassInstance mEntryWithGuid = getEntryByGuid(iGuidAttempt);
                if (mEntryWithGuid == null)
                {
                    _setEntryGuid(mEntry, iGuidAttempt);
                    return true;
                }
            }
            log("ERROR - unable to find any new guids. Every guid appears to be taken!");
            return false;
        }

        public string getDatabaseConfigPathAndFile() { return m_DatabaseConfig.getDatabaseConfigPathAndFile(); }

        public void cleanDatabase()
        {
            m_Instances.Clear();
            m_InstancesByGuid.Clear();
            m_InstancesByName.Clear();
            m_InstancesByFileName.Clear();            
        }

        //logs a message
        public bool log(string strLogMessage)
        {
            return m_DatabaseManager.log(strLogMessage);
        }

        public List<ClassInstance> getEntries()
		{
			return m_Instances;
		}

		//returns the instance by name
		public ClassInstance getEntryByName(string strName)
		{
            if (m_InstancesByName.ContainsKey(strName.ToUpper()))
            {
                return m_InstancesByName[strName.ToUpper()];
            }
            return null;
		}

		public ClassInstance getEntryByGuid(int iGuid)
		{
            iGuid = iGuid & m_DatabaseConfig.uniqueMask;
            iGuid = iGuid | m_DatabaseConfig.guidMask;
            if( m_InstancesByGuid.ContainsKey(iGuid))
            {
                return m_InstancesByGuid[iGuid];
            }
            return null;
		}
		public ClassInstance getEntryByFileName(string strFileName)
		{
            if (m_InstancesByFileName.ContainsKey(strFileName.ToUpper()))
            {
                return m_InstancesByFileName[strFileName.ToUpper()];
            }
            return null;
		}

        public string strCreateUniqueName(string strUniqueName)
        {
            int iCount = 0;
            while(true)
            {
                if(getEntryByName(databaseName + "_entry" + iCount.ToString()) == null)
                {
                    return databaseName + "_entry" + iCount.ToString();
                }
                iCount++;
            }
        }

        public EERROR_ADDING createEntry(string strUniqueName)
        {
            Type mType = m_DatabaseManager.getDatabaseEntryType(databaseName);
            if (mType == null)
            {
                log("Unable to create classes(" + databaseEntryClass + ") for database " + databaseName + ". Class type not found.");                
                return EERROR_ADDING.CLASS_TYPE_NOT_FOUND;
            }
            if (getEntryByName(strUniqueName) != null)
            {
                return EERROR_ADDING.NAME_ALREADY_TAKEN;
            }
            try
            {
             
                object mObjectCreated = Activator.CreateInstance(mType);
                ClassInstance mInstance = mObjectCreated as ClassInstance;
                _setEntryName(mInstance, strUniqueName);
                _setEntryFileName(mInstance, getEntryFileName(mInstance));
                if(_createUniqueGuidForEntry(mInstance) == false )
                {
                    return EERROR_ADDING.NO_GUIDS_AVAILABLE;
                }
                EERROR_ADDING eError = addEntry(mInstance);
                if (eError == EERROR_ADDING.NO_ERRORS)
                {
                    mInstance.m_bIsDirty = true;
                }

                return eError;

            }
            catch (Exception e)
            {
                log("Error in creating class: " + mType.Name + ". Exception thrown was: " + e.Message);
            }
            return EERROR_ADDING.EXCEPTION_THROWN;
        }

		public EERROR_ADDING addEntry(ClassInstance mNewEntry)
		{          
			if(mNewEntry == null)
			{
                log("ERROR - unable to add entry. Entry was null. Database: " + databaseName);
				return EERROR_ADDING.OBJECT_WAS_NULL;
			}
            if( getEntryByName(getEntryName(mNewEntry)) != null )
            {
                log("ERROR - unable to add entry. Entry name is not unique. Name specified: " + getEntryName(mNewEntry) + ".  Database: " + databaseName);
                return EERROR_ADDING.NAME_ALREADY_TAKEN;
            }
            int iGuid = getEntryGuid(mNewEntry);
            if (iGuid == -1 || iGuid == 0 || getEntryByGuid(iGuid) != null)
            {
                log("ERROR - unable to add entry. Entry guid is not unique or valid. Guid specified: " + iGuid.ToString() + ".  Database: " + databaseName);
                return EERROR_ADDING.GUID_ALREADY_TAKEN;
            }            
            mNewEntry.m_LogFile = m_DatabaseManager.logFile;
            mNewEntry.m_CallBacks.Add(this);
            m_Instances.Add(mNewEntry);            
            m_InstancesByGuid[getEntryGuid(mNewEntry)] = mNewEntry;
            m_InstancesByName[getEntryName(mNewEntry)] = mNewEntry;
            m_InstancesByFileName[getEntryFileName(mNewEntry)] = mNewEntry;
            
            return EERROR_ADDING.NO_ERRORS;
		}

        public bool loadDatabase(string strPathToFile)
        {
            isLoaded = false;
            DatabaseConfig mNewConfig = DatabaseConfig.createDatabaseConfigFromXML(this, strPathToFile);
            if(mNewConfig != null)
            {
                m_DatabaseConfig = mNewConfig;
                _loadData();
            }
            return false;
        }
        public bool saveDatabase()
        {
            return m_DatabaseConfig.saveDatabaseConfigToXML(this);
        }



        private void _loadData()
        {
            cleanDatabase();
            Type mType = m_DatabaseManager.getDatabaseEntryType(databaseName);
            if (mType == null )
            {
                log("Unable to create classes(" + databaseEntryClass + ") for database " + databaseName + ". Class type not found.");
                isLoaded = true;
                return;
            }

            string strPathToSearch = Path.GetFullPath(getDatabaseConfigPathAndFile());
            string[] mFiles = Directory.GetFiles(strPathToSearch);
            System.Xml.Serialization.XmlSerializer mXmlSerailizer = new System.Xml.Serialization.XmlSerializer(mType);
            foreach (string strFile in mFiles)
            {
                try
                {
                    
                    
                    ClassInstance mEntry = (ClassInstance)mXmlSerailizer.Deserialize(File.OpenRead(strFile));
                    if (mEntry == null)
                    {
                        log("ERROR - Unable to deserialize file: " + strFile);
                    }
                    addEntry(mEntry);
                    _setEntryFileName(mEntry, Path.GetFileName(strFile));
                    mEntry.m_bIsDirty = false;
                }
                catch (Exception e)
                {
                        log("ERROR - Unable to deserialize Database config. File attempting to deserialize is: " + strFile + Environment.NewLine + "Error message was: " + e.Message); 
                }
            }
            isLoaded = true;
        }

    } //end class
}//end namespace
