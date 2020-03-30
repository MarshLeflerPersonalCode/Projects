#pragma once
//defines to help ease the transition to and from UE4 and standard library


/////////////////////Took this straight from UE4
#ifdef USING_UE4
#include "CoreMinimal.h"

#else
///////////////////////////////////////////////////////////////////////////
/////////////////////////////////NONE UE4//////////////////////////////////
///////////////////////////////////////////////////////////////////////////
// Standard typedefs
typedef signed char         schar;
typedef signed char         int8;
typedef short               int16;
typedef int                 int32;
#ifdef _MSC_VER
typedef __int64             int64;
#else
typedef long long           int64;
#endif /* _MSC_VER */

typedef unsigned char      uint8;
typedef unsigned short     uint16;
typedef unsigned int       uint32;
#ifdef _MSC_VER
typedef unsigned __int64   uint64;
#else
typedef unsigned long long uint64;

#endif /* _MSC_VER */

#ifndef FORCEINLINE
#if (_MSC_VER >= 1200)
#define FORCEINLINE __forceinline
#else
#define FORCEINLINE __inline
#endif
#endif



#include <string>
#include <iostream>
#include <stdio.h>
#include <vector>
#include <map>

#define nullptr 0
#define null 0

//UE4 Macros
#define UPROPERTY(...)
#define UCLASS(...)
#define UENUM(...)
#define USTRUCT(...)

#define KCCORE_API



///////////////////////////////////////////////////////////////////////////
/////////////////////////////////NONE UE4//////////////////////////////////
///////////////////////////////////////////////////////////////////////////

#endif	//note using_ue4

#define KC_NEW new
#define KC_NEW_ARRAY new []
#define KCString std::string
#define EMPTY_KCSTRING ""
#define INVALID -1
#define DELETE_SAFELY(mObjToDelete)			{ delete mObjToDelete; mObjToDelete=nullptr; }
#define DELETE_ARRAY_SAFELY(mObjToDelete)	{ delete [] mObjToDelete; mObjToDelete=nullptr; }




#include "Containers/KCTArray.h"
