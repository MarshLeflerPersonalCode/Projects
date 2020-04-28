#include "KCUnitTypeManager.h"
#include "KCDefinedUnitTypes.h"


KCUnitTypeManager::KCUnitTypeManager()
{
	m_Categories.setGrowBy(5);
}

KCUnitTypeManager::~KCUnitTypeManager()
{
	_clean();
	
}


void KCUnitTypeManager::_clean()
{
	m_Categories.clean();
}

bool KCUnitTypeManager::configureUnitTypeByConfigFile(const TCHAR *strPath)
{
	_clean();
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
		m_Categories.add(KC_NEW KCUnitTypeCategory(getCoreData()));
		m_Categories.last()->_parse(mByteReader);
	}
	//define the unit types predefined.

	_defineUnitTypes(this);

	//note this 
	return true;
}


