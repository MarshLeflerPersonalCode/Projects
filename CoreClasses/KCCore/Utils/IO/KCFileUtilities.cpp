//copyright Marsh Lefler 2000-...
#include "KCFileUtilities.h"
#include "Utils/KCAsserts.h"
#include "Utils/KCStringUtils.h"
#if USING_UE4
#include "Misc/Paths.h"
#include "Misc/FileHelper.h"
#include "HAL/FileManager.h"
#else
#include "windows.h"
#include <direct.h>
#endif


bool KCFileUtilities::loadFile(const TCHAR *strFile, TArray<uint8> &mArray)
{
#if USING_UE4
	FString strFullPath = strFile;	
	if (FFileHelper::LoadFileToArray(mArray, *strFullPath, 0))
	{		
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

bool KCFileUtilities::saveToFile(const TCHAR* strFile, const TArray<uint8> mArray)
{
#if USING_UE4
	FString strFullPath = strFile;
	TArray<uint8> mUE4Array(mArray.GetData(), (int32)mArray.Num());	
	if (FFileHelper::SaveArrayToFile(mUE4Array, *strFullPath, 0))
	{		
		return true;
	}

#else	 
	FILE *pFileReader = null;
	if (_wfopen_s(&pFileReader, strFile, L"w") == 0)
	{
		fwrite(mArray.GetData(),  sizeof(uint8), (size_t)mArray.Num(), pFileReader);
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
	FString strFullPath = strFile;
	TArray<uint8> mUE4Array((const uint8 *)pArray, (int32)iCount);
	if (FFileHelper::SaveArrayToFile(mUE4Array, *strFullPath, 0))
	{

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

int32 KCFileUtilities::getFilesInDirectory(const TCHAR* strPath, const TCHAR* strSearchPattern, TArray<KCWString> &mListOfFiles, bool bRecusive /*= true*/)
{
	int32 iCount(mListOfFiles.Num());
#if USING_UE4
	TArray<FString> mFiles;
	if (bRecusive)
	{
		IFileManager::Get().FindFilesRecursive(mFiles, strPath, strSearchPattern, true, false);
	}
	else
	{
		IFileManager::Get().FindFiles(mFiles, strPath, strSearchPattern);
	}
	for (int32 iIndex = 0; iIndex < mFiles.Num(); iIndex++)
	{
		mListOfFiles.Add(*mFiles[iIndex]);
	}
#else
	
	bool bDone = false;
	bool bAddedApplicationDirectory = false;
	std::wstring strRootPath = strPath;
	std::wstring strPathAndSearchPattern = std::wstring(strPath) + std::wstring(strSearchPattern);
	_WIN32_FIND_DATAW mFileData;
	HANDLE mFileHandle = FindFirstFileW(strPathAndSearchPattern.c_str(), &mFileData);	
	if (mFileHandle == INVALID_HANDLE_VALUE)
	{
		strPathAndSearchPattern = getApplicationDirectoryWide() + strPathAndSearchPattern;
		strRootPath = getApplicationDirectoryWide() + strRootPath;
		bAddedApplicationDirectory = true;
		mFileHandle = FindFirstFileW(strPathAndSearchPattern.c_str(), &mFileData);
	}
	
	while (!bDone && mFileHandle != INVALID_HANDLE_VALUE )
	{

		if (mFileData.cFileName == null ||
			mFileData.cFileName[0] == '.' ||
			mFileData.dwFileAttributes & FILE_ATTRIBUTE_HIDDEN)
		{
			bDone = !FindNextFileW(mFileHandle, &mFileData);
			continue;
		}		
		
		if (!(mFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
		{
			mListOfFiles.add(strRootPath + std::wstring(mFileData.cFileName));

		}
		

		bDone = !FindNextFileW(mFileHandle, &mFileData);
	}

	if (bRecusive )
	{
		bAddedApplicationDirectory = false;
		bDone = false;
		strPathAndSearchPattern = std::wstring(strPath) + std::wstring(strSearchPattern);
		strRootPath = strPath;
		mFileHandle = FindFirstFileW((strRootPath + L"*.").c_str(), &mFileData);
		if (mFileHandle == INVALID_HANDLE_VALUE)
		{
			bAddedApplicationDirectory = true;
			strPathAndSearchPattern = getApplicationDirectoryWide() + strPathAndSearchPattern;
			strRootPath = getApplicationDirectoryWide() + strRootPath;
		}

		while (!bDone && mFileHandle != INVALID_HANDLE_VALUE)
		{

			if (mFileData.cFileName == null ||
				mFileData.cFileName[0] == '.' ||
				mFileData.dwFileAttributes & FILE_ATTRIBUTE_HIDDEN)
			{
				bDone = !FindNextFileW(mFileHandle, &mFileData);
				continue;
			}

			if (mFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
			{
				std::wstring strNewPath = ((bAddedApplicationDirectory) ? getApplicationDirectoryWide() : L"") + std::wstring(strPath) + std::wstring(mFileData.cFileName) + L"\\";
				getFilesInDirectory(strNewPath.c_str(), strSearchPattern, mListOfFiles, true);
			}

			bDone = !FindNextFileW(mFileHandle, &mFileData);
		}
	}

	FindClose(mFileHandle);
#endif
	return mListOfFiles.Num() - iCount;

}

const KCString & KCFileUtilities::getApplicationDirectory()
{
	static KCString g_strApplicationPath;
	if (g_strApplicationPath.length() != 0)
	{
		return g_strApplicationPath;
	}
#if USING_UE4
	g_strApplicationPath = KCStringUtils::convertWideToUtf8( *FPaths::ProjectContentDir() );

#else
//in the future we can redefine this for unix or whatever
#define GetCurrentDir _getcwd

	char cCurrentPath[FILENAME_MAX];
	if (!GetCurrentDir(cCurrentPath, sizeof(cCurrentPath)))
	{
		return g_strApplicationPath;
	}
	
	cCurrentPath[sizeof(cCurrentPath) - 1] = '\0'; /* not really required */
	g_strApplicationPath = cCurrentPath;
	g_strApplicationPath = g_strApplicationPath + "\\";
#endif
	return g_strApplicationPath;

}

const std::wstring & KCFileUtilities::getApplicationDirectoryWide()
{
	static std::wstring g_strApplicationPath;
	if (g_strApplicationPath.length() != 0)
	{
		return g_strApplicationPath;
	}
	g_strApplicationPath = KCStringUtils::toWide(getApplicationDirectory());
	return g_strApplicationPath;
}
