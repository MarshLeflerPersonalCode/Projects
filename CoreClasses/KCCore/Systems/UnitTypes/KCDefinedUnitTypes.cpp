//copyright Marsh Lefler 2000-...
#include "KCDefinedUnitTypes.h"
#include "KCUnitTypeManager.h"





void UNITTYPE::defineUnitTypes(UNITTYPE::KCUnitTypeManager *pManager)
{
	UNITTYPE_ITEMS::ANY = pManager->getUnitTypeID("ITEMS", "ANY");
}

