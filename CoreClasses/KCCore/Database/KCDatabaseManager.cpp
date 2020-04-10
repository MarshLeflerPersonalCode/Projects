//copyright Marsh Lefler 2000-...
#include "KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"

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

void KCDatabaseManager::_initialize()
{

}
