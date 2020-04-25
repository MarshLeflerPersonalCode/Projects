#include "KCStatManager.h"
#include "Database/KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"
#include "Systems/Stats/KCStatsDefined.h"

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
	_clean();
}


void STATS::KCStatManager::_clean()
{
	m_StatsByName.clear();
	m_StatsByID.Empty();
	m_CharacterStatsByID.Empty();
	m_ItemsStatsByID.Empty();
	m_DefaultValues.Empty();
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
		const STATS::FKCStatDefinition *pStat = pStatTable->getEntryByIndex(iStatIndex);
		KCEnsureAlwaysContinue(pStat);
		m_StatsByName[pStat->m_strName] = (KCStatID)iStatIndex;
		m_StatsByID.Add(pStat->m_strName);		
		if (pStat->m_bApplicableToCharacters)
		{
			m_CharacterStatsByID.setBit(iStatIndex, true);
		}
		if (pStat->m_bApplicableToItems)
		{
			m_ItemsStatsByID.setBit(iStatIndex, true);
		}
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

const KCTArray<STATS::FKCStatDefinition * > * STATS::KCStatManager::getStatDefinitions() const
{
	KCEnsureAlwaysReturnVal(KCDatabaseManager::getSingleton(), nullptr);
	const KCDBTable<STATS::FKCStatDefinition> *pStatTable = (const KCDBTable<STATS::FKCStatDefinition> *)KCDatabaseManager::getSingleton()->getTable(DATABASE::EDATABASE_TABLES::STATS);
	KCEnsureAlwaysReturnVal(pStatTable, nullptr);
	return &pStatTable->getEntries();

}
