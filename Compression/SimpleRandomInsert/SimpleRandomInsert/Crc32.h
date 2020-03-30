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
// This code was taken from http://www.codeguru.com/cpp/cpp/algorithms/checksum/article.php/c5103
// Free of charge
//////////////////////////////////////////////////////////////////////////
#pragma once
#ifndef _CRC32STATIC_H_
#define _CRC32STATIC_H_


class CCrc32Static
{
public:
	CCrc32Static();
	virtual ~CCrc32Static();
	
	//static uint32 createChecksumFromFile(LPCTSTR szFilename, uint32 &dwCrc32);
	static uint32 createChecksumFromBytes( const byte *pBytes, DWORD iBytes, uint32 &dwCrc32);

	static uint32 getHash32( const void *key, int len, uint32 seed = 43342 );

protected:
	//static bool GetFileSizeQW(const HANDLE hFile, int64 &qwSize);
	static inline void CalcCrc32(const BYTE byte, uint32 &dwCrc32);

	static uint32 s_arrdwCrc32Table[256];
};

#endif
