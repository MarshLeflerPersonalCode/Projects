//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"

enum class EDATAGROUP_VARIABLE_TYPES : uint8
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

static KCString g_strDataGroupVariableTypeNames[(int32)EDATAGROUP_VARIABLE_TYPES::COUNT] = {
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

//this expects a string patching the type "STRING" == EDATAGROUP_VARIABLE_TYPES::STRING. If not found returns EDATAGROUP_VARIABLE_TYPES::COUNT
static EDATAGROUP_VARIABLE_TYPES getDataGroupTypeByString(const KCString &strTypeUpperCase)
{
	for (int32 i = 0; i < (int32)EDATAGROUP_VARIABLE_TYPES::COUNT; i++)
	{
		if (g_strDataGroupVariableTypeNames[i] == strTypeUpperCase)
		{
			return (EDATAGROUP_VARIABLE_TYPES)i;
		}
	}
	return EDATAGROUP_VARIABLE_TYPES::COUNT;
}

//this expects something like "1.5" == FLOAT or "this is a string" == STRING or "3" == INT32
static EDATAGROUP_VARIABLE_TYPES configureDataGroupTypeFromStringValue(const KCString &strValue)
{
	if (strValue.size() == 0)
	{
		return EDATAGROUP_VARIABLE_TYPES::INT32;
	}
	
	return getDataGroupTypeByString(KCStringUtils::getVariableType(strValue));
}


enum class EDATAGROUP_BINARY_VERSION : uint8
{
	one,
	COUNT	//always last
};

#define KDATAGROUP_BINARY_VERSION			(EDATAGROUP_VARIABLE_TYPES)((uint8)EDATAGROUP_BINARY_VERSION::COUNT - 1)
#define KDATAGROUP_BINARY_VERSION_ERROR		0xFF