//copyright Marsh Lefler 2000-...
#include "KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"
#include "Systems/Stats/Private/KCStatDefinition.h"


KCDatabaseManager::KCDatabaseManager()
{
	for (uint32 iIndex = 0; iIndex < (uint32)DATABASE::EDATABASE_TABLES::COUNT; iIndex++)
	{
		m_Tables[iIndex] = nullptr;
	}
}

KCDatabaseManager::~KCDatabaseManager()
{
	for (uint32 iIndex = 0; iIndex < (uint32)DATABASE::EDATABASE_TABLES::COUNT; iIndex++) 
	{
		DELETE_SAFELY( m_Tables[iIndex] );
	}

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
