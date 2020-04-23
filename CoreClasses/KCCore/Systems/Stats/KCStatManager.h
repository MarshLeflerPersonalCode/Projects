//copyright Marsh Lefler 2000-...
#pragma once
#include "Private/KCStatInclude.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This loads/creates/ manages the global stat definitions.
//
//There is a stat editor that saves out the stats for this editor to load and manage.
//
//
/////////////////////////////////////////////////////////

class KCDataGroup;

namespace STATS
{

	class KCCORE_API KCStatManager
	{
	public:
		KCStatManager();
		~KCStatManager();

		//returns the singleton of this class
		static KCStatManager *		getSingleton();

		//loads individual stats from a directory. Returns the count reloaded
		int32						reloadStatsFromDirectory(KCString strLooseFilesFolder);

		//stat files can hold multiple stats
		bool						reloadStatFile(KCString strStatFile);


	private:
		bool						_createStat(KCDataGroup *pDataGroup);
		
		
		//NOTE - maybe in the future better to make this a map
		
	};


}; //end namespace UNITTYPE

