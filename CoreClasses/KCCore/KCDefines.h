//copyright Marsh Lefler 2000-...
#pragma once


#include "Utils/TypeDefinitions/KCPrimitiveTypesDefinition.h"

#ifdef USING_UE4


#else
///////////////////////////////////////////////////////////////////////////
/////////////////////////////////NONE UE4//////////////////////////////////
///////////////////////////////////////////////////////////////////////////
#include <iostream>
#include <stdio.h>
#include <vector>


#define nullptr 0

#ifndef FORCEINLINE
#if (_MSC_VER >= 1200)
#define FORCEINLINE __forceinline
#else
#define FORCEINLINE __inline
#endif
#endif

//UE4 Macros
#define UPROPERTY(...)
#define UCLASS(...)
#define UENUM(...)
#define USTRUCT(...)
#define UMETA(...)


#define KCCORE_API
///////////////////////////////////////////////////////////////////////////
/////////////////////////////////NONE UE4//////////////////////////////////
///////////////////////////////////////////////////////////////////////////


#endif	//end USING_UE4

//sometimes we want to just have our own properties
#define KCPROPERTY(...)


//defines to help ease the transition to and from UE4 and standard library
#ifdef UE_BUILD_DEBUG
#define DEBUG_FORCEINLINE
#else
#ifdef _DEBUG
#define DEBUG_FORCEINLINE
#else
#define DEBUG_FORCEINLINE FORCEINLINE
#ifndef NDEBUG
#define NDEBUG
#endif
#endif
#endif


//these defines help create the macro for defining what the get an set are for the serializer.
//the c# serialize app, builds these macro's by naming them KCSERIALIZEOBJECT_ plus line number.
//
//NOTE - not sure why I have to add the MACRO_HACK. It won't let me combine them without calling another macro function
#define KCCOMBINE_MACROS(MacroA,MacroB) MacroA##MacroB
#define KCCOMBINE_MACRO_HACK(MacroA,MacroB) KCCOMBINE_MACROS(MacroA,MacroB)
#define KCSERIALIZE_CODE(...) KCCOMBINE_MACRO_HACK(KCSERIALIZEOBJECT_, __LINE__);





#define null 0
#define KC_NEW new
#define KC_NEW_ARRAY new []
#define INVALID -1
#define DELETE_SAFELY(mObjToDelete)			{ delete mObjToDelete; mObjToDelete=nullptr; }
#define DELETE_ARRAY_SAFELY(mObjToDelete)	{ delete [] mObjToDelete; mObjToDelete=nullptr; }



#include <unordered_map>
#include <mutex>
#include <map>
#include <cctype>
#include "Database/Private/KCDBDefinition.h"
#include "Utils/Helpers/KCBitAndMaskUtils.h"



