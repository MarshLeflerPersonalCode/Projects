#include "KCStats.h"
#include "Systems/Stats/KCStatManager.h"
#include "KCCoreData.h"

KCStats::KCStats(KCCoreData *pCoreData, ESTAT_HANDLER_TYPE eType) :
	KCCoreObject(pCoreData)
{
	_configure(pCoreData, eType);
}


KCStats::~KCStats()
{
	_clean();
}



void KCStats::_configure(KCCoreData *pCoreData, ESTAT_HANDLER_TYPE eType)
{
	_clean();
	m_eType = eType;	
	KCEnsureAlwaysReturn(getStatManager());
	m_pStatDefintions = getStatManager()->getStatDefinitions();
	switch (eType)
	{
	case ESTAT_HANDLER_TYPE::CHARACTER:
		m_StatsOwned = getStatManager()->getCharacterHasStatBitArray();
		break;
	case ESTAT_HANDLER_TYPE::ITEM:
		m_StatsOwned = getStatManager()->getItemHasStatBitArray();
		break;
	}
	m_Stats = getStatManager()->getDefaultValues();
}

bool KCStats::isValidStatID(const KCName &strName) const
{
	if (getStatManager() == nullptr ||
		m_pStatDefintions == nullptr)
	{
		return false;
	}
	KCStatID iStatID = getStatManager()->getStatID(strName);
	if (iStatID < m_Stats.Num() &&
		m_StatsOwned[iStatID])
	{
		return true;
	}
	return false;
}

KCStatID KCStats::getStatIDByName(const KCName &strStatName) const
{

	return getStatManager()->getStatID(strStatName);
}

coreUnionData32Bit KCStats::getDefaultValue(KCStatID iStat)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStat), coreUnionData32Bit());
	return getStatManager()->getDefaultValues()[iStat];
}


float KCStats::getValue(KCStatID iStat, float fDefaultValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStat), fDefaultValue);
	const FKCStatDefinition *pStatDefinition = (*m_pStatDefintions)[iStat];
	if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
	{
		return (float)getValue(iStat, (int32)fDefaultValue);
	}
	float fCurrentValue = m_Stats[iStat].m_fValue;	
	for (int32 iIndexOfMathFunc = 0; iIndexOfMathFunc < pStatDefinition->m_MathFunctions.Num(); iIndexOfMathFunc++)
	{
		fCurrentValue = pStatDefinition->m_MathFunctions[iIndexOfMathFunc].calculateStat(this, pStatDefinition, fCurrentValue);
	}
	return fCurrentValue;
}

int32 KCStats::getValue(KCStatID iStat, int32 iDefaultValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStat), iDefaultValue);
	const FKCStatDefinition *pStatDefinition = (*m_pStatDefintions)[iStat];
	if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::FLOAT)
	{
		return (int32)getValue(iStat, (float)iDefaultValue);
	}
	int32 iCurrentValue = m_Stats[iStat].m_iValue32;
	
	for (int32 iIndexOfMathFunc = 0; iIndexOfMathFunc < pStatDefinition->m_MathFunctions.Num(); iIndexOfMathFunc++)
	{
		iCurrentValue = pStatDefinition->m_MathFunctions[iIndexOfMathFunc].calculateStat(this, pStatDefinition, iCurrentValue);
	}
	return iCurrentValue;
}


void KCStats::_clean()
{
	m_Stats.Empty();	
	m_StatsOwned.Empty();
	m_pStatDefintions = nullptr;
}



