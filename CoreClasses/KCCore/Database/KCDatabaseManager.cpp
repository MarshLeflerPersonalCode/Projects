//copyright Marsh Lefler 2000-...
#include "KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"



static KCDatabaseManager *g_pDatabaseManager = null;

KCDatabaseManager::KCDatabaseManager()
{
	for (uint32 iIndex = 0; iIndex < (uint32)DATABASE::EDATABASE_TABLES::COUNT; iIndex++)
	{
		m_Tables[iIndex] = nullptr;
	}
	g_pDatabaseManager = this;
}

KCDatabaseManager::~KCDatabaseManager()
{
	for (uint32 iIndex = 0; iIndex < (uint32)DATABASE::EDATABASE_TABLES::COUNT; iIndex++) 
	{
		DELETE_SAFELY( m_Tables[iIndex] );
	}

}

KCDatabaseManager * KCDatabaseManager::getSingleton()
{
	if (g_pDatabaseManager == null)
	{
		g_pDatabaseManager = KC_NEW KCDatabaseManager();
	}
	return g_pDatabaseManager;
}



void KCDatabaseManager::reload()
{
	_clean();
	_initialize();
}

void KCDatabaseManager::_clean()
{

}

void KCDatabaseManager::_initialize()
{
	KCEnsuceOnceMsgReturn(KCDataGroupManager::getSingleton(), "Data Group Manager must be created first");
	m_Tables[(int32)DATABASE::EDATABASE_TABLES::STATS] = (KCDBTable<FKCDBEntry>*)( KC_NEW KCDBTable<STATS::FKCStatDefinition>(DATABASE::EDATABASE_TABLES::STATS, "Content/Databases/Stats/"));
	
}

const STATS::FKCStatDefinition * KCDatabaseManager::getStatDefinitionByName(const KCName &strName)
{
	return (const STATS::FKCStatDefinition *)g_pDatabaseManager->m_Tables[(int32)DATABASE::EDATABASE_TABLES::STATS]->getEntry(strName);
}

const STATS::FKCStatDefinition * KCDatabaseManager::getStatDefinitionByGuid(KCDatabaseGuid iGuid)
{
	return (const STATS::FKCStatDefinition *)g_pDatabaseManager->m_Tables[(int32)DATABASE::EDATABASE_TABLES::STATS]->getEntry(iGuid);
}