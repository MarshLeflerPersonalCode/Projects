//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"


//We use a lot of casting and loading of different types in different systems.
//This is meant to help bridge and make one central area for things to be casted about


enum class EDATATYPES : uint8
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


namespace DATATYPES_UTILS
{



	//returns the data type name
	const KCString &		getDataTypeName(EDATATYPES eType);

	//this expects a string patching the type "STRING" == EDATATYPES::STRING. If not found returns EDATATYPES::COUNT
	EDATATYPES				getDataTypeByDataTypeName(const KCString &strTypeUpperCase);

	//this expects something like "1.5" == FLOAT or "this is a string" == STRING or "3" == INT32
	EDATATYPES				getDataTypeRepresentingValue(const KCString &strValue, coreUnionData64Bit &mResult = EMPTY_64BIT_UNIONDATA, EDATATYPES eDefaultType = EDATATYPES::INT32);

	KCString 				getAsString(int64 iValue);
	KCString 				getAsString(uint64 iValue);
	KCString 				getAsString(int32 iValue);
	KCString 				getAsString(uint32 iValue);
	KCString 				getAsString(bool bValue);
	KCString 				getAsString(int8 iValue);
	KCString 				getAsString(uint8 iValue);
	KCString 				getAsString(int16 iValue);
	KCString 				getAsString(uint16 iValue);
	KCString 				getAsString(float fValue);
	
	int64					getAsInt64(const KCString &strValue);
	uint64					getAsUInt64(const KCString &strValue);
	int32					getAsInt32(const KCString &strValue);
	uint32					getAsUInt32(const KCString &strValue);
	bool					getAsBool(const KCString &strValue);
	int8					getAsInt8(const KCString &strValue);
	uint8					getAsUInt8(const KCString &strValue);
	int16					getAsInt16(const KCString &strValue);
	uint16					getAsUInt16(const KCString &strValue);
	float					getAsFloat(const KCString &strValue);
	
}; //end of namespace