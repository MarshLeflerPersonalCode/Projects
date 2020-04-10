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
		fread_s(mArray._attemptMemoryCopy((uint32)iFileSize), iFileSize, sizeof(uint8), iFileSize, pFileReader);
		fclose(pFileReader);
		return true;
	}
	
#endif
	KCEnsureAlways(false);
	return false;

}

bool KCFileUtilities::saveToFile(const TCHAR* strFile, const KCTArray<uint8> mArray)
{
#if USING_UE4
	FString strFullPath = FPaths::ProjectContentDir() + strFile;
	TArray<uint8> mUE4Array;
	mUE4Array.mer
	if (FFileHelper::SaveArrayToFile(mUE4Array, *strFullPath, 0))
	{
		mArray.copyMemoryUE4(mUE4Array);
		return true;
	}

#else	 
	FILE *pFileReader = null;
	if (_wfopen_s(&pFileReader, strFile, L"w") == 0)
	{
		fwrite(mArray.getMemory(),  sizeof(uint8), (size_t)mArray.getCount(), pFileReader);
		fclose(pFileReader);
		return true;
	}

#endif
	KCEnsureAlways(false);
	return false;
}

bool KCFileUtilities::saveToFile(const TCHAR* strFile, const char *pArray, size_t iCount)
{
#if USING_UE4
	FString strFullPath = FPaths::ProjectContentDir() + strFile;
	TArray<uint8> mUE4Array;
	mUE4Array.mer
		if (FFileHelper::SaveArrayToFile(mUE4Array, *strFullPath, 0))
		{
			mArray.copyMemoryUE4(mUE4Array);
			return true;
		}

#else	 
	FILE *pFileReader = null;
	if (_wfopen_s(&pFileReader, strFile, L"w") == 0)
	{
		fwrite(pArray, sizeof(char), iCount, pFileReader);
		fclose(pFileReader);
		return true;
	}

#endif
	KCEnsureAlways(false);
	return false;
}
