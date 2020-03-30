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
#pragma once
#include <fstream>
#include <map>


class CFileBinaryWriter
{
public:
	CFileBinaryWriter( const std::wstring &fileAndPath,
					   int32 iSaveFlags = std::fstream::binary );
	~CFileBinaryWriter(void);


	//!closes the file - NOTE if the fileBinaryWriter loses scope it'll close the file too
	void		close( void );

	//!saves a string
	void		saveString( const std::wstring &stringToSave );			//value to save
	//!saves a bit of data
	void		saveValue( void *pValue,								//data
						   size_t iSize,								//size of data
						   const std::string &name = EMPTY_STRING );	//name to write to later

	//!rewrites the data to a given location
	void		rewriteValue( const std::string &name,
							  void *pValue,
							  size_t iSize );



	//!writes out the data to the file
	template< class T> void saveValue( T const &value,
									   const std::string &name = EMPTY_STRING )
	{
		saveValue( (void *)&value,
			sizeof( T ),
			name );
	}


	//!rewrites the data to a given location
	template <class T> void rewriteValue( const std::string &name,
										  T const &value )
	{
		rewriteValue( name,
					  (void *)&value,
					  sizeof( T ));
	}
	//!returns the file name
	const std::wstring & getFileName( void )		{	return m_FileName;	}
	//!returns the path
	const std::wstring & getFilePath( void )		{	return m_FilePath;	}
	//!returns the path with file name
	std::wstring		getFilePathAndName( void )	{	return m_FilePath + m_FileName;	}

	//!returns if the file is open
	inline bool			isOpen( void )				{	return ( m_pFile != NULL )?true:false;	} 

	//!returns the current location of the file
	inline size_t			getBytesWritten( void )		{	return (size_t)m_pFile->tellp();	}

	//!returns the hash of the file	
	size_t				getHashOfMemory( size_t iStep = 10 );
	
private:

	std::fstream		*m_pFile;

	std::wstring		m_FileName;
	std::wstring		m_FilePath;


	std::map< std::string, std::pair< size_t, size_t > >		m_FileMemoryLocations;
};

