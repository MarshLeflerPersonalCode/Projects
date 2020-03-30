#include "stdafx.h"
#include "GameCompression.h"
#include "FileIO/FileBinaryReader.h"
#include "FileIO/FileBinaryWriter.h"
#include "BitWritter.h"

#define HASH_CODE 7343985


struct GameSeqment
{
	size_t	m_iIndex;					//for saving and loading
	byte	m_OriginialData[9];			//the original data
	uint32	m_iOffset;
	bool	m_bForwardPlusMinus;		//  -- not used
	///////////////////////////////////////////////////////////	
	bool	m_bWorkedOrNot1Bit;			//1
	bool	m_bPlusOrMinus6Bits[6];		//6	
	byte	m_Value0_0;					//4
	byte	m_Value1_1;					//4
	byte	m_Value2_2;					//4
	byte	m_HashVal1;					//4
	byte	m_HashVal2;					//4
	byte	m_HashValAll;				//4
	
};//------------------------------------//31bits out of ( 9 * 4 bits + 1 bit for worked or not for 37 bits)

struct GameComputedValues
{
	byte	m_RangeStart[ 9 ];
	byte	m_RangeEnd[ 9 ];	
	bool	m_ValueComputed[9];
};

TThreadSafeArrayList< GameSeqment *>	m_Data;
uint32 g_iKeyValue( 7637223 );
static uint8 g_Map[16][16][16];
std::vector< uint32 > g_MapInverse[16];

inline byte _computeHashTo4Bits( byte *pArray, uint32 iSize )
{
	byte iHashValue = getHash8( pArray, iSize, HASH_CODE );
	if( (iHashValue & 0xF ) > 0 )
	{
		return iHashValue & 0xF;
	}

	return (iHashValue >> 4 ) & 0xF;
}
void createMap()
{
	byte iValues[3];
	int32 iSize( sizeof( byte ) * 3 );
	for( uint32 iFirstValue = 0; iFirstValue < 16; iFirstValue++ )
	{
		iValues[0] = (byte)iFirstValue;
		for( uint32 iSecondValue = 0; iSecondValue < 16; iSecondValue++ )
		{
			iValues[1] = (byte)iSecondValue;
			for( uint32 iThirdValue = 0; iThirdValue < 16; iThirdValue++ )
			{
				
				iValues[2] = (byte)iThirdValue;
				uint8 iHashValue( _computeHashTo4Bits( iValues, iSize ) );
				g_Map[ iFirstValue ][ iSecondValue ][ iThirdValue ] = iHashValue;
				g_MapInverse[ iHashValue ].push_back( iFirstValue + ( iSecondValue << 8 ) + ( iThirdValue << 16 ) );

			}
		}
	}
}
//|-----------|
//| 0 | 1 | 2 |
//|-----------|
//| 3 | 4 | 5 |
//|-----------|
//| 6 | 7 | 8 |
//|-----------|

void _fillPlusAndMinus( GameSeqment &mSegment, GameComputedValues &mComputedValues )
{
	//PATTERN 0
	// goes in order
	//PATERN 1
	// goes opposite order
	uint32 iForwardPattern( 0 );
	uint32 iBackwardPattern( 0 );
	int16	mDifferencesBetweenDataForward[9];
	int16	mDifferencesBetweenDataBackward[9];
	for( uint32 t = 0; t < 8; t++ )
	{
		mDifferencesBetweenDataForward[ t ] = abs( mSegment.m_OriginialData[t] - mSegment.m_OriginialData[t + 1] );
		if( t != 3 &&
			t != 7 )
		{
			iForwardPattern += mDifferencesBetweenDataForward[ t ];
		}
	}

	for( int32 t = 8; t >= 1; t-- )
	{
		mDifferencesBetweenDataBackward[ t ] = abs( mSegment.m_OriginialData[t] - mSegment.m_OriginialData[ t - 1] );
		if( t != 1 &&
			t != 5 )
		{
			iBackwardPattern += mDifferencesBetweenDataBackward[ t ];
		}
	}
	
	mSegment.m_bForwardPlusMinus = true;//( iForwardPattern <= iBackwardPattern )?true:false;

	if( mSegment.m_bForwardPlusMinus )
	{
		mSegment.m_bPlusOrMinus6Bits[0] = ( mDifferencesBetweenDataForward[ 0 ] <= mDifferencesBetweenDataForward[ 1 ] )?0:1;	
		mSegment.m_bPlusOrMinus6Bits[1] = ( mDifferencesBetweenDataForward[ 1 ] <= mDifferencesBetweenDataForward[ 2 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[2] = ( mDifferencesBetweenDataForward[ 2 ] <= mDifferencesBetweenDataForward[ 3 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[3] = ( mDifferencesBetweenDataForward[ 4 ] <= mDifferencesBetweenDataForward[ 5 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[4] = ( mDifferencesBetweenDataForward[ 5 ] <= mDifferencesBetweenDataForward[ 6 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[5] = ( mDifferencesBetweenDataForward[ 6 ] <= mDifferencesBetweenDataForward[ 7 ] )?0:1;
	}
	else
	{
		mSegment.m_bPlusOrMinus6Bits[0] = ( mDifferencesBetweenDataForward[ 8 ] <= mDifferencesBetweenDataForward[ 7 ] )?0:1;	
		mSegment.m_bPlusOrMinus6Bits[1] = ( mDifferencesBetweenDataForward[ 7 ] <= mDifferencesBetweenDataForward[ 6 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[2] = ( mDifferencesBetweenDataForward[ 6 ] <= mDifferencesBetweenDataForward[ 5 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[3] = ( mDifferencesBetweenDataForward[ 4 ] <= mDifferencesBetweenDataForward[ 3 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[4] = ( mDifferencesBetweenDataForward[ 3 ] <= mDifferencesBetweenDataForward[ 2 ] )?0:1;
		mSegment.m_bPlusOrMinus6Bits[5] = ( mDifferencesBetweenDataForward[ 2 ] <= mDifferencesBetweenDataForward[ 1 ] )?0:1;
	}

}



void _fillHashCodes( GameSeqment &mSegment )
{
	byte iHash0[3] = { mSegment.m_OriginialData[1], mSegment.m_OriginialData[2], mSegment.m_OriginialData[5] };
	byte iHash1[3] = { mSegment.m_OriginialData[3], mSegment.m_OriginialData[6], mSegment.m_OriginialData[7] };
	byte iHashAll[6] = { iHash0[0], iHash0[1], iHash0[2], iHash1[0], iHash1[1], iHash1[2] };
	mSegment.m_HashVal1 = _computeHashTo4Bits( iHash0, 3 );
	mSegment.m_HashVal2 = _computeHashTo4Bits( iHash1, 3 );
	mSegment.m_HashValAll = _computeHashTo4Bits( iHashAll, 6 );


}

inline void _computeRange( const bool bRangeType, const byte &mCompareValueStart, const byte &mCompareValueEnd, byte &mValueStart, byte &mValueEnd )
{
	if( bRangeType )
	{
		mValueStart = 0;
		mValueEnd = mCompareValueStart;
	}
	else
	{
		mValueStart = mCompareValueEnd + 1;
		mValueEnd = 15;

	}
}
void _computeRanges( GameSeqment &mSegment, GameComputedValues &mComputedValues )
{
	mComputedValues.m_RangeEnd[ 0 ] = mSegment.m_Value0_0;
	mComputedValues.m_RangeEnd[ 4 ] = mSegment.m_Value1_1;
	mComputedValues.m_RangeEnd[ 8 ] = mSegment.m_Value2_2;
	mComputedValues.m_RangeStart[ 0 ] = mSegment.m_Value0_0;
	mComputedValues.m_RangeStart[ 4 ] = mSegment.m_Value1_1;
	mComputedValues.m_RangeStart[ 8 ] = mSegment.m_Value2_2;	
	mComputedValues.m_ValueComputed[ 0 ] = true;
	mComputedValues.m_ValueComputed[ 4 ] = true;
	mComputedValues.m_ValueComputed[ 8 ] = true;

	if( mSegment.m_bForwardPlusMinus )
	{
		_computeRange( mSegment.m_bPlusOrMinus6Bits[0], mComputedValues.m_RangeStart[ 0 ], mComputedValues.m_RangeEnd[ 0 ], mComputedValues.m_RangeStart[ 1 ], mComputedValues.m_RangeEnd[ 1 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[1], mComputedValues.m_RangeStart[ 1 ], mComputedValues.m_RangeEnd[ 1 ], mComputedValues.m_RangeStart[ 2 ], mComputedValues.m_RangeEnd[ 2 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[2], mComputedValues.m_RangeStart[ 2 ], mComputedValues.m_RangeEnd[ 2 ], mComputedValues.m_RangeStart[ 3 ], mComputedValues.m_RangeEnd[ 3 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[3], mComputedValues.m_RangeStart[ 4 ], mComputedValues.m_RangeEnd[ 4 ], mComputedValues.m_RangeStart[ 5 ], mComputedValues.m_RangeEnd[ 5 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[4], mComputedValues.m_RangeStart[ 5 ], mComputedValues.m_RangeEnd[ 5 ], mComputedValues.m_RangeStart[ 6 ], mComputedValues.m_RangeEnd[ 6 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[5], mComputedValues.m_RangeStart[ 6 ], mComputedValues.m_RangeEnd[ 6 ], mComputedValues.m_RangeStart[ 7 ], mComputedValues.m_RangeEnd[ 7 ] );
	}
	else
	{
		_computeRange( mSegment.m_bPlusOrMinus6Bits[0], mComputedValues.m_RangeStart[ 8 ], mComputedValues.m_RangeEnd[ 8 ], mComputedValues.m_RangeStart[ 7 ], mComputedValues.m_RangeEnd[ 7 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[1], mComputedValues.m_RangeStart[ 7 ], mComputedValues.m_RangeEnd[ 7 ], mComputedValues.m_RangeStart[ 6 ], mComputedValues.m_RangeEnd[ 6 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[2], mComputedValues.m_RangeStart[ 6 ], mComputedValues.m_RangeEnd[ 6 ], mComputedValues.m_RangeStart[ 5 ], mComputedValues.m_RangeEnd[ 5 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[3], mComputedValues.m_RangeStart[ 4 ], mComputedValues.m_RangeEnd[ 4 ], mComputedValues.m_RangeStart[ 3 ], mComputedValues.m_RangeEnd[ 3 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[4], mComputedValues.m_RangeStart[ 3 ], mComputedValues.m_RangeEnd[ 3 ], mComputedValues.m_RangeStart[ 2 ], mComputedValues.m_RangeEnd[ 2 ] );
		_computeRange( mSegment.m_bPlusOrMinus6Bits[5], mComputedValues.m_RangeStart[ 2 ], mComputedValues.m_RangeEnd[ 2 ], mComputedValues.m_RangeStart[ 1 ], mComputedValues.m_RangeEnd[ 1 ] );
	}
}

bool _findMatches( GameSeqment &mSegment, GameComputedValues &mComputedValues )
{
	std::vector< GameSeqment > mValidValues;
	
	GameSeqment mValuesToTest = mSegment;
	for( uint32 iValIndex0 = 0; iValIndex0 < g_MapInverse[ mSegment.m_HashVal1 ].size(); iValIndex0++ )
	{
		for( uint32 iValIndex1 = 0; iValIndex1 < g_MapInverse[ mSegment.m_HashVal2 ].size(); iValIndex1++ )
		{
			mValuesToTest.m_OriginialData[1] = g_MapInverse[ mSegment.m_HashVal1 ][ iValIndex0 ] & 0xF;
			mValuesToTest.m_OriginialData[2] = (g_MapInverse[ mSegment.m_HashVal1 ][ iValIndex0 ] >> 8 ) & 0xF;
			mValuesToTest.m_OriginialData[5] = (g_MapInverse[ mSegment.m_HashVal1 ][ iValIndex0 ] >> 16) & 0xF;
			mValuesToTest.m_OriginialData[3] = g_MapInverse[ mSegment.m_HashVal2 ][ iValIndex1 ] & 0xF;
			mValuesToTest.m_OriginialData[6] = (g_MapInverse[ mSegment.m_HashVal2 ][ iValIndex1 ] >> 8 ) & 0xF;
			mValuesToTest.m_OriginialData[7] = (g_MapInverse[ mSegment.m_HashVal2 ][ iValIndex1 ] >> 16) & 0xF;
			_fillPlusAndMinus( mValuesToTest, mComputedValues );
			if( mValuesToTest.m_bForwardPlusMinus != mSegment.m_bForwardPlusMinus ){ continue; } //not it
			bool bValid( true );
			for( uint32 iPlusOrMinus = 0; iPlusOrMinus < 6; iPlusOrMinus ++ )
			{
				if( mValuesToTest.m_bPlusOrMinus6Bits[ iPlusOrMinus ] != mSegment.m_bPlusOrMinus6Bits[ iPlusOrMinus ] )
				{
					bValid = false;
					break;
				}
			}
			if( bValid == false ){ continue; }
			_fillHashCodes( mValuesToTest );
			if( mValuesToTest.m_HashValAll != mSegment.m_HashValAll ){ continue; }
			
			mValidValues.push_back( mValuesToTest );			
		}
	}
	mSegment.m_bWorkedOrNot1Bit = false;
	if( mValidValues.size() == 0 )
	{
		
		return false;
	}
	mSegment.m_iOffset = INVALID;
	uint32 iIndexValid( INVALID );
	for( iIndexValid = 0; iIndexValid < mValidValues.size(); iIndexValid++ )
	{
		bool bValid( true );
		for( uint32 iOriginalIndex = 0; iOriginalIndex < 9; iOriginalIndex++ )
		{
			if( mValidValues[ iIndexValid ].m_OriginialData[ iOriginalIndex ] != mSegment.m_OriginialData[ iOriginalIndex ] )
			{
				bValid = false;
				break;
			}
		}
		if( bValid )
		{
			break;
		}
	}
	mSegment.m_iOffset = iIndexValid;
	if( iIndexValid == 0)
	{
		
		mSegment.m_bWorkedOrNot1Bit = true;
		return true;
	}
	return false;
}

void _doGame( GameSeqment &mSegment, GameComputedValues &mComputedValues )
{
	_computeRanges( mSegment, mComputedValues );
	_findMatches( mSegment, mComputedValues );
	//use lookup table to find hash code simularities.
	//Then attempt to hash it all!

}

void createGameData( GameSeqment *pSegment )
{
	GameComputedValues mComputedValues;
	ZeroMemory( &mComputedValues, sizeof( GameComputedValues ) );
	_fillPlusAndMinus( *pSegment, mComputedValues );
	_fillHashCodes( *pSegment );
	_doGame( *pSegment, mComputedValues );
}

void parseData( uint32 iMaxThreads, byte *pArray, size_t iSize )
{
	
	size_t iCount = iSize/9;
	size_t iRemainder = iCount - 9;
	size_t iMemIndex( 0 );
	
	m_Data.setGrowBy( iCount + 1 );
	
	TThreadSafeArrayList< std::thread * > m_Threads;
	uint32 iByteIndex( 0 );
	GameSeqment *pSegment( NULL );

	for( size_t iIndex = 0; iIndex < iSize; iIndex++ )
	{
		if( iIndex >= iSize - 5 )
		{
			break;
		}
		byte mLow = pArray[ iIndex ] & 0xF;
		byte mHigh = (pArray[ iIndex ] >> 4) & 0xF;
		if( iByteIndex >= 9 ||
			pSegment == NULL )
		{
			if( pSegment )
			{
				std::thread *pThread = new std::thread( createGameData,
														pSegment );
			
				m_Threads.add( pThread );
			}
			pSegment = new GameSeqment();
			pSegment->m_iIndex = m_Data.size();
			m_Data.add( pSegment );
			iByteIndex = 0;
		}
		pSegment->m_OriginialData[ iByteIndex++ ] = mLow;
		if( iByteIndex >= 9 )
		{
			
			std::thread *pThread = new std::thread( createGameData,
													pSegment );

		    m_Threads.add( pThread );
			pSegment = new GameSeqment();
			pSegment->m_iIndex = m_Data.size();
			m_Data.add( pSegment );
			iByteIndex = 0;
		}
		pSegment->m_OriginialData[ iByteIndex++ ] = mHigh;
		
		
		if( m_Threads.size() > iMaxThreads )
		{
			for( uint32 t = 0; t < m_Threads.size(); t++ )
			{
				m_Threads[t]->join();
			}			
			m_Threads.deleteAll( false );
		}

	}

	size_t iValid( 0 );
	uint32 iLargestDiffence( 0 );
	std::vector< uint32 > mCounts;
	for( size_t i = 0; i < m_Data.size(); i++ )
	{
		if( m_Data[i]->m_iOffset < 1024 )
		{
			while( mCounts.size() <= m_Data[i]->m_iOffset )
			{
				mCounts.push_back( 0 );
			}
			mCounts[ m_Data[i]->m_iOffset ]++;// = mCounts[ m_Data[i]->m_iOffset ] + 1;
		}
		if( m_Data[i]->m_bWorkedOrNot1Bit )
		{
			iValid++;
		}
	}

	size_t iValidSegmentSizes( iValid * 31 ); 
	size_t iInvalidSegmentSizes( (m_Data.size() - iValid ) * 37 );
	float32 fPercentSaved( (float32)((float64)( iInvalidSegmentSizes + iValidSegmentSizes)/(float64)(iSize * 8) ) );
	
	printf("New Percent of File:%1.5f\n", fPercentSaved );
}

void _doGameCompression()
{
	createMap();
	size_t  iSize( 200 );
	byte	mData[200];
	Rand mRand( 3438712 );
	for( size_t t = 0; t < iSize; t++ )
	{
		mData[t] = (byte)mRand.Int32();
	}
	parseData( 6, mData, iSize);
	
}

