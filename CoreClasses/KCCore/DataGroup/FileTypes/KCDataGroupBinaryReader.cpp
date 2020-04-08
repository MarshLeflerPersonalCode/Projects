#include "KCDataGroupBinaryReader.h"
#include "IO/KCFileUtilities.h"
#include "DataGroup/KCDataGroup.h"
#include "IO/KCMemoryReader.h"
#include <sstream>

struct FKCDataGroupBinaryData
{
	EDATAGROUP_BINARY_VERSION	m_eVersion = (EDATAGROUP_BINARY_VERSION)KDATAGROUP_BINARY_VERSION;
	std::map<uint16, KCString>	m_StringLookupTable;
	KCByteReader				m_Reader;	
	KCString					m_strFile;
	KCDataGroup					*m_pParent = nullptr;
};

bool _getStringByID(uint16 iID, KCString &strOutputString, FKCDataGroupBinaryData &mData)
{
	auto mFound = mData.m_StringLookupTable.find(iID);
	if (mFound == mData.m_StringLookupTable.end())
	{		
		return false;
	}
	strOutputString = mFound->second;
	return true;
}

bool _readStringFromFile(KCString &strOutputString, FKCDataGroupBinaryData &mData)
{
	//reads the uint16	
	strOutputString = "ERROR";
	uint16 iID(INVALID);
	mData.m_Reader << iID;
	KCEnsureAlwaysMsgReturnVal(iID != INVALID, "Invalid ID for string in binary file: " + mData.m_strFile, false);	
	bool bFound = _getStringByID(iID, strOutputString, mData);
	KCEnsureAlwaysMsgReturnVal(bFound, "Invalid ID for string in binary file: " + mData.m_strFile, false);
	return true;
}

bool _readDataGroup(FKCDataGroupBinaryData &mData, KCDataGroup &mParentDataGroup)
{
	KCString strTmpString;		
	KCEnsureAlwaysMsgReturnVal(_readStringFromFile(strTmpString, mData), "Unable to read parent data group name from binary file: " + mData.m_strFile, false);
	KCDataGroup &mDataGroup = (mData.m_pParent == &mParentDataGroup)?mParentDataGroup:mParentDataGroup.getOrCreateChildGroup(strTmpString);
	mDataGroup.setGroupName(strTmpString);
	uint8 iCountOfProperties(INVALID);
	mData.m_Reader << iCountOfProperties;
	KCEnsureAlwaysMsgReturnVal(iCountOfProperties != INVALID, "Invalid count of propertis in binary file: " + mData.m_strFile, false);
	for (uint8 iIndex = 0; iIndex < iCountOfProperties; iIndex++)
	{
		uint8 eType(INVALID);
		KCEnsureAlwaysMsgReturnVal(_readStringFromFile(strTmpString, mData), "Unable to read data property name from binary file: " + mData.m_strFile, false);
		KCString strNameOfProperty = strTmpString;
		mData.m_Reader << eType;
		KCEnsureAlwaysMsgReturnVal(eType < (uint8)EDATATYPES::COUNT, "Unable to type of property(" + strNameOfProperty + ") from binary file: " + mData.m_strFile, false);
		KCDataProperty &mProperty = mDataGroup.getOrCreateProperty(strNameOfProperty);		
		mProperty.m_eType = (EDATATYPES)eType;
		switch (mProperty.m_eType)
		{
		case EDATATYPES::BOOL:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_bValue[0], sizeof(bool)), "Bad bool property(" + strNameOfProperty  + ") in binary file: " + mData.m_strFile, false );
			break;
		case EDATATYPES::CHAR:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_cValue[0], sizeof(char)), "Bad char property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::INT8:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_iValue8[0], sizeof(int8)), "Bad int8 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::UINT8:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_uiValue8[0], sizeof(uint8)), "Bad uint8 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::INT16:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_iValue16[0], sizeof(int16)), "Bad int16 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::UINT16:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_uiValue16[0], sizeof(uint16)), "Bad uint16 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		default:
		case EDATATYPES::INT32:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_iValue32, sizeof(int32)), "Bad int32 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::UINT32:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_uiValue32, sizeof(uint32)), "Bad uint32 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::INT64:
			{
				int64 iInt64;
				KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&iInt64, sizeof(int64)), "Bad int64 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
				mProperty << iInt64;
			}			
			break;
		case EDATATYPES::UINT64:
			{
				int64 uInt64;
				KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&uInt64, sizeof(uint64)), "Bad uint64 property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
				mProperty << uInt64;
			}
			break;
		case EDATATYPES::FLOAT:
			KCEnsureAlwaysMsgReturnVal(mData.m_Reader.readValue(&mProperty.m_Data.m_fValue, sizeof(float)), "Bad float property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);
			break;
		case EDATATYPES::STRING:
			{
				KCEnsureAlwaysMsgReturnVal(_readStringFromFile(strTmpString, mData), "Bad string property(" + strNameOfProperty + ") in binary file: " + mData.m_strFile, false);							
				mProperty << strTmpString;
			}
			break;
		}		
	}

	uint16 iChildrenCount(INVALID);
	mData.m_Reader << iChildrenCount;
	KCEnsureAlwaysMsgReturnVal(iChildrenCount != INVALID, "Invalid count of child data groups in binary file: " + mData.m_strFile, false);	
	for (uint16 iChildGroupIndex = 0; iChildGroupIndex < iChildrenCount; iChildGroupIndex++)
	{		
		if (_readDataGroup(mData, mDataGroup) == false)
		{
			return false;
		}
		
	}
	
	return true;
}

bool _loadHeader(FKCDataGroupBinaryData &mData)
{

	uint8 iVersion(KDATAGROUP_BINARY_VERSION_ERROR);
	mData.m_Reader << iVersion;
	KCEnsureAlwaysMsgReturnVal(iVersion >= (uint8)KDATAGROUP_BINARY_VERSION, "Bad version in binary file: " + mData.m_strFile, false);
	mData.m_eVersion = (EDATAGROUP_BINARY_VERSION)iVersion;
	return true;
}

bool _loadStringTable(FKCDataGroupBinaryData &mData)
{
	uint16 iCount(INVALID);
	mData.m_Reader << iCount;
	KCEnsureAlwaysMsgReturnVal(iCount != INVALID, "Bad count in string table for binary file: " + mData.m_strFile, false);
	KCString strStringLoaded;
	for (uint16 iIndex = 0; iIndex < iCount; iIndex++ )
	{		
		uint16 iId(0);
		mData.m_Reader << iId;
		mData.m_Reader << strStringLoaded;
		mData.m_StringLookupTable[iId] = strStringLoaded;
	}
	return true;
}

bool KCDataGroupBinaryReader::parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	KCTArray<uint8> mMemoryArray;
	KCEnsureAlwaysMsgReturnVal(KCFileUtilities::loadFile(strPathAndFile, mMemoryArray), "Unable to load file", false);
	FKCDataGroupBinaryData mData;
	mData.m_strFile = KCStringUtils::converWideToUtf8(strPathAndFile);
	mData.m_pParent = &mDataGroup;
	mData.m_Reader.configureByTArray(mMemoryArray);
	if (_loadHeader(mData) == false)
	{
		return false;
	}
	if (_loadStringTable(mData) == false)
	{
		return false;
	}	
	if (_readDataGroup(mData, mDataGroup) == false)
	{
		return false;
	}
	
	
	return true;
}
