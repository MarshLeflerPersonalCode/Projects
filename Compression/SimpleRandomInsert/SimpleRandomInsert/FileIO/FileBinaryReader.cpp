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
//	CFileBinaryWriter.cpp - writes a binary file
//////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "FileBinaryReader.h"
#include <fstream>


CFileBinaryReader::CFileBinaryReader( const std::wstring &fileAndPath,									  
									  bool bFreeMemory ) :
					m_bFreeMemory( bFreeMemory ),
					m_pFileBuffer( NULL ),
					m_iBytesReadPastEnd( 0 ),
					m_iBytesTotal( 0 ),
					m_iFileIndex( 0 )
{
	m_FileName = fileAndPath;
	//m_FilePath = fileAndPath.substr( 0, fileAndPath.size() - m_FileName.size() );
	//PERF_TRACKER( Loading BINARY FILE );
	//AUTOPERF( "BINARY FILE:"  )//+ STRINGS::StringConvertToNarrow(fileAndPath.c_str())).c_str() );
	openFile();
}

CFileBinaryReader::~CFileBinaryReader(void)
{
	
	if( m_pFileBuffer )
	{
		DELETE_ARRAY_SAFELY( m_pFileBuffer );
	}	

	m_pFileBuffer = NULL;
}

//!reads in the file
void CFileBinaryReader::openFile( void )
{

	m_iFileIndex = 0;

	std::ifstream mFile( m_FileName.c_str(), std::ifstream::in | std::ifstream::binary);	
	if( mFile.is_open() == false )
	{
		return;
	}

	mFile.seekg( 0, std::ifstream::end );
	
	m_iBytesTotal = (size_t)mFile.tellg();
	m_iFileIndex = 0;
	
	mFile.seekg( 0, std::ifstream::beg );

	if( m_iBytesTotal == 0 )
	{
		return;
	}
	DELETE_ARRAY_SAFELY( m_pFileBuffer );
	m_pFileBuffer = new byte[m_iBytesTotal];// (byte *)OGRE_MALLOC( sizeof( char ) * m_iBytesTotal, MEMCATEGORY_GENERAL );
	mFile.read( (char*)m_pFileBuffer, sizeof( char ) * m_iBytesTotal );

	mFile.close();
	
}

//!reads a bit of data
void CFileBinaryReader::readValue( void *pValue,				//data
								   size_t iSize )				//size of data
								  
{
	if( pValue == NULL ||
		iSize == 0 ||
		m_pFileBuffer == NULL )
	{
		ASSERT_RETURN( false );
	}
	if( iSize + m_iFileIndex > m_iBytesTotal )
	{

		m_iBytesReadPastEnd += iSize;
		ASSERT_RETURN( false );
	}
	/*
	ASSERT_RETURN( pValue && 
				   m_pFileBuffer &&
				   iSize &&
				   iSize + m_iFileIndex <= m_iBytesTotal );
				   */
	
	memcpy_s( pValue, iSize, &m_pFileBuffer[ m_iFileIndex ], iSize );

	m_iFileIndex += iSize;
	
}


//!saves a string
void CFileBinaryReader::readString( std::wstring &stringToFill )			//value to save
{
	uint16 iSize( (uint16)stringToFill.size() );
	readValue( iSize );
	if( iSize == 0 )
	{
		stringToFill = EMPTY_WSTRING;
		return;
	}
	static wchar16 iCharactersToReadFileLength[ 256 ];
	wchar16 *pCharactersToRead = &iCharactersToReadFileLength[0];
	bool bDeleteString( false );
	if( iSize > 256 - 1 )
	{
		bDeleteString = true;
		pCharactersToRead = new wchar16[ iSize + 1 ];
	}

	readValue( pCharactersToRead, sizeof( wchar16 ) * iSize );
	pCharactersToRead[ iSize ] = '\0';
	stringToFill = pCharactersToRead;

	if( bDeleteString )
	{
		DELETE_ARRAY_SAFELY( pCharactersToRead );
	}
}

