#pragma once
//copyright Marsh Lefler 2000-...
#include "KCDefines.h"


class KCDataGroup;




class KCDataGroupBinaryReader
{
public:
	static bool				parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup);
	
};
