//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once
#include "KCDefines.h"
#include "Utils/Containers/KCTArray.h"




class KCFileUtilities
{
public:
	//loads a file and puts the bits into the array
	static bool						loadFile(const TCHAR *strFile, TArray<uint8> &mArray);

	//saves a file
	static bool						saveToFile(const TCHAR* strFile, const TArray<uint8> mArray);
	static bool						saveToFile(const TCHAR* strFile, const char *pArray, size_t iCount);

	//returns a list of files in a directory, returns the count
	static int32					getFilesInDirectory(const TCHAR* strPath, const TCHAR* strSearchPattern, TArray<KCWString> &mListOfFiles, bool bRecusive = true);
	//returns the base directory
	static const KCString &			getApplicationDirectory();
	//returns the base directory
	static const std::wstring &		getApplicationDirectoryWide();
};	//end namespace
	
