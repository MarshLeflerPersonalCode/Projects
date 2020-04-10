//copyright Marsh Lefler 2000-...
#include "KCDefines.h"



enum class EUNITTYPES_ITEMS
{
          ANY      = 0,
          NEW      = 1,
          NEW1     = 2,
          NEW2     = 3,
          NEW3     = 4,
          NEW4     = 5,
          NEW5     = 6,
          NEW6     = 7,
          NEW7     = 8,
          NEW8     = 9,
};

#define EUNITTYPES_ITEMS_COUNT 10
#define EUNITTYPES_ITEMS_BYTES_NEEDED 1



namespace UNITTYPE
{
struct FUnitTypeData_ITEMS
{
     KCString             m_strName;
     EUNITTYPES_ITEMS     m_eType;
     int32                m_Data[EUNITTYPES_ITEMS_BYTES_NEEDED]; 
};

static FUnitTypeData_ITEMS g_UnitTypeDataArray_ITEMS[EUNITTYPES_ITEMS_COUNT] = {
     //Name of Unit Type(m_strName)     Unit Type Enum(m_eType)                                 Data(m_Data)
     "ANY",                             EUNITTYPES_ITEMS::ANY,                                  {1},
     "NEW",                             EUNITTYPES_ITEMS::NEW,                                  {3},
     "NEW1",                            EUNITTYPES_ITEMS::NEW1,                                 {7},
     "NEW2",                            EUNITTYPES_ITEMS::NEW2,                                 {15},
     "NEW3",                            EUNITTYPES_ITEMS::NEW3,                                 {31},
     "NEW4",                            EUNITTYPES_ITEMS::NEW4,                                 {63},
     "NEW5",                            EUNITTYPES_ITEMS::NEW5,                                 {127},
     "NEW6",                            EUNITTYPES_ITEMS::NEW6,                                 {255},
     "NEW7",                            EUNITTYPES_ITEMS::NEW7,                                 {511},
     "NEW8",                            EUNITTYPES_ITEMS::NEW8,                                 {1023},
};

//ISA checks to see if A is a B. So if A was a sword, and sword was a weapon and an item, and B could be sword, weapon or item and this would return true. Else it would return false
static bool ISA(EUNITTYPES_ITEMS A_is_a, EUNITTYPES_ITEMS B )
{	
	int32 iBitOffsetIntoInt = (int32)B % 32;
	int32 iIndexIntoArray= (int32)B / 32;
	return (g_UnitTypeDataArray_ITEMS[(int32)A_is_a].m_Data[iIndexIntoArray] & ( 1 << iBitOffsetIntoInt))?true:false;
}

//Returns the unit type name as a string
static KCString getUnitTypeName(EUNITTYPES_ITEMS A )
{
	return g_UnitTypeDataArray_ITEMS[(int32)A].m_strName;
}

//returns the unit type by a string
static EUNITTYPES_ITEMS getUnitTypeByNameFor_ITEMS(KCString strName )
{
	for(int32 iIndex = 0; iIndex < EUNITTYPES_ITEMS_COUNT; iIndex++)	
	{
		if( g_UnitTypeDataArray_ITEMS[iIndex].m_strName == strName )
		{
			return (EUNITTYPES_ITEMS)iIndex;
		}
	}
	return (EUNITTYPES_ITEMS)0;
}


}; //end namespace UNITTYPE
