#include "KCUnitTypeManager.h"
#include "Interfaces/KCByteReader.h"
#include "IO/KCUnitTypeBinaryFileParser.h"


using namespace UNITTYPE;

static KCUnitTypeManager *g_pUnitTypeManager(nullptr);

KCUnitTypeManager::KCUnitTypeManager()
{
	if (g_pUnitTypeManager == nullptr)
	{
		g_pUnitTypeManager = this;
	}
}

KCUnitTypeManager::~KCUnitTypeManager()
{

}



KCUnitTypeManager * KCUnitTypeManager::getSingleton()
{
	return g_pUnitTypeManager;
}

bool KCUnitTypeManager::parseUnitTypeFile(const int8 *pArray, const int32 iCount)
{
	
	if (KCUnitTypeBinaryFileParser::parseUnitTypeForUnitTypeManager(this, pArray, iCount))
	{
		//todo
	}
	//note this 
	return true;
}

