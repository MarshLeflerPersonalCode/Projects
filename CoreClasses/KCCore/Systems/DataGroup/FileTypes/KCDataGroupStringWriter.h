#pragma once
//copyright Marsh Lefler 2000-...
#include "KCDefines.h"


class KCDataGroup;
class KCByteWriter;

class KCDataGroupStringWriter
{
public:
	static bool				writeDataGroupToFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup);
	static KCString			writeDataGroupToString(KCDataGroup &mDataGroup);
};
