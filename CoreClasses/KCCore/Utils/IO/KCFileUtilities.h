//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once
#include "KCIncludes.h"




class KCFileUtilities
{
public:
	//loads a file and puts the bits into the array
	static bool						loadFile(const TCHAR *strFile, KCTArray<uint8> &mArray);

	//saves a file
	static bool						saveToFile(const TCHAR* strFile, const KCTArray<uint8> mArray);
	static bool						saveToFile(const TCHAR* strFile, const char *pArray, size_t iCount);
};	//end namespace
	
