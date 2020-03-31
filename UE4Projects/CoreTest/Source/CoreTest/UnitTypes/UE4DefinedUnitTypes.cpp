//copyright Marsh Lefler 2000-...
#include "UE4DefinedUnitTypes.h"






void UNITTYPE::defineUnitTypes(class KCUnitTypeManager *pManager)
{
	UNITTYPE_ITEMS::ANY = pManager->getUnitTypeID("ITEMS", "ANY");	
}

