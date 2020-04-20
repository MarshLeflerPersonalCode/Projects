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
        private void log(LogFile mLog, string strMessage)
        {
            if( mLog != null)
            {
                mLog.log(strMessage);
            }
        }
        private void _serailizeArrayAsDataProperty(object[] mArray, string strPropertyName, LogFile mLogFile)
        {
            if (mArray == null)
            {
                return;
            }

            DataGroup mArrayGroup = getOrCreateDataGroup(strPropertyName);
            mArrayGroup.setProperty("COUNT", (ushort)mArray.Length);
            int iCount = 0;
            foreach (object mObjectInArray in mArray)
            {
                mArrayGroup._serializeObjectAsDataProperty(mObjectInArray, iCount.ToString(), mLogFile);
                iCount++;
            }
        }
        private void _serializeObjectAsDataProperty(Object mObject, string strPropertyName, LogFile mLogFile)
        {
            if( mObject == null )
            {
                return;
            }
            Type mObjectType = mObject.GetType();
            if( mObjectType.IsPrimitive )
            {
                setPropertyByPrimitiveType(strPropertyName, mObject);
                return;
            }
            if(mObjectType.IsEnum)
            {
                string strEnumValue = mObject.ToString();
                setProperty(strPropertyName, strEnumValue);
                return;
            }
            if(mObjectType == Type.GetType("System.String") )
            {
                string strValue = mObject as string;
                setPropertyByPrimitiveType(strPropertyName, mObject);
                return;

            }
            if (mObjectType == Type.GetType("System.DateTime"))
            {
                DateTime mDateTime = (DateTime)mObject;
                setProperty(strPropertyName, mDateTime.Ticks);
                return;

            }

            if (mObjectType.IsArray)
            {
                object[] mArray = mObject as object[];
                _serailizeArrayAsDataProperty(mArray, strPropertyName, mLogFile);
                return;
            }
            IList mList = mObject as IList;
            
            if(mList != null)
            {
                object[] mArray = new object[mList.Count];
                for(int iIndex = 0; iIndex <mList.Count;iIndex++)
                {
                    mArray[iIndex] = mList[iIndex];
                }
                _serailizeArrayAsDataProperty(mArray, strPropertyName, mLogFile);
                return;
            }

            IDictionary mDictionary = mObject as IDictionary;
            if( mDictionary != null)
            {
                DataGroup mDictionaryGroup = getOrCreateDataGroup(strPropertyName);
                mDictionaryGroup.setProperty("COUNT", (ushort)mDictionary.Count);                
                object[] mKeyArray = new object[mDictionary.Count];
                object[] mValueArray = new object[mDictionary.Count];
                ICollection mDictKeys = mDictionary.Keys;
                ICollection mDictValues = mDictionary.Values;
                int iIndex = 0;
                foreach (object mDictKey in mDictKeys)
                {
                    mKeyArray[iIndex++] = mDictKey;
                }
                iIndex = 0;
                foreach (object mDictValue in mDictValues)
                {
                    mValueArray[iIndex++] = mDictValue;
                }
                for (int iCount = 0; iCount < mDictionary.Count; iCount++)
                {
                    DataGroup mKeyValueGroup = mDictionaryGroup.getOrCreateDataGroup(iCount.ToString());
                    mKeyValueGroup._serializeObjectAsDataProperty(mKeyArray[iCount], "key", mLogFile);
                    mKeyValueGroup._serializeObjectAsDataProperty(mValueArray[iCount], "value", mLogFile);
                }
                

                return;
            }
            //at this point it's got to be an object we need to serialize.
            //which means a new group
            DataGroup mArrayGroup = getOrCreateDataGroup(strPropertyName);
            mArrayGroup.serialize(mObject, mLogFile);

        }

        private void _serializeProperty(Object mObject, MemberInfo mMember, PropertyInfo mProperty, LogFile mLogFile)
        {
            Type mPropertyType = mProperty.GetType();
            if (mPropertyType.IsSerializable == false)
            {
                return;
            }
            string strPropertyName = mProperty.Name;
            
            object mPropertyValue = mProperty.GetValue(mObject);
            _serializeObjectAsDataProperty(mPropertyValue, strPropertyName, mLogFile);
        }
        private void _serializeField(Object mObject, MemberInfo mMember, FieldInfo mFieldInfo, LogFile mLogFile)
        {
            if( mFieldInfo.IsNotSerialized)
            {
                return;
            }
            Type mType = mObject.GetType();

            string strFieldName = mFieldInfo.Name;
            if (mType.IsEnum)
            {
                return;
            }
            object mFieldValue = mFieldInfo.GetValue(mObject);
            _serializeObjectAsDataProperty(mFieldValue, strFieldName, mLogFile);
        }
        public void serialize(Object mObject, LogFile mLogFile)
        {
            if(mObject == null )
            {
                log(mLogFile, "ERROR - Unable to serialize object into Data Group. Object was null.");
            }
            
            try
            {
                Type mType = mObject.GetType();
                if( mType.IsSerializable == false )
                {
                    log(mLogFile, "ERROR - Object type: " + mType.Name + " is not serializable.");
                    return;
                }
                if (dataGroupName == "")
                {
                    dataGroupName = mType.Name;
                }
                setProperty("CSHARP", mType.AssemblyQualifiedName);
                MemberInfo[] mMembers = mType.GetMembers();
                
                Type mStringType = Type.GetType("System.String");
                foreach (MemberInfo mMember in mMembers)
                {
                    MemberTypes mMemberTypeInfo = mMember.MemberType;
                    if (mMemberTypeInfo == MemberTypes.Property)
                    {
                        _serializeProperty(mObject, mMember, mMember as PropertyInfo, mLogFile);
                        continue;
                    }
                    else if(mMemberTypeInfo == MemberTypes.Field)
                    {
                        _serializeField(mObject, mMember, mMember as FieldInfo, mLogFile);
                        continue;
                    }
                    else if(mMemberTypeInfo == MemberTypes.Method)
                    {
                        continue;
                    }
                }
                
            }
            catch(Exception e)
            {
                log(mLogFile, "ERROR - Unable to serialize object into Data Group. Exception was: " + e.Message);
            }
            
        }


		


	} //end class
}//end namespace
