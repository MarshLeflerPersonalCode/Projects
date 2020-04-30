//copyright Marsh Lefler 2000-...
#include "KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"
#include "KCCoreData.h"



KCDatabaseManager::KCDatabaseManager()
{
	for (uint32 iIndex = 0; iIndex < (uint32)DATABASE::EDATABASE_TABLES::COUNT; iIndex++)
	{
		m_Tables[iIndex] = nullptr;
	}
	
}

KCDatabaseManager::~KCDatabaseManager()
{
	_clean();
}




void KCDatabaseManager::reload()
{
	_clean();
	_initialize();
}

void KCDatabaseManager::_clean()
{
	for (uint32 iIndex = 0; iIndex < (uint32)DATABASE::EDATABASE_TABLES::COUNT; iIndex++)
	{
		DELETE_SAFELY(m_Tables[iIndex]);
	}
}

void KCDatabaseManager::_initialize()
{
	KCEnsuceOnceMsgReturn(getCoreData(), "Data Group Manager must be created first");
	m_Tables[(int32)DATABASE::EDATABASE_TABLES::STATS_DATABASE] = (KCDBTable<FKCDBEntry>*)( KC_NEW KCDBTable<FKCStatDefinition>(getCoreData()->getDataGroupManager(), DATABASE::EDATABASE_TABLES::STATS_DATABASE, "Databases/Stats/"));
	
}

const FKCStatDefinition * KCDatabaseManager::getStatDefinitionByName(const KCName &strName) const
{
	return (const FKCStatDefinition *)m_Tables[(int32)DATABASE::EDATABASE_TABLES::STATS_DATABASE]->getEntry(strName);
}

const FKCStatDefinition * KCDatabaseManager::getStatDefinitionByGuid(KCDatabaseGuid iGuid) const
{
	return (const FKCStatDefinition *)m_Tables[(int32)DATABASE::EDATABASE_TABLES::STATS_DATABASE]->getEntry(iGuid);
}