//copyright Marsh Lefler 2000-...
//bunch of string functions for helping parse strings
#include "KCStringUtils.h"
#include "Utils/TypeDefinitions/KCDataTypes.h"
#include <codecvt>
#include <sstream>


KCString KCStringUtils::converWideToUtf8(const WCHAR *pStringToConvert)
{
	if (pStringToConvert == nullptr)
	{
		return "";
	}
	return converWideToUtf8(std::wstring(pStringToConvert));
	
}

bool KCStringUtils::isNumber(const std::string &strStringToTest)
{
	if (strStringToTest.empty())
	{
		return false;
	}
	auto mStartInter = strStringToTest.begin();
	auto mEndIter = strStringToTest.end();
	if (strStringToTest.c_str()[0] == '-')
	{
		mStartInter++;
	}
	if (strStringToTest.c_str()[strStringToTest.length() - 1] == 'f')
	{
		mEndIter--;
	}
	if (mStartInter == mEndIter)
	{
		return false;
	}
	return std::find_if(mStartInter, mEndIter, [](unsigned char c) {
		return c != '.' && !std::isdigit(c); 
	}) == mEndIter;
}


KCString KCStringUtils::converWideToUtf8(const std::wstring &strStringToConvert)
{
	//setup converter
	using convert_type = std::codecvt_utf8<WCHAR>;		
	return std::wstring_convert<convert_type, WCHAR>().to_bytes(strStringToConvert);
}

void KCStringUtils::removeCharactersFromEndOfString(std::string &strString, const std::string &strRemove /*= " "*/)
{
	const size_t strBegin = strString.find_first_not_of(strRemove);
	if (strBegin != std::string::npos)
	{
		const size_t strEnd = strString.find_last_not_of(strRemove);
		const size_t strRange = strEnd - strBegin + 1;
		strString = strString.substr(strBegin, strRange);
	}
}

void KCStringUtils::removeCharactersFromFrontOfString(std::string &strString, const std::string &strRemove /*= " "*/)
{
	auto beginSpace = strString.find_first_of(strRemove);
	while (beginSpace != std::string::npos)
	{
		const size_t endSpace = strString.find_first_not_of(strRemove, beginSpace);
		const size_t range = endSpace - beginSpace;

		strString.replace(beginSpace, range, "");

		const size_t newStart = beginSpace;
		beginSpace = strString.find_first_of(strRemove, newStart);
	}
}


bool KCStringUtils::chopEndOffOfString(std::string &strStringModifying, const std::string &strTokenToSearchFor, bool bIgnoreCase )
{
	if (strStringModifying.length() == 0)
	{
		return false;
	}
	size_t iTokenIndex(INVALID);
	if (bIgnoreCase)
	{
		std::string strInputUpper = toUpperNewString(strStringModifying);
		std::string strTokenUpper = toUpperNewString(strTokenToSearchFor);
		iTokenIndex = strInputUpper.rfind(strTokenUpper);
	}
	else
	{
		iTokenIndex = strStringModifying.rfind(strTokenToSearchFor);
	}
	if (iTokenIndex == std::string::npos)
	{		
		return false;
	}
	strStringModifying.resize(iTokenIndex);
	return true;
}

bool KCStringUtils::getTagName(std::string &strOutputString, const std::string &strStringSearching, const std::string &strTagOpen, const std::string &strTagClosed)
{
	strOutputString = "";
	size_t iTagBegin = strStringSearching.find_first_of(strTagOpen);
	size_t iTagEnd = strStringSearching.find_first_of(strTagClosed);
	if (iTagBegin < 0 || iTagEnd < 0)
	{
		return false;
	}
	strOutputString = strStringSearching.substr(iTagBegin + 1, iTagEnd - iTagBegin - 1);
	return true;
}

std::string KCStringUtils::getAsString(const WCHAR *pStringToConvert)
{
	return converWideToUtf8(pStringToConvert);	
}

std::string KCStringUtils::getAsString(bool bValue)
{
	if (bValue)
	{
		return "true";
	}
	return "false";
}
