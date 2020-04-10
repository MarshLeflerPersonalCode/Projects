//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"



enum class EDATAGROUP_BINARY_VERSION : uint8
{
	ONE,
	COUNT	//always last
};

#define KDATAGROUP_BINARY_VERSION			(EDATAGROUP_BINARY_VERSION)((uint8)EDATAGROUP_BINARY_VERSION::COUNT - 1)
#define KDATAGROUP_BINARY_VERSION_ERROR		0xFF