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
//	CFileBinaryReader.cpp - reads a binary file
//////////////////////////////////////////////////////////////////////////
#pragma once

class CFileBinaryReader
{
public:
	CFileBinaryReader( const std::wstring &fileAndPath,					   
					   bool bFreeMemory = true );

	~CFileBinaryReader(void);


	//!reads a string
	void		readString( std::wstring &stringToFill);			//value to save

	//!reads a bit of data
	void		readValue( void *pValue,									//data
						   size_t iSize );									//size of data



	//!reads data out of the file
	template< class T> void readValue( T &value )
	{
		readValue( (void *)&value,
				   sizeof( T ) );
	}


	//!returns the file name
	inline const std::wstring &	getFileName( void )				{	return m_FileName;	}
	//!returns the path
	inline const std::wstring &	getFilePath( void )				{	return m_FilePath;	}
	//!returns the path with file name
	inline std::wstring			getFilePathAndName( void )		{	return m_FilePath + m_FileName;	}

	//!returns if the file was read ok
	virtual bool			openedFile( void )				{	return ( m_iBytesTotal != 0 )?true:false;	} 

	//!returns the current location of the file
	inline size_t					tellg( void )					{	return m_iFileIndex;	}

	//!seeks to a given location
	inline void					seek( size_t iFileLocation )	{	m_iFileIndex = min( iFileLocation, m_iBytesTotal );	}

	//!returns the bytes of the file
	inline byte *					getBytes( void )				{	return m_pFileBuffer;	}
	//!returns the total file size
	inline size_t					getNumberOfBytes( void )		{	return m_iBytesTotal;	}

	//!returns the total number of bytes read past the end of the file
	inline size_t					getNumberOfBytesReadPastEOF( void )	{	return m_iBytesReadPastEnd;	}

protected:
	//!reads in the file
	virtual void			openFile( );

	byte*					m_pFileBuffer;
	size_t					m_iBytesTotal;
	size_t					m_iFileIndex;
	

	std::wstring			m_FileName;
	std::wstring			m_FilePath;

	bool					m_bFreeMemory;

	size_t					m_iBytesReadPastEnd;

	
};

