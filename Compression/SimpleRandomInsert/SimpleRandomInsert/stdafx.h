// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

// TODO: reference additional headers your program requires here
#include "windows.h"
#include <stdint.h>
#include <iostream>
#include <thread>
#include <mutex>
#include <vector>
#include <string>
#include <assert.h>

typedef int8_t			int8;
typedef uint8_t			uint8;
typedef int16_t			int16;
typedef uint16_t		uint16;
typedef int32_t			int32;
typedef uint32_t		uint32;
typedef int64_t			int64;
typedef uint64_t		uint64;
typedef TCHAR			wchar16;

typedef float			float32;
typedef float			Real;
typedef double			float64;
typedef wchar_t			wchar;

#ifndef NULL
#define NULL						0
#endif
#ifndef INVALID
#define INVALID						0xFFFFFFFF
#endif
#ifndef INVALID_GUID
#define INVALID_GUID				(int64)-1
#endif
typedef union UNIONDATA8BIT
{
	char			m_cValue;
	int8			m_iValue8;
	bool			m_bValue;

   UNIONDATA8BIT() : m_iValue8(0) {}
} unionData8Bit;

typedef union UNIONDATA16BIT
{
	wchar16			m_wValue;
	int16			m_iValue16;	
	int8			m_iValue8[2];
	char			m_cValue[2];
	bool			m_bValue;

   UNIONDATA16BIT() : m_wValue(0) {}
} unionData16Bit;


typedef union UNIONDATA32BIT
{
	int32			m_iValue;
	unsigned int	m_uiValue;
	float32			m_fValue;	
	wchar16			m_wValue[2];
	int16			m_iValue16[2];	
	int8			m_iValue8[4];
	char			m_cValue[4];
	bool			m_bValue;

   UNIONDATA32BIT() : m_iValue(0) {}
} unionData32Bit;
typedef union UNIONDATA64BIT
{
	float64			m_fValue64;
	int64			m_iValue64;
	uint64			m_uiValue64;
	int32			m_iValue[2];
	unsigned int	m_uiValue[2];
	float32			m_fValue[2];
	int16			m_iValue16[4];	
	wchar16			m_wValue[4];
	int8			m_iValue8[8];
	byte			m_Byte[8];
	char			m_cValue[8];
	bool			m_bValue;
   UNIONDATA64BIT() : m_iValue64(0) {}
} unionData64Bit;

const static std::string			EMPTY_STRING("");
const static std::wstring			EMPTY_WSTRING(L"");

#define ASSERT( i ){ assert( i ); }
#define ASSERT_RETURN( i ){ assert( i ); if( !(i) ){ return; } }
#define ASSERT_CONTINUE( i ){ assert( i ); if( !(i) ){ continue; } }
#define ASSERT_BREAK( i ){ assert( i ); if( !(i) ){ break; } }
#define ASSERT_RETVAL( i, v ){ assert( i ); if( !(i) ){ return v; } }
#define ASSERT_RETZERO( i ) { assert( i ); if( !(i) ){ return 0; } }



#define DELETE_NORMAL_SAFELY(o)						{ delete (o);	 (o)=NULL; }
//!Use for deleting an object. This will set the variable to NULL
#define DELETE_SAFELY(o)							{ delete (o);	 (o)=NULL; }
//!Use for deleting an array of objects. This will set the array to KNULL
#define DELETE_ARRAY_SAFELY(o)						{ delete[] (o);   (o)=NULL; }
// TODO: reference additional headers your program requires here



static uint32 getHash32( const void *key, int len, uint32 seed )
{
	// 'm' and 'r' are mixing constants generated offline.
	// They're not really 'magic', they just happen to work well.

	const uint32 m = 0x5bd1e995;
	const int32 r = 24;

	// Initialize the hash to a 'random' value

	uint32 h = seed ^ len;

	// Mix 4 bytes at a time into the hash

	const unsigned char * data = (const unsigned char *)key;

	while(len >= 4)
	{
		uint32 k = *(uint32 *)data;

		k *= m; 
		k ^= k >> r; 
		k *= m; 
		
		h *= m; 
		h ^= k;

		data += 4;
		len -= 4;
	}
	
	// Handle the last few bytes of the input array

	switch(len)
	{
	case 3: h ^= data[2] << 16;
	case 2: h ^= data[1] << 8;
	case 1: h ^= data[0];
	        h *= m;
	};

	// Do a few final mixes of the hash to ensure the last few
	// bytes are well-incorporated.

	h ^= h >> 13;
	h *= m;
	h ^= h >> 15;

	return h;
}

static uint64 getHash64( const void *key, int len, uint32 seed )
{
	const uint32 m = 0x5bd1e995;
	const int32 r = 24;

	uint32 h1 = seed ^ len;
	uint32 h2 = 0;

	const uint32 * data = (const uint32 *)key;

	while(len >= 8)
	{
		uint32 k1 = *data++;
		k1 *= m; k1 ^= k1 >> r; k1 *= m;
		h1 *= m; h1 ^= k1;
		len -= 4;

		uint32 k2 = *data++;
		k2 *= m; k2 ^= k2 >> r; k2 *= m;
		h2 *= m; h2 ^= k2;
		len -= 4;
	}

	if(len >= 4)
	{
		uint32 k1 = *data++;
		k1 *= m; k1 ^= k1 >> r; k1 *= m;
		h1 *= m; h1 ^= k1;
		len -= 4;
	}

	switch(len)
	{
	case 3: h2 ^= ((unsigned char*)data)[2] << 16;
	case 2: h2 ^= ((unsigned char*)data)[1] << 8;
	case 1: h2 ^= ((unsigned char*)data)[0];
			h2 *= m;
	};

	h1 ^= h2 >> 18; h1 *= m;
	h2 ^= h1 >> 22; h2 *= m;
	h1 ^= h2 >> 17; h1 *= m;
	h2 ^= h1 >> 19; h2 *= m;

	uint64 h = h1;

	h = (h << 32) | h2;

	return h;
}


static byte getHash8( const void *key, int len, uint32 seed )
{
	// 'm' and 'r' are mixing constants generated offline.
	// They're not really 'magic', they just happen to work well.

	const uint32 m = 0x5bd1e995;
	const int32 r = 24;

	// Initialize the hash to a 'random' value

	uint32 h = seed ^ len;

	// Mix 4 bytes at a time into the hash

	const unsigned char * data = (const unsigned char *)key;

	while(len >= 4)
	{
		uint32 k = *(uint32 *)data;

		k *= m; 
		k ^= k >> r; 
		k *= m; 
		
		h *= m; 
		h ^= k;

		data += 4;
		len -= 4;
	}
	
	// Handle the last few bytes of the input array

	switch(len)
	{
	case 3: h ^= data[2] << 16;
	case 2: h ^= data[1] << 8;
	case 1: h ^= data[0];
	        h *= m;
	};

	// Do a few final mixes of the hash to ensure the last few
	// bytes are well-incorporated.

	h ^= h >> 13;
	h *= m;
	h ^= h >> 15;

	for( int32 i = 0; i <= 24; i = i + 8 )
	{
		if( (( h >> i) & 0xFF) != 0 )
		{
			return (byte)(( h >> i) & 0xFF );
		}
	}

	return (byte)h;
}


//creates a mask
#define MASK_CREATE( iBit ) ( 1 << iBit )
//tests a mask
#define MASK_TEST( iMaskOne, iMaskTwo )( ( ( iMaskOne & iMaskTwo ) == iMaskTwo )?true:false )
//tests a bit in the mask
#define MASK_TEST_BIT( iMaskOne, iBitIndex )( ( iMaskOne & MASK_CREATE( iBitIndex ) )?true:false )
//removes a mask
#define MASK_REMOVE( iMaskOne, IMaskRemove )( iMaskOne = iMaskOne & ~IMaskRemove )
//Adds a value to a mask
#define MASK_ADD_MASK( iMaskOne, IMaskAdd )( iMaskOne = iMaskOne | IMaskAdd )
//removes a mask
#define MASK_REMOVE_BIT( iMaskOne, iBitIndex )( iMaskOne = iMaskOne & ~MASK_CREATE( iBitIndex ) )
//Adds a value to a mask
#define MASK_SET_BIT( iMaskOne, iBitIndex )( iMaskOne = iMaskOne | MASK_CREATE( iBitIndex ) )

#include "ThreadSafeArrayList.h"
#include "rand.h"
