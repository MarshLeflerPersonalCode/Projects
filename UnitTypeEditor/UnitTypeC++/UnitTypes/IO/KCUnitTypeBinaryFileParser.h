#pragma once
#include "KCDefines.h"

///////////////////////////////////////////////////////////
//HOW TO USE
//
//Parses the file exported from the C# tool
//
/////////////////////////////////////////////////////////

namespace UNITTYPE
{
	class KCUnitTypeBinaryFileParser
	{
	public:
		//parses the 
		static bool parseUnitTypeForUnitTypeManager(class KCUnitTypeManager *pManager, const int8 *pArray, const int32 iCount);
	};
}; //end namespace UNITTYPE

