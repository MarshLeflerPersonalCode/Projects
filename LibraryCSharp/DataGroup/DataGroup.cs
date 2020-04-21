using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Library.IO;


namespace Library
{
	public class DataGroup
	{
		
		private Dictionary<string, DataProperty> m_Properties = new Dictionary<string, DataProperty>();
		private Dictionary<string, DataGroup> m_ChildGroups = new Dictionary<string, DataGroup>();
		public DataGroup() 
		{
			dataGroupName = "";
		}
		public DataGroup(string strName)
		{
			dataGroupName = strName;
		}
		//get/set the name of the datagroup
		public string dataGroupName { get; set; }
		public DataGroup getOrCreateDataGroup(string strDataGroupName)
		{
			if (m_ChildGroups.ContainsKey(strDataGroupName.ToUpper()) == false)
			{
				m_ChildGroups[strDataGroupName.ToUpper()] = new DataGroup(strDataGroupName);
			}
			return m_ChildGroups[strDataGroupName.ToUpper()];
		}
		public DataGroup getChildDataGroup(string strDataGroupName)
		{
			if (m_ChildGroups.ContainsKey(strDataGroupName.ToUpper()) == false)
			{
				return null;
			}
			return m_ChildGroups[strDataGroupName.ToUpper()];
		}
		public DataProperty getOrCreateDataProperty(string strDataProperty)
		{
			if (m_Properties.ContainsKey(strDataProperty.ToUpper()) == false)
			{
				m_Properties[strDataProperty.ToUpper()] = new DataProperty(strDataProperty);				
			}
			return m_Properties[strDataProperty.ToUpper()];
		}
		public DataProperty getDataProperty(string strDataProperty)
		{
			if(m_Properties.ContainsKey(strDataProperty.ToUpper()) == false )
			{
				return null;
			}
			return m_Properties[strDataProperty.ToUpper()];
		}

		public void setProperty(string strPropertyName, bool bValue) { getOrCreateDataProperty(strPropertyName).setProperty(bValue); }
		public void setProperty(string strPropertyName, sbyte iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, byte iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, short iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, ushort iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, int iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, uint iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, long iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, ulong iValue) { getOrCreateDataProperty(strPropertyName).setProperty(iValue); }
		public void setProperty(string strPropertyName, float fValue) { getOrCreateDataProperty(strPropertyName).setProperty(fValue); }
		public void setProperty(string strPropertyName, double fValue) { getOrCreateDataProperty(strPropertyName).setProperty(fValue); }
		public void setProperty(string strPropertyName, string strValue) { getOrCreateDataProperty(strPropertyName).setProperty(strValue); }
        public void setPropertyByPrimitiveType(string strPropertyName, Object mObject) { getOrCreateDataProperty(strPropertyName).setPropertyByPrimitiveType(mObject); }

        //gets/sets the data property name
        public bool getProperty(string strPropertyName, bool bDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null)?mProperty.getAsBool(): bDefauleValue; }
		public sbyte getProperty(string strPropertyName, sbyte iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsInt8() : iDefauleValue; }
		public byte getProperty(string strPropertyName, byte iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsUInt8() : iDefauleValue; }
		public short getProperty(string strPropertyName, short iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsInt16() : iDefauleValue; }
		public ushort getProperty(string strPropertyName, ushort iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsUInt16() : iDefauleValue; }
		public int getProperty(string strPropertyName, int iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsInt32() : iDefauleValue; }
		public uint getProperty(string strPropertyName, uint iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsUInt32() : iDefauleValue; }
		public long getProperty(string strPropertyName, long iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsInt64() : iDefauleValue; }
		public ulong getProperty(string strPropertyName, ulong iDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsUInt64() : iDefauleValue; }
		public float getProperty(string strPropertyName, float fDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsFloat() : fDefauleValue; }
		public double getProperty(string strPropertyName, double fDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? (double)mProperty.getAsFloat() : fDefauleValue; }
		public string getProperty(string strPropertyName, string strDefauleValue) { DataProperty mProperty = getDataProperty(strPropertyName); return (mProperty != null) ? mProperty.getAsString() : strDefauleValue; }


		//returns "" if it was successful, otherwise the error message
		public string saveToFile(string strFile)
		{
			if( dataGroupName == "" )
			{
				dataGroupName = "ROOT";				
			}
			string strOutputText = getDataGroupAsString();
			File.WriteAllText(strFile, strOutputText);
			if( File.Exists(strFile) == false)
			{
				return "Error - wasn't able to write file at location: " + strFile;
			}
			return "";
		}
        public string getDataGroupAsString() { return _getDataGroupAsString(""); }
        private string _getDataGroupAsString(string strTab)
        {
            string strGroupName = dataGroupName;
            string strOutput = strTab + "[" + strGroupName + "]" + Environment.NewLine;
            string strTabForChildren = strTab + "     ";
            foreach (DataProperty mProperty in m_Properties.Values)
            {
                strOutput = strOutput + strTabForChildren + mProperty.getSaveString();
            }
            foreach (DataGroup mDataGroup in m_ChildGroups.Values)
            {
                strOutput = strOutput + mDataGroup._getDataGroupAsString(strTabForChildren);
            }
            strOutput = strOutput + strTab + "[/" + strGroupName + "]" + Environment.NewLine;
            return strOutput;
        }

        //returns "" if it was successful, otherwise the error message
        public string loadFromFile(string strFile)
        {
            if (File.Exists(strFile) == false)
            {
                return "file not found: " + strFile;
            }

            StringReader mFile = new StringReader( File.ReadAllText(strFile));
            string strError = "";
            dataGroupName = "";
            _parseDataGroup(mFile, ref strError);
            return strError;
        }
        public string loadFromString(string strDataGroupContents)
        {
           

            StringReader mFile = new StringReader(strDataGroupContents);
            string strError = "";
            dataGroupName = "";
            _parseDataGroup(mFile, ref strError);
            return strError;
        }
        private bool _parseDataGroup(StringReader mFile, ref string strError )
        {
            bool bHadErrors = false;
            string strLine = null;
            while ((strLine = mFile.ReadLine().Trim()) != null )
            {
                if( strLine.Length == 0)
                {
                    continue;
                }
                if(strLine[0] == '[')
                {
                    if( strLine[1] == '/')
                    {
                        return true;    //we are done reading this group
                    }
                    //new group
                    string strNewGroupName = strLine.Substring(1, strLine.Length -2).Trim();    //minus 2 because it removes the back ]
                    if( strNewGroupName == "" )
                    {
                        strError = strError + "Group name couldn't be parsed: " + strLine + Environment.NewLine;
                        bHadErrors = true;
                        continue;
                    }
                    if( dataGroupName == "")
                    {
                        dataGroupName = strNewGroupName;
                        continue;
                    }
                    DataGroup mChild = getOrCreateDataGroup(strNewGroupName);
                    if( mChild._parseDataGroup(mFile, ref strError) == false )
                    {
                        bHadErrors = true;
                        continue;
                    }
                }
                else if(strLine[0] == '<')
                {
                    DataProperty mNewProperty = DataProperty.createPropertyFromSaveString(strLine);
                    if( mNewProperty == null )
                    {
                        strError = strError + "Unable to parse child value: " + strLine + Environment.NewLine;
                        bHadErrors = true;
                        continue;
                    }
                    m_Properties[mNewProperty.propertyName.ToUpper()] = mNewProperty;
                }
            }			

			return bHadErrors;
		}

		public static DataGroup createFromFile(string strFile, ref string strError)
		{

			DataGroup mDataGroup = new DataGroup();
			strError = mDataGroup.loadFromFile(strFile);
			if( strError != "")
			{
				return null;
			}

			return mDataGroup;
		}

        public static DataGroup createFromString(string strDataGroupContents, ref string strError)
        {

            DataGroup mDataGroup = new DataGroup();
            strError = mDataGroup.loadFromString(strDataGroupContents);
            if (strError != "")
            {
                return null;
            }

            return mDataGroup;
        }




    } //end class
}//end namespace
