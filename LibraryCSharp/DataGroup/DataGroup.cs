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
using System.Xml.Serialization;
using System.Xml;
namespace Library.DataGroup
{
	public class DataGroup
	{
		static string ARRAY_INDEX_TAG = "_A_";
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
		public DataGroup(Object obj)
		{
			dataGroupName = "";
			_serialize(obj);
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
			string strOutputText = getDataGroupAsString();
			File.WriteAllText(strFile, strOutputText);
			if( File.Exists(strFile) == false)
			{
				return "Error - wasn't able to write file at location: " + strFile;
			}
			return "";
		}
		//returns "" if it was successful, otherwise the error message
		public string loadFromFile(string strFile)
		{
			if (File.Exists(strFile) == false)
			{
				return "file not found: " + strFile;
			}

			string strFileContents = File.ReadAllText(strFile);
			if(strFileContents == null ||
				strFileContents == "")
			{
				return "file contents were empty for file at location: " + strFile;
			}
			try
			{
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.LoadXml(strFileContents);
				if (_parseXmlStream(xmlDoc, this) == false)
				{
					return "unable to parse file: " + strFile;
				}

			}
			catch( Exception e )
			{
				return e.Message;
			}

			return "";
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


		public string getDataGroupAsString() { return _getDataGroupAsString(""); }
		private string _getDataGroupAsString(string strTab)
		{
			string strGroupName = dataGroupName;
			string strIndex = "0";
			int iLastIndex = dataGroupName.LastIndexOf(ARRAY_INDEX_TAG);
			if (iLastIndex > 0)
			{
				strGroupName = dataGroupName.Substring(0, iLastIndex);
				strIndex = dataGroupName.Substring(iLastIndex + ARRAY_INDEX_TAG.Length);
				
			}
			string strOutput = strTab + "<" + strGroupName + " Type=\"GROUP\" Index=\"" + strIndex + "\">" + Environment.NewLine;
			string strTabForChildren = strTab + "     ";
			foreach (DataProperty mProperty in m_Properties.Values)
			{
				strOutput = strOutput + strTabForChildren + mProperty.getSaveString() + Environment.NewLine;
			}
			foreach (DataGroup mDataGroup in m_ChildGroups.Values)
			{
				strOutput = strOutput + mDataGroup._getDataGroupAsString(strTabForChildren);
			}
			strOutput = strOutput + strTab + "</" + strGroupName + ">" + Environment.NewLine;
			return strOutput;
		}

		public static DataGroup parseDataGroupByString(string strSerializedDataGroup)
		{
			DataGroup mDataGroup = new DataGroup();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(strSerializedDataGroup);
			if( mDataGroup._parseXmlStream(xmlDoc, mDataGroup) == false )
			{
				return null;
			}
			return mDataGroup;
		}

		

		private void _configureDataGroupByChildXMLNode(XmlNode mNode)
		{
			while (mNode != null)
			{
				string strName = mNode.Name;
				string strType = "UNKNOWN";
				if (mNode.Attributes != null)
				{
					XmlNode mTypeNode = mNode.Attributes.GetNamedItem("Type");
					if (mTypeNode != null)
					{
						strType = mTypeNode.Value.ToUpper();
					}
				}

				if (strType == "GROUP")
				{
					string strGroupName = strName;
					int iCount = 1;
					DataGroup mChild = getChildDataGroup(strGroupName);
					while (mChild != null)
					{
						strGroupName = strName + ARRAY_INDEX_TAG + iCount.ToString();
						mChild = getChildDataGroup(strGroupName);
						iCount++;
					}
					mChild = getOrCreateDataGroup(strGroupName);
					if (mNode.HasChildNodes)
					{
						mChild._configureDataGroupByChildXMLNode(mNode.FirstChild);
					}
				}
				else if(strName != "" && strType != "")
				{
					//todo
					string strPropertyName = strName;
					int iCount = 1;
					DataProperty mProperty = getDataProperty(strPropertyName);
					while (mProperty != null)
					{
						strPropertyName = strName + ARRAY_INDEX_TAG + iCount.ToString();
						mProperty = getDataProperty(strPropertyName);
						iCount++;
					}
					mProperty = getOrCreateDataProperty(strPropertyName);
					if( mProperty.setValueByString(mNode.InnerText, strType) == false )
					{
						m_Properties.Remove(strPropertyName);
					}
				}
				mNode = mNode.NextSibling;
			}				
		}
		private void _parseChildren(XmlNode mNode)
		{ 
			//	XmlAttribute mAttribute = mElement.GetAttributeNode("Type");
			if (mNode.HasChildNodes)
			{
				
				foreach (XmlNode mChildNode in mNode.ChildNodes)
				{
					_configureDataGroupByChildXMLNode(mChildNode);
				}
			}		

		}
		private bool _parseXmlStream(XmlDocument mXmlDoc, DataGroup mOriginalDataGroup)
		{
			dataGroupName = mXmlDoc.DocumentElement.Name;
			_configureDataGroupByChildXMLNode(mXmlDoc.DocumentElement.FirstChild);
			if( m_Properties.Count== 0 &&
				m_ChildGroups.Count == 0 )
			{
				return false;
			}
			return true;
		}
		
		public static DataGroup serialize(Object mObjectToSerialize)
		{
			if(mObjectToSerialize == null) { return null; }
			DataGroup mDataGroup = new DataGroup();
			mDataGroup._serialize(mObjectToSerialize);
			return mDataGroup;
		}
		
		//returns how many tags got filled out.
		private int _getTag(string strLine, ref string strFirstTag, ref string strData, ref string strLastTag)
		{
			strFirstTag = "";
			strData = "";
			strLastTag = "";
			strLine = strLine.Trim();
			int iFirstTagOpen = strLine.IndexOf("<");
			if( iFirstTagOpen < 0 )
			{
				return 0;
			}
			int iFirstTagClose = strLine.IndexOf(">", iFirstTagOpen);
			if( iFirstTagClose < 0)
			{
				return 0;
			}
			strFirstTag = strLine.Substring(iFirstTagOpen, iFirstTagClose - iFirstTagOpen + 1);
			if(strFirstTag == strLine)
			{
				return 1;
			}
			int iCloseTagOpen = strLine.LastIndexOf("<");
			int iCloseTagClose = strLine.LastIndexOf(">");
			if ( iCloseTagOpen > 0 &&
				iCloseTagClose > 0 )
			{
				strData = strLine.Substring(iFirstTagClose + 1, iCloseTagOpen - iFirstTagClose - 1);
				strLastTag = strLine.Substring(iCloseTagOpen, iCloseTagClose - iCloseTagOpen + 1);
			}
			else
			{
				strData = strLine.Substring(iFirstTagClose + 1, strLine.Length - iFirstTagClose - 1);
			}
			
			
			return 3;
		}

		
		private string _getTypeNameForSerialization(string strType)
		{
			switch (strType)
			{
				case "SINGLE":
					return "FLOAT";
				case "BYTE":
					return "INT8";
				case "SBYTE":
					return "UINT8";
			}
			return strType;
		}


		private void _defineTypesInXML(Type mTypeOfObject, MemberInfo mParentMemberInfo, ref StringReader mReader, ref StringWriter mWriter)
		{
			Type mParentPropertyType = (mParentMemberInfo != null && mParentMemberInfo.MemberType == MemberTypes.Property) ? ((PropertyInfo)mParentMemberInfo).PropertyType : null;
			string strFirstTag = "";
			string strMiddelData = "";
			string strLastTag = "";			
			while (true)
			{
				string strRawLine = mReader.ReadLine();
				
				if( strRawLine == null)
				{
					return;
				}
				string strCleanLine = strRawLine.Trim();
				if(strCleanLine.Contains("testShort"))
				{
					Console.Write("test");
				}
				if (strCleanLine.StartsWith("<?"))
				{
					continue; //we don't save this.
				}
				int iIndexOfHack = strRawLine.IndexOf("xmlns:xsi");
				if( iIndexOfHack > 0)
				{
					strRawLine = strRawLine.Substring(0, iIndexOfHack - 1) + ">";
				}
				if ( strCleanLine.StartsWith("<") == false)
				{
					mWriter.WriteLine(strRawLine);
					continue;
				}
				int iTagsFound = _getTag(strRawLine, ref strFirstTag, ref strMiddelData, ref strLastTag);
				if( iTagsFound == 0 )
				{
					mWriter.WriteLine(strRawLine);
					continue;
				}
				if (strFirstTag.IndexOf("</") == 0)
				{
					mWriter.WriteLine(strRawLine);
					return;
				}
				string strMemberInfo = strFirstTag.Replace("<", "").Replace(">", "");
				int iLastIndex = strMemberInfo.LastIndexOf(ARRAY_INDEX_TAG);
				if ( iLastIndex > 0 )
				{
					strMemberInfo = strMemberInfo.Substring(0, iLastIndex);
				}
				//starting a new object
				
				MemberInfo[] mMemberInfoByName = mTypeOfObject.GetMember(strMemberInfo);
				
				if (mMemberInfoByName == null )
				{
					mWriter.WriteLine(strRawLine);
					continue;
				}
				MemberInfo mMemberInfo = (mMemberInfoByName.Count() > 0) ? mMemberInfoByName[0] : null;
				Type mMemberType = (mMemberInfo != null && mMemberInfo.MemberType == MemberTypes.Property) ? mMemberInfo.GetType():null;
				PropertyInfo mPropertyInfo = (mMemberInfo != null && mMemberInfo.MemberType == MemberTypes.Property) ? ((PropertyInfo)mMemberInfoByName[0]) : null;
				Type mProperty = (mPropertyInfo != null) ? mPropertyInfo.PropertyType: null;
				FieldInfo mFieldInfo = (mMemberInfo != null && mMemberInfo.MemberType == MemberTypes.Field) ? ((FieldInfo)mMemberInfoByName[0]) : null;
				
				int indexOfTag = strRawLine.IndexOf(strFirstTag);
				string strInsertTag = "";
				bool bNewObject = false;
				switch (iTagsFound)
				{
					case 0:
						break;
					case 1:
						{
							//length 0 is the root object.
							bNewObject = (mMemberInfoByName.Length > 0 )?true:false;
							strInsertTag = " Type=\"GROUP\"";		
							if( bNewObject == false &&
								mParentPropertyType != null )
							{
								
								Type[] mTypes = mParentPropertyType.GetGenericArguments();
								foreach( Type mType in mTypes)
								{
									if( mType.Name.Contains(strMemberInfo))
									{
										bNewObject = true;
										mProperty = mType;
										break;
									}
								}

							}
						}
						break;
					case 2:						
					case 3:
						{
							//hard part
							if (mFieldInfo != null)
							{
								strInsertTag = " Type=\"" + _getTypeNameForSerialization(mFieldInfo.FieldType.Name.ToUpper()) + "\"";
							}
							else if(mProperty != null)
							{
								strInsertTag = " Type=\"" + _getTypeNameForSerialization(mProperty.Name.ToUpper()) + "\"";
							}

						}
						break;
				}
				strRawLine = strRawLine.Insert(indexOfTag + strFirstTag.Length - 1, strInsertTag);
				mWriter.WriteLine(strRawLine);
				if (bNewObject &&
					mProperty != null )
				{
					_defineTypesInXML(mProperty, mMemberInfo, ref mReader, ref mWriter);
				}

			};
		}
		
		private bool _serialize(Object mObjectToSerialize)
		{

			Type mType = mObjectToSerialize.GetType();
			System.Xml.Serialization.XmlSerializer mXmlSerailizer = new System.Xml.Serialization.XmlSerializer(mType);			
			StringWriter mWriterXml = new StringWriter();			
			mXmlSerailizer.Serialize(mWriterXml, mObjectToSerialize);			
			string strValue = mWriterXml.ToString();
			StringWriter mWriter = new StringWriter();
			StringReader mReader = new StringReader(strValue);
			//now fix up the tags...
			_defineTypesInXML(mObjectToSerialize.GetType(), null, ref mReader, ref mWriter);
			this.dataGroupName = "";
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(mWriter.ToString());
			bool bParsedOkay = _parseXmlStream(xmlDoc, this);
			Type mTypeTest = Type.GetType(mType.AssemblyQualifiedName);
			setProperty("CSharpObject", mType.GetTypeInfo().AssemblyQualifiedName);
			return bParsedOkay;
			/*File.WriteAllText("test.txt", strValue, Encoding.ASCII);
			StringWriter mWriter = new StringWriter();
			StringReader mReader = new StringReader(File.ReadAllText("test.txt"));
			//Object mNewObject = mXmlSerailizer.Deserialize(mReader);
			_defineTypesInXML(mObjectToSerialize.GetType(), null, ref mReader, ref mWriter);
			File.WriteAllText("test.txt", mWriter.ToString(), Encoding.ASCII);
			*/
			
			
		}


		public static Object deserializeObjectFromFile(string strFile, ref string strError)
		{
			if( File.Exists(strFile) == false )
			{
				strError = "File doesn't exist. Unable to parse.";
				return null;
			}
			DataGroup mDataGroup = new DataGroup();
			strError = mDataGroup.loadFromFile(strFile);
			if( strError != "")
			{
				return null;
			}
			return mDataGroup.attemptToDeserializeIntoObject(ref strError);
				
		}


		public object attemptToDeserializeIntoObject(ref string strError)
		{

			Type mType = null;
			string strAssemblyName = "";
			try
			{
				string strCSharpObjectName = getProperty("CSharpObject", "");
				if (strCSharpObjectName != "")
				{					
					mType = Type.GetType(strCSharpObjectName);
				}
			}
			catch
			{

			}

			try
			{
				if (mType == null)
				{
					mType = Type.GetType(dataGroupName);
				}
			}
			catch
			{

			}
		
			if( mType == null )
			{				
				strError = "Unable to get Type: (" + dataGroupName + ") or (" + strAssemblyName + ")";
				return null;
			}
			try
			{

				
				
				System.Xml.Serialization.XmlSerializer mXmlSerailizer = new System.Xml.Serialization.XmlSerializer(mType);
				Object mNewObject = mXmlSerailizer.Deserialize(new StringReader(getDataGroupAsString()));
				if (mNewObject == null)
				{
					strError = "Unable to deserialize the object from data group: " + mType.Name;

				}
				return mNewObject;
			}
			catch ( Exception e)
			{
				strError = e.Message;
			}
			return null;
		}



	}
}
