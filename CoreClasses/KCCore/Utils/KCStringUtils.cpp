//copyright Marsh Lefler 2000-...
//bunch of string functions for helping parse strings
#include "KCStringUtils.h"
#include <codecvt>
#include <sstream>


KCString KCStringUtils::converWideToUtf8(const WCHAR *pStringToConvert)
{
	
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

std::string KCStringUtils::getVariableType(const std::string &strValue)
{
	if( strValue.empty()){return "STRING"; }
	if (isNumber(strValue) == false)
	{
		if (strValue.length() == 4 || strValue.length() == 5)
		{
			if (strValue.c_str()[0] == 't' || strValue.c_str()[0] == 'T' ||
				strValue.c_str()[0] == 'f' || strValue.c_str()[0] == 'F')
			{
				KCString strValueUpper = KCStringUtils::toUpperNewString(strValue);
				if (strValue == "TRUE" ||
					strValue == "FALSE")
				{
					return "BOOL";
				}
			}
		}
		return "STRING";
	}

	if (strValue.size() == 1 &&
		strValue.c_str()[0] == '0')
	{
		return "INT32";
	}
	bool isFloat = std::find_if(strValue.begin(), strValue.end(), [](unsigned char c) { return c == '.'; }) != strValue.end();
	if (isFloat)
	{
		return "FLOAT";
	}
	std::stringstream mStream(strValue);		
	int64 iInt64Value(0);
	mStream >> iInt64Value;
		
	if (mStream.fail() == false)
	{
		if (iInt64Value >= INT_MIN && iInt64Value <= INT_MAX)
		{
			return "INT32";
		}
		else if (iInt64Value >= 0 && iInt64Value <= UINT_MAX)
		{
			return "UINT32";
		}
		return "INT64";
	}
	else if( strValue.c_str()[0] != '-')
	{
		std::stringstream mStream64UintTest(strValue);
		mStream64UintTest << strValue;
		uint64 iuInt64Value(0);
		mStream64UintTest >> iuInt64Value;
		if (mStream64UintTest.fail() == false)
		{
			//has to be a uint64
			return "UINT64";
		}
	}

	return "UNKNOWN";
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
