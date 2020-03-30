// SimpleRandomInsert.cpp : main project file.

#include "stdafx.h"
#include "FileIO/FileBinaryReader.h"
#include "FileIO/FileBinaryWriter.h"
#include "BitWritter.h"
#include "GameCompression.h"
enum EDATA_TYPE
{
	KDATA_TYPE_NO_SAVINGS,
	KDATA_TYPE_RANDOM
};



struct mData
{
	EDATA_TYPE m_eType;
	UNIONDATA64BIT m_Data;
};


static TThreadSafeArrayList< mData > g_Data( 1000 );
static TThreadSafeArrayList< std::thread *> g_Threads;
static TThreadSafeArrayList< uint32 > g_Keys[ 16 ];
static uint32 g_iSucceededCount( 0 );




//returns the directory path
std::wstring GetApplicationPath( void )
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

void testSub( uint32 iIndexIntoData, byte *pArray, uint32 iStart, uint32 iEnd )
{
	Rand mRand(0);
	int32 iRandomValue( 0 );
	bool bSucceeded( false );
	uint32 iIndexToUse( pArray[iStart] & 0xF );
	for( uint32 iRandomValue = 0; iRandomValue < g_Keys[ iIndexToUse ].size(); iRandomValue++ )
	{
		mRand.setState( g_Keys[ iIndexToUse ][iRandomValue] + 1 );
		bool bSucceeded( true );
		for ( uint32 t = iStart; t < iEnd; t++ )
		{
			int32 iValue =  mRand.Int32( 0, 15 );
			int32 iByteValueLow = (int32)(pArray[t] & 0xF);
			int32 iByteValueHigh = (int32)(pArray[t] >> 4);
			if( abs( iValue -iByteValueLow ) != 0 )//> 4 )
			{
				bSucceeded = false;
				break;
			}
			iValue =  mRand.Int32( 0, 15 );
			if( abs( iValue -iByteValueHigh ) != 0 )//> 4 )
			{
				bSucceeded = false;
				break;
			}
		}
		if( bSucceeded )
		{
			mData mDataToSave;
			mDataToSave.m_eType = KDATA_TYPE_RANDOM;
			mDataToSave.m_Data.m_iValue[0] = iRandomValue;			
			g_Data.set( iIndexIntoData, mDataToSave );
			g_iSucceededCount++;
			return;
		}
	}

	mData mDataToSave;
	mDataToSave.m_eType = KDATA_TYPE_NO_SAVINGS;
	for( uint32 t = 0; t < iEnd - iStart; t++ )
	{
		mDataToSave.m_Data.m_Byte[t] = pArray[iStart + t];
	}
	g_Data.set( iIndexIntoData, mDataToSave );

	
	
}
void fireThread( uint32 iIndexIntoData, byte *pBytes, int32 iStart, int32 iEnd )
{
	

	std::thread  *threadIt = new std::thread( testSub, 
											   iIndexIntoData, 
											   pBytes,
											   iStart,
											   iEnd );

	g_Threads.add( threadIt );
}
void _saveData( std::wstring strFile )
{
	
	CBitWritter mBitWritter( g_Data.size() * 8 );
	for( uint32 t = 0; t < g_Data.size(); t++ )
	{
		mData mDataToSave = g_Data[t];
		mBitWritter.add( (mDataToSave.m_eType != KDATA_TYPE_NO_SAVINGS )?true:false );
		switch( mDataToSave.m_eType )
		{
		case KDATA_TYPE_NO_SAVINGS:
			mBitWritter.add( false );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[0] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[1] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[2] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[3] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[4] );
			break;
		case KDATA_TYPE_RANDOM:
			mBitWritter.add( true );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[0] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[1] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[2] );
			mBitWritter.copyByte( mDataToSave.m_Data.m_Byte[3] );
			break;
		}
	}
	
	CFileBinaryWriter gFileWriter( GetApplicationPath() +  strFile );
	if( gFileWriter.isOpen() )
	{
		gFileWriter.saveValue( mBitWritter.size() );
		size_t iSizeInBytes = mBitWritter.getSizeInBytes();
		gFileWriter.saveValue( mBitWritter.getData(), iSizeInBytes  );
		gFileWriter.close();
	}
}

void _uncompressData( const std::wstring &strFileToUncompress,
					  const std::wstring &strSaveAsFile )
{
	CFileBinaryReader mReader( GetApplicationPath() +  strFileToUncompress, false );
	uint32 iBits( 0 );
	mReader.readValue( iBits );
	byte *pBytes = mReader.getBytes();
	size_t iSize = mReader.getNumberOfBytes();
	CFileBinaryWriter mWritter( GetApplicationPath() +  strSaveAsFile, false );
	CBitWritter mBitWritter;
	mBitWritter.setData( &pBytes[4], iSize - 4, iBits ); //skip the count at the beginning
	for( uint32 iBitIndex = 0; iBitIndex < iBits; iBitIndex++ )
	{
		EDATA_TYPE eData = (mBitWritter[ iBitIndex ])?KDATA_TYPE_RANDOM:KDATA_TYPE_NO_SAVINGS;
		switch( eData )
		{
		case KDATA_TYPE_NO_SAVINGS:
			for( uint32 t = 0; t < 5; t++ )
			{
				byte mByte(0);
				mBitWritter.setByte( iBitIndex, mByte );
				iBitIndex++;
				mWritter.saveValue( mByte );
			}
			break;
		case KDATA_TYPE_RANDOM:
			
			for( uint32 t = 0; t < 5; t++ )
			{
				unionData32Bit mByteData;
				for( uint32 i = 0; i < 4; i++ )
				{					
					mBitWritter.setByte( iBitIndex, (byte&)mByteData.m_iValue8[i] );
					iBitIndex++;					
				}
				Rand mRand( (uint32)mByteData.m_iValue );
				for( uint32 t = 0; t < 5; t++ )
				{
					int32 iValueLowBits =  mRand.Int32( 0, 15 );
					int32 iValueHighBits =  mRand.Int32( 0, 15 );
					byte mByte = (byte)(( iValueHighBits << 4 ) | ( iValueLowBits & 0xF ));
					mWritter.saveValue( mByte );
				}
			}
			break;
		}
	}
	
}

void computeKeys( uint32 iStart, uint32 iEnd )
{

	for( uint32 t = iStart; t < iEnd; t++ )
	{
		Rand mRand( t );
		if( t % 10000 == 0)
		{
			printf( ".");
		}
		byte mByte = mRand.Int32( 0, 15 );
		g_Keys[ mByte ].setGrowBy( 500000 );
		g_Keys[ mByte ].add( t );
	}
	printf( "***");
	
}

void randomInsert()
{
printf( "Computing keys");
	uint32 iMaxValue( 0xFFFF );
	uint32 iDevideBy( iMaxValue/8 );
	uint32 iTotalSoFar( 0 );
	for( uint32 t = 0; t < 8; t++ )
	{
		std::thread  *threadIt = new std::thread( computeKeys, 
												  iTotalSoFar,
												  iDevideBy + iTotalSoFar );
		iTotalSoFar += iDevideBy;

		g_Threads.add( threadIt );		
	}
	//get any remainder
	computeKeys( iTotalSoFar, iMaxValue );

	for( uint32 t = 0; t < g_Threads.size(); t++ )
	{
		g_Threads[t]->join();
	}	
	g_Threads.deleteAll();
	printf( "Done keys\n");
	std::wstring strFilePaths[11] = { L"TEST.ZIP",
									  L"TEST_RANOMDPACK.RND",
									  L"TEST_RANOMDPACK2.RND",
									  L"TEST_RANOMDPACK3.RND",
									  L"TEST_RANOMDPACK4.RND",
									  L"TEST_RANOMDPACK5.RND",
									  L"TEST_RANOMDPACK6.RND",
									  L"TEST_RANOMDPACK7.RND",
									  L"TEST_RANOMDPACK8.RND",
									  L"TEST_RANOMDPACK9.RND",
									  L"TEST_RANOMDPACK10.RND" };
	//_uncompressData( strFilePaths[ 1 ], L"test2.zip" );
	
	for( uint32 iFilePath = 0; iFilePath < 1; iFilePath++ )
	{
		CFileBinaryReader mReader( GetApplicationPath() +  strFilePaths[ iFilePath ] );
		byte *pBytes = mReader.getBytes();
		size_t iSize = mReader.getNumberOfBytes();
		g_iSucceededCount = 0;
		g_Data.clear( false );
		for( uint32 t = 0; t < iSize; t++ )
		{
			g_Data.add( mData() );
		}
		uint32 iBytesStart(0);
		uint32 iIndexIntoData( 0 );
		for( uint32 t = 0; t < iSize; t = t + 3 )
		{
		
			fireThread( iIndexIntoData, pBytes, t, t + 3 );// t, iSize, g_iCountOfValues[t], 0, 10000 );		
			iIndexIntoData++;
			if( g_Threads.size() > 6 )
			{
				for( uint32 i = 0; i < g_Threads.size(); i++ )
				{
					g_Threads[i]->join();
				}
				printf( "DONE.\n");
				g_Threads.deleteAll();
				printf( "Parsing bytes( %d-%d)/%d ...",iBytesStart,t,iSize);
				iBytesStart = t;
			}
		}
		int32 iBlocks( (int32)iSize/10 + 1 );
		int32 iBlocksBitValue( (int32)iSize );
		int32 iBitsSaved = ((int32)g_iSucceededCount * 8) - iBlocksBitValue;
		int32 iBytesSaved = iBitsSaved/8 - 1;
		int32 iTotalBytes = iBytesSaved;//iBytesSaved - ( iBlocks + iBlocksBitValue );
		printf("Bytes saved:%d\n", iTotalBytes );
		printf("Percent Saved:%1.5f\n", 1.0f - (float32)iTotalBytes/(float32)iSize );

		for( uint32 t = 0; t < g_Threads.size(); t++ )
		{
			g_Threads[t]->join();
		}
	
		g_Threads.deleteAll();

	
		_saveData( strFilePaths[ iFilePath + 1 ] );
	}
	//_uncompressData( strFilePaths[ 1 ], L"test2.zip" );
	
}



int main()
{
	std::wstring strFilePaths[11] = { L"TEST.ZIP",
									  L"TEST_RANOMDPACK.RND",
									  L"TEST_RANOMDPACK2.RND",
									  L"TEST_RANOMDPACK3.RND",
									  L"TEST_RANOMDPACK4.RND",
									  L"TEST_RANOMDPACK5.RND",
									  L"TEST_RANOMDPACK6.RND",
									  L"TEST_RANOMDPACK7.RND",
									  L"TEST_RANOMDPACK8.RND",
									  L"TEST_RANOMDPACK9.RND",
									  L"TEST_RANOMDPACK10.RND" };
	//_uncompressData( strFilePaths[ 1 ], L"test2.zip" );
	//_doGameCompression();
	_doFindRandomBits();
	/*
	for( uint32 iFilePath = 0; iFilePath < 1; iFilePath++ )
	{
		CFileBinaryReader mReader( GetApplicationPath() +  strFilePaths[ iFilePath ] );
		byte *pBytes = mReader.getBytes();
		uint32 iSize = mReader.getNumberOfBytes();
		CBitWritter mBitWritter;
		mBitWritter.setData( pBytes, iSize, iSize * sizeof( byte ) * 8 );
		for( uint32 t = 0; t < 0xFF; t++ )
		{
			uint32 iCount_0( 0 );
			bool bCount0Good( true );
			uint32 iCount_1( 0 );
			bool bCount1Good( true );
			Rand mZeroRand;
			Rand mOneRand;
			while( bCount0Good && bCount1Good )
			{

			}
		}
		mBitWritter.setData( NULL, 0, 0, false );

	}
	*/
	system("pause");
    return 0;
}
