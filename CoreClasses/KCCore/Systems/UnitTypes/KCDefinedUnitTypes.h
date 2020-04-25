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



namespace UNITTYPE_ITEMS
{
	static KCUnitType ANY;
};


namespace UNITTYPE
{
	////////////////////////////////////////////////
	//gets called from unit type manager
	////////////////////////////////////////////////
	void					defineUnitTypes(class KCUnitTypeManager *pManager);
	////////////////////////////////////////////////
	//gets called from unit type manager
	////////////////////////////////////////////////


}; //end namespace UNITTYPE

