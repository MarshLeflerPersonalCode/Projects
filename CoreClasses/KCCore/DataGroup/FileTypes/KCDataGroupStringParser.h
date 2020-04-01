//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"

class KCDataGroup;
class KCByteWriter;

class KCDataGroupStringParser
{
public:
	static bool				parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup);
};
