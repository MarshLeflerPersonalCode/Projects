//copyright Marsh Lefler 2000-...
#include "KCDefinedUnitTypes.h"
#include "KCUnitTypeManager.h"


static const KCUnitTypeCategory	*g_pGameTypesCategory(nullptr);

KCUnitType						UNITTYPES::ANY = INVALID;
KCUnitType						UNITTYPES::CHARACTER = INVALID;
KCUnitType						UNITTYPES::ITEM = INVALID;
KCUnitType						UNITTYPES::WEAPON = INVALID;
KCUnitType						UNITTYPES::ARMOR = INVALID;

void _defineUnitTypes(KCUnitTypeManager *pManager)
{
	g_pGameTypesCategory = pManager->getCategoryByName("GAME_TYPES");
	KCEnsureAlwaysReturn(g_pGameTypesCategory);
	UNITTYPES::ANY = pManager->getUnitTypeID(g_pGameTypesCategory, "ANY");
	UNITTYPES::ITEM = pManager->getUnitTypeID(g_pGameTypesCategory, "ITEM");
	UNITTYPES::CHARACTER = pManager->getUnitTypeID(g_pGameTypesCategory, "CHARACTER");
	UNITTYPES::WEAPON = pManager->getUnitTypeID(g_pGameTypesCategory, "WEAPON");
	UNITTYPES::ARMOR = pManager->getUnitTypeID(g_pGameTypesCategory, "ARMOR");
}



bool UNITTYPES::IsA(KCUnitType iItemType, KCUnitType iItemIsA)
{
	KCEnsureAlwaysReturnVal(g_pGameTypesCategory, false);
	return g_pGameTypesCategory->IsA(iItemType, iItemIsA);
}


