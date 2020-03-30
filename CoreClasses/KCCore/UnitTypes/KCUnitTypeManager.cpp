#include "KCUnitTypeManager.h"

#include "IO/KCByteReader.h"





static UNITTYPE::KCUnitTypeManager *g_pUnitTypeManager(nullptr);

UNITTYPE::KCUnitTypeManager::KCUnitTypeManager()
{
	if (g_pUnitTypeManager == nullptr)
	{
		g_pUnitTypeManager = this;
	}
	m_Categories.setGrowBy(5);
}

UNITTYPE::KCUnitTypeManager::~KCUnitTypeManager()
{
	
	m_Categories.clean();
}



UNITTYPE::KCUnitTypeManager * UNITTYPE::KCUnitTypeManager::getSingleton()
{
	if (g_pUnitTypeManager == nullptr)
	{
		KC_NEW KCUnitTypeManager();
	}
	return g_pUnitTypeManager;
}

bool UNITTYPE::KCUnitTypeManager::parseUnitTypeFile(const int8 *pArray, const int32 iCount)
{

	static int8 UNITTYPE_FILE_VERSION_ONE(1);			//first Version!
	static int8 UNITTYPE_FILE_VERSION(UNITTYPE_FILE_VERSION_ONE);
	
	KCByteReader mByteReader(pArray, iCount);
	int8 iVersion = mByteReader.readInt8(UNITTYPE_FILE_VERSION);
	int32 iNumberOfCategories = (int32)mByteReader.readUInt16(0);
	m_Categories.reserve(iNumberOfCategories);
	KCTArray<int> mArrayOfBitLookUps;
	for (int32 iCategoryIndex = 0; iCategoryIndex < iNumberOfCategories; iCategoryIndex++)
	{
		m_Categories.add(KCUnitTypeCategory());
		m_Categories.last()._parse(mByteReader);
	}


	//note this 
	return true;
}

