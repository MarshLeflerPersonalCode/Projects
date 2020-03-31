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
	DOUBLE,
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
	"DOUBLE",
	"STRING"
};

