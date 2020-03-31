//copyright Marsh Lefler 2000-...
#include "KCFileUtilities.h"

bool KCFileUtilities::loadFile(const TCHAR *strFile, KCTArray<uint8> &mArray)
{
#if USING_UE4
	FString strFullPath = FPaths::ProjectContentDir() + strFile;
	TArray<uint8> mUE4Array;
	if (FFileHelper::LoadFileToArray(mUE4Array, *strFullPath, 0))
	{
		mArray.copyMemoryUE4(mUE4Array);
		return true;
	}
	
#else	 
	FILE *pFileReader = null;
	if (_wfopen_s(&pFileReader, strFile, L"r") == 0)
	{
		fseek(pFileReader, 0, SEEK_END);
		size_t iFileSize = ftell(pFileReader);
		fseek(pFileReader, 0, SEEK_SET);		
		fread_s(mArray._AttemptMemoryCopy((uint32)iFileSize), iFileSize, sizeof(uint8), iFileSize, pFileReader);
		fclose(pFileReader);
		return true;
	}
	
#endif
	return false;

}
