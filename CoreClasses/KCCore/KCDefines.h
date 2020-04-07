//copyright Marsh Lefler 2000-...
#pragma once
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

#ifndef _WCHAR_DEFINED
#define _WCHAR_DEFINED
typedef wchar_t				WCHAR;
typedef WCHAR				TCHAR;
#endif // !_WCHAR_DEFINED


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




#include <iostream>
#include <stdio.h>
#include <vector>


#define nullptr 0


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

#endif	//note using_ue4



typedef union _UNIONDATA8BIT_
{
	bool            m_bValue;
	char            m_cValue;
	int8            m_iValue8;	
	uint8            m_uiValue8;
	_UNIONDATA8BIT_() : m_iValue8(0) {}
	_UNIONDATA8BIT_(bool bValue) { m_bValue = bValue; }
	_UNIONDATA8BIT_(char cValue) { m_cValue = cValue; }
	_UNIONDATA8BIT_(int8 iValue) { m_iValue8 = iValue; }
	_UNIONDATA8BIT_(uint8 iValue) { m_uiValue8 = iValue; }
} coreUnionData8Bit;

typedef union _UNIONDATA16BIT_
{
	bool			m_bValue[2];
	char            m_cValue[2];
	int8			m_iValue8[2];
	uint8			m_uiValue8[2];
	int16           m_iValue16;
	uint16          m_uiValue16;	
	_UNIONDATA16BIT_() : m_iValue16(0) {}
	_UNIONDATA16BIT_(bool bValue)  { m_iValue16 = (bValue) ? 1 : 0; }
	_UNIONDATA16BIT_(int8 iValue)  { m_iValue16 = (int16)iValue; }
	_UNIONDATA16BIT_(uint8 iValue)  { m_uiValue16 = (uint16)iValue; }
	_UNIONDATA16BIT_(int16 iValue)  { m_iValue16 = (int16)iValue; }
	_UNIONDATA16BIT_(uint16 iValue)  { m_uiValue16 = (uint16)iValue; }

} coreUnionData16Bit;


typedef union _UNIONDATA32BIT_
{
	bool			m_bValue[4];
	char			m_cValue[4];
	int8			m_iValue8[4];
	uint8			m_uiValue8[4];
	int16           m_iValue16[2];
	uint16          m_uiValue16[2];
	int32           m_iValue32;
	uint32			m_uiValue32;
	float			m_fValue;
	
	
	

	_UNIONDATA32BIT_() : m_iValue32(0) {}
	_UNIONDATA32BIT_(bool bValue) { m_iValue32 = (bValue)?1:0; }
	_UNIONDATA32BIT_(int8 iValue) { m_iValue32 = (int32)iValue; }
	_UNIONDATA32BIT_(uint8 iValue) { m_uiValue32 = (uint32)iValue; }
	_UNIONDATA32BIT_(int16 iValue) { m_iValue32 = (int32)iValue; }
	_UNIONDATA32BIT_(uint16 iValue) { m_uiValue32 = (uint32)iValue; }
	_UNIONDATA32BIT_(int32 iValue) { m_iValue32 = iValue; }
	_UNIONDATA32BIT_(uint32 iValue) { m_uiValue32 = iValue; }
	_UNIONDATA32BIT_(float fValue)  { m_fValue = fValue; }
	
} coreUnionData32Bit;

typedef union UNIONDATA64BIT
{
	bool				m_bValue[8];
	char				m_cValue[8];
	int8				m_iValue8[8];
	uint8				m_uiValue8[8];
	int16				m_iValue16[4];
	uint16				m_uiValue16[4];
	int32				m_iValue32[2];
	uint32				m_uiValue32[2];
	int64				m_iValue64;
	uint64				m_uiValue64;
	float				m_fValue[2];
	double				m_dValue64;

	UNIONDATA64BIT() : m_iValue64(0) {}
	UNIONDATA64BIT(bool bValue) { m_iValue64 = (bValue) ? 1 : 0; }
	UNIONDATA64BIT(char iValue) : m_iValue64((int64)iValue) {}
	UNIONDATA64BIT(int8 iValue) : m_iValue64((int64)iValue) {}
	UNIONDATA64BIT(uint8 uiValue) : m_uiValue64((uint64)uiValue) {}
	UNIONDATA64BIT(int16 iValue) : m_iValue64((int64)iValue) {}
	UNIONDATA64BIT(uint16 uiValue) : m_uiValue64((uint64)uiValue) {}
	UNIONDATA64BIT(int32 iValue) : m_iValue64((int64)iValue) {}
	UNIONDATA64BIT(uint32 uiValue) : m_uiValue64((uint64)uiValue) {}
	UNIONDATA64BIT(float fValue) : m_dValue64((double)fValue) {}
	UNIONDATA64BIT(double fValue) : m_dValue64(fValue) {}
} coreUnionData64Bit;







#define null 0
#define KC_NEW new
#define KC_NEW_ARRAY new []
#define KCString std::string
static const KCString EMPTY_KCSTRING = "";
#define INVALID -1
#define DELETE_SAFELY(mObjToDelete)			{ delete mObjToDelete; mObjToDelete=nullptr; }
#define DELETE_ARRAY_SAFELY(mObjToDelete)	{ delete [] mObjToDelete; mObjToDelete=nullptr; }



#include <unordered_map>
#include <mutex>
#include <map>
#include <string>
#include <cctype>
#include "Serialization/KCSerializationDefines.h"


