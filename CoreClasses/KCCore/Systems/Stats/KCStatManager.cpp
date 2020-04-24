#include "KCStatManager.h"
#include "Database/KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"
#include "KCStats.h"

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


void STATS::KCStatManager::_clean()
{
	m_StatsByName.clear();
	m_StatsByID.Empty();
}


STATS::KCStatManager * STATS::KCStatManager::getSingleton()
{
	if (g_pStatManager == nullptr)
	{
		KC_NEW KCStatManager();
	}
	return g_pStatManager;
}

bool STATS::KCStatManager::initialize(KCDatabaseManager *pDatabaseManager)
{
	_clean();
	KCEnsureAlwaysReturnVal(pDatabaseManager, false);
	const KCDBTable<STATS::FKCStatDefinition> *pStatTable = (const KCDBTable<STATS::FKCStatDefinition> *)pDatabaseManager->getTable(DATABASE::EDATABASE_TABLES::STATS);
	KCEnsureAlwaysReturnVal(pStatTable, false);
	m_StatsByID.Reserve(pStatTable->getCountOfEntries());
	for (uint32 iStatIndex = 0; iStatIndex < pStatTable->getCountOfEntries(); iStatIndex++)
	{
		const FKCStatDefinition *pStat = pStatTable->getEntryByIndex(iStatIndex);
		KCEnsureAlwaysContinue(pStat);
		m_StatsByName[pStat->m_strName] = (KCStatID)iStatIndex;
		m_StatsByID.Add(pStat->m_strName);
	}
	STATS::defineStats(this);
	return (m_StatsByID.Num() > 0)?true:false;
}

const STATS::FKCStatDefinition * STATS::KCStatManager::getStatDefinitionByID(KCStatID iID) const
{
	
	KCEnsureAlwaysReturnVal((uint32)iID < m_StatsByID.Num(), nullptr);
	return KCDatabaseManager::getStatDefinitionByName(m_StatsByID[iID]);
}

const STATS::FKCStatDefinition * STATS::KCStatManager::getStatDefinitionByName(KCName strName) const
{
	return KCDatabaseManager::getStatDefinitionByName(strName);
}
