#include "KCStats.h"
#include "KCStatManager.h"

STATS::KCStats::KCStats(STATS::KCStatManager *pStatManager, ESTAT_HANDLER_TYPE eType)
{
	m_eType = eType;
	m_pStatManager = pStatManager;	
	KCEnsureAlwaysReturn(m_pStatManager);
	m_pStatDefintions = m_pStatManager->getStatDefinitions();
	switch (eType)
	{
	case ESTAT_HANDLER_TYPE::CHARACTER:
		m_StatsOwned = pStatManager->getCharacterHasStatBitArray();
		break;
	case ESTAT_HANDLER_TYPE::ITEM:
		m_StatsOwned = pStatManager->getItemHasStatBitArray();
		break;
	}
	m_Stats = m_pStatManager->getDefaultValues();
}

STATS::KCStats::~KCStats()
{
	_clean();
}

KCStatID STATS::KCStats::getStatIDByName(const KCName &strStatName) const
{

	return m_pStatManager->getStatID(strStatName);
}

coreUnionData32Bit STATS::KCStats::getDefaultValue(KCStatID iStat)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStat), coreUnionData32Bit());
	return m_pStatManager->getDefaultValues()[iStat];
}


float STATS::KCStats::getCalculatedValueAsFloat(KCStatID iStat)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStat), 0);
	const FKCStatDefinition *pStatDefinition = (*m_pStatDefintions)[iStat];
	if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
	{
		return (float)getCalculatedValueAsInt32(iStat);
	}
	if (m_StatsCalculated[iStat])
	{
		return m_StatsCalculatedValues[iStat].m_fValue;
	}
	float fCurrentValue = m_Stats[iStat].m_fValue;	
	for (uint32 iIndexOfMathFunc = 0; iIndexOfMathFunc < pStatDefinition->m_MathFunctions.Num(); iIndexOfMathFunc++)
	{
		fCurrentValue = pStatDefinition->m_MathFunctions[iIndexOfMathFunc].calculateStat(this, pStatDefinition, fCurrentValue);
	}
	m_StatsCalculatedValues[iStat].m_fValue = fCurrentValue;
	m_StatsCalculated[iStat] = true;
	return fCurrentValue;
}

int32 STATS::KCStats::getCalculatedValueAsInt32(KCStatID iStat)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStat), 0);
	const FKCStatDefinition *pStatDefinition = (*m_pStatDefintions)[iStat];
	if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::FLOAT)
	{
		return (int32)getCalculatedValueAsFloat(iStat);
	}
	if (m_StatsCalculated[iStat])
	{
		return m_StatsCalculatedValues[iStat].m_iValue32;
	}
	int32 iCurrentValue = m_Stats[iStat].m_iValue32;
	
	for (uint32 iIndexOfMathFunc = 0; iIndexOfMathFunc < pStatDefinition->m_MathFunctions.Num(); iIndexOfMathFunc++)
	{
		iCurrentValue = pStatDefinition->m_MathFunctions[iIndexOfMathFunc].calculateStat(this, pStatDefinition, iCurrentValue);
	}
	m_StatsCalculatedValues[iStat].m_iValue32 = iCurrentValue;
	m_StatsCalculated[iStat] = true;
	return iCurrentValue;
}

void STATS::KCStats::_clean()
{
	m_Stats.Empty();
	m_pStatManager = 0;
	m_iFlags = 0;
	m_StatsOwned.Empty();
}



