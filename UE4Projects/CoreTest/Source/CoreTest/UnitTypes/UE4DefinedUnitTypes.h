//copyright Marsh Lefler 2000-...
#pragma once
#include "CoreMinimal.h"
#include "CoreClasses/UnitTypes/KCUnitTypeManager.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This makes a bunch of external defined unit types to help speed things up(so we don't have to use strings)
//
//Because statics can't be shared between dll's these can't be UE4. UE4 needs to copy this file
//
/////////////////////////////////////////////////////////


namespace UNITTYPE_ITEMS
{
	static uint32 ANY;
};


namespace UNITTYPE
{
	////////////////////////////////////////////////
	//gets called from unit type manager
	////////////////////////////////////////////////
	void					defineUnitTypes(KCUnitTypeManager *pManager);
	////////////////////////////////////////////////
	//gets called from unit type manager
	////////////////////////////////////////////////


}; //end namespace UNITTYPE

