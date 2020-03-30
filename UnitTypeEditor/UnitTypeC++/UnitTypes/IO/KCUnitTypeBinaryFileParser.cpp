#include "KCUnitTypeBinaryFileParser.h"
#include "./Interfaces/KCByteReader.h"
#include "./UnitTypes/KCUnitTypeManager.h"


#define UNITTYPE_FILE_VERSION_ONE 1
#define UNITTYPE_FILE_VERSION UNITTYPE_FILE_VERSION_ONE

using namespace UNITTYPE;


bool KCUnitTypeBinaryFileParser::parseUnitTypeForUnitTypeManager(KCUnitTypeManager *pManager, const int8 *pArray, const int32 iCount)
{
	KCByteReader mByteReader(pArray, iCount);
	int8 iVersion = mByteReader.readInt8(UNITTYPE_FILE_VERSION);
	int32 iNumberOfCategories = (int32)mByteReader.readInt16(0);
	for (int32 iCategoryIndex = 0; iCategoryIndex < iNumberOfCategories; iCategoryIndex++)
	{

	}
	return true;
}
