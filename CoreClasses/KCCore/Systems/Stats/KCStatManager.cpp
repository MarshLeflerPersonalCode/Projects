#include "KCStatManager.h"

static STATS::KCStatManager *g_pStatManager(nullptr);

STATS::KCStatManager::KCStatManager()
{
	if (g_pStatManager == nullptr)
	{
		g_pStatManager = this;
	}
	
}

STATS::KCStatManager::~KCStatManager()
{
	
	
}



STATS::KCStatManager * STATS::KCStatManager::getSingleton()
{
	if (g_pStatManager == nullptr)
	{
		KC_NEW KCStatManager();
	}
	return g_pStatManager;
}

int32 STATS::KCStatManager::reloadStatsFromDirectory(KCString strLooseFilesFolder)
{
	int32 iCountCreated(0);
	/*
	KCTArray<KCString> mFiles(100);
	KCFileUtilities::getFilesInDirectory(KCStringUtils::toWide( strLooseFilesFolder ).c_str(), L"*.dat", mFiles, false);
	for (uint32 iIndex = 0; iIndex < mFiles.Num(); iIndex++)
	{
		if (reloadStatFile(mFiles[iIndex]))
		{
			iCountCreated++;
		}
	}*/
	return iCountCreated;
	
}


bool STATS::KCStatManager::reloadStatFile(KCString strStatFile)
{

	return true;
}

