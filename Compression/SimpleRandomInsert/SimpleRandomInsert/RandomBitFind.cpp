#include "stdafx.h"
#include "GameCompression.h"
#include "FileIO/FileBinaryReader.h"
#include "FileIO/FileBinaryWriter.h"
#include "BitWritter.h"

struct ReturnData
{
	CBitWritter mData;
};
static bool g_SaveBothOneAndZero( false );
//static size_t g_BitsSaving( (g_SaveBothOneAndZero)?6:5 );
//#define BITS_SAVING 6
//bit 1 is works or not
//bit 2-4 is random key index
//bit 5-7 is count
#define RANDOM_NUMBER_MIN_COUNT 7
#define RANDOM_NUMBER_ADDITION 7
#define RANDOM_NUMBER_MAX_COUNT 14
#define COUNT_OF_BITS_COUNT_PAST_BITS_SAVING 3



#define RANDOM_COUNT 0x8
#define COUNT_OF_BITS_RANDOM_KEYS 3
static uint32 g_RandomKeys[ RANDOM_COUNT ] = { 342348, 986434, 834834, 736243, 543634, 634, 23, 3424 };//, 16, 455, 500987, 62396, 799221, 854300, 98234 };


#define COUNT_OF_BITS_IN_HEADER 6


struct RandomBitData
{
	CBoolArrayList			m_Data;
	byte					m_iRandomKeyAndCount;	
	std::vector< size_t >	m_BitIndexRemoved;
	inline void				setRandomKey( byte iFirstThreeBits ){ m_iRandomKeyAndCount = (byte)(( (int32)m_iRandomKeyAndCount & 0xF8 ) | ( iFirstThreeBits & 0x7 )); }
	inline byte				getRandomKey( void ) { return (int32)(m_iRandomKeyAndCount & 0x7); }
	inline void				setCountPastBitsSaving( byte iBits4Through7 )
	{ 
		m_iRandomKeyAndCount = ( m_iRandomKeyAndCount & 0x7 ) | (( iBits4Through7 & 0x7 ) << 3 );
	}
	inline byte				getCountPastBitsSaving( void ) 
	{ 
		return (byte)(m_iRandomKeyAndCount >> 3); 
	}
};

#define HEADER_DATA_BITS_TO_SAVE 6
struct Header
{
	//first 2 bits is the values past the initial savings
	//third bit is if the last data was original or not
	//fourth bit is searching for 1 or zero
	//fifth through 8 is the size mult
	byte					m_Data;	
	size_t					m_iCalculatedSavings;	
	byte					*m_pDataCompressed;
	size_t					m_iDataCompressedSize;
	//inline void				setBitsPast6( int32 iBits0_1_2 ){ m_Data = (byte)(( (int32)m_Data & 0xFC ) | ( iBits0_1_2 & 0x3 )); }
	//inline int32			getBitsPast6( void ) { return (int32)(m_Data & 0x3); }
	inline void				setUncompressToOrignial( bool iFirstBit ) { m_Data = (m_Data & 0xFE) | (( iFirstBit )?0x1:0); }
	inline bool				getUncompressToOriginal( void )	{ return ( m_Data & 0x1 )?true:false; }
	inline void				setSearchingForZero( bool bSecondBit ){ m_Data = (m_Data & 0xFD) | (( bSecondBit )?0:0x2); }
	inline bool				getSearchingForZero( void ){ return (m_Data & 0x2)?false:true; } //little weird but if it's 1 it's searching for 1 but thefunction is searchign for zero
	inline void				setSizeMult( int32 i3Bits ) 
	{ 
		m_Data = ( m_Data & 0x3 ) | ( i3Bits << 2 ); 
	}
	inline int32			getSizeMult( void ) 
	{ 
		return (int32)(m_Data >> 2); 
	}

};



size_t getRandom( Rand &mRand, 
				  size_t iStart,
				  size_t iEnd,
				  std::vector< size_t > &mList )
{
	size_t iIndex = mRand.UInt64( iStart, iEnd );
	for( uint32 t = 0; t < mList.size(); t++ )
	{
		if( mList[t] == iIndex )
		{
			iIndex = mRand.UInt64( iStart, iEnd );
			t = 0;
			continue;
		}
	}
	mList.push_back( iIndex );
	return iIndex;
}
size_t _findBestRow( RandomBitData &mBitData, bool bLookingForZero )
{
	static int32 iCount( 0 );
	iCount++;
	Rand mRand( 1 );
	size_t iCountMost( (size_t)mBitData.m_BitIndexRemoved.size() );
	mBitData.m_iRandomKeyAndCount = 0;
	std::vector< size_t >	mChoicesSoFar;
	for( byte iKeyIndex = 0; iKeyIndex < RANDOM_COUNT; iKeyIndex++ )
	{
		mRand.setState( g_RandomKeys[ iKeyIndex ] );
		mChoicesSoFar.clear();
		
		bool bFoundBadValue( false );
		do
		{
			size_t iIndex( getRandom( mRand, 0, mBitData.m_Data.size(), mChoicesSoFar ) );
			if( iIndex == -1 ){ break; }
			bool bFoundZero( (mBitData.m_Data[ iIndex ] == 0 )?true:false );			
			if( bFoundZero != bLookingForZero)
			{ 
				if( mChoicesSoFar.size() >= RANDOM_NUMBER_MAX_COUNT )
				{
					mChoicesSoFar.clear();
					//mChoicesSoFar.pop_back(); //Weird case. But if the last bit isn't the bit we are looking for we have to remove it.
				}
				break; 
			}
			/*if( mChoicesSoFar.size() == MAX_COUNT_OF_RANDOM_NUMBERS )
			{
				mChoicesSoFar.clear();
				break; //we allow the last bit to be the bit we are looking for if it's the max bit else we exit!
			}*/
			
		}
		while( true );

		if( mChoicesSoFar.size() > iCountMost )
		{			
			mBitData.setRandomKey( iKeyIndex );
			mBitData.m_BitIndexRemoved = mChoicesSoFar;
			iCountMost = mChoicesSoFar.size();
		}
	}
	if( iCountMost >= RANDOM_NUMBER_MIN_COUNT )
	{
		if( iCountMost >= RANDOM_NUMBER_MAX_COUNT )
		{
			iCountMost = RANDOM_NUMBER_MAX_COUNT;
		}
		byte iMax( (byte)( min( RANDOM_NUMBER_MAX_COUNT, iCountMost ) ) );
		iMax -= RANDOM_NUMBER_MIN_COUNT;
		if( iMax > RANDOM_NUMBER_ADDITION )
		{
			printf( "BAD" );
		}
		mBitData.setCountPastBitsSaving( iMax );
	}
	return iCountMost;
}

bool findBestRandom( RandomBitData *pBitData, size_t iMinBits, bool bLookingForZero )
{
	pBitData->m_iRandomKeyAndCount = 0;
	if( g_SaveBothOneAndZero )
	{
		_findBestRow( *pBitData, false );
		_findBestRow( *pBitData, true );
	}
	else
	{
		_findBestRow( *pBitData, bLookingForZero );//bLookingForZero );
	}
	size_t iBest = pBitData->m_BitIndexRemoved.size();
	if( iBest < iMinBits ) 
	{ 
		pBitData->m_BitIndexRemoved.clear();
		return false; 
	}
	//_compressData( mBitData );
	return true;
}


//returns the directory path
std::wstring GetApplicationPath2( void )
{
	static std::wstring absolutePath = L"";
	if( absolutePath.size() == 0 )
	{
		wchar16       szAppPath[MAX_PATH] = L"";
		std::wstring strAppPath;
		::GetModuleFileNameW(0, szAppPath, MAX_PATH);
		// Extract name
		strAppPath = szAppPath;
		strAppPath = strAppPath.substr(0, strAppPath.rfind(L"\\") + 1);
		//absolutePath = STRINGS::StringUpper( CleanPath(strAppPath) );
		absolutePath = strAppPath;
	}
	return absolutePath;	
}

void _saveGoodData( CBitWritter &mBitWritter, RandomBitData *pBitData )//, size_t iBitsToHaveMoreOf )
{
	static int32 iCount( 0 );	
	iCount++;
	mBitWritter.add( true );								//first bit says it's good
	byte iRandomKeyIndex( pBitData->getRandomKey() );
	byte iCountOfBitsToSave( pBitData->getCountPastBitsSaving() );
	mBitWritter.copyBits( iRandomKeyIndex, 0, COUNT_OF_BITS_RANDOM_KEYS );	//random key
	mBitWritter.copyBits( iCountOfBitsToSave, 0, COUNT_OF_BITS_COUNT_PAST_BITS_SAVING );	//random key
	for( size_t t = 0; t < pBitData->m_Data.size(); t++ )
	{
		bool bAllowBitToSave( true );
		for( size_t i = 0; i < pBitData->m_BitIndexRemoved.size(); i++ ) //note they might have more then the bits to have but we only savethat many.
		{
			if( pBitData->m_BitIndexRemoved[i] == t )
			{
				bAllowBitToSave = false;	//this is one of the bits we don't save
				break;
			}
		}
		if( bAllowBitToSave )
		{
			mBitWritter.add( pBitData->m_Data[t] );
		}
	}
}

inline void _saveBadData( bool bWriteFirstBadBit, CBitWritter &mBitWritter, RandomBitData *pBitData )
{
	if( bWriteFirstBadBit )
	{
		mBitWritter.add( false ); //first bit says it's good 
	}
	for( size_t t = 0; t < pBitData->m_Data.size(); t++ )
	{
		mBitWritter.add( pBitData->m_Data[t] );	
	}
}
inline void _testSaveList( size_t iTestAgainst, size_t &iIndex, CBitWritter &mBitWritter, std::vector< RandomBitData * > &mArray )
{
	if( iIndex + iTestAgainst <= mArray.size() )
	{
		mBitWritter.copyBits( iTestAgainst, 0, 4 );
		for( size_t t = 0; t < iTestAgainst; t++ )
		{
			_saveBadData( false, mBitWritter, mArray[ iIndex++ ] );
		}
	}
}
void _saveBadData( CBitWritter &mBitWritter, std::vector< RandomBitData * > &mArray )
{
	
	//we can only break it up into 3 so here goes nothing.
	
	/*size_t iIndex( 0 );	
	while( iIndex < mArray.size() )
	{
		mBitWritter.add( false ); //first bit says it's good 
		for( int64 iTest = 16; iTest >= 1; iTest-- )
		{
			_testSaveList( iTest, iIndex, mBitWritter, mArray );
		}
		
	}	*/
	
	for( size_t t = 0; t < mArray.size(); t++ )
	{
		_saveBadData( true, mBitWritter, mArray[ t ] );
	}
	
}
void _saveData( CBitWritter &mBitWritter, std::vector< RandomBitData *> &mData, size_t iBitsToHaveMoreOf )
{
	for( size_t iDataCount = 0; iDataCount < mData.size(); iDataCount++ )
	{
		RandomBitData *pBitData = mData[ iDataCount ];		
		if( pBitData->m_BitIndexRemoved.size() >= RANDOM_NUMBER_MIN_COUNT )
		{			
			_saveGoodData( mBitWritter, pBitData );		
		}
		else
		{
			std::vector< RandomBitData * > mInARow;						
			for( iDataCount; iDataCount < mData.size(); iDataCount++ )
			{
				pBitData = mData[ iDataCount ];
				if( pBitData->m_BitIndexRemoved.size() < RANDOM_NUMBER_MIN_COUNT )
				{
					mInARow.push_back( pBitData );
				}
				else
				{
					iDataCount--; //we'll get one added on when it loops
					break;
				}
			}	
			_saveBadData( mBitWritter, mInARow );				

		}
	}
}

size_t _compressDataWithHeader( byte *pBytes, size_t iSize, Header *pHeader, size_t iBestSizeSoFar )
{
	size_t iByteIndex( 0 );
	size_t iSplitSizeInBytes( 100 + ( 100 * pHeader->getSizeMult() ) );
	size_t iBitsToFindInRow( RANDOM_NUMBER_MIN_COUNT );
	size_t iNumberOfSplits( iSize/iSplitSizeInBytes );
	int64 iRemainder( iSize - ( iSplitSizeInBytes * iNumberOfSplits ) - 1 );
	size_t iSucceeded( 0 );
	size_t iFailed( 0 );
	size_t iSuccessfulInARow( 0 );
	size_t iFailedInARow( 0 );
	bool bSearchingForZero = pHeader->getSearchingForZero();
	std::vector< std::thread * > m_Threads;
	std::vector< RandomBitData *> mData;
	for( size_t iSplitIndex = 0; iSplitIndex < iNumberOfSplits; iSplitIndex ++)
	{
		mData.push_back( new RandomBitData() );
		mData[ iSplitIndex ]->m_Data.setData( &pBytes[ iByteIndex ], iSplitSizeInBytes, iSplitSizeInBytes * 8 );
		iByteIndex += iSplitSizeInBytes;
		findBestRandom( mData[ iSplitIndex ], iBitsToFindInRow, bSearchingForZero );
	}
	if( iRemainder > 0 ) //make sure to save the last bits if we can
	{
		mData.push_back( new RandomBitData() );
		mData[ mData.size() - 1 ]->m_Data.setData( &pBytes[ iByteIndex ], iRemainder, iRemainder * 8 );
		iByteIndex += iRemainder;
		//findBestRandom( mData[ mData.size() - 1 ], iBitsToFindInRow, bSearchingForZero );
	}
	CBitWritter mBitWritter( iSize );
	
	size_t iTempSuccess( 0 );
	size_t iTempFailed( 0 );
	for( size_t iDataIndex = 0; iDataIndex < mData.size(); iDataIndex++ )
	{
		//TODO: save data		
		
		if( mData[iDataIndex]->m_BitIndexRemoved.size() >= RANDOM_NUMBER_MIN_COUNT )
		{
			iSucceeded++;
			iTempSuccess++;
			iTempFailed = 0;
		}
		else
		{
			iFailed++;
			iTempSuccess = 0;
			iTempFailed++;
		}
		if( iTempFailed > iFailedInARow ){ iFailedInARow = iTempFailed; }
		if( iTempSuccess > iSuccessfulInARow ){ iSuccessfulInARow = iTempSuccess; }
	}
	mBitWritter.copyBits( pHeader->m_Data, 0, COUNT_OF_BITS_IN_HEADER ); //save header info
	_saveData( mBitWritter, mData, iBitsToFindInRow );
	//if( iBitsSaved > 0 || iBestSizeSoFar == 0 )
	{
		for( size_t iDataIndex = 0; iDataIndex < mData.size(); iDataIndex++ )
		{
			mData[ iDataIndex ]->m_Data.setData( 0, 0, 0, false ); //clearing out all the shared memory
			DELETE_SAFELY( mData[ iDataIndex ] );
		}
		pHeader->m_pDataCompressed = mBitWritter.getData();
		pHeader->m_iDataCompressedSize = mBitWritter.getSizeInBytes();
		mBitWritter.setData( 0, 0, 0, false );
	}
	pHeader->m_iCalculatedSavings = mBitWritter.getSizeInBytes();
	printf("Bytes Saved:%i  Total:%i/%i   In A Row:%i/%i\n", iSize - pHeader->m_iDataCompressedSize, iSucceeded, iFailed, iSuccessfulInARow, iFailedInARow );
	return pHeader->m_iDataCompressedSize;

}

size_t _compressBits( byte *pBytes, size_t iSize, bool bOriginalData, ReturnData &mReturnData )
{

	std::vector< std::thread * > m_Threads;
	std::vector< Header * > mHeaders;
	size_t iBestSizeSoFar( 0 );

	int32 iSearchForZero( 0 );
	int32 iBitsPast( 0 );
	int32 iMultCount( 0 );
	for( int32 iSearchForZero = 0; iSearchForZero < 2; iSearchForZero++ )
	{
		//for( int32 iBitsPast = 0; iBitsPast < 4; iBitsPast++ )
		{
			for( int32 iMultCount = 0; iMultCount < 16; iMultCount++ )
			{
				Header *pHeader = new Header();
				pHeader->m_pDataCompressed = NULL;
				pHeader->m_iDataCompressedSize = 0;
				pHeader->m_iCalculatedSavings = 0;
				pHeader->setSearchingForZero( ( iSearchForZero == 0 )?true:false );
				//pHeader->setBitsPast6( iBitsPast );
				pHeader->setSizeMult( iMultCount );
				pHeader->setUncompressToOrignial( bOriginalData );
				std::thread *pThread = new std::thread( _compressDataWithHeader,
														pBytes,
														iSize,
														pHeader,
														iBestSizeSoFar );
				m_Threads.push_back( pThread );						
				mHeaders.push_back( pHeader );
				if( m_Threads.size() > 4 )
				{
					for( uint32 t = 0; t < m_Threads.size(); t++ )
					{
						m_Threads[t]->join();
						DELETE_SAFELY( 	m_Threads[t] );	
					}
					m_Threads.clear();
					for( uint32 t = 0; t < mHeaders.size(); t++ )
					{
						if( mHeaders[t]->m_iCalculatedSavings > iBestSizeSoFar )
						{
							iBestSizeSoFar = mHeaders[t]->m_iCalculatedSavings;
						}
					}
				}	
			}
		}
	}
	for( uint32 t = 0; t < m_Threads.size(); t++ )
	{
		m_Threads[t]->join();
		DELETE_SAFELY( 	m_Threads[t] );	
	}	
	m_Threads.clear();

	Header *pBestHeader( NULL );
	for( uint32 t = 0; t < mHeaders.size(); t++ )
	{
		if( pBestHeader == NULL ||
			( mHeaders[t]->m_iDataCompressedSize != 0 &&
			  pBestHeader->m_iDataCompressedSize > mHeaders[t]->m_iDataCompressedSize ) )
		{
			pBestHeader = mHeaders[t];
		}
	}
	
	mReturnData.mData.setData( pBestHeader->m_pDataCompressed, pBestHeader->m_iDataCompressedSize, pBestHeader->m_iDataCompressedSize * 8, true );
	pBestHeader->m_pDataCompressed = NULL;
	size_t iTotalBytes = mReturnData.mData.getSizeInBytes();// pBestHeader->m_iDataCompressedSize;

	for( uint32 t = 0; t < mHeaders.size(); t++ )
	{
		DELETE_SAFELY( mHeaders[t]->m_pDataCompressed );
		DELETE_SAFELY( mHeaders[t] );

	}


	return iTotalBytes;
}

void _uncompressFile( const std::wstring &strFile,
					  byte *pData,
					  size_t iSize,
					  byte *pDataToTest,
					  size_t iSizeToTest )
{
	CBitWritter mBitReader;
	CBitWritter mBitWritter;
	Header mHeaderDetails;
	mBitReader.setData( pData, iSize, iSize * 8, true );
	mBitWritter.setGrowBy( iSize * 12 );
	mHeaderDetails.m_Data = mBitReader.getByteStartingAt( 0, 0, COUNT_OF_BITS_IN_HEADER );
//	bool bLookingForZero = mHeaderDetails.getBitsPast6
	//size_t iBitsToRead = (size_t)(g_BitsSaving + 1 + mHeaderDetails.getBitsPast6());
	size_t iSizeToReadInBytes = 100 + (size_t)( 100 * mHeaderDetails.getSizeMult() );
	size_t iSizeToReadInBits = iSizeToReadInBytes * 8;
	bool bSearchingForZero = mHeaderDetails.getSearchingForZero();
	
	size_t iBitIndex( COUNT_OF_BITS_IN_HEADER );
	size_t iCount( 0 );
	do 
	{
		//iCount++;
		if( pDataToTest )
		{
			size_t iSizeInBytes( mBitWritter.getSizeInBytes() );
			for( size_t t = 0; t < iSizeInBytes; t++ )
			{
				if( pDataToTest[t] != mBitWritter.getData()[t] )
				{
					printf("broke");
				}
			}
		}
		byte *pDataToLookAt = (byte*)mBitWritter.getData();
		if( mBitReader[ iBitIndex++ ] ) //if it's 1 it's a good save
		{			
			//random values
			byte iIndexOfRandomNumb = mBitReader.getByteStartingAt( iBitIndex, 0, COUNT_OF_BITS_RANDOM_KEYS );			
			iBitIndex += COUNT_OF_BITS_RANDOM_KEYS;
			byte iCountPastBitsSaving = mBitReader.getByteStartingAt( iBitIndex, 0, COUNT_OF_BITS_COUNT_PAST_BITS_SAVING );	
			iBitIndex += COUNT_OF_BITS_COUNT_PAST_BITS_SAVING;
			size_t iRandomNumbers( iCountPastBitsSaving + RANDOM_NUMBER_MIN_COUNT );
					
			Rand mRand( g_RandomKeys[ iIndexOfRandomNumb ] ) ;			
			std::vector< size_t > mChoicesSoFar;
			for( size_t t = 0; t < iRandomNumbers; t++ )
			{
				size_t iIndex( getRandom( mRand, 0, iSizeToReadInBits, mChoicesSoFar ) );
			}
			size_t iSizeToRead = min( iSizeToReadInBits - iRandomNumbers + iBitIndex, mBitReader.size() );
			size_t iBitsWroteSoFar( 0 );
			for( iBitIndex; iBitIndex < iSizeToRead; iBitIndex++ )
			{
				for( size_t i = 0; i < mChoicesSoFar.size(); i++ )
				{
					if( mChoicesSoFar[i] == iBitsWroteSoFar )
					{
						if( i == mChoicesSoFar.size() - 1 )
						{
							mBitWritter.add( (bSearchingForZero)?1:0 );					
						}
						else
						{
							mBitWritter.add( (bSearchingForZero)?0:1 );					
						}
						iBitsWroteSoFar++;
						i = -1; //they could be in a row
					}
				}
				mBitWritter.add( mBitReader[ iBitIndex ] );
				iBitsWroteSoFar++;
			}
			iCount++;
		}
		else //else it's zero so we need to find out how many are in a row now
		{
			//just copy the bits
			
			size_t iCountInARow = 1;//(size_t)mBitReader.getByteStartingAt( iBitIndex, 0, COUNT_IN_A_ROW_FAILED_BITS );
			//iBitIndex += COUNT_IN_A_ROW_FAILED_BITS;
			for( size_t iInARow = 0; iInARow < iCountInARow; iInARow++ )
			{
				size_t iSizeToRead = min( iBitIndex + iSizeToReadInBits, mBitReader.size() );			

				for( iBitIndex; iBitIndex < iSizeToRead; iBitIndex++ )
				{
					mBitWritter.add( mBitReader[ iBitIndex ] );
				}
			}
			iCount += iCountInARow;
		}
		
	} 
	while (iBitIndex < mBitReader.size());
	if( pDataToTest )
	{
		for( size_t t = 0; t < mBitWritter.getSizeInBytes(); t++ )
		{
			if( pDataToTest[t] != mBitWritter.getData()[t] )
			{
				printf("broke");
			}
		}
	}
	CFileBinaryWriter mWritter( GetApplicationPath2() + strFile );
	mWritter.saveValue( mBitWritter.getData(), sizeof( byte ) * (mBitWritter.getSizeInBytes() ) );
	mWritter.close();
	mBitReader.setData( 0, 0, 0, false );
}

void _doFindRandomBits()
{

	CFileBinaryReader mReader( GetApplicationPath2() +  L"TEST.ZIP", true );
	uint32 iBits( 0 );
	mReader.readValue( iBits );
	byte *pBytes = mReader.getBytes();
	size_t iSize = mReader.getNumberOfBytes();
	size_t iSizeToBeat( iSize );
	ReturnData mReturnData;	
	byte *pCompressedData( NULL );
	size_t iTotalBytes = _compressBits( pBytes, iSize, true, mReturnData );
	size_t iCountOfLoops( 1 );
	if( false && iTotalBytes < iSizeToBeat )
	{
		while( iTotalBytes < iSizeToBeat )
		{
			if( pCompressedData )
			{
				DELETE_SAFELY( pCompressedData );
			}
			pCompressedData = mReturnData.mData.getData();
			iSizeToBeat = mReturnData.mData.getSizeInBytes();
			mReturnData.mData.setData( NULL, 0, 0, false );			
			iTotalBytes = _compressBits( pCompressedData, iSizeToBeat, false, mReturnData );
			iCountOfLoops++;
			printf( "\nLoop: %i : Size so far:%i \n", iCountOfLoops, iTotalBytes );
		}
		CFileBinaryWriter mWritter( GetApplicationPath2() + L"TEST.BLA" );
		mWritter.saveValue( pCompressedData, iSizeToBeat );
		mWritter.close();
		_uncompressFile( L"TEST2.ZIP", pCompressedData, iSizeToBeat, NULL, 0 );
	}
	else
	{
		/*CFileBinaryWriter mWritter( GetApplicationPath2() + L"TEST.BLA" );
		mWritter.saveValue( mReturnData.mData.getData(), iTotalBytes );
		mWritter.close();
		_uncompressFile( L"TEST2.ZIP", mReturnData.mData.getData(), iTotalBytes, pBytes, iSize );*/
	}
	DELETE_SAFELY( pCompressedData );
	printf( "\nBytes Saved: %i\n", iSize - iTotalBytes );
	printf( "Total Loops: %i\n", iSize - iTotalBytes );

}

