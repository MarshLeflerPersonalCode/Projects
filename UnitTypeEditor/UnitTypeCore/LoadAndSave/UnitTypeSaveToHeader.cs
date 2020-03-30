using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UnitTypeCore.LoadAndSave
{
	public class UnitTypeSaveToHeader
	{
		static string m_strInt8 = "int8";
		static string m_strInt32 = "int32";
		static string m_strString = "std::string";
		public UnitTypeSaveToHeader()
		{

		}


		public static string makeStringLength(string strToAddSpacesTo, int iLengthRequested)
		{
			if(iLengthRequested < 0 ||
				strToAddSpacesTo == null)
			{
				return "";
			}

			while(strToAddSpacesTo.Length < iLengthRequested)
			{
				strToAddSpacesTo = strToAddSpacesTo + " ";
			}
			return strToAddSpacesTo;
		}

		private static void defineVariableTypes(UnitTypeCategory mCategory)
		{
			UnitTypeManagerConfig mConfig = mCategory.unitTypeManager.getUnitTypeManagerConfig();
			m_strInt8 = mConfig.defineInt8;
			m_strInt32 = mConfig.defineInt32;
			m_strString = mConfig.defineString;
		}

		private static string _getEnumCountName(UnitTypeCategory mCategory)
		{
			return mCategory.categoryConfig.enumName + "_COUNT";
		}

		private static string _getVariableForNumberOfBytes(UnitTypeCategory mCategory)
		{
			return mCategory.categoryConfig.enumName + "_BYTES_NEEDED";
		}

		private static string _getUnitTypeStructName(UnitTypeCategory mCategory)
		{
			return "FUnitTypeData_" + mCategory.categoryName;
		}
		private static string _getGlobalDataArrayName(UnitTypeCategory mCategory)
		{
			return "g_UnitTypeDataArray_" + mCategory.categoryConfig.categoryName;
		}


		private static string _getEnumLookupData(UnitTypeCategory mCategory)
		{
			int iNumberOfBits = mCategory.unitTypes[0].bitLookupArray.Count;
			int iLengthOfName = mCategory.categoryConfig.enumName.Length + 5;
			string strReturnString = "";
			strReturnString = strReturnString + "struct " + _getUnitTypeStructName(mCategory) + System.Environment.NewLine;
			strReturnString = strReturnString + "{" + System.Environment.NewLine;
			strReturnString = strReturnString  + "     " + makeStringLength(m_strString, iLengthOfName) + "m_strName;" + System.Environment.NewLine;
			strReturnString = strReturnString + "     " + makeStringLength(mCategory.categoryConfig.enumName, iLengthOfName) + "m_eType;" + System.Environment.NewLine;
			strReturnString = strReturnString + "     " + makeStringLength(m_strInt32, iLengthOfName) + "m_Data[" + _getVariableForNumberOfBytes(mCategory) + "]; " + System.Environment.NewLine;
			strReturnString = strReturnString + "};" + System.Environment.NewLine;
			return strReturnString;
		}

		private static bool _buildLookupData(UnitTypeCategory mCategory, ref string strLine)
		{
			int iLongestLength = 35;
			foreach (UnitType mUnitType in mCategory.getUnitTypes())
			{
				if (mUnitType.unitTypeName.Length > iLongestLength)
				{
					iLongestLength = mUnitType.unitTypeName.Length;
				}
			}
			iLongestLength += 5;
			strLine = strLine + _getEnumLookupData(mCategory) + System.Environment.NewLine;			
			strLine = strLine + "static " + _getUnitTypeStructName(mCategory) + " " + _getGlobalDataArrayName(mCategory) + "[" + _getEnumCountName(mCategory) + "] = {" + System.Environment.NewLine;
			strLine = strLine + makeStringLength("     //Name of Unit Type(m_strName)", iLongestLength) + makeStringLength("Unit Type Enum(m_eType)", iLongestLength + mCategory.categoryConfig.enumName.Length) + "Data(m_Data)" + System.Environment.NewLine;
			List<UnitType> mUnitTypes = mCategory.getUnitTypes();
			foreach (UnitType mUnitType in mUnitTypes)
			{
				string strCodeLine = makeStringLength("     \"" + mUnitType.unitTypeName + "\",", iLongestLength);
				strCodeLine = strCodeLine + makeStringLength(mCategory.categoryConfig.enumName + "::" + mUnitType.unitTypeName + ",", iLongestLength + mCategory.categoryConfig.enumName.Length);
				mUnitType._calculateBitArray();
				string strNumbers = "";
				for(int iNumIndex = 0; iNumIndex < mUnitType.bitLookupArray.Count; iNumIndex++)
				{
					strNumbers = strNumbers + mUnitType.bitLookupArray[iNumIndex].ToString();
					if( iNumIndex != mUnitType.bitLookupArray.Count - 1)
					{
						strNumbers = strNumbers + ", ";
					}					
				}
				strCodeLine = strCodeLine + "{" + strNumbers + "}," + System.Environment.NewLine;
				strLine = strLine + strCodeLine;
			}			
			strLine = strLine + "};" + System.Environment.NewLine;
			return true;
		}

		private static bool _buildFunctions(UnitTypeCategory mCategory, ref string strLine)
		{
			string strFunctions = @"
//ISA checks to see if A is a B. So if A was a sword, and sword was a weapon and an item, and B could be sword, weapon or item and this would return true. Else it would return false
static bool ISA([UnitType] A_is_a, [UnitType] B )
{	
	UnitTypeInt32 iBitOffsetIntoInt = (UnitTypeInt32)B % 32;
	UnitTypeInt32 iIndexOfArray = (UnitTypeInt32)B / 32;
	return ([GlobalArray][(UnitTypeInt32)A_is_a].m_Data[iIndexOfArray] & ( 1 << iBitOffsetIntoInt ))?true:false;	
}

//Returns the unit type name as a string
static UnitTypeString getUnitTypeName([UnitType] A )
{
	return [GlobalArray][(UnitTypeInt32)A].m_strName;
}

//returns the unit type by a string
static [UnitType] getUnitTypeByNameFor_[CategoryName](UnitTypeString strName )
{
	for(UnitTypeInt32 iIndex = 0; iIndex < EUNITTYPES_ITEMS_COUNT; iIndex++)	
	{
		if( [GlobalArray][iIndex].m_strName == strName )
		{
			return ([UnitType])iIndex;
		}
	}
	return ([UnitType])0;
}
";

			strFunctions = strFunctions.Replace("[UnitType]", mCategory.categoryConfig.enumName);
			strFunctions = strFunctions.Replace("UnitTypeString", m_strString);
			strFunctions = strFunctions.Replace("UnitTypeInt32", m_strInt32);
			strFunctions = strFunctions.Replace("[CategoryName]", mCategory.categoryConfig.categoryName);
			strFunctions = strFunctions.Replace("[GlobalArray]", _getGlobalDataArrayName(mCategory));
			strLine = strLine + strFunctions + System.Environment.NewLine;
			return true;
		}

		private static bool _replaceWithEnum(UnitTypeCategory mCategory, ref string strLine)
		{
			int iIndexOfEnum = strLine.IndexOf("[ENUM]");
			if( iIndexOfEnum < 0 )
			{
				return true;
			}
			int iLongestLength = 1;
			foreach (UnitType mUnitType in mCategory.getUnitTypes())
			{
				if( mUnitType.unitTypeName.Length > iLongestLength )
				{
					iLongestLength = mUnitType.unitTypeName.Length;
				}
			}
			iLongestLength += 5;
			string strEnumLayout = @"enum class " + mCategory.categoryConfig.enumName + System.Environment.NewLine;
			strEnumLayout = strEnumLayout + "{" + System.Environment.NewLine;
			for ( int iIndex = 0; iIndex < mCategory.getUnitTypes().Count; iIndex++ )
			{
				UnitType mUnitType = mCategory.getUnitTypes()[iIndex];
				string strLineToAdd = makeStringLength("", 5);
				strLineToAdd = strLineToAdd + makeStringLength(strLineToAdd + mUnitType.unitTypeName, iLongestLength + 5);
				strLineToAdd = strLineToAdd + "= " + iIndex.ToString() + ",";
				if (mUnitType.description != null &&
					mUnitType.description != "" )
				{
					strLineToAdd = makeStringLength(strLineToAdd, 50) + "//" + mUnitType.description;
				}
				strEnumLayout = strEnumLayout + strLineToAdd + System.Environment.NewLine;
			}
			strEnumLayout = strEnumLayout + "};" + System.Environment.NewLine;
			strEnumLayout = strEnumLayout + System.Environment.NewLine;
			strEnumLayout = strEnumLayout + "#define " + _getEnumCountName(mCategory) + " " + mCategory.getUnitTypes().Count.ToString() + System.Environment.NewLine;
			strEnumLayout = strEnumLayout + "#define " + _getVariableForNumberOfBytes(mCategory) + " " + mCategory.unitTypes[0].bitLookupArray.Count.ToString() + System.Environment.NewLine;
			strEnumLayout = strEnumLayout + System.Environment.NewLine;
			strLine = strEnumLayout;
			return true;
			
		}
		private static bool _replaceWithFunctions(UnitTypeCategory mCategory, ref string strLine)
		{
			int iIndexOfEnum = strLine.IndexOf("[FUNCTIONS]");
			if (iIndexOfEnum < 0)
			{
				return true;
			}
			strLine = "";
			if ( _buildLookupData(mCategory, ref strLine) == false )
			{
				return false;
			}
			return _buildFunctions(mCategory, ref strLine);
		}
		public static bool _parseLine(UnitTypeCategory mCategory, ref string strLine)
		{
			if( strLine.Contains("//") ||
				strLine.Contains("/*"))
			{
				return true;
			}
			if( _replaceWithEnum(mCategory, ref strLine) == false ||
				_replaceWithFunctions(mCategory, ref strLine) == false )
			{
				return false;
			}
			return true;
		}

		public static bool saveToHeader(UnitTypeCategory mCategory)
		{
			if(mCategory == null )
			{
				return false;
			}
			UnitTypeManager mManager = mCategory.unitTypeManager;
			string strHeaderFileLocation = UnitTypeFile.getValidFileAndPath( Path.Combine( mCategory.categoryConfig.enumHeaderDirectory, mCategory.categoryConfig.enumHeaderFile ));
			if( strHeaderFileLocation == "" ||
				mManager.getUnitTypeManagerConfig().enumHeaderLayout == null ||
				mManager.getUnitTypeManagerConfig().enumHeaderLayout == "" ||
				mCategory.categoryConfig.enumName == null ||
				mCategory.categoryConfig.enumName == "" ||
				mCategory.categoryConfig.unitTypes.Count == 0)
			{
				return false;
			}
			StreamWriter mHeaderFileStream = null;
			try
			{
				defineVariableTypes(mCategory);
				//header layout
				string strEnumHeaderLayout = mManager.getUnitTypeManagerConfig().enumHeaderLayout;
				StringReader stringReader = new StringReader(strEnumHeaderLayout);

				//lets try to open the file for writing
				mHeaderFileStream = new StreamWriter(strHeaderFileLocation);
				if (mHeaderFileStream == null)
				{
					return false;
				}
				string strHeaderLayoutLine = "";
				while ((strHeaderLayoutLine = stringReader.ReadLine()) != null)
				{
					_parseLine(mCategory, ref strHeaderLayoutLine);
					mHeaderFileStream.WriteLine(strHeaderLayoutLine);

				}
				mHeaderFileStream.Close();
				stringReader.Close();
			}
			catch(Exception e)
			{
				if(mHeaderFileStream != null)
				{
					mHeaderFileStream.Close();
				}
				// Let the user know what went wrong.
				Console.WriteLine("The header file could not be saved:");
				Console.WriteLine(e.Message);
				return false;
			}

			return true;
		}

	}
}
