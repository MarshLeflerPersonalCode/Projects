#pragma once
//copyright Marsh Lefler 2000-...
#include "KCDefines.h"


class KCDataGroup;
class KCByteWriter;

class KCDataGroupBinaryWriter
{
public:
	static bool				writeDataGroupToFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup);
	
};
