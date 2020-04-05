#pragma once
//copyright Marsh Lefler 2000-...
//bunch of string functions for helping parse strings

#include "KCDefines.h"
#include <algorithm>

namespace KCStringUtils
{
	//converts a string to upper
	FORCEINLINE std::string & toUpper( std::string &strToUpper){ transform(strToUpper.begin(), strToUpper.end(), strToUpper.begin(), ::toupper);  return strToUpper;}
	//converts a string to upper making a new string
	FORCEINLINE std::string toUpperNewString(std::string strToUpper) { transform(strToUpper.begin(), strToUpper.end(), strToUpper.begin(), ::toupper);  return strToUpper; }
	//converts a string to lower
	FORCEINLINE std::string & toLower(std::string &strToLower) { transform(strToLower.begin(), strToLower.end(), strToLower.begin(), ::tolower);  return strToLower; }
	//converts a string to lower making a new string
	FORCEINLINE std::string toLowerNewString(std::string strToLower) { transform(strToLower.begin(), strToLower.end(), strToLower.begin(), ::tolower);  return strToLower; }

	//converts a WChar to a String. 
	KCString			converWideToUtf8(const WCHAR *pStringToConvert);
	FORCEINLINE KCString toNarrowUtf8(const WCHAR *pStringToConvert){ return converWideToUtf8(pStringToConvert); }
	//converts a wstring to a String. 
	KCString			converWideToUtf8(const std::wstring &strStringToConvert);

	//returns if it's a number. Supports 1.0f as well.
	bool				isNumber(const std::string &strStringToTest);

	//will return STRING, INT64, INT32, FLOAT, BOOL from
	std::string			getVariableType(const std::string &strValue);

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

};	//end namespace