using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Library.IO;

namespace Library
{
    public interface IDataGroupConvertTypeRecast
    {
        Type _dataGroupTypeRecast(Type mTypeRequest);
    }
    public class DataGroupConvert
    {

        public class DataGroupConvertParams
        {
            public DataGroupConvertParams m_Parent = null;
            public DataGroup m_DataGroup = null;
            public object m_Object = null;
            public MemberInfo m_MemberInfo = null;
            public PropertyInfo m_PropertyInfo = null;
            public FieldInfo m_FieldInfo = null;
            public string m_strCurrentPropertyOrFieldName = "";
        };
        #region STATIC_FUNCTIONS
        public static DataGroup serializeIntoDataGroup(Object mObject, LogFile mLogFile)
        {
            ClassInstance mParentOfObject = null;
            ClassInstance mInstanceSaving = mObject as ClassInstance;
            if (mInstanceSaving != null)
            {
                mParentOfObject = mInstanceSaving.getOwneringClass();
                mInstanceSaving.setOwningClass(null);
            }

            DataGroup mDataGroup = new DataGroup();
            DataGroupConvert mDataGroupConvert = new DataGroupConvert();
            mDataGroupConvert.logFile = mLogFile;
            mDataGroupConvert.serializeIntoDataGroup(mDataGroup, mObject);

            if (mParentOfObject != null)
            {
                mInstanceSaving.setOwningClass(mParentOfObject);
            }
            return mDataGroup;

        }
        public static string serializeObjectIntoString(Object mObject, LogFile mLogFile)
        {

            return serializeIntoDataGroup(mObject, mLogFile).getDataGroupAsString();

        }
        public static object deserializeObjectFromFile(string strPathToFile, Type mType)
        {
            return deserializeObjectFromFile(strPathToFile, mType, null, null);
        }
        public static object deserializeObjectFromFile(string strPathToFile, Type mType, LogFile mLogFile)
        {
            return deserializeObjectFromFile(strPathToFile, mType, mLogFile, null);
        }
        public static object deserializeObjectFromFile(string strPathToFile, Type mType, LogFile mLogFile, IDataGroupConvertTypeRecast mRecastInterface)
        {
            try
            {
                if (strPathToFile == null ||
                    strPathToFile == "")
                {
                    if (mLogFile != null)
                    {
                        mLogFile.log("ERROR - file path was empty. Unable to deserialize.");
                    }
                    return null;
                }
                if (File.Exists(strPathToFile))
                {
                    return deserializeObjectFromString(File.ReadAllText(strPathToFile), mType, mLogFile, mRecastInterface);
                }
                if (mLogFile != null)
                {
                    mLogFile.log("ERROR - unable to deserialize because file doesn't exist at: " + strPathToFile);
                }
                return null;
            }
            catch (Exception e)
            {
                if (mLogFile != null)
                {
                    mLogFile.log("ERROR - unable to deserialize object from string. Exception was: " + e.Message);
                }
            }
            return null;
        }
        public static object deserializeObjectFromString(string strDataGroup, Type mType)
        {
            return deserializeObjectFromString(strDataGroup, mType, null, null);
        }
        public static object deserializeObjectFromString(string strDataGroup, Type mType, LogFile mLogFile)
        {
            return deserializeObjectFromString(strDataGroup, mType, mLogFile, null);
        }
        public static object deserializeObjectFromString(string strDataGroup, Type mType, LogFile mLogFile, IDataGroupConvertTypeRecast mRecastInterface)
        {
            try
            {
                if (strDataGroup == null ||
                    strDataGroup == "")
                {
                    if (mLogFile != null)
                    {
                        mLogFile.log("ERROR - string was empty. Unable to deserialize.");
                    }
                    return null;
                }
                if (mType == null)
                {
                    if (mLogFile != null)
                    {
                        mLogFile.log("ERROR - type was null. Unable to deserialize.");
                    }
                    return null;
                }
                string strError = "";
                DataGroup mDataGroup = DataGroup.createFromString(strDataGroup, ref strError);
                if (strError != "")
                {
                    if (mLogFile != null)
                    {
                        mLogFile.log("ERROR - unable to deserialize object from string. " + Environment.NewLine + strError);
                    }
                    return null;
                }
                return deserializeObjectFromDataGroup(mDataGroup, mType, mLogFile, mRecastInterface);
            }
            catch (Exception e)
            {
                if (mLogFile != null)
                {
                    mLogFile.log("ERROR - unable to deserialize object from string. Exception was: " + e.Message);
                }
            }
            return null;
        }
        public static object deserializeObjectFromDataGroup(DataGroup mDataGroup, Type mType)
        {
            return deserializeObjectFromDataGroup(mDataGroup, mType, null, null);
        }
        public static object deserializeObjectFromDataGroup(DataGroup mDataGroup, Type mType, LogFile mLogFile)
        {
            return deserializeObjectFromDataGroup(mDataGroup, mType, mLogFile, null);
        }
        public static object deserializeObjectFromDataGroup(DataGroup mDataGroup, Type mType, LogFile mLogFile, IDataGroupConvertTypeRecast mRecastInterface)
        {

            try
            {
                if (mDataGroup == null)
                {
                    if (mLogFile != null)
                    {
                        mLogFile.log("ERROR - datagroup was null");
                    }
                    return null;
                }
                if (mType == null)
                {
                    if (mLogFile != null)
                    {
                        mLogFile.log("ERROR - type was null");
                    }
                    return null;
                }

                object mObject = Activator.CreateInstance(mType);
                DataGroupConvert dataGroupConvert = new DataGroupConvert();
                dataGroupConvert.logFile = mLogFile;
                dataGroupConvert.typeCastInterface = mRecastInterface;
                if (dataGroupConvert.deserializeObjectFromDataGroup(mObject, mDataGroup))
                {
                    return mObject;
                }
            }
            catch (Exception e)
            {
                if (mLogFile != null)
                {
                    mLogFile.log("ERROR - there was an error deserializing data group. Exception was: " + e.Message);
                }
            }
            return null;
        }
        #endregion
        #region HELPER_FUNCTIONS
        public IDataGroupConvertTypeRecast typeCastInterface { get; set; }
        public Type _recastType(Type mType )
        {
            if(typeCastInterface != null)
            {
                return typeCastInterface._dataGroupTypeRecast(mType);
            }
            return mType;
        }
        private Type _getTypeForDataGroup(DataGroup mDataGroup)
        {


            string strValue = mDataGroup.getProperty("CSHARP", "");
            if (strValue != "")
            {
                return Type.GetType(strValue);
            }
            return null;
        }
        private Type _getTypeForChildDataGroup(DataGroup mDataGroup, string strChildName)
        {

            DataGroup mChild = mDataGroup.getChildDataGroup(strChildName);
            if (mChild != null)
            {
                return _getTypeForDataGroup(mChild);
            }

            return null;
        }
        private Type _getListType(Type mTypeOfList)
        {
            var enumerable_type = mTypeOfList
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GenericTypeArguments.Length == 1)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerable_type != null)
                return enumerable_type.GenericTypeArguments[0];

            return null;
        }
        private Type _getListType(IList myList)
        {
            Type[] mTypes = myList.GetType().GetGenericArguments();
            if (mTypes != null &&
                mTypes.Length > 0)
            {
                return mTypes.Single();
            }
            Type mType = _getListType(myList);

            if (mType != null)
                return mType;

            if (myList.Count == 0)
                return null;

            return myList[0].GetType();
        }
        private void log(string strMessage)
        {
            if (logFile != null)
            {
                logFile.log(strMessage);
            }
        }
        private LogFile logFile { get; set; }
        #endregion
        #region SERIALIZE
        public void serializeIntoDataGroup(DataGroup mDataGroup, object mObject)
        {
            serializeIntoDataGroup(null, mDataGroup, mObject);
        }
        public void serializeIntoDataGroup(DataGroupConvertParams mParentParams, DataGroup mDataGroup, object mObject)
        {

            try
            {
                if (mObject == null)
                {
                    log("ERROR - Unable to serialize object into Data Group. Object was null.");
                    return;
                }
                if (mDataGroup == null)
                {
                    log("ERROR - Data Group was null.");
                    return;
                }
                DataGroupConvertParams mParams = new DataGroupConvertParams();
                mParams.m_Parent = mParentParams;
                mParams.m_DataGroup = mDataGroup;
                mParams.m_Object = mObject;
                Type mType = mObject.GetType();
                if (mDataGroup.dataGroupName == "")
                {
                    mDataGroup.dataGroupName = mType.Name;
                }
                
                MemberInfo[] mMembers = mType.GetMembers();

                Type mStringType = Type.GetType("System.String");
                foreach (MemberInfo mMember in mMembers)
                {
                    mParams.m_MemberInfo = mMember;
                    mParams.m_strCurrentPropertyOrFieldName = mMember.Name;
                    MemberTypes mMemberTypeInfo = mMember.MemberType;
                    if (mMemberTypeInfo == MemberTypes.Property)
                    {
                        mParams.m_PropertyInfo = mMember as PropertyInfo;
                        _serializeProperty(mParams);
                        continue;
                    }
                    else if (mMemberTypeInfo == MemberTypes.Field)
                    {
                        mParams.m_FieldInfo = mMember as FieldInfo;
                        _serializeField(mParams);
                        continue;
                    }
                    else if (mMemberTypeInfo == MemberTypes.Method)
                    {
                        continue;
                    }
                }

            }
            catch (Exception e)
            {
                log( "ERROR - Unable to serialize object into Data Group. Exception was: " + e.Message);
            }


        }

        private bool _serailizeArrayAsDataProperty(DataGroupConvertParams mParams, DataGroup mParentDataGroup, object mObject, string strPropertyName)
        {

            Type mObjectType = mObject.GetType();
            if (mObjectType.IsArray == false)
            {
                return false;
            }
            object[] mArray = null;
            try
            {
                mArray = mObject as object[];
            }
            catch { }

            if (mArray == null)
            {
                log("ERROR - in attempting to cast array to object array.");
                return true;
            }

            DataGroup mArrayGroup = mParams.m_DataGroup.getOrCreateDataGroup(strPropertyName);
            mArrayGroup.setProperty("COUNT", (ushort)mArray.Length);
            Type mElementType = mArray.GetType().GetElementType();
            int iCount = 0;
            foreach (object mObjectInArray in mArray)
            {
                if (mObjectInArray == null)
                {
                    continue;
                }
                _serializeObjectAsDataProperty(mParams, mArrayGroup, mObjectInArray, iCount.ToString());
                DataGroup mChild = mArrayGroup.getChildDataGroup(iCount.ToString());
                if (mChild == null)
                {
                    continue;
                }
                if (mObjectInArray.GetType() != mElementType)
                {
                    mChild.setProperty("CSHARP", mObjectInArray.GetType().AssemblyQualifiedName);
                }
                iCount++;
            }
            if (iCount == 0)    //everything is default so we don't need to save.
            {
                mParentDataGroup.deleteChildDataGroup(mArrayGroup);
            }
            else
            {
                mArrayGroup.setProperty("COUNT", (ushort)iCount);
            }
            return true;
        }

        private bool _serailizeListAsDataGroup(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, object mObject, string strPropertyName)
        {
            IList mList = mObject as IList;
            if (mList == null)
            {
                return false;
            }

            DataGroup mArrayGroup = mActiveDataGroup.getOrCreateDataGroup(strPropertyName);
            mArrayGroup.setProperty("COUNT", (ushort)mList.Count);
            Type mElementType = _getListType(mList);
            int iCount = 0;
            foreach (object mObjectInArray in mList)
            {
                if (mObjectInArray == null)
                {
                    continue;
                }
                _serializeObjectAsDataProperty(mParams, mArrayGroup, mObjectInArray, iCount.ToString());
                DataGroup mChild = mArrayGroup.getChildDataGroup(iCount.ToString());
                if (mChild == null)
                {
                    continue;
                }
                if (mObjectInArray.GetType() != mElementType)
                {
                    mChild.setProperty("CSHARP", mObjectInArray.GetType().AssemblyQualifiedName);
                }
                iCount++;
            }
            if (iCount == 0)    //it's empty so we don't need to save it.
            {
                mActiveDataGroup.deleteChildDataGroup(mArrayGroup);
            }
            else
            {
                mArrayGroup.setProperty("COUNT", (ushort)iCount);
            }
            return true;
        }

        private bool _serializeDictionaryAsDataGroup(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, object mObject, string strPropertyName)
        {
            IDictionary mDictionary = mObject as IDictionary;
            if (mDictionary != null)
            {
                Type[] mKeyAndValueTypes = mObject.GetType().GetGenericArguments();
                DataGroup mDictionaryGroup = mActiveDataGroup.getOrCreateDataGroup(strPropertyName);
                mDictionaryGroup.setProperty("COUNT", (ushort)mDictionary.Count);
                object[] mKeyArray = new object[mDictionary.Count];
                object[] mValueArray = new object[mDictionary.Count];
                ICollection mDictKeys = mDictionary.Keys;
                ICollection mDictValues = mDictionary.Values;
                Type mKeyType = null;
                Type mValueType = null;
                if (mKeyAndValueTypes != null &&
                    mKeyAndValueTypes.Count() == 2)
                {
                    mKeyType = mKeyAndValueTypes[0];
                    mValueType = mKeyAndValueTypes[1];
                }
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
                    _serializeObjectAsDataProperty(mParams, mKeyValueGroup, mKeyArray[iCount], "KEY");
                    if (mKeyArray[iCount] != null &&
                        mKeyArray[iCount].GetType() != mKeyType)
                    {
                        DataGroup mChild = mKeyValueGroup.getChildDataGroup("key");
                        if (mChild != null)
                        {
                            mChild.setProperty("CSHARP", mKeyArray[iCount].GetType().AssemblyQualifiedName);
                        }
                    }
                    _serializeObjectAsDataProperty(mParams, mKeyValueGroup, mValueArray[iCount], "VALUE");
                    if (mValueArray[iCount] != null &&
                        mValueArray[iCount].GetType() != mValueType)
                    {
                        DataGroup mChild = mKeyValueGroup.getChildDataGroup("value");
                        if (mChild != null)
                        {
                            mChild.setProperty("CSHARP", mValueArray[iCount].GetType().AssemblyQualifiedName);
                        }
                    }
                }


                return true;
            }
            return false;
        }

        private void _serializeObjectAsDataProperty(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, Object mObject, string strPropertyName)
        {
            if (mObject == null)
            {
                return;
            }
            Type mObjectType = mObject.GetType();
            if (mObjectType.IsPrimitive)
            {
                mActiveDataGroup.setPropertyByPrimitiveType(strPropertyName, mObject);
                return;
            }
            if (mObjectType.IsEnum)
            {
                string strEnumValue = mObject.ToString();
                mActiveDataGroup.setProperty(strPropertyName, strEnumValue);
                return;
            }
            if (mObjectType == Type.GetType("System.String"))
            {
                string strValue = mObject as string;
                mActiveDataGroup.setPropertyByPrimitiveType(strPropertyName, mObject);
                return;

            }
            if (mObjectType == Type.GetType("System.DateTime"))
            {
                DateTime mDateTime = (DateTime)mObject;
                mActiveDataGroup.setProperty(strPropertyName, mDateTime.Ticks);
                return;

            }
            if (_serailizeArrayAsDataProperty(mParams, mActiveDataGroup, mObject, strPropertyName))
            {
                return;
            }

            if (_serailizeListAsDataGroup(mParams, mActiveDataGroup, mObject, strPropertyName))
            {
                return;
            }
            if (_serializeDictionaryAsDataGroup(mParams, mActiveDataGroup, mObject, strPropertyName))
            {
                return;
            }

            //at this point it's got to be an object we need to serialize.
            //which means a new group
            DataGroup mNewObjectDataGroup = mActiveDataGroup.getOrCreateDataGroup(strPropertyName);
            serializeIntoDataGroup(mParams, mNewObjectDataGroup, mObject);
            if (mNewObjectDataGroup.isEmpty())   //everything is default no reason to save.
            {
                mActiveDataGroup.deleteChildDataGroup(mNewObjectDataGroup);
            }
        }

        private void _serializeProperty(DataGroupConvertParams mParams)
        {
            Type mPropertyType = mParams.m_PropertyInfo.GetType();
            if (mPropertyType.IsSerializable == false ||
                mParams.m_PropertyInfo.CanWrite == false) //if we can't set the value we don't save it.
            {
                return;
            }
            string strPropertyName = mParams.m_PropertyInfo.Name;
            object mPropertyValue = mParams.m_PropertyInfo.GetValue(mParams.m_Object);
            PropertyInfo mDefaultProperty = mParams.m_Object.GetType().GetProperty(strPropertyName + "_Default");
            if (mDefaultProperty != null)
            {
                object mDefaultObjectProperty = mDefaultProperty.GetValue(mParams.m_Object);
                if (mDefaultObjectProperty != null)
                {
                    if (mDefaultObjectProperty.ToString() == mPropertyValue.ToString())
                    {
                        return; //we don't save this!
                    }
                }
            }

            if (mPropertyValue == null)
            {
                if (mParams.m_PropertyInfo.PropertyType == Type.GetType("System.String"))
                {
                    string strObject = "";
                    _serializeObjectAsDataProperty(mParams, mParams.m_DataGroup, strObject, strPropertyName);
                }
                return;
            }
            _serializeObjectAsDataProperty(mParams, mParams.m_DataGroup, mPropertyValue, strPropertyName);
        }
        private void _serializeField(DataGroupConvertParams mParams)
        {
            if (mParams.m_FieldInfo.IsNotSerialized)
            {
                return;
            }
            Type mType = mParams.m_Object.GetType();


            if (mType.IsEnum)
            {
                return;
            }
            string strFieldName = mParams.m_FieldInfo.Name;
            object mFieldValue = mParams.m_FieldInfo.GetValue(mParams.m_Object);
            _serializeObjectAsDataProperty(mParams, mParams.m_DataGroup, mFieldValue, strFieldName);
        }

        #endregion
        #region DESERIALIZE
        public bool deserializeObjectFromDataGroup(Object mObject, DataGroup mDataGroup)
        {
            return deserializeObjectFromDataGroup(null, mObject, mDataGroup);
        }
        public bool deserializeObjectFromDataGroup(DataGroupConvertParams mParentParams, Object mObject, DataGroup mDataGroup)
        {
            
            if (mDataGroup == null)
            {
                log("ERROR - datagroup was null");
                return false;
            }
            if (mObject == null)
            {
                log( "ERROR - object was null");
                return false;
            }

            try
            {
                DataGroupConvertParams mParams = new DataGroupConvertParams();
                mParams.m_Parent = mParentParams;
                mParams.m_DataGroup = mDataGroup;
                mParams.m_Object = mObject;
                Type mType = mObject.GetType();
                MemberInfo[] mMembers = mType.GetMembers();

                Type mStringType = Type.GetType("System.String");
                foreach (MemberInfo mMember in mMembers)
                {
                    mParams.m_MemberInfo = mMember;
                    mParams.m_strCurrentPropertyOrFieldName = mMember.Name;
                    MemberTypes mMemberTypeInfo = mMember.MemberType;
                    if (mMemberTypeInfo == MemberTypes.Property)
                    {
                        mParams.m_PropertyInfo = mMember as PropertyInfo;
                        _deserializeProperty(mParams);
                        continue;
                    }
                    else if (mMemberTypeInfo == MemberTypes.Field)
                    {
                        mParams.m_FieldInfo = mMember as FieldInfo;
                        _deserializeField(mParams);
                        continue;
                    }
                    else if (mMemberTypeInfo == MemberTypes.Method)
                    {
                        continue;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                log( "ERROR - there was an error deserializing data group into object type: " + mObject.GetType().Name + " Exception was: " + e.Message);
            }
            return false;
        }
        





        
        private void _deserializeProperty(DataGroupConvertParams mParams)
        {
            Type mPropertyType = mParams.m_PropertyInfo.GetType();
            if (mPropertyType.IsSerializable == false)
            {
                return;
            }
            string strPropertyName = mParams.m_PropertyInfo.Name;   //testing
            try
            {
                object mValueToSet = _deserializeObjectFromDataGroup(mParams, mParams.m_DataGroup, mParams.m_PropertyInfo.PropertyType, strPropertyName);
                if (mValueToSet != null)
                {
                    mParams.m_PropertyInfo.SetValue(mParams.m_Object, mValueToSet);
                }
            }
            catch(Exception e)
            {
                log("ERROR - in attempting to set property: " + strPropertyName + ". Exception was: " + e.Message);
            }
        }

        private void _deserializeField(DataGroupConvertParams mParams)
        {
            if (mParams.m_FieldInfo.IsNotSerialized)
            {
                return;
            }
            Type mType = mParams.m_Object.GetType();            
            if (mType.IsEnum)
            {
                return;
            }
            string strFieldName = mParams.m_FieldInfo.Name;
            try
            {
                
                object mValueToSet = _deserializeObjectFromDataGroup(mParams, mParams.m_DataGroup, mParams.m_FieldInfo.FieldType, strFieldName);
                if (mValueToSet != null)
                {
                    mParams.m_FieldInfo.SetValue(mParams.m_Object, mValueToSet);
                }
            }
            catch (Exception e)
            {
                log( "ERROR - in attempting to set field property: " + strFieldName + ". Exception was: " + e.Message);
            }
        }

        private bool _deserializeArray(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, ref object mObjectOut, Type mTypeOfProperty)
        {
            if (mTypeOfProperty.IsArray)
            {

                
                int iCount = mActiveDataGroup.getProperty("COUNT", 0);
                try
                {
                    Type mElementType = mTypeOfProperty.GetElementType();
                    IList mArray = Activator.CreateInstance(mTypeOfProperty, new object[] { iCount }) as IList;
                    for (int iIndex = 0; iIndex < iCount; iIndex++)
                    {
                        Type mChildType = null;// _getTypeForChildDataGroup(mParentDataGroup, iIndex.ToString());                       
                        object mObject = _deserializeObjectFromDataGroup(mParams, mActiveDataGroup, (mChildType != null)?mChildType: mElementType, iIndex.ToString());
                        mArray[iIndex] = mObject;
                        
                    }
                    mObjectOut = mArray;
                }
                catch(Exception e)
                {
                    log("ERROR - in attempting to create array property: " + mActiveDataGroup.dataGroupName + ". Exception was: " + e.Message);
                }
                return true;
            }
            return false;

      
        }
      
        private bool _deserializeList(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, ref object mObjectOut, Type mTypeOfProperty)
        {
            try
            {
                if (mTypeOfProperty.GetGenericTypeDefinition() != typeof(List<>))
                {
                    return false;
                }
            }
            catch { return false; }
            try
            {
                IList mArray = Activator.CreateInstance(mTypeOfProperty) as IList;// as List<object>;
                Type mTypeOf = _getListType(mArray);

                int iCount = mActiveDataGroup.getProperty("COUNT", 0);
                
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    Type mChildType = _getTypeForChildDataGroup(mActiveDataGroup, iIndex.ToString());                    
                    object mObject = _deserializeObjectFromDataGroup(mParams, mActiveDataGroup, (mChildType != null) ? mChildType : mTypeOf, iIndex.ToString());               
                    mArray.Add(mObject);
                }
                mObjectOut = mArray;                  
                return true;
                
            }
            catch(Exception e)
            {
                log("ERROR - in creating list with property name: " + mActiveDataGroup.dataGroupName + ". Exception was: " + e.Message);
            }
             
            return false;
        }
        private bool _deserializeDictionary(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, ref object mObjectOut, Type mTypeOfProperty)
        {
            try
            {
                if (mTypeOfProperty.GetGenericTypeDefinition() != typeof(Dictionary<,>))
                {
                    return false;
                }
            }
            catch { return false; }
            try
            {
                Type[] mKeyAndValueTypes = mTypeOfProperty.GetGenericArguments();
                int iCount = mActiveDataGroup.getProperty("COUNT", 0);
                IDictionary mDictionary = Activator.CreateInstance(mTypeOfProperty) as IDictionary;
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    DataGroup mChild = mActiveDataGroup.getChildDataGroup(iIndex.ToString());
                    if( mChild == null)
                    {                        
                        continue;
                    }
                    object mKey = _deserializeObjectFromDataGroup(mParams, mActiveDataGroup, mKeyAndValueTypes[0], "KEY");
                    object mValue = _deserializeObjectFromDataGroup(mParams, mActiveDataGroup, mKeyAndValueTypes[1], "VALUE");
                    if (mKey != null)
                    {
                        mDictionary.Add(mKey, mValue);
                    }
                }
                mObjectOut = mDictionary;
                return true;

            }
            catch (Exception e)
            {
                log( "ERROR - in creating list with property name: " + mActiveDataGroup.dataGroupName + ". Exception was: " + e.Message);
            }

            return false;
        }

        private object _deserializeObjectFromDataGroup(DataGroupConvertParams mParams, DataGroup mActiveDataGroup, Type mTypeOfPropertyOrObject, string strPropertyOrGroupName)
        {
            DataProperty mProperty = mActiveDataGroup.getDataProperty(strPropertyOrGroupName);
            if( mProperty != null)
            {
                if(mTypeOfPropertyOrObject.IsEnum)
                {
                    try
                    {
                        return Enum.Parse(mTypeOfPropertyOrObject, mProperty.getAsString());
                    }
                    catch(Exception e)
                    {
                        log( "ERROR - attempting to set enum " + mTypeOfPropertyOrObject.Name + " " + strPropertyOrGroupName + " to value " + mActiveDataGroup.getDataGroupAsString() + " failed. Exception was: " + e.Message);
                    }
                    return null;
                }
                if(mTypeOfPropertyOrObject.Name == "DateTime")
                {
                    return new DateTime(mProperty.getAsInt64());
                }
                if(mTypeOfPropertyOrObject.IsPrimitive ||                     
                    mTypeOfPropertyOrObject.Name == "String")
                {
                    return mProperty.getPropertyAsObjectByType(mTypeOfPropertyOrObject);
                }
                return null;    //weird..why?
            }
            if (mTypeOfPropertyOrObject.IsPrimitive ||
                mTypeOfPropertyOrObject.IsEnum ||
                mTypeOfPropertyOrObject.Name == "DateTime" ||
                mTypeOfPropertyOrObject.Name == "String")
            {
                return null;
            }
            mTypeOfPropertyOrObject = _recastType(mTypeOfPropertyOrObject); //recast interface
            DataGroup mDataGroupOfObject = mActiveDataGroup.getChildDataGroup(strPropertyOrGroupName);            
            if(mDataGroupOfObject == null )
            {
                //must be an object with no properties
                try
                {
                    return Activator.CreateInstance(mTypeOfPropertyOrObject);
                }
                catch (Exception e)
                {
                    log("Warning - deserializing data group " + mActiveDataGroup.dataGroupName + ". It appears to not be serialized - attempted to make object for it but failed. type: " + mTypeOfPropertyOrObject.ToString() + Environment.NewLine + "Error was: " + e.Message);
                }
                return null;
            }
            object mObjectReturn = null;
            if( _deserializeArray(mParams, mDataGroupOfObject, ref mObjectReturn, mTypeOfPropertyOrObject))
            {
                return mObjectReturn;
            }
            if (_deserializeList(mParams, mDataGroupOfObject, ref mObjectReturn, mTypeOfPropertyOrObject))
            {
                return mObjectReturn;
            }
            if(_deserializeDictionary(mParams, mDataGroupOfObject, ref mObjectReturn, mTypeOfPropertyOrObject))
            {
                return mObjectReturn;
            }
            //must be an object
            try
            {
                object mNewObject = Activator.CreateInstance(mTypeOfPropertyOrObject);
                deserializeObjectFromDataGroup(mParams, mNewObject, mDataGroupOfObject);
                return mNewObject;
            }
            catch(Exception e)
            {
                log( "ERROR - in attempting to create object(" + mTypeOfPropertyOrObject.Name + ") for property " + strPropertyOrGroupName + ". Exception was: " + e.Message);
            }
            
            return null;
        }
        #endregion
    }//end DataGroupConvert

}//end namespace
