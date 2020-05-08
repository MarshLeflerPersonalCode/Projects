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
void KCDataGroupManager::_configureFileInheritance()
{
	KCName strParentFile = "_INHERIT_FILE_";
	for (std::unordered_map<KCString, KCDataGroup *>::iterator iter = m_DataGroups.begin(); iter != m_DataGroups.end(); iter++)
	{
		KCDataGroup *pDataGroup = iter->second;
		if (pDataGroup->hasPropertyNotInherited(strParentFile))
		{
			const KCDataGroup *pParentDataGroup = getDataGroupByFileName(pDataGroup->getProperty(strParentFile, EMPTY_KCSTRING));
			if (pParentDataGroup)
			{
				pDataGroup->setParentDataGroup(pParentDataGroup);
			}
		}
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
	_configureFileInheritance();
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

const KCDataGroup * KCDataGroupManager::getDataGroupByFileName(const KCString &strPath) const
{
	KCString strPathUpper = KCStringUtils::toUpperNewString(strPath);
	std::unordered_map<KCString, KCDataGroup *>::const_iterator mIter = m_DataGroups.find(strPathUpper);
	if (mIter != m_DataGroups.end())
	{
		return mIter->second;
	}
	KCEnsureAlwaysMsg(false, "Unable to find parent data group with file name: " + strPath + ". The path should be relative and not include the content folder. For example if the file was on c:\\testing\\content\\data\\file.dat - the path should look like: data/file.dat");
	return nullptr;
}

void KCDataGroupManager::addInhertianceByPropertyName(const TArray<const KCDataGroup *> &mDataGroups, const KCName &strPropertyName, const KCName &strInheritanceProperty) const
{
	if (mDataGroups.Num() == null)
	{
		return;
	}
	std::unordered_map<KCString, const KCDataGroup *> mDataGroupPropertyValues;
	TArray<KCString> mDataGroupInheritanceValues;	
	mDataGroupInheritanceValues.Reserve(mDataGroups.Num());
	for (int32 iIndex = 0; iIndex < mDataGroups.Num(); iIndex++)
	{
		const KCDataGroup *pDataGroupConst = mDataGroups[iIndex];		
		KCString strPropertyNameValue = pDataGroupConst->getProperty(strPropertyName, EMPTY_KCSTRING);
		if (strPropertyNameValue == EMPTY_KCSTRING)
		{
			continue;
		}
		mDataGroupPropertyValues[strPropertyNameValue] = pDataGroupConst;
		mDataGroupInheritanceValues.Add(pDataGroupConst->getProperty(strInheritanceProperty, EMPTY_KCSTRING));
		
	}
	for (int32 iIndex = 0; iIndex < mDataGroups.Num(); iIndex++)
	{
		if (mDataGroupInheritanceValues[iIndex] == EMPTY_KCSTRING)
		{
			continue;
		}
		auto mIter = mDataGroupPropertyValues.find(mDataGroupInheritanceValues[iIndex]);
		KCEnsureAlwaysMsgContinue(mIter != mDataGroupPropertyValues.end(), "Unable to find parent with name(it is case sensitive): " + mDataGroupInheritanceValues[iIndex]);
		KCDataGroup *pDataGroup = (KCDataGroup *)(void *)mDataGroups[iIndex];	//remove the const - yeah this is bad( but it's SUPER FAST!)
		KCEnsureAlwaysMsgContinue(pDataGroup->getParentDataGroup() == null, "Datagroup already has a PARENT!");
		pDataGroup->setParentDataGroup(mIter->second);
	}
	
}

