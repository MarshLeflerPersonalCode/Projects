//////////////////////////////////////////////////////////////////////////
// PROPERTY OF RUNIC GAMES
// 1500 4th Ave Seattle WA 98101
//
//      Copyright (c) 2008.
//      All Rights Reserved.
//    
//    This Software may not be used/copied or disclosed to any person or persons
// that are not employed by Runic Games.
//
// http://www.runicgames.com
//
//
// CBoolArrayList<T> - Template for array lists of bools
//////////////////////////////////////////////////////////////////////////
#pragma once
#ifndef _CBITWRITTER_H
#define _CBITWRITTER_H
#include "stdafx.h"
#include "BoolArrayList.h"

class CBitWritter : public CBoolArrayList
{
public:
	CBitWritter( size_t nGrowBy = 10 ) :
		CBoolArrayList( nGrowBy )
	{
	}

	~CBitWritter( void )
	{
		
		
	}

	void		setByte( size_t iIndex, byte &mByte )
	{
		mByte = 0;
		size_t iCount( 0 );
		for( int32 t = 7; t >= 0; t-- )
		{
			int32 iValue = (get( iIndex + iCount )?1:0);
			iCount++;
			mByte = mByte | (1 << iValue );
		}
	}

	template <class T >
	void		copyBits( T &iValue, 
						  int8 iStartingBit = 0,
						  int8 iEndingBit = -1 )
	{
		int64 iNumOfBits = (int64)((sizeof( T )*8) - 1 );
		if( iEndingBit > 0  )
		{
			iNumOfBits = iEndingBit - 1;
		}
		for( int64 t = iNumOfBits; t >= iStartingBit; t-- )
		{
			add( (( (int64)iValue & ((int64)1 << t )) != 0)?true:false );
		}
	}

	void		copyByte( byte &mByte )
	{
		for( int32 t = 7; t >= 0; t-- )
		{
			add( (( mByte & (1 << t )) != 0)?true:false );
		}
	}
	
	
	byte		getByteStartingAt( size_t iIndex, 
								   int8 iStartingBit = 0,
								   int8 iEndingBit = 8 )
	{
		byte bByteData( 0 );
		for( int8 iBitShift = iEndingBit - 1; iBitShift >= iStartingBit; iBitShift-- )
		{
			bByteData = bByteData | (get( iIndex++ ) << iBitShift);
		}
		return bByteData;
	}

	

	//!copies in a int64
	template <class T>
	void		copy( T &iValue )
	{
		int32 iSize = (int32)sizeof( T );

		for( int32 t = iSize - 1; t >= 0; t++ )
		{
			copyByte( (byte)(( iValue >> (t * 8) ) & 0xF ) );
		}
	}
}; 

#endif