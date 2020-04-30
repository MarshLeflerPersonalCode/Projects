//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This is where you define stats to be used in the core. The init stats function
//will be called by the manager which will intern set these properties.
/////////////////////////////////////////////////////////





class KCCORE_API KCSTATS
{
public:
	//must be called to define the stats
	

	static KCStatID			RANK;				//holds the rank of all the characters and monsters
	
};

void					_defineStats(class KCStatManager *pManager);

