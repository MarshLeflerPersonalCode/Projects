#include "Utils/TypeDefinitions/KCDataTypes.h"
#include "Utils/KCStringUtils.h"
#include <sstream>

static const KCString g_strDataTypeNames[(int32)EDATATYPES::COUNT] = {
	"BOOL",
	"CHAR",
	"INT8",
	"UINT8",
	"INT16",
	"UINT16",
	"INT32",
	"UINT32",
	"INT64",
	"UINT64",
	"FLOAT",
	"STRING"
};

const KCString & DATATYPES_UTILS::getDataTypeName(EDATATYPES eType)
{
	return g_strDataTypeNames[(int32)eType];
}

EDATATYPES DATATYPES_UTILS::getDataTypeByDataTypeName(const KCString &strTypeUpperCase)
{
	for (int32 i = 0; i < (int32)EDATATYPES::COUNT; i++)
	{
		if (g_strDataTypeNames[i] == strTypeUpperCase)
		{
			return (EDATATYPES)i;
		}
	}
	return EDATATYPES::COUNT;
}

EDATATYPES DATATYPES_UTILS::getDataTypeRepresentingValue(const KCString &strValue, coreUnionData64Bit &mResult, EDATATYPES eDefaultType)
{
	mResult.m_uiValue64 = 0;
	if (strValue.empty()) { return eDefaultType; }
	if (KCStringUtils::isNumber(strValue) == false)
	{
		if (strValue.length() == 4 || strValue.length() == 5)
		{
			if (strValue.c_str()[0] == 't' || strValue.c_str()[0] == 'T')
			{
				mResult.m_iValue64 = 1;
				return EDATATYPES::BOOL;
			}
			else if(strValue.c_str()[0] == 'f' || strValue.c_str()[0] == 'F')
			{								
				return EDATATYPES::BOOL;
			}
		}
		return EDATATYPES::STRING;
	}

	if (strValue.size() == 1 &&
		strValue.c_str()[0] == '0')
	{
		return EDATATYPES::INT32;
	}
	bool isFloat = std::find_if(strValue.begin(), strValue.end(), [](unsigned char c) { return c == '.'; }) != strValue.end();
	if (isFloat)
	{
		std::stringstream mStreamFloat(strValue);
		mStreamFloat >> mResult.m_fValue[0];
		if (mStreamFloat.fail() == false)
		{
			return EDATATYPES::COUNT;
		}
		return EDATATYPES::FLOAT;
	}
	std::stringstream mStream(strValue);	
	mStream >> mResult.m_iValue64;

	if (mStream.fail() == false)
	{
		if (mResult.m_iValue64 >= INT_MIN && mResult.m_iValue64 <= INT_MAX)
		{
			return EDATATYPES::INT32;
		}
		else if (mResult.m_iValue64 >= 0 && mResult.m_iValue64 <= UINT_MAX)
		{
			return EDATATYPES::UINT32;
		}
		return EDATATYPES::INT64;
	}
	else if (strValue.c_str()[0] != '-')
	{
		std::stringstream mStream64UintTest(strValue);
		mStream64UintTest << strValue;
		mResult.m_uiValue64 = 0;
		mStream64UintTest >> mResult.m_uiValue64;
		if (mStream64UintTest.fail() == false)
		{
			//has to be a uint64
			return EDATATYPES::UINT64;
		}
	}
	mResult.m_iValue64 = 0;
	return EDATATYPES::COUNT;
}


int64 DATATYPES_UTILS::getAsInt64(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT){ return mData.m_iValue64;}
	return INVALID;
}


uint64 DATATYPES_UTILS::getAsUInt64(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_iValue64; }
	return INVALID;
}

int32 DATATYPES_UTILS::getAsInt32(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_iValue32[0]; }
	return INVALID;

}

uint32 DATATYPES_UTILS::getAsUInt32(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_uiValue32[0]; }
	return INVALID;
}

bool DATATYPES_UTILS::getAsBool(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_bValue[0]; }
	return false;
}

int8 DATATYPES_UTILS::getAsInt8(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_iValue8[0]; }
	return INVALID;
}

uint8 DATATYPES_UTILS::getAsUInt8(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_uiValue8[0]; }
	return INVALID;
}

int16 DATATYPES_UTILS::getAsInt16(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_iValue16[0]; }
	return INVALID;
}

uint16 DATATYPES_UTILS::getAsUInt16(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_uiValue16[0]; }
	return INVALID;
}

float DATATYPES_UTILS::getAsFloat(const KCString &strValue)
{
	coreUnionData64Bit mData;
	if (getDataTypeRepresentingValue(strValue, mData) != EDATATYPES::COUNT) { return mData.m_fValue[0]; }
	return INVALID;
}


KCString DATATYPES_UTILS::getAsString(int64 iValue) { return KCStringUtils::getAsString( iValue); }

KCString DATATYPES_UTILS::getAsString(float fValue) { return KCStringUtils::getAsString(fValue); }

KCString DATATYPES_UTILS::getAsString(uint16 iValue) { return KCStringUtils::getAsString(iValue); }

KCString DATATYPES_UTILS::getAsString(int16 iValue) { return KCStringUtils::getAsString(iValue); }

KCString DATATYPES_UTILS::getAsString(uint8 iValue) { return KCStringUtils::getAsString(iValue); }

KCString DATATYPES_UTILS::getAsString(int8 iValue) { return KCStringUtils::getAsString(iValue); }

KCString DATATYPES_UTILS::getAsString(bool bValue) { return KCStringUtils::getAsString(bValue); }

KCString DATATYPES_UTILS::getAsString(uint32 iValue) { return KCStringUtils::getAsString(iValue); }

KCString DATATYPES_UTILS::getAsString(int32 iValue) { return KCStringUtils::getAsString(iValue); }

KCString DATATYPES_UTILS::getAsString(uint64 iValue) { return KCStringUtils::getAsString(iValue); }
