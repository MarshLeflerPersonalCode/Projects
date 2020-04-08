#include "KCDataTypes.h"
#include "KCStringUtils.h"
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

EDATATYPES DATATYPES_UTILS::getDataTypeRepresentingValue(const KCString &strValue, EDATATYPES eDefaultType)
{
	if (strValue.empty()) { return eDefaultType; }
	if (KCStringUtils::isNumber(strValue) == false)
	{
		if (strValue.length() == 4 || strValue.length() == 5)
		{
			if (strValue.c_str()[0] == 't' || strValue.c_str()[0] == 'T' ||
				strValue.c_str()[0] == 'f' || strValue.c_str()[0] == 'F')
			{
				KCString strValueUpper = KCStringUtils::toUpperNewString(strValue);
				if (strValue == "TRUE" ||
					strValue == "FALSE")
				{
					return EDATATYPES::BOOL;
				}
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
		return EDATATYPES::FLOAT;
	}
	std::stringstream mStream(strValue);
	int64 iInt64Value(0);
	mStream >> iInt64Value;

	if (mStream.fail() == false)
	{
		if (iInt64Value >= INT_MIN && iInt64Value <= INT_MAX)
		{
			return EDATATYPES::INT32;
		}
		else if (iInt64Value >= 0 && iInt64Value <= UINT_MAX)
		{
			return EDATATYPES::UINT32;
		}
		return EDATATYPES::INT64;
	}
	else if (strValue.c_str()[0] != '-')
	{
		std::stringstream mStream64UintTest(strValue);
		mStream64UintTest << strValue;
		uint64 iuInt64Value(0);
		mStream64UintTest >> iuInt64Value;
		if (mStream64UintTest.fail() == false)
		{
			//has to be a uint64
			return EDATATYPES::UINT64;
		}
	}

	return EDATATYPES::COUNT;
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
