using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Library.Utilities
{
    public class SerializationToJson
    {

        static public bool serializeObject(object mObjectToSerialize, ref string strSerializedJsonText, ref string strErrors)
        {
            try
            {
                if(mObjectToSerialize == null)
                {
                    strErrors = "ERROR - Unable to serialize object. It was null.";
                    return false;
                }
                strSerializedJsonText = JsonConvert.SerializeObject(mObjectToSerialize, Newtonsoft.Json.Formatting.Indented);
                if(strSerializedJsonText == null ||
                    strSerializedJsonText == "" )
                {
                    strSerializedJsonText = "";
                    strErrors = "ERROR - unable to parse json object: " + mObjectToSerialize.GetType().Name;
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                strSerializedJsonText = "";
                strErrors = "ERROR - in serializing database config. " + Environment.NewLine;
                if (e.InnerException != null)
                {
                    strErrors = e.Message + Environment.NewLine + e.InnerException.Message;
                }
                else
                {
                    strErrors = e.Message;
                }
            }
            return true;
        }
        static public bool serializeObjectToFile(object mObjectToSerialize, string strPathToFile, ref string strErrors)
        {
            try
            {
                string strSerializedJsonText = "";
                if(serializeObject(mObjectToSerialize, ref strSerializedJsonText, ref strErrors) == false )
                {
                    strErrors = "ERROR - in parsing file: " + strPathToFile + Environment.NewLine + strErrors;
                    return false;
                }
                File.WriteAllText(strPathToFile, strSerializedJsonText, Encoding.ASCII);
                if (File.Exists(strPathToFile) == false)
                {
                    strErrors = "ERROR - unable to write database config to: " + strPathToFile;
                    return false;
                }
                return true;
            }
            catch(Exception e)
            {
                strErrors = "ERROR - in parsing file: " + strPathToFile + Environment.NewLine + "Exception was:" + e.Message + Environment.NewLine + strErrors;
            }
            return false;
        }
        static public object deserializeObject(Type mType, string strSerializedJsonText, ref string strErrors)
        {

            try
            {
                if (mType == null)
                {
                    strErrors = "ERROR - No type specified in deserializing of json object.";
                    return null;
                }

                if (strSerializedJsonText == null ||
                    strSerializedJsonText == "" )
                {
                    strErrors = "ERROR - Unable to parse json text for object " + mType.Name + ". The text was empty.";
                    return null;
                }
                
                object mNewObject = JsonConvert.DeserializeObject(strSerializedJsonText, mType);
                if (mNewObject == null)
                {
                    strErrors = "ERROR - Unable to deserialize json object: " + mType.Name;
                    return null;

                }
                return mNewObject;
            }
            catch (Exception e)
            {
                strErrors = "ERROR - Unable to deserialize json object: " + mType.Name + Environment.NewLine + "Error message was: " + e.Message;

            }
            return null;
        }
        static public object deserializeObjectFromFile(Type mType, string strPathToFile, ref string strErrors)
        {
            if (File.Exists(strPathToFile) == false)
            {
                strErrors = "ERROR - Parsing json file failed. File doesn't exist: " + strPathToFile;
                return null;
            }
            object mNewObject = deserializeObject(mType, File.ReadAllText(strPathToFile), ref strErrors);
            if( mNewObject == null )
            {
                strErrors = "ERROR - in parsing file: " + strPathToFile + Environment.NewLine + strErrors;
            }
            return mNewObject;
        }
    }
}
