using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Library
{
	public enum EDATAGROUP_VARIABLE_TYPES
	{
		BOOL,
		CHAR,
		INT8,
		UINT8,
		INT16,
		UINT16,
		INT32,
		UINT32,
		INT64,
		UINT64,
		FLOAT,	
		STRING,
		COUNT
	};
    public static class EDATAGROUP_CSHARP_TYPES_NAMES
    {
        
        public static KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>[] g_Names = new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>[] {
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Boolean", EDATAGROUP_VARIABLE_TYPES.BOOL ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Byte",EDATAGROUP_VARIABLE_TYPES.CHAR ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "SByte",EDATAGROUP_VARIABLE_TYPES.INT8 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Byte",EDATAGROUP_VARIABLE_TYPES.UINT8 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Int16",EDATAGROUP_VARIABLE_TYPES.INT16 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "UInt16",EDATAGROUP_VARIABLE_TYPES.UINT16 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Int32",EDATAGROUP_VARIABLE_TYPES.INT32 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "UInt32",EDATAGROUP_VARIABLE_TYPES.UINT32 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Int64",EDATAGROUP_VARIABLE_TYPES.INT64 ),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "UInt64",EDATAGROUP_VARIABLE_TYPES.UINT64),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Single",EDATAGROUP_VARIABLE_TYPES.FLOAT),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "Double",EDATAGROUP_VARIABLE_TYPES.FLOAT),
            new KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES>( "String", EDATAGROUP_VARIABLE_TYPES.STRING )
		};

    }


    public class DataProperty
	{
		private static Dictionary<string, EDATAGROUP_VARIABLE_TYPES> g_NamesToTypes = new Dictionary<string, EDATAGROUP_VARIABLE_TYPES>();
		
		private long m_iValue = 0;
		private double m_fValue = 0;
		private string m_strValue = "";
        private static Mutex m_Initializer = new Mutex();

		public DataProperty() { _initialize(); }
		public DataProperty(string strName)
		{
			_initialize();
			propertyName = strName;
		}
		public DataProperty(string strName, EDATAGROUP_VARIABLE_TYPES eType, string strValueAsString)
		{
			_initialize();
			propertyName = strName;			
			setValueByString(strValueAsString, eType);
		}

		public string propertyName { get; set; }
		public EDATAGROUP_VARIABLE_TYPES propertyType { get; set; }

		private static void _initialize()
		{
            

            if (g_NamesToTypes.Count == 0)
			{
                m_Initializer.WaitOne();
                if (g_NamesToTypes.Count != 0)
                {
                    m_Initializer.ReleaseMutex();
                    return;
                }
                for (int iIndexOfType = 0; iIndexOfType < (int)EDATAGROUP_VARIABLE_TYPES.COUNT; iIndexOfType++)
				{
					EDATAGROUP_VARIABLE_TYPES eType = (EDATAGROUP_VARIABLE_TYPES)iIndexOfType;
					g_NamesToTypes[getPropertyTypeName(eType)] = eType;
				}
                m_Initializer.ReleaseMutex();
			}
		}

		public bool setValueByString(string strValue, string strVariableType)
		{
			if( strVariableType == "DATETIME")
			{
				strVariableType = "STRING";
			}
			if (g_NamesToTypes.ContainsKey(strVariableType))
			{
				return setValueByString(strValue, g_NamesToTypes[strVariableType]);
			}
			return false;
		}

		public bool setValueByString(string strValue)
		{
			return setValueByString(strValue, propertyType);
		}

		public bool setValueByString(string strValue, EDATAGROUP_VARIABLE_TYPES eType)
		{

			propertyType = EDATAGROUP_VARIABLE_TYPES.COUNT;
			m_iValue = 0;
			m_fValue = 0;
			m_strValue = "";
			switch (eType)
			{
				case EDATAGROUP_VARIABLE_TYPES.COUNT:
					return false;
				case EDATAGROUP_VARIABLE_TYPES.BOOL:
					{
						bool bValue = false;
						if(Boolean.TryParse(strValue, out bValue))
						{
							setProperty(bValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.CHAR:
                case EDATAGROUP_VARIABLE_TYPES.UINT8:
                {
						byte iValue = 0;
						if (Byte.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
                case EDATAGROUP_VARIABLE_TYPES.INT8:                
					{
						sbyte iValue = 0;
						if (SByte.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.INT16:
					{
						Int16 iValue = 0;
						if (Int16.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.UINT16:
					{
						UInt16 iValue = 0;
						if (UInt16.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				default:
				case EDATAGROUP_VARIABLE_TYPES.INT32:
					{
						int iValue = 0;
						if (Int32.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.UINT32:
					{
						uint iValue = 0;
						if (UInt32.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.INT64:
					{
						long iValue = 0;
						if (Int64.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.UINT64:
					{
						ulong iValue = 0;
						if (UInt64.TryParse(strValue, out iValue))
						{
							setProperty(iValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.FLOAT:
					{
						double fValue = 0;
						if (Double.TryParse(strValue, out fValue))
						{
							setProperty(fValue);
							return true;
						}
					}
					break;
				case EDATAGROUP_VARIABLE_TYPES.STRING:
					{
						setProperty(strValue);
					}
					return true;
			}			
			return false;
		}


		public void setProperty(bool bValue) { m_iValue = Convert.ToInt64(bValue); propertyType = EDATAGROUP_VARIABLE_TYPES.BOOL; }
		public void setProperty(sbyte iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.INT8; }
		public void setProperty(byte iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.UINT8; }
		public void setProperty(short iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.INT16; }
		public void setProperty(ushort iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.UINT16; }
		public void setProperty(int iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.INT32; }
		public void setProperty(uint iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.UINT32; }
		public void setProperty(long iValue) { m_iValue = iValue; propertyType = EDATAGROUP_VARIABLE_TYPES.INT64; }
		public void setProperty(ulong iValue) { m_iValue = Convert.ToInt64(iValue); propertyType = EDATAGROUP_VARIABLE_TYPES.UINT64; }
		public void setProperty(float fValue) { m_fValue = Convert.ToDouble(fValue); propertyType = EDATAGROUP_VARIABLE_TYPES.FLOAT; }
		public void setProperty(double fValue) { m_fValue = Convert.ToDouble(fValue); propertyType = EDATAGROUP_VARIABLE_TYPES.FLOAT; }
		public void setProperty(string strValue) { m_strValue = strValue; propertyType = EDATAGROUP_VARIABLE_TYPES.STRING; }
        public object getPropertyAsObjectByType(Type mType)
        {
            foreach (KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES> mTypeConvert in EDATAGROUP_CSHARP_TYPES_NAMES.g_Names)
            {
                if (mTypeConvert.Key == mType.Name)
                {
                    try
                    {
                        switch (mTypeConvert.Value)
                        {
                            case EDATAGROUP_VARIABLE_TYPES.COUNT:
                                return null;
                            case EDATAGROUP_VARIABLE_TYPES.BOOL:                            
                                return getAsBool();                            
                            case EDATAGROUP_VARIABLE_TYPES.INT8:
                                return getAsInt8();
                            case EDATAGROUP_VARIABLE_TYPES.CHAR:
                            case EDATAGROUP_VARIABLE_TYPES.UINT8:
                                return getAsUInt8();
                            case EDATAGROUP_VARIABLE_TYPES.INT16:
                                return getAsInt16();
                            case EDATAGROUP_VARIABLE_TYPES.UINT16:
                                return getAsUInt16();                           
                            case EDATAGROUP_VARIABLE_TYPES.INT32:
                                return getAsInt32();
                            case EDATAGROUP_VARIABLE_TYPES.UINT32:
                                return getAsUInt32();
                            case EDATAGROUP_VARIABLE_TYPES.INT64:
                                return getAsInt64();
                            case EDATAGROUP_VARIABLE_TYPES.UINT64:
                                return getAsUInt64();
                            case EDATAGROUP_VARIABLE_TYPES.FLOAT:
                                return getAsFloat();
                            case EDATAGROUP_VARIABLE_TYPES.STRING:
                                return getAsString();
                        }                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return null;
                }
            }
            return null;
        }
        public bool setPropertyByPrimitiveType(Object mObject)
        {
            if( mObject == null)
            {
                return false;
            }
            Type mType = mObject.GetType();
            foreach(KeyValuePair<string, EDATAGROUP_VARIABLE_TYPES> mTypeConvert in EDATAGROUP_CSHARP_TYPES_NAMES.g_Names)
            {
                if (mTypeConvert.Key == mType.Name)
                {
                    try
                    {
                        switch (mTypeConvert.Value)
                        {
                            case EDATAGROUP_VARIABLE_TYPES.COUNT:
                                return false;
                            case EDATAGROUP_VARIABLE_TYPES.BOOL:
                            {
                                setProperty((bool)mObject);
                            }
                            break;

                            case EDATAGROUP_VARIABLE_TYPES.INT8:
                            {
                                setProperty((sbyte)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.CHAR:
                            case EDATAGROUP_VARIABLE_TYPES.UINT8:
                            {
                                setProperty((byte)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.INT16:
                            {
                                setProperty((short)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.UINT16:
                            {
                                setProperty((ushort)mObject);
                            }
                            break;
                            default:
                            case EDATAGROUP_VARIABLE_TYPES.INT32:
                            {
                                setProperty((int)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.UINT32:
                            {
                                setProperty((uint)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.INT64:
                            {
                                setProperty((long)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.UINT64:
                            {
                                setProperty((ulong)mObject);
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.FLOAT:
                            {
                                try
                                {
                                    setProperty((Single)mObject);
                                }
                                catch
                                {
                                    setProperty((double)mObject);
                                }
                            }
                            break;
                            case EDATAGROUP_VARIABLE_TYPES.STRING:
                            {
                                setProperty((string)mObject);
                            }
                            break;
                        }
                        return true;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return false;
                }
            }
            return false;
        }

        //gets/sets the data property name

        public bool getAsBool() { return Convert.ToBoolean(m_iValue); }
		public sbyte getAsInt8() { return Convert.ToSByte(m_iValue); }
		public byte getAsUInt8() { return Convert.ToByte(m_iValue); }
		public short getAsInt16() { return Convert.ToInt16(m_iValue); }
		public ushort getAsUInt16() { return Convert.ToUInt16(m_iValue); }
		public int getAsInt32() { return Convert.ToInt32(m_iValue); }
		public uint getAsUInt32() { return Convert.ToUInt32(m_iValue); }
		public long getAsInt64() { return m_iValue; }
		public ulong getAsUInt64() { return Convert.ToUInt64(m_iValue); }
		public float getAsFloat() { return (float)m_fValue; }
		public string getAsString() { return getPropertyValueAsString(); }

		
		public string getPropertyValueAsString()
		{
			switch (propertyType)
			{
				case EDATAGROUP_VARIABLE_TYPES.COUNT:
					return "UNKNOWN";
				case EDATAGROUP_VARIABLE_TYPES.BOOL:
					return getAsBool().ToString();
				case EDATAGROUP_VARIABLE_TYPES.CHAR:
					return getAsUInt8().ToString();
				case EDATAGROUP_VARIABLE_TYPES.INT8:
					return getAsInt8().ToString();
				case EDATAGROUP_VARIABLE_TYPES.UINT8:
					return getAsUInt8().ToString();
				case EDATAGROUP_VARIABLE_TYPES.INT16:
					return getAsInt16().ToString();
				case EDATAGROUP_VARIABLE_TYPES.UINT16:
					return getAsUInt16().ToString();
				default:
				case EDATAGROUP_VARIABLE_TYPES.INT32:
					return getAsInt32().ToString();
				case EDATAGROUP_VARIABLE_TYPES.UINT32:
					return getAsUInt32().ToString();
				case EDATAGROUP_VARIABLE_TYPES.INT64:
					return getAsInt64().ToString();
				case EDATAGROUP_VARIABLE_TYPES.UINT64:
					return getAsUInt64().ToString();
				case EDATAGROUP_VARIABLE_TYPES.FLOAT:
					return getAsFloat().ToString();
				case EDATAGROUP_VARIABLE_TYPES.STRING:
					return m_strValue;
			}
		}

		public string getPropertyTypeName()
		{
			return getPropertyTypeName(propertyType);
		}
		public static string getPropertyTypeName(EDATAGROUP_VARIABLE_TYPES eType)
		{
			switch (eType)
			{
				case EDATAGROUP_VARIABLE_TYPES.COUNT:
					return "UNKNOWN";
				case EDATAGROUP_VARIABLE_TYPES.BOOL:
					return "BOOL";
				case EDATAGROUP_VARIABLE_TYPES.CHAR:
					return "CHAR";
				case EDATAGROUP_VARIABLE_TYPES.INT8:
					return "INT8";
				case EDATAGROUP_VARIABLE_TYPES.UINT8:
					return "UINT8";
				case EDATAGROUP_VARIABLE_TYPES.INT16:
					return "INT16";
				case EDATAGROUP_VARIABLE_TYPES.UINT16:
					return "UINT16";
				default:
				case EDATAGROUP_VARIABLE_TYPES.INT32:
					return "INT32";
				case EDATAGROUP_VARIABLE_TYPES.UINT32:
					return "UINT32";
				case EDATAGROUP_VARIABLE_TYPES.INT64:
					return "INT64";
				case EDATAGROUP_VARIABLE_TYPES.UINT64:
					return "UINT64";
				case EDATAGROUP_VARIABLE_TYPES.FLOAT:
					return "FLOAT";
				case EDATAGROUP_VARIABLE_TYPES.STRING:
					return "STRING";
			}
		}

		public string getSaveString() 
		{ 
			return "<" + getPropertyTypeName() + ">"+ propertyName +":" + getPropertyValueAsString() + Environment.NewLine; 
		}

		public static DataProperty createPropertyFromSaveString(string strStringToParse)
		{
			_initialize();
			strStringToParse = strStringToParse.Trim();
			if(strStringToParse.StartsWith("<") == false) { return null; }
			int iNextBracket = strStringToParse.IndexOf(">");
			if( iNextBracket < 0) { return null; }
			string strType = strStringToParse.Substring(1, iNextBracket - 1);
			if (g_NamesToTypes.ContainsKey(strType) == false) { return null; }
			EDATAGROUP_VARIABLE_TYPES eType = g_NamesToTypes[strType];

			string strNameAndValue = strStringToParse.Substring(iNextBracket + 1, strStringToParse.Length - iNextBracket - 1);
			int iSplit = strNameAndValue.IndexOf(":");
			if( iSplit < 1) { return null; }
			string strName = strNameAndValue.Substring(0, iSplit);
			if( strName == null || strName == "") { return null; }
			string strValue = strNameAndValue.Substring(iSplit + 1, strNameAndValue.Length- iSplit - 1);
			DataProperty mDataProperty = new DataProperty(strName);
			if( mDataProperty.setValueByString(strValue, eType))
			{
				return mDataProperty;
			}
			return null;
		}

	}
	
}
