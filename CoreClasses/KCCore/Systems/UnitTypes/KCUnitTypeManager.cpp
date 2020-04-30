#include "KCUnitTypeManager.h"
#include "KCDefinedUnitTypes.h"


KCUnitTypeManager::KCUnitTypeManager()
{
	
}

KCUnitTypeManager::~KCUnitTypeManager()
{
	_clean();
	
}


void KCUnitTypeManager::_clean()
{
	m_Categories.Empty();
}

bool KCUnitTypeManager::configureUnitTypeByConfigFile(const TCHAR *strPath)
{
	_clean();
	TArray<uint8> mFileBytes;
	if (KCFileUtilities::loadFile(strPath, mFileBytes) == false ||
		mFileBytes.Num() == 0 )
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

	m_Categories.Reserve(iNumberOfCategories);
	TArray<int> mArrayOfBitLookUps;
	for (int32 iCategoryIndex = 0; iCategoryIndex < iNumberOfCategories; iCategoryIndex++)
	{
		m_Categories.Add(KC_NEW KCUnitTypeCategory(getCoreData()));
		m_Categories.Last()->_parse(mByteReader);
	}
	//define the unit types predefined.

	_defineUnitTypes(this);

	//note this 
	return true;
}


