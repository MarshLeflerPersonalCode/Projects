#include "KCUnitTypeManager.h"
#include "KCDefinedUnitTypes.h"






static UNITTYPE::KCUnitTypeManager *g_pStatManager(nullptr);

UNITTYPE::KCUnitTypeManager::KCUnitTypeManager()
{
	if (g_pStatManager == nullptr)
	{
		g_pStatManager = this;
	}
	m_Categories.setGrowBy(5);
}

UNITTYPE::KCUnitTypeManager::~KCUnitTypeManager()
{
	
	m_Categories.clean();
}



UNITTYPE::KCUnitTypeManager * UNITTYPE::KCUnitTypeManager::getSingleton()
{
	if (g_pStatManager == nullptr)
	{
		KC_NEW KCUnitTypeManager();
	}
	return g_pStatManager;
}

bool UNITTYPE::KCUnitTypeManager::parseUnitTypeFile(const TCHAR *strPath)
{
	KCTArray<uint8> mFileBytes;
	if (KCFileUtilities::loadFile(strPath, mFileBytes) == false ||
		mFileBytes.getCount() == 0 )
	{
		return false;
	}

	static int8 UNITTYPE_FILE_VERSION_ONE(1);			//first Version!
	static int8 UNITTYPE_FILE_VERSION(UNITTYPE_FILE_VERSION_ONE);

	KCByteReader mByteReader(mFileBytes);
	int8 iVersion(UNITTYPE_FILE_VERSION);
	uint16 iNumberOfCategories(0);
	mByteReader << iVersion;
	mByteReader << iNumberOfCategories;

	m_Categories.reserve(iNumberOfCategories);
	KCTArray<int> mArrayOfBitLookUps;
	for (int32 iCategoryIndex = 0; iCategoryIndex < iNumberOfCategories; iCategoryIndex++)
	{
		m_Categories.add(KCUnitTypeCategory());
		m_Categories.last()._parse(mByteReader);
	}
	//define the unit types predefined.

	UNITTYPE::defineUnitTypes(this);

	//note this 
	return true;
}


