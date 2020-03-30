#pragma once
#include "KCDefines.h"
#include "UnitTypes/UnitTypes/KCUnitTypesItems.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This is the matching c++ part of the unittypes from the c# editor.
//
//The c# editor can save unit types into two different types of formats. 
//
//Format 1: header files. Selecting the category in the editor will allow you to specify where to save the header.
//
//Format 2: Binary Lookup File. The tool also exports a unittype.bin file that can be loaded by the UnitTypeManager. The
//			advantages of this format is that it doesn't require a new build for adding unit types or inheritances.
//
/////////////////////////////////////////////////////////
namespace UNITTYPE
{

	class KCUnitTypeManager
	{
	public:
		KCUnitTypeManager();
		~KCUnitTypeManager();

		//returns the singleton of this class
		static KCUnitTypeManager *			getSingleton();

		
		//parses the bytes of the unit type file
		bool								parseUnitTypeFile(const int8 *pArray, const int32 iCount);


	};


}; //end namespace UNITTYPE

