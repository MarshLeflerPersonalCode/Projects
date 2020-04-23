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
    public class DataGroupConvert
    {

        public static DataGroup serializeIntoDataGroup(Object mObject, LogFile mLogFile)
        {

            DataGroup mDataGroup = new DataGroup();
            serializeIntoDataGroup(mDataGroup, mObject, mLogFile);
            return mDataGroup;

        }
        public static string serializeObjectIntoString(Object mObject, LogFile mLogFile)
        {

            DataGroup mDataGroup = new DataGroup();
            serializeIntoDataGroup(mDataGroup, mObject, mLogFile);
            return mDataGroup.getDataGroupAsString();

        }
        public static void serializeIntoDataGroup(DataGroup mDataGroup, Object mObject, LogFile mLogFile)
        {

            try
            {
                if (mObject == null)
                {
                    log(mLogFile, "ERROR - Unable to serialize object into Data Group. Object was null.");
                    return;
                }
                if (mDataGroup == null)
                {
                    log(mLogFile, "ERROR - Data Group was null.");
                    return;
                }
                Type mType = mObject.GetType();
                /*if (mType.IsSerializable == false)
                {
                    log(mLogFile, "ERROR - Object type: " + mType.Name + " is not serializable.");
                    return;
                }*/
                if (mDataGroup.dataGroupName == "")
                {
                    mDataGroup.dataGroupName = mType.Name;
                }

                //mDataGroup.setProperty("CSHARP", mType.AssemblyQualifiedName);
                MemberInfo[] mMembers = mType.GetMembers();

                Type mStringType = Type.GetType("System.String");
                foreach (MemberInfo mMember in mMembers)
                {
                    MemberTypes mMemberTypeInfo = mMember.MemberType;
                    if (mMemberTypeInfo == MemberTypes.Property)
                    {
                        _serializeProperty(mDataGroup, mObject, mMember, mMember as PropertyInfo, mLogFile);
                        continue;
                    }
                    else if (mMemberTypeInfo == MemberTypes.Field)
                    {
                        _serializeField(mDataGroup, mObject, mMember, mMember as FieldInfo, mLogFile);
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
                log(mLogFile, "ERROR - Unable to serialize object into Data Group. Exception was: " + e.Message);
            }

        }
        public static object deserializeObjectFromFile(string strPathToFile, Type mType, LogFile mLogFile)
        {
            try
            {
                if (strPathToFile == null ||
                    strPathToFile == "")
                {
                    log(mLogFile, "ERROR - file path was empty. Unable to deserialize.");
                    return null;
                }
                if (File.Exists(strPathToFile))
                {
                    return deserializeObjectFromString(File.ReadAllText(strPathToFile), mType, mLogFile);
                }
                log(mLogFile, "ERROR - unable to deserialize because file doesn't exist at: " + strPathToFile);
                return null;
            }
            catch (Exception e)
            {
                log(mLogFile, "ERROR - unable to deserialize object from string. Exception was: " + e.Message);
            }
            return null;
        }
        public static object deserializeObjectFromString(string strDataGroup, Type mType, LogFile mLogFile)
        {
            try
            {
                if (strDataGroup == null ||
                    strDataGroup == "")
                {
                    log(mLogFile, "ERROR - string was empty. Unable to deserialize.");
                    return null;
                }
                if (mType == null)
                {
                    log(mLogFile, "ERROR - type was null. Unable to deserialize.");
                    return null;
                }
                string strError = "";
                DataGroup mDataGroup = DataGroup.createFromString(strDataGroup, ref strError);
                if( strError != "")
                {
                    log(mLogFile, "ERROR - unable to deserialize object from string. " + Environment.NewLine + strError);
                    return null;
                }
                return deserializeObjectFromDataGroup(mDataGroup, mType, mLogFile);
            }
            catch(Exception e)
            {
                log(mLogFile, "ERROR - unable to deserialize object from string. Exception was: " + e.Message);
            }
            return null;
        }

        public static object deserializeObjectFromDataGroup(DataGroup mDataGroup, Type mType, LogFile mLogFile)
        {

            try
            {
                if (mDataGroup == null)
                {
                    log(mLogFile, "ERROR - datagroup was null");
                    return null;
                }
                if (mType == null)
                {
                    log(mLogFile, "ERROR - type was null");
                    return null;
                }

                object mObject = Activator.CreateInstance(mType);
                if (DeserializeObjectFromDataGroup(mObject, mDataGroup, mLogFile))
                {
                    return mObject;
                }
            }
            catch (Exception e)
            {
                log(mLogFile, "ERROR - there was an error deserializing data group. Exception was: " + e.Message);
            }
            return null;
        }

        public static bool DeserializeObjectFromDataGroup(Object mObject, DataGroup mDataGroup, LogFile mLogFile)
        {
            if (mDataGroup == null)
            {
                log(mLogFile, "ERROR - datagroup was null");
                return false;
            }
            if (mObject == null)
            {
                log(mLogFile, "ERROR - object was null");
                return false;
            }
            try
            {
                Type mType = mObject.GetType();
                MemberInfo[] mMembers = mType.GetMembers();

                Type mStringType = Type.GetType("System.String");
                foreach (MemberInfo mMember in mMembers)
                {
                    MemberTypes mMemberTypeInfo = mMember.MemberType;
                    if (mMemberTypeInfo == MemberTypes.Property)
                    {
                        _deserializeProperty(mDataGroup, mObject, mMember, mMember as PropertyInfo, mLogFile);
                        continue;
                    }
                    else if (mMemberTypeInfo == MemberTypes.Field)
                    {
                        _deserializeField(mDataGroup, mObject, mMember, mMember as FieldInfo, mLogFile);
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
                log(mLogFile, "ERROR - there was an error deserializing data group into object type: " + mObject.GetType().Name + " Exception was: " + e.Message);
            }
            return false;
        }




        private static Type _getTypeForDataGroup(DataGroup mDataGroup)
        {

            /*
            string strValue = mDataGroup.getProperty("CSHARP", "");
            if( strValue != "")
            {
                return Type.GetType(strValue);
            }
            return null;
            */
            return null;
        }
        private static Type _getTypeForChildDataGroup(DataGroup mDataGroup, string strChildName)
        {
            /*
            DataGroup mChild = mDataGroup.getChildDataGroup(strChildName);
            if( mChild != null)
            {
                return _getTypeForDataGroup(mChild);
            }
            */
            return null;
        }
        private static Type _getListType(IList myList)
        {
            var enumerable_type =
                myList.GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GenericTypeArguments.Length == 1)
                .FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (enumerable_type != null)
                return enumerable_type.GenericTypeArguments[0];

            if (myList.Count == 0)
                return null;

            return myList[0].GetType();
        }
        private static void log(LogFile mLog, string strMessage)
        {
            if (mLog != null)
            {
                mLog.log(strMessage);
            }
        }
        private static bool _serailizeArrayAsDataProperty(DataGroup mParentDataGroup, object mObject, string strPropertyName, LogFile mLogFile)
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
            }catch{}
            
            if (mArray == null)
            {
                log(mLogFile, "ERROR - in attempting to cast array to object array.");
                return true;
            }

            DataGroup mArrayGroup = mParentDataGroup.getOrCreateDataGroup(strPropertyName);
            mArrayGroup.setProperty("COUNT", (ushort)mArray.Length);
            //mArrayGroup.setProperty("CSHARP", mObject.GetType().AssemblyQualifiedName);
            Type mElementType = mArray.GetType().GetElementType();

            if (mElementType != null)
            {
                mArrayGroup.setProperty("CSHARP", mElementType.AssemblyQualifiedName);
            }
            int iCount = 0;
            foreach (object mObjectInArray in mArray)
            {                
                _serializeObjectAsDataProperty(mArrayGroup, mObjectInArray, iCount.ToString(), mLogFile);
                if (mObjectInArray != null &&
                    mObjectInArray.GetType() != mElementType)
                {
                    DataGroup mChild = mArrayGroup.getChildDataGroup(iCount.ToString());
                    if(mChild != null)
                    {
                        //in case the object in the array has some kind of hierarchy
                        //mArrayGroup.setProperty("CSHARP", mObjectInArray.GetType().AssemblyQualifiedName);
                    }
                }
                iCount++;
            }
            return true;
        }

        private static bool _serailizeListAsDataGroup(DataGroup mParentDataGroup, object mObject, string strPropertyName, LogFile mLogFile)
        {
            IList mList = mObject as IList;
            if (mList == null)
            {
                return false;
            }

            DataGroup mArrayGroup = mParentDataGroup.getOrCreateDataGroup(strPropertyName);
            mArrayGroup.setProperty("COUNT", (ushort)mList.Count);
            //mArrayGroup.setProperty("CSHARP", mObject.GetType().AssemblyQualifiedName);
            Type mElementType = _getListType(mList);// mList.GetType().GetGenericTypeDefinition();
            if (mElementType != null)
            {
                //mArrayGroup.setProperty("CSHARP", mElementType.AssemblyQualifiedName);
            }
            int iCount = 0;
            foreach (object mObjectInArray in mList)
            {
                _serializeObjectAsDataProperty(mArrayGroup, mObjectInArray, iCount.ToString(), mLogFile);
                if (mObjectInArray != null &&
                    mObjectInArray.GetType() != mElementType)
                {
                    DataGroup mChild = mArrayGroup.getChildDataGroup(iCount.ToString());
                    if (mChild != null)
                    {
                        //in case the object in the array has some kind of hierarchy
                        //mArrayGroup.setProperty("CSHARP", mObjectInArray.GetType().AssemblyQualifiedName);
                    }
                }
                iCount++;
            }
            return true;
        }

        private static bool _serializeDictionaryAsDataGroup(DataGroup mParentDataGroup, object mObject, string strPropertyName, LogFile mLogFile)
        {
            IDictionary mDictionary = mObject as IDictionary;
            if (mDictionary != null)
            {
                Type[] mKeyAndValueTypes = mObject.GetType().GetGenericArguments();
                DataGroup mDictionaryGroup = mParentDataGroup.getOrCreateDataGroup(strPropertyName);
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
                if (mKeyType != null &&
                    mValueType != null)
                {
                    //mDictionaryGroup.setProperty("CSHARP_KEY", mKeyType.AssemblyQualifiedName);
                    //mDictionaryGroup.setProperty("CSHARP_VALUE", mValueType.GetType().AssemblyQualifiedName);
                }
                //mDictionaryGroup.setProperty("CSHARP", mObject.GetType().AssemblyQualifiedName);
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
                    _serializeObjectAsDataProperty(mKeyValueGroup, mKeyArray[iCount], "KEY", mLogFile);
                    /*if (mKeyArray[iCount] != null &&
                        mKeyArray[iCount].GetType() != mKeyType)
                    {
                        DataGroup mChild = mKeyValueGroup.getChildDataGroup("key");
                        if (mChild != null)
                        {
                            mChild.setProperty("CSHARP", mKeyArray[iCount].GetType().AssemblyQualifiedName);
                        }
                    }*/
                    _serializeObjectAsDataProperty(mKeyValueGroup, mValueArray[iCount], "VALUE", mLogFile);
                    /*if (mValueArray[iCount] != null &&
                        mValueArray[iCount].GetType() != mValueType)
                    {
                        DataGroup mChild = mKeyValueGroup.getChildDataGroup("value");
                        if (mChild != null)
                        {
                            mChild.setProperty("CSHARP", mValueArray[iCount].GetType().AssemblyQualifiedName);
                        }
                    }*/
                }


                return true;
            }
            return false;
        }

        private static void _serializeObjectAsDataProperty(DataGroup mParentDataGroup, Object mObject, string strPropertyName, LogFile mLogFile)
        {
            if (mObject == null)
            {                
                return;
            }
            Type mObjectType = mObject.GetType();
            if (mObjectType.IsPrimitive)
            {
                mParentDataGroup.setPropertyByPrimitiveType(strPropertyName, mObject);
                return;
            }
            if (mObjectType.IsEnum)
            {
                string strEnumValue = mObject.ToString();
                mParentDataGroup.setProperty(strPropertyName, strEnumValue);
                return;
            }
            if (mObjectType == Type.GetType("System.String"))
            {
                string strValue = mObject as string;
                mParentDataGroup.setPropertyByPrimitiveType(strPropertyName, mObject);
                return;

            }
            if (mObjectType == Type.GetType("System.DateTime"))
            {
                DateTime mDateTime = (DateTime)mObject;
                mParentDataGroup.setProperty(strPropertyName, mDateTime.Ticks);
                return;

            }
            if(_serailizeArrayAsDataProperty(mParentDataGroup, mObject, strPropertyName, mLogFile))
            {
                return;
            }

            if (_serailizeListAsDataGroup(mParentDataGroup, mObject, strPropertyName, mLogFile))
            {               
                return;
            }
            if(_serializeDictionaryAsDataGroup(mParentDataGroup, mObject, strPropertyName, mLogFile))
            {
                return;
            }

            //at this point it's got to be an object we need to serialize.
            //which means a new group
            DataGroup mArrayGroup = mParentDataGroup.getOrCreateDataGroup(strPropertyName);
            serializeIntoDataGroup(mArrayGroup, mObject, mLogFile);

        }

        private static void _serializeProperty(DataGroup mParentDataGroup, Object mObject, MemberInfo mMember, PropertyInfo mProperty, LogFile mLogFile)
        {
            Type mPropertyType = mProperty.GetType();
            if (mPropertyType.IsSerializable == false)
            {
                return;
            }
            string strPropertyName = mProperty.Name;

            object mPropertyValue = mProperty.GetValue(mObject);
            if( mPropertyValue == null)
            {
                if (mProperty.PropertyType == Type.GetType("System.String"))
                {
                    string strObject = "";
                    _serializeObjectAsDataProperty(mParentDataGroup, strObject, strPropertyName, mLogFile);
                }
                return;
            }
            _serializeObjectAsDataProperty(mParentDataGroup, mPropertyValue, strPropertyName, mLogFile);
        }
        private static void _serializeField(DataGroup mParentDataGroup, Object mObject, MemberInfo mMember, FieldInfo mFieldInfo, LogFile mLogFile)
        {
            if (mFieldInfo.IsNotSerialized)
            {
                return;
            }
            Type mType = mObject.GetType();

            
            if (mType.IsEnum)
            {
                return;
            }
            string strFieldName = mFieldInfo.Name;
            object mFieldValue = mFieldInfo.GetValue(mObject);
            _serializeObjectAsDataProperty(mParentDataGroup, mFieldValue, strFieldName, mLogFile);
        }
        

        
        private static void _deserializeProperty(DataGroup mParentDataGroup, Object mObject, MemberInfo mMember, PropertyInfo mProperty, LogFile mLogFile)
        {
            Type mPropertyType = mProperty.GetType();
            if (mPropertyType.IsSerializable == false)
            {
                return;
            }
            string strPropertyName = mProperty.Name;

            try
            {
                object mValueToSet = _deserializeObjectFromDataGroup(mParentDataGroup, mProperty.PropertyType, strPropertyName, mLogFile);
                mProperty.SetValue(mObject, mValueToSet);
            }
            catch(Exception e)
            {
                log(mLogFile, "ERROR - in attempting to set property: " + strPropertyName + ". Exception was: " + e.Message);
            }
        }

        private static void _deserializeField(DataGroup mParentDataGroup, Object mObject, MemberInfo mMember, FieldInfo mFieldInfo, LogFile mLogFile)
        {
            if (mFieldInfo.IsNotSerialized)
            {
                return;
            }
            Type mType = mObject.GetType();            
            if (mType.IsEnum)
            {
                return;
            }
            string strFieldName = mFieldInfo.Name;
            try
            {
                
                object mValueToSet = _deserializeObjectFromDataGroup(mParentDataGroup, mFieldInfo.FieldType, strFieldName, mLogFile);
                mFieldInfo.SetValue(mObject, mValueToSet);
            }
            catch (Exception e)
            {
                log(mLogFile, "ERROR - in attempting to set field property: " + strFieldName + ". Exception was: " + e.Message);
            }
        }

        private static bool _deserializeArray(DataGroup mParentDataGroup, ref object mObjectOut, Type mTypeOfProperty, LogFile mLogFile)
        {
            if (mTypeOfProperty.IsArray)
            {

                
                int iCount = mParentDataGroup.getProperty("COUNT", 0);
                try
                {
                    Type mElementType = mTypeOfProperty.GetElementType();
                    IList mArray = Activator.CreateInstance(mTypeOfProperty, new object[] { iCount }) as IList;
                    for (int iIndex = 0; iIndex < iCount; iIndex++)
                    {
                        Type mChildType = null;// _getTypeForChildDataGroup(mParentDataGroup, iIndex.ToString());                       
                        object mObject = _deserializeObjectFromDataGroup(mParentDataGroup, (mChildType != null)?mChildType: mElementType, iIndex.ToString(), mLogFile);
                        mArray[iIndex] = mObject;
                    }
                    mObjectOut = mArray;
                }
                catch(Exception e)
                {
                    log(mLogFile, "ERROR - in attempting to create array property: " + mParentDataGroup.dataGroupName + ". Exception was: " + e.Message);
                }
                return true;
            }
            return false;

      
        }
        private static bool _deserializeList(DataGroup mParentDataGroup, ref object mObjectOut, Type mTypeOfProperty, LogFile mLogFile)
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
                int iCount = mParentDataGroup.getProperty("COUNT", 0);
                IList mArray = Activator.CreateInstance(mTypeOfProperty) as IList;
                Type mTypeOf = _getListType(mArray);
                
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    Type mChildType = _getTypeForChildDataGroup(mParentDataGroup, iIndex.ToString());
                    mChildType = null;
                    object mObject = _deserializeObjectFromDataGroup(mParentDataGroup, (mChildType != null) ? mChildType : mTypeOf, iIndex.ToString(), mLogFile);
                    mArray.Add( mObject );
                }
                mObjectOut = mArray;                    
                return true;
                
            }
            catch(Exception e)
            {
                log(mLogFile, "ERROR - in creating list with property name: " +mParentDataGroup.dataGroupName + ". Exception was: " + e.Message);
            }
             
            return false;
        }
        private static bool _deserializeDictionary(DataGroup mParentDataGroup, ref object mObjectOut, Type mTypeOfProperty, LogFile mLogFile)
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
                int iCount = mParentDataGroup.getProperty("COUNT", 0);
                IDictionary mDictionary = Activator.CreateInstance(mTypeOfProperty) as IDictionary;
                for (int iIndex = 0; iIndex < iCount; iIndex++)
                {
                    DataGroup mChild = mParentDataGroup.getChildDataGroup(iIndex.ToString());
                    if( mChild == null)
                    {                        
                        continue;
                    }
                    object mKey = _deserializeObjectFromDataGroup(mChild, mKeyAndValueTypes[0], "KEY", mLogFile);
                    object mValue = _deserializeObjectFromDataGroup(mChild, mKeyAndValueTypes[1], "VALUE", mLogFile);
                    mDictionary.Add(mKey, mValue);
                }
                mObjectOut = mDictionary;
                return true;

            }
            catch (Exception e)
            {
                log(mLogFile, "ERROR - in creating list with property name: " + mParentDataGroup.dataGroupName + ". Exception was: " + e.Message);
            }

            return false;
        }

        private static object _deserializeObjectFromDataGroup(DataGroup mParentDataGroup, Type mTypeOfProperty, string strPropertyName, LogFile mLogFile)
        {
            DataProperty mProperty = mParentDataGroup.getDataProperty(strPropertyName);
            if( mProperty != null)
            {
                if(mTypeOfProperty.IsEnum)
                {
                    try
                    {
                        return Enum.Parse(mTypeOfProperty, mProperty.getAsString());
                    }
                    catch(Exception e)
                    {
                        log(mLogFile, "ERROR - attempting to set enum " + mTypeOfProperty.Name + " " + strPropertyName + " to value " + mParentDataGroup.getDataGroupAsString() + " failed. Exception was: " + e.Message);
                    }
                    return null;
                }
                if(mTypeOfProperty.Name == "DateTime")
                {
                    return new DateTime(mProperty.getAsInt64());
                }
                if(mTypeOfProperty.IsPrimitive ||                     
                    mTypeOfProperty.Name == "String")
                {
                    return mProperty.getPropertyAsObjectByType(mTypeOfProperty);
                }
                return null;    //weird..why?
            }
            
            DataGroup mDataGroupProperty = mParentDataGroup.getChildDataGroup(strPropertyName);            
            if(mDataGroupProperty == null )
            {
                log(mLogFile, "Warning - deserializing data group " + mParentDataGroup.dataGroupName + ". It appears to not be serialized - object is probably null. type: " + mTypeOfProperty.ToString());
                return null;
            }
            object mObjectReturn = null;
            if( _deserializeArray(mDataGroupProperty, ref mObjectReturn, mTypeOfProperty, mLogFile))
            {
                return mObjectReturn;
            }
            if (_deserializeList(mDataGroupProperty, ref mObjectReturn, mTypeOfProperty, mLogFile))
            {
                return mObjectReturn;
            }
            if(_deserializeDictionary(mDataGroupProperty, ref mObjectReturn, mTypeOfProperty, mLogFile))
            {
                return mObjectReturn;
            }
            //must be an object
            try
            {
                object mNewObject = Activator.CreateInstance(mTypeOfProperty);
                DeserializeObjectFromDataGroup(mNewObject, mDataGroupProperty, mLogFile);
                return mNewObject;
            }
            catch(Exception e)
            {
                log(mLogFile, "ERROR - in attempting to create object(" + mTypeOfProperty.Name + ") for property " + strPropertyName + ". Exception was: " + e.Message);
            }
            
            return null;
        }

    }//end DataGroupConvert
}//end namespace
