using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Library.Database
{
    public class PropertyFilterData
    {
        public PropertyFilterData()
        {
            VariableName = "";
            OverrideName = "";
            ColumnWidth = 60;
        }
        public PropertyFilterData(string strVarName, int columnWidth)
        {
            VariableName = strVarName;
            OverrideName = "";
            ColumnWidth = columnWidth;
        }
        public string VariableName { get; set; }
        public int ColumnWidth { get; set; }
        public string OverrideName { get; set; }
    }
    public class DatabaseConfig
    {
        private string m_strConfigPath = "";
        public DatabaseConfig( )
        {
            
            databaseEntryClass = "unknown";
            databaseName = "unknown";
            // 0x‭8000000‬;
            uniqueMask = 134217728;
            //0xFFFF
            guidMask = 65535;
            propertyFilters = new List<PropertyFilterData>();
            propertyFilters.Add(new PropertyFilterData("m_strName", 120));
            propertyFilters.Add(new PropertyFilterData("m_DatabaseGuid", 80));

        }

        public string getDatabaseConfigPathAndFile() { return m_strConfigPath; }
        public void setDatabaseConfigPathAndFile(string strPath) { m_strConfigPath = strPath; }
        
        [DisplayName("Name"), Description("The name of the database.")]
        public string databaseName { get; set; }

        [DisplayName("Database Entry Class"), Description("The class in which each entry in the database will be created as.")]
        public string databaseEntryClass { get; set; }

        [DisplayName("Unique Mask"), Description("Each Database saves a guid. That guid will have this mask applied to it. For instance 0x‭8000000‬ is the 27th bit as a 1(0x0000‭1000000000000000000000000000‬). This allows for each guid to know what database they are in and can make them unique against other databases entries with the same mask.")]
        public int uniqueMask { get; set; }

        [DisplayName("Guid Mask"), Description("This is the mask in which the guid is actually generated in. Should be a number that represents something like 0xFFFF(65535)")]
        public int guidMask { get; set; }

        [Category("Properties To Filter By"), DisplayName("Variables"), Description("This should be a variable you want to display. This will be the code value. Such as m_strName")]
        public List<PropertyFilterData> propertyFilters { get; set; }




        //load the database - returning any errors.
        public static DatabaseConfig createDatabaseConfigFromJson(Database mDatabase, string strPathToFile)
        {
            
            if (File.Exists(strPathToFile) == false)
            {
                if (mDatabase != null)
                {
                    mDatabase.log("ERROR - in attempting to load database. Database file couldn't be found at location: " + strPathToFile);
                }
                return null;
            }
            try
            {
                Type mType = Type.GetType("Library.Database.DatabaseConfig");
                DatabaseConfig mNewConfig = (DatabaseConfig)JsonConvert.DeserializeObject(File.ReadAllText(strPathToFile), mType);
                if (mNewConfig == null)
                {
                    if (mDatabase != null)
                    {
                        mDatabase.log( "ERROR - Unable to deserialize Database config. File attempting to deserialize is: " + strPathToFile);
                    }

                }
                mNewConfig.propertyFilters.RemoveAt(0); //removes the default m_strName
                mNewConfig.propertyFilters.RemoveAt(0); //removes the default m_Guid
                mNewConfig.setDatabaseConfigPathAndFile(strPathToFile);
                return mNewConfig;
            }
            catch (Exception e)
            {
                if (mDatabase != null)
                {
                    mDatabase.log("ERROR - Unable to deserialize Database config. File attempting to deserialize is: " + strPathToFile + Environment.NewLine + "Error message was: " + e.Message);
                }
            }
            return null;
        }

        public bool saveDatabaseConfigToJson(Database mDatabase)
        {
            return saveDatabaseConfigToJson(mDatabase, m_strConfigPath);
        }
        public bool saveDatabaseConfigToJson(Database mDatabase, string strPathToFile)
        {
            m_strConfigPath = strPathToFile;
          
            try
            {

                string strValue = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(m_strConfigPath, strValue, Encoding.ASCII);
                if (File.Exists(m_strConfigPath) == false)
                {
                    if (mDatabase != null)
                    {
                        mDatabase.log("ERROR - unable to write database config to: " + strPathToFile);
                    }
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                if (mDatabase != null)
                {
                    string strError = "ERROR - in serializing database config. " + Environment.NewLine;
                    if (e.InnerException != null)
                    {
                        strError = e.Message + Environment.NewLine + e.InnerException.Message;
                    }
                    else
                    {
                        strError = e.Message;
                    }
                    mDatabase.log(strError);
                }
                
            }
            return false;
        }

    } //end of class
}//end of namespace
