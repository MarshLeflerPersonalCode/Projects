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
#include "FileBinaryWriter.h"

CFileBinaryWriter::CFileBinaryWriter( const std::wstring &fileAndPath,
									  int32 iSaveFlags ) :
					m_pFile( NULL )
{
	m_FileName = fileAndPath;
	m_FilePath = fileAndPath.substr( 0, fileAndPath.size() - m_FileName.size() );
	//::DeleteFileW( fileAndPath.c_str() );
	m_pFile = new std::fstream(fileAndPath.c_str(), std::fstream::trunc | iSaveFlags | std::fstream::out | std::fstream::in );	
	if( m_pFile->is_open() == false )
	{
		delete m_pFile;
		m_pFile = NULL;

		std::ofstream mStream( fileAndPath.c_str(), std::ofstream::out | std::ofstream::binary );
		mStream.close();

		m_pFile = new std::fstream(fileAndPath.c_str(), std::fstream::trunc | iSaveFlags | std::fstream::out | std::fstream::in );	
		if( m_pFile->is_open() == false )
		{
			delete m_pFile;
			m_pFile = NULL;
		}

	}
}

CFileBinaryWriter::~CFileBinaryWriter(void)
{
	if( m_pFile )
	{
		close();
	}	
}

//!closes the file - NOTE if the fileBinaryWriter loses scope it'll close the file too
void CFileBinaryWriter::close( void )
{
	m_FileMemoryLocations.clear();
	if( m_pFile )
	{
		
		m_pFile->close();
		delete m_pFile;
		m_pFile = NULL;
	}
}

//!saves a bit of data
void CFileBinaryWriter::saveValue( void *pValue,				//data
								   size_t iSize,				//size of data
								   const std::string &name )	//name to write to later
{
	ASSERT_RETURN( m_pFile && pValue && iSize );
	if( name.empty() == false )
	{
		m_FileMemoryLocations[ name ] = std::pair< size_t, size_t >( (size_t)m_pFile->tellp(), iSize );
	}
	m_pFile->write( (char * )pValue, iSize );
}

//!rewrites the data to a given location
void CFileBinaryWriter::rewriteValue( const std::string &name,
									  void *pValue,
									  size_t iSize )
{
	ASSERT_RETURN( m_pFile && pValue && iSize );
	std::map< std::string, std::pair< size_t, size_t > >::iterator iter = m_FileMemoryLocations.find( name );
	//make sure we saved the location to write
	ASSERT_RETURN( iter != m_FileMemoryLocations.end() )

	//check the pair value for the size and make sure they 
	ASSERT_RETURN( iSize == iter->second.second );

	m_pFile->seekp( iter->second.first,
					std::fstream::beg );	//go to the place to write the data
	
	saveValue( pValue,		//save the data
			   iSize );

	m_pFile->seekp( 0, 
					std::fstream::end );						//go back to the end

}

//!saves a string
void CFileBinaryWriter::saveString( const std::wstring &stringToSave )			//value to save
{
	uint16 iSize( (uint16)stringToSave.size() );
	saveValue( iSize );
	if( iSize > 0 )
	{
		saveValue( (void *)stringToSave.c_str(),
				   sizeof( wchar16 ) * stringToSave.size() );
	}
}

//!returns the hash of the file	
size_t CFileBinaryWriter::getHashOfMemory( size_t iStep )
{
	size_t iStepOfBytes = (size_t)getBytesWritten()/iStep;
	if( iStepOfBytes == 0 )
	{
		iStepOfBytes = (size_t)getBytesWritten()/2;
	}
	size_t iCurrentSize = (size_t)getBytesWritten();
	size_t h = 8234;
	for( size_t t = 0; t < iCurrentSize; t = t + iStep )
	{		
		m_pFile->seekg( t, std::fstream::beg );
		char mByte(0);
		m_pFile->read( &mByte, 1 );
		h = ((h << 5) + h) + (size_t)mByte;	
	}
	m_pFile->seekg( 0, std::fstream::end );
	return h;
}
