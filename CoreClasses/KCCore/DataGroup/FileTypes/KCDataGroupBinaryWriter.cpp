#include "KCDataGroupBinaryWriter.h"
#include "IO/KCFileUtilities.h"
#include "IO/KCByteWriter.h"
#include "DataGroup/KCDataGroup.h"
#include <sstream>




uint16 _getStringID(const KCString &strString, std::map<KCString, uint16> &mStringLookupTable)
{
	auto mFound = mStringLookupTable.find(strString);
	if (mFound == mStringLookupTable.end())
	{
		mStringLookupTable[strString] = (uint16)mStringLookupTable.size();
		return (uint16)(mStringLookupTable.size() - 1);
	}
	return mFound->second;
}

uint16 _getStringID(const KCName &strString, std::map<KCString, uint16> &mStringLookupTable)
{
	return _getStringID(strString.toString(), mStringLookupTable);
}
void _addString(const KCString &strString, std::map<KCString, uint16> &mStringLookupTable)
{
	auto mFound = mStringLookupTable.find(strString);
	if (mFound == mStringLookupTable.end())
	{
		mStringLookupTable[strString] = (uint16)mStringLookupTable.size();
	}
}
void _addString(const KCName &strString, std::map<KCString, uint16> &mStringLookupTable)
{
	_addString(strString.toString(), mStringLookupTable);
}

//builds the string lookup table
void _buildStringLookupTable(KCDataGroup &mDataGroup, std::map<KCString, uint16> &mStringLookupTable)
{
	_addString( mDataGroup.getGroupNameAsString(), mStringLookupTable);
	for (auto mPropertyIter : mDataGroup.getProperties())
	{
		KCDataProperty &mProperty = mPropertyIter.second;
		_addString(mProperty.m_strLookupName, mStringLookupTable);
		if (mProperty.m_eType == EDATAGROUP_VARIABLE_TYPES::STRING)
		{
			_addString(mProperty.getAsString(), mStringLookupTable);
		}
	}

	for (auto mChildIter : mDataGroup.getChildGroups())
	{
		_buildStringLookupTable(mChildIter.second, mStringLookupTable);
	}
}

void _writeDataGroup(KCDataGroup &mDataGroup, KCByteWriter &mWriter, std::map<KCString, uint16> &mStringLookupTable)
{
	mWriter << _getStringID(mDataGroup.getGroupName(), mStringLookupTable);
	uint8 iCountOfProperties((uint8)mDataGroup.getProperties().size());
	mWriter << iCountOfProperties;
	for (auto mPropertyIter : mDataGroup.getProperties())
	{
		KCDataProperty &mProperty = mPropertyIter.second;
		uint8 eType = (uint8)mProperty.m_eType;
		mWriter << _getStringID(mProperty.m_strLookupName, mStringLookupTable);
		mWriter << eType;
		switch (mProperty.m_eType)
		{
		case EDATAGROUP_VARIABLE_TYPES::BOOL:
			mWriter << mProperty.m_Data.m_bValue[0];
			break;
		case EDATAGROUP_VARIABLE_TYPES::CHAR:
			mWriter << mProperty.m_Data.m_cValue[0];
			break;
		case EDATAGROUP_VARIABLE_TYPES::INT8:
			mWriter << mProperty.m_Data.m_iValue8[0];
			break;
		case EDATAGROUP_VARIABLE_TYPES::UINT8:
			mWriter << mProperty.m_Data.m_uiValue8[0];
			break;
		case EDATAGROUP_VARIABLE_TYPES::INT16:
			mWriter << mProperty.m_Data.m_iValue16[0];
			break;
		case EDATAGROUP_VARIABLE_TYPES::UINT16:
			mWriter << mProperty.m_Data.m_uiValue16[0];
			break;
		default:
		case EDATAGROUP_VARIABLE_TYPES::INT32:
			mWriter << mProperty.m_Data.m_iValue32;
			break;
		case EDATAGROUP_VARIABLE_TYPES::UINT32:
			mWriter << mProperty.m_Data.m_uiValue32;
			break;
		case EDATAGROUP_VARIABLE_TYPES::INT64:
			mWriter << mProperty.getAsInt64();
			break;
		case EDATAGROUP_VARIABLE_TYPES::UINT64:
			mWriter << mProperty.getAsUInt64();
			break;
		case EDATAGROUP_VARIABLE_TYPES::FLOAT:
			mWriter << mProperty.m_Data.m_fValue;
			break;
		case EDATAGROUP_VARIABLE_TYPES::STRING:
			{
				mWriter << _getStringID(mProperty.getAsString(), mStringLookupTable);
			}
			break;
		}		
	}

	uint16 iChildrenCount((uint16)mDataGroup.getChildGroups().size());
	mWriter << iChildrenCount;
	for (auto mChildIter : mDataGroup.getChildGroups())
	{
		_writeDataGroup(mChildIter.second, mWriter, mStringLookupTable);

	}
	

}

void _writeHeader(KCByteWriter &mWriter)
{

	uint8 iVersion((uint8)KDATAGROUP_BINARY_VERSION);
	mWriter << iVersion;
}

void _writeStringTable(KCByteWriter &mWriter, std::map<KCString, uint16> &mStringLookupTable)
{
	uint16 iCount((uint16)mStringLookupTable.size());
	mWriter << iCount;
	for (auto mStringIter : mStringLookupTable)
	{
		mWriter << mStringIter.second;
		mWriter << mStringIter.first;	

	}
}

bool KCDataGroupBinaryWriter::writeDataGroupToFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	
	mDataGroup.setProperty("_FILE_", strPathAndFile);
	std::map<KCString, uint16> mStringLookupTable;
	KCByteWriter mWriter;	
	_buildStringLookupTable(mDataGroup, mStringLookupTable);
	_writeHeader(mWriter);
	_writeStringTable(mWriter, mStringLookupTable);
	_writeDataGroup(mDataGroup, mWriter, mStringLookupTable);
	KCFileUtilities::saveToFile(strPathAndFile, mWriter.getByteArrayAsChar(), mWriter.getByteArrayCount());
	
	return true;
}
