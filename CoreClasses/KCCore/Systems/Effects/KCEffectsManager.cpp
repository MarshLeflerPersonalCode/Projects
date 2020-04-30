#include "KCStatManager.h"
#include "Database/KCDatabaseManager.h"
#include "Database/Private/KCDBTable.h"
#include "Systems/Stats/KCStatsDefined.h"
#include "KCCoreData.h"

KCStatManager::KCStatManager()
{

	
}

KCStatManager::~KCStatManager()
{
	_clean();
}


void KCStatManager::_clean()
{
	m_StatsByName.clear();
	m_StatsByID.Empty();
	m_CharacterStatsByID.Empty();
	m_ItemsStatsByID.Empty();
	m_DefaultValues.Empty();
	m_StatDefinitions.Empty();
}


bool KCStatManager::initialize()
{
	_clean();
	KCEnsureAlwaysReturnVal(getCoreData(), false);
	KCEnsureAlwaysReturnVal(getDatabaseManager(), false);
	const KCDBTable<FKCStatDefinition> *pStatTable = (const KCDBTable<FKCStatDefinition> *)getDatabaseManager()->getTable(DATABASE::EDATABASE_TABLES::STATS_DATABASE);
	KCEnsureAlwaysReturnVal(pStatTable, false);
	m_StatsByID.Reserve(pStatTable->getCountOfEntries());
	m_StatDefinitions.Reserve(pStatTable->getCountOfEntries());
	for (uint32 iStatIndex = 0; iStatIndex < pStatTable->getCountOfEntries(); iStatIndex++)
	{
		const FKCStatDefinition *pStat = pStatTable->getEntryByIndex(iStatIndex);
		KCEnsureAlwaysContinue(pStat);
		m_StatDefinitions.Add(pStat);
		m_StatsByName[pStat->m_strName] = (KCStatID)iStatIndex;
		m_StatsByID.Add(pStat->m_strName);		
		if (pStat->m_eStatType == ESTAT_PRIMITIVE_TYPES::FLOAT)
		{
			m_DefaultValues.Add(DATATYPES_UTILS::getAsFloat(pStat->m_strDefaultValue));
			if (m_DefaultValues.Last().m_fValue == INVALID)
			{
				m_DefaultValues.Last().m_fValue = 0;
			}
		}
		else
		{
			m_DefaultValues.Add(DATATYPES_UTILS::getAsInt32(pStat->m_strDefaultValue));
			if (m_DefaultValues.Last().m_iValue32 == INVALID)
			{
				m_DefaultValues.Last().m_iValue32 = 0;
			}
		}
		
		if (pStat->m_bApplicableToCharacters)
		{
			m_CharacterStatsByID.setBit(iStatIndex, true);
		}
		if (pStat->m_bApplicableToItems)
		{
			m_ItemsStatsByID.setBit(iStatIndex, true);
		}
	}
	_defineStats(this);
	return (m_StatsByID.Num() > 0)?true:false;
}

const FKCStatDefinition * KCStatManager::getStatDefinitionByID(KCStatID iID) const
{
	
	KCEnsureAlwaysReturnVal((uint32)iID < (uint32)m_StatDefinitions.Num(), nullptr);
	return m_StatDefinitions[iID];
}

const FKCStatDefinition * KCStatManager::getStatDefinitionByName(KCName strName) const
{
	KCEnsureAlwaysReturnVal(getDatabaseManager(), nullptr);
	return getDatabaseManager()->getStatDefinitionByName(strName);
}

const TArray<FKCStatDefinition * > * KCStatManager::getStatDefinitions() const
{
	KCEnsureAlwaysReturnVal(getDatabaseManager(), nullptr);
	const KCDBTable<FKCStatDefinition> *pStatTable = (const KCDBTable<FKCStatDefinition> *)getDatabaseManager()->getTable(DATABASE::EDATABASE_TABLES::STATS_DATABASE);
	KCEnsureAlwaysReturnVal(pStatTable, nullptr);
	return &pStatTable->getEntries();

}
