#pragma once
//copyright Marsh Lefler 2000-...
//bunch of string functions for helping parse strings
#include "KCDefines.h"


namespace KCStringUtils
{

	//converts a WChar to a String. 
	KCString			converWideToUtf8(const WCHAR *pStringToConvert);
	//converts a wstring to a String. 
	KCString			converWideToUtf8(const std::wstring &strStringToConvert);

	//just as it says. Removes characters/string from the end of a string
	void				removeCharactersFromEndOfString(std::string &strString, 
														const std::string &strRemove = " ");
	//just as it says. Removes characters/string from the start of a string
	void				removeCharactersFromFrontOfString(std::string &strString, const std::string &strRemove = " ");

	//returns a string between two strings.
	bool				getTagName(std::string &strOutputString, 
								   const std::string &strStringSearching, 
								   const std::string &strTagOpen, 
								   const std::string &strTagClosed);

};	//end namespace