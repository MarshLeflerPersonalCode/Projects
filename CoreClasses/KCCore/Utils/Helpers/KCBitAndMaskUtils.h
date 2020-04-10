//copyright Marsh Lefler 2000-...
#pragma once
#include "Utils/TypeDefinitions/KCPrimitiveTypesDefinition.h"


//creates a mask in 32 bits
#define MASK_CREATE_32BIT( iBit ) ( (int32)(1 << ((int32)iBit)) )
//creates a mask in 64 bits
#define MASK_CREATE_64BIT( iBit ) ( (int64)(1LL << ((int64)iBit)) )
//removes a bit from a mask
#define MASK_REMOVE_BIT_32BIT( iMaskResult, iBit )( iMaskResult = iMaskResult & ~CREATE_32BIT_MASK( iBit ) )
#define MASK_REMOVE_BIT_64BIT( iMaskResult, iBit )( iMaskResult = iMaskResult & ~CREATE_64BIT_MASK( iBit ) )
//sets a bit on a mask
#define MASK_SET_BIT_32BIT( iMaskResult, iBit )( iMaskResult = iMaskResult | CREATE_32BIT_MASK( iBit ) )
#define MASK_SET_BIT_64BIT( iMaskResult, iBit )( iMaskResult = iMaskResult | CREATE_64BIT_MASK( iBit ) )
//tests a specific bit in a 32bit mask or a 64 bit mask
#define MASK_TEST_BIT_32BIT( iMask, iBit )( ( iMask & CREATE_32BIT_MASK( iBit ) )?true:false )
#define MASK_TEST_BIT_64BIT( iMask, iBit )( ( iMask & CREATE_64BIT_MASK( iBit ) )?true:false )

//////////////////////////////////////////////////////////////////
//////////////////////MASK MANIPULATIONS//////////////////////////
//tests two masks that must be exactly the same

#define MASK_TEST_EXACT( iMaskOne, iMaskTwo )( ( iMaskOne == iMaskTwo )?true:false )
//tests a mask against any other mask returning if any bits match
#define MASK_TEST_ANY_BITS( iMaskOne, iMaskTwo )( ( ( iMaskOne & iMaskTwo ) != 0 )?true:false )
//removes a mask's bits from another
#define MASK_REMOVE_SECOND_MASK( iMaskResult, iMaskToRemove )( iMaskResult = iMaskResult & ~iMaskToRemove )
//adds a masks value onto another mask
#define MASK_ADD_SECOND_MASK( iMaskResult, iMaskToAdd )( iMaskResult = iMaskResult | iMaskToAdd )

//////////////////////MASK MANIPULATIONS//////////////////////////
//////////////////////////////////////////////////////////////////

