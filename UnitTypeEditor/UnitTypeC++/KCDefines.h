#pragma once
//defines to help ease the transition to and from UE4 and standard library


/////////////////////Took this straight from UE4
#ifdef USING_UE4
#include "CoreMinimal.h"
#define KCString FName
#else
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

#include <string>
#include <iostream>
#include <stdio.h>
#include <vector>
#include <map>

#define FORCEINLINE inline
#define KCString std::string
#define nullptr 0
#define null 0
#define EMPTY_KCSTRING ""


//UE4 Macros
#define UPROPERTY(...)
#define UCLASS(...)
#define UENUM(...)
#define USTRUCT(...)
#endif

#define INVALID -1
#define DELETE_SAFELY(mObjToDelete)			{ delete mObjToDelete; mObjToDelete=nullptr; }
#define DELETE_ARRAY_SAFELY(mObjToDelete)	{ delete [] mObjToDelete; mObjToDelete=nullptr; }



