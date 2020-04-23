#pragma once
//copyright Marsh Lefler 2000-...
//bunch of string functions for helping parse strings

#include "KCDefines.h"
#include <algorithm>

namespace KCStringUtils
{
	//replaces all the char cLookFor with cReplaceWith. 
	void				replace(std::string &strString, char cLookFor, char cReplaceWith);

	//converts a string to upper
	FORCEINLINE std::string & toUpper( std::string &strToUpper){ transform(strToUpper.begin(), strToUpper.end(), strToUpper.begin(), ::toupper);  return strToUpper;}
	//converts a string to upper making a new string
	FORCEINLINE std::string toUpperNewString(std::string strToUpper) { transform(strToUpper.begin(), strToUpper.end(), strToUpper.begin(), ::toupper);  return strToUpper; }
	//converts a string to lower
	FORCEINLINE std::string & toLower(std::string &strToLower) { transform(strToLower.begin(), strToLower.end(), strToLower.begin(), ::tolower);  return strToLower; }
	//converts a string to lower making a new string
	FORCEINLINE std::string toLowerNewString(std::string strToLower) { transform(strToLower.begin(), strToLower.end(), strToLower.begin(), ::tolower);  return strToLower; }

	//converts a wide to a char
	std::wstring		toWide(const std::string &strString);

	//converts a WChar to a String. 
	KCString			convertWideToUtf8(const WCHAR *pStringToConvert);
	FORCEINLINE KCString toNarrowUtf8(const WCHAR *pStringToConvert){ return convertWideToUtf8(pStringToConvert); }
	//converts a wstring to a String. 
	KCString			convertWideStringToUtf8(const std::wstring &strStringToConvert);

	//returns if it's a number. Supports 1.0f as well.
	bool				isNumber(const std::string &strStringToTest);

	//just as it says. Removes characters/string from the end of a string
	void				removeCharactersFromEndOfString(std::string &strString, 
														const std::string &strRemove = " ");
	//just as it says. Removes characters/string from the start of a string
	void				removeCharactersFromFrontOfString(std::string &strString, const std::string &strRemove = " ");

	//Keeps the front of the string. returns true if it did anything to the strStringModifying.
	bool				chopEndOffOfString( std::string &strStringModifying,
										    const std::string &strTokenToSearchFor,
										    bool bIgnoreCase = false );

	//returns a string between two strings.
	bool				getTagName(std::string &strOutputString, 
								   const std::string &strStringSearching, 
								   const std::string &strTagOpen, 
								   const std::string &strTagClosed);

	FORCEINLINE std::string		getAsString(int64 iValue){ return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(uint64 iValue) { return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(int32 iValue) { return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(uint32 iValue) { return std::to_string(iValue); }
	std::string 				getAsString(bool bValue);
	FORCEINLINE std::string 	getAsString(int8 iValue) { return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(uint8 iValue) { return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(int16 iValue) { return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(uint16 iValue) { return std::to_string(iValue); }
	FORCEINLINE std::string 	getAsString(float fValue) { return std::to_string(fValue); }
	FORCEINLINE std::string		getAsString(double dValue) { return std::to_string(dValue); }
	std::string 				getAsString(const WCHAR *pStringToConvert);

};	//end namespace