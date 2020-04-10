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
namespace STATS
{

	class KCCORE_API KCStatManager
	{
	public:
		KCStatManager();
		~KCStatManager();

		//returns the singleton of this class
		static KCStatManager *		getSingleton();



	private:
		//NOTE - maybe in the future better to make this a map
		
	};


}; //end namespace UNITTYPE

