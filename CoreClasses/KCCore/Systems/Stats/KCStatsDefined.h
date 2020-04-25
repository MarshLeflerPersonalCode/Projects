//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This is where you define stats to be used in the core. The init stats function
//will be called by the manager which will intern set these properties.
/////////////////////////////////////////////////////////





namespace STATS
{
	//must be called to define the stats
	void					defineStats(class KCStatManager *pManager);

	static KCStatID			RANK;				//holds the rank of all the characters and monsters
	
};

