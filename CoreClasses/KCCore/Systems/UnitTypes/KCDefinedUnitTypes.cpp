//copyright Marsh Lefler 2000-...
#include "KCDefinedUnitTypes.h"






void CORE_UNITTYPE::defineUnitTypes(class UNITTYPE::KCUnitTypeManager *pManager)
{
	UNITTYPE_ITEMS::ANY = pManager->getUnitTypeID("ITEMS", "ANY");
}

