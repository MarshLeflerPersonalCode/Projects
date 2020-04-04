#pragma once
//copyright Marsh Lefler 2000-...
#include "KCDefines.h"


class KCDataGroup;

class KCDataGroupSimpleXMLReader
{
public:
	static bool				parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup);
	static bool				parseDataGroupFromString(const KCString &mData, KCDataGroup &mDataGroup);
};