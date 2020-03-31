//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once
#include "KCIncludes.h"




class KCFileUtilities
{
public:
	//loads a file and puts the bits into the array
	static bool						loadFile(const TCHAR *strFile, KCTArray<uint8> &mArray);


};	//end namespace
	
