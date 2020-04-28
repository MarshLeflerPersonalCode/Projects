//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"

///////////////////////////////////////////////////////////
//HOW TO USE
//
//This makes a bunch of external defined unit types to help speed things up(so we don't have to use strings)
//
//Because statics can't be shared between dll's these can't be used UE4. 
//For this reason unittypes for core functionality are named CORE_UNITTYPE. Be great to find a way 
//around this.
/////////////////////////////////////////////////////////

class KCUnitTypeManager;
class KCUnitTypeCategory;


class UNITTYPES
{
public:

	static KCUnitType	ANY;
	static KCUnitType	CHARACTER;
	static KCUnitType	ITEM;
	static KCUnitType	WEAPON;
	static KCUnitType	ARMOR;
	static bool			IsA(KCUnitType iItemType, KCUnitType iItemIsA);
};


////////////////////////////////////////////////
//gets called from unit type manager
////////////////////////////////////////////////
void					_defineUnitTypes(KCUnitTypeManager *pManager);
////////////////////////////////////////////////
//gets called from unit type manager
////////////////////////////////////////////////


