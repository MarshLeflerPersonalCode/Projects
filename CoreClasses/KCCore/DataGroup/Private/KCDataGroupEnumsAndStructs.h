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


enum class EDATAGROUP_BINARY_VERSION : uint8
{
	one,
	COUNT	//always last
};

#define KDATAGROUP_BINARY_VERSION			(EDATAGROUP_VARIABLE_TYPES)((uint8)EDATAGROUP_BINARY_VERSION::COUNT - 1)
#define KDATAGROUP_BINARY_VERSION_ERROR		0xFF