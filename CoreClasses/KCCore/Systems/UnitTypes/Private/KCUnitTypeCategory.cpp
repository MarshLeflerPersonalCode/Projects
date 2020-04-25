#include "KCUnitTypeCategory.h"




UNITTYPE::KCUnitTypeCategory::KCUnitTypeCategory()
{
	m_strName = "UNIDENTIFIED";	
}



UNITTYPE::KCUnitTypeCategory::~KCUnitTypeCategory()
{
	for(uint32 iUnitTypeIndex = 0; iUnitTypeIndex < m_iNumberOfUnitTypes; iUnitTypeIndex++)
	{ 
		DELETE_ARRAY_SAFELY(m_UnitTypes[iUnitTypeIndex].m_BitLookUpArray);
	}
	DELETE_ARRAY_SAFELY(m_UnitTypes);
}

bool UNITTYPE::KCUnitTypeCategory::_parse(KCByteReader &mByteReader)
{

	
	uint16 iNumberOfUnitTypes(0);
	uint8 iNumberOfBitLookIndexes(0);
	mByteReader << m_strName;
	mByteReader << iNumberOfUnitTypes;
	mByteReader << iNumberOfBitLookIndexes;
	m_iNumberOfUnitTypes = (uint32)iNumberOfUnitTypes;
	m_iNumberOfBitLookIndexs = (uint32)iNumberOfBitLookIndexes;
	m_UnitTypes = new KCUnitTypeDef[m_iNumberOfUnitTypes];

	for (uint32 iUnitTypeIndex = 0; iUnitTypeIndex < m_iNumberOfUnitTypes; iUnitTypeIndex++)
	{
		mByteReader << m_UnitTypes[iUnitTypeIndex].m_strString;
		m_UnitTypes[iUnitTypeIndex].m_BitLookUpArray = new int32[m_iNumberOfBitLookIndexs];
		for (uint32 iNumberOfBitsIndex = 0; iNumberOfBitsIndex < m_iNumberOfBitLookIndexs; iNumberOfBitsIndex++)
		{
			mByteReader << m_UnitTypes[iUnitTypeIndex].m_BitLookUpArray[iNumberOfBitsIndex];			
		}
		m_UnitTypes[iUnitTypeIndex].m_iIndex = (uint16)iUnitTypeIndex;
		m_MapLookUp[m_UnitTypes[iUnitTypeIndex].m_strString] = iUnitTypeIndex;
	}
	return true;
}