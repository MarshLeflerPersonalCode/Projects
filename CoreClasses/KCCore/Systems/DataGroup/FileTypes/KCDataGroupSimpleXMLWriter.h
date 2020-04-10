#pragma once
//copyright Marsh Lefler 2000-...
#include "KCDefines.h"


class KCDataGroup;


class KCDataGroupSimpleXMLWriter
{
public:
	static bool				writeDataGroupToFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup);
	static KCString			writeDataGroupToString(KCDataGroup &mDataGroup);
};
