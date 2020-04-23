#include "KCDataGroupStringParser.h"
#include "KCIncludes.h"
#include "Systems/DataGroup/KCDataGroup.h"
#include <sstream>
#include <iostream>

void _removeFromBack(std::string &strString, std::string strRemove = " ")
{
	const size_t strBegin = strString.find_first_not_of(strRemove);
	if (strBegin != std::string::npos)
	{
		const size_t strEnd = strString.find_last_not_of(strRemove);
		const size_t strRange = strEnd - strBegin + 1;
		strString = strString.substr(strBegin, strRange);
	}
}

void _removeFromFront(std::string &strString, std::string strRemove = " ")
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

void _removeComments(std::string &strString )
{
	if (strString.size() == 0)
	{
		return;
	}	
	if (strString.c_str()[0] == '#')
	{
		strString = "";
		return;
	}
	if (strString.size() > 2 &&
		strString.c_str()[0] == '/' &&
		strString.c_str()[1] == '/')
	{
		strString = "";
		return;
	}

}

void _cleanString(std::string &strString)
{
	
	_removeFromBack(strString, " ");
	_removeFromFront(strString, " ");
	_removeFromFront(strString, "\t");
	_removeComments(strString);
}

bool _getTagName(std::string &strOutputString, const std::string &strString, const std::string &strTagOpen, const std::string &strTagClosed)
{
	strOutputString = "";
	size_t iTagBegin = strString.find_first_of(strTagOpen);
	size_t iTagEnd = strString.find_first_of(strTagClosed);
	KCEnsureAlwaysMsgReturnVal( (iTagBegin >= 0 && iTagEnd >= 0), ("Open or Close Tag(s) not found(" + strTagOpen + strTagClosed + ") in string " + strString).c_str(), false );
	strOutputString = strString.substr(iTagBegin + 1, iTagEnd - iTagBegin - 1);
	return true;
}

std::string _getGroupName(const std::string &strString)
{
	std::string strOutput;
	_getTagName(strOutput, strString, "[", "]");
	return strOutput;
}


bool _parseDataGroupProperty(KCDataGroup &mDataGroupParent, std::string &strPropertyLine)
{
	size_t iTagBegin = strPropertyLine.find_first_of('<');
	size_t iTagEnd = strPropertyLine.find_first_of('>');
	KCEnsureAlwaysMsgReturnVal((iTagBegin >= 0 && iTagEnd >= 0), ("Open or Close Property Tag(s) not found(<...>) in string " + strPropertyLine).c_str(), false);
	std::string strType = strPropertyLine.substr(iTagBegin + 1, iTagEnd - iTagBegin - 1);
	EDATATYPES eType = DATATYPES_UTILS::getDataTypeByDataTypeName(KCStringUtils::toUpper(strType));
	KCEnsureAlwaysMsgReturnVal((eType != EDATATYPES::COUNT), ("Unknown Type " + strType + " found in string " + strPropertyLine).c_str(), false);
	size_t iPropertyStart = iTagEnd - iTagBegin + 1;
	KCEnsureAlwaysMsgReturnVal((iPropertyStart < strPropertyLine.length()), ("Incorrectly formatted property line:" + strPropertyLine).c_str(), false);
	std::string strNameAndValue = strPropertyLine.substr(iPropertyStart, strPropertyLine.length() - iPropertyStart);
	size_t iPropertySplit = strNameAndValue.find_first_of(':');
	KCEnsureAlwaysMsgReturnVal((iPropertySplit > 0), ("Incorrectly formatted property line:" + strNameAndValue).c_str(), false);
	std::string strName = strNameAndValue.substr(0, iPropertySplit);
	KCEnsureAlwaysMsgReturnVal(strName.size() > 0, ("Incorrectly formatted property line:" + strNameAndValue).c_str(), false);
	std::string strValue = strNameAndValue.substr(iPropertySplit + 1, strNameAndValue.length() - iPropertySplit - 1);
	if (strValue.size() == 0)
	{
		if (eType != EDATATYPES::STRING)
		{
			strValue = "0";
		}
	}
	
	KCDataProperty &mProperty = mDataGroupParent.getOrCreateProperty(strName);	
	bool bSetOkay = mProperty.setValueByString(strValue, eType);
	KCEnsureAlwaysMsgReturnVal(bSetOkay, ("Was unable to set property " + strName + " with value " + strValue + ". Complete string is:" + strPropertyLine).c_str(), false);
	return true;
}

bool _parseDataGroup(KCDataGroup &mDataGroupParent, std::stringstream &mStringStream)
{
	std::string strString;
	while (std::getline(mStringStream, strString, '\n'))
	{
		_cleanString(strString);
		if (strString == "") { continue; }
		if (strString.c_str()[0] == '[')
		{
			//we need to create a group
			KCEnsureAlwaysMsgReturnVal( (strString.size() != 1), "String has only single value.", false);
			

			if (strString.c_str()[1] == '/')
			{
				return true;//this is the end tag. End tags can actually just be [/] 
			}
			

			std::string strGroupName = _getGroupName(strString);
			KCEnsureAlwaysMsgReturnVal((strGroupName != ""), "String starts with tag '[' but no end tag found.", false);
			KCDataGroup &mChildDataGroup = mDataGroupParent.getOrCreateChildGroup(strGroupName);			
			_parseDataGroup(mChildDataGroup, mStringStream);
		}
		else if (strString.c_str()[0] == '<') //property
		{
			if (_parseDataGroupProperty(mDataGroupParent, strString) == false)
			{
				return false;
			}
		}
	}
	return true;
}

bool KCDataGroupStringParser::parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	KCTArray<uint8> mArray;
	KCEnsureAlwaysMsgReturnVal(KCFileUtilities::loadFile(strPathAndFile, mArray), "Unable to load file", false);
	std::stringstream   mStringStream((const char*)mArray.getMemory());
	std::string strString;
	while (std::getline(mStringStream, strString, '\n'))
	{
		_cleanString(strString);
		if( strString == "" ){ continue; }
		if (strString.c_str()[0] == '[')
		{
			//we found the first group.

			std::string strGroupName = _getGroupName(strString);
			if (strGroupName == "")
			{
				return false;
			}
			mDataGroup.setGroupName(strGroupName);
			_parseDataGroup(mDataGroup, mStringStream);
		}
	}
	mDataGroup.setProperty("_FILE_", strPathAndFile);
	return true;
}
