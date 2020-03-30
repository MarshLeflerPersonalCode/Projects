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
	m_strName = mByteReader.readString();
	m_iNumberOfUnitTypes = (uint32)mByteReader.readUInt16(0);
	m_iNumberOfBitLookIndexs = (uint32)mByteReader.readInt8(0);
	m_UnitTypes = new KCUnitType[m_iNumberOfUnitTypes];
	for (uint32 iUnitTypeIndex = 0; iUnitTypeIndex < m_iNumberOfUnitTypes; iUnitTypeIndex++)
	{
		m_UnitTypes[iUnitTypeIndex].m_strString = mByteReader.readString("");
		m_UnitTypes[iUnitTypeIndex].m_BitLookUpArray = new int32[m_iNumberOfBitLookIndexs];
		mByteReader.readInt32Array(m_UnitTypes[iUnitTypeIndex].m_BitLookUpArray, m_iNumberOfBitLookIndexs);
		m_UnitTypes[iUnitTypeIndex].m_iIndex = (uint16)iUnitTypeIndex;
		m_MapLookUp[m_UnitTypes[iUnitTypeIndex].m_strString] = iUnitTypeIndex;
	}
	return true;
}