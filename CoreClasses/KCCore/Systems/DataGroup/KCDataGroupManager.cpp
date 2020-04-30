#include "KCDataGroupManager.h"
#include "Systems/DataGroup/FileTypes/KCDataGroupStringParser.h"


KCDataGroupManager::KCDataGroupManager()
{
	
}

KCDataGroupManager::~KCDataGroupManager()
{
	_clean();
}

void KCDataGroupManager::_clean()
{

	for (std::unordered_map<KCString, KCDataGroup *>::iterator iter = m_DataGroups.begin(); iter != m_DataGroups.end(); iter++)
	{
		DELETE_SAFELY(iter->second);
	}
}

int32 KCDataGroupManager::loadLooseFiles(const WCHAR *pPath)
{
	
	_clean();	
	KCString strPathAsNarrow = KCStringUtils::toNarrowUtf8(pPath);
	KCStringUtils::replace(strPathAsNarrow, '\\', '/');
	TArray<std::wstring> mFiles;
	KCFileUtilities::getFilesInDirectory(pPath, L"*.dat", mFiles);
	mFiles.Reserve(mFiles.Num());
	for (int32 iIndex = 0; iIndex < mFiles.Num(); iIndex++)
	{
		KCDataGroup *pDataGroup = KC_NEW KCDataGroup();
		if (KCDataGroupStringParser::parseDataGroupFromFile(mFiles[iIndex].c_str(), *pDataGroup) == false)
		{
			DELETE_SAFELY(pDataGroup);
			continue;
		}

		KCString strPath = KCStringUtils::convertWideToUtf8(mFiles[iIndex].c_str());		
		KCStringUtils::replace(strPath, '\\', '/');
		size_t iIndexOf = strPath.find(strPathAsNarrow);
		if (iIndexOf >= 0)
		{
			strPath = strPath.substr(strPathAsNarrow.length() + iIndexOf, strPath.length() - (strPathAsNarrow.length() + iIndexOf));
		}
		while (strPath.c_str()[0] == '.')
		{
			strPath = strPath.substr(1, strPath.length() - 1);
		}
		while (strPath.c_str()[0] == '/')
		{
			strPath = strPath.substr(1, strPath.length() - 1);
		}
		m_DataGroups[KCStringUtils::toUpper(strPath)] = pDataGroup;
	}
	return (int32)m_DataGroups.size();
}

bool KCDataGroupManager::createMasterFile(const WCHAR *pPathToLooseFiles, const WCHAR *pPathToMasterFile)
{
	//todo
	return false;
}

int32 KCDataGroupManager::getDataGroupsInDirectory(const KCString &strPath, TArray<const KCDataGroup *> &mDataGroups) const
{
	int iCount = mDataGroups.Num();
	KCString strPathUpper = KCStringUtils::toUpperNewString(strPath);
	for (std::unordered_map<KCString, KCDataGroup *>::const_iterator iter = m_DataGroups.begin(); iter != m_DataGroups.end(); iter++)
	{
		
		if ((int32)iter->first.find(strPathUpper.c_str(), 0, strPathUpper.length()) >= 0)
		{
			mDataGroups.Add(iter->second);
		}
	}
	return mDataGroups.Num() - iCount;
}

