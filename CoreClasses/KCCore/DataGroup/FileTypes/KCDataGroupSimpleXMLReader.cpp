#include "KCDataGroupSimpleXMLReader.h"
#include "IO/KCMemoryReader.h"
#include "IO/KCFileUtilities.h"
#include "DataGroup/KCDataGroup.h"
#include <sstream>
#include <iostream>


struct KCXMLParserData
{
	KCCharReader			m_CharReader;
		
};


struct KCXMLTag
{
	bool							m_bValid = false;
	KCString						m_strTagName;
	KCString						m_strInnterText;
	std::map<KCString, KCString>	m_Attributes;
};


bool _findAttributes(KCXMLParserData &mData, KCXMLTag &mTag)
{
	
	//the end of the tag I'm in
	size_t iIndexOfEndTag = mData.m_CharReader.findIndexOfNextMemoryValue('>', false);
	if( iIndexOfEndTag == INVALID){ return false;}
	size_t iIndexOfFirstNoneSpace = mData.m_CharReader.findIndexOfNextMemoryValueNotMatching(' ', false);
	if (iIndexOfFirstNoneSpace == INVALID ||
		iIndexOfFirstNoneSpace > iIndexOfEndTag )	//there is a space but it's further past the tag end
	{
		return true;//we have no attributes
	}
	mData.m_CharReader.seek(iIndexOfFirstNoneSpace); //move to the location now that we know it's valid and go past it to the value we want
	size_t iIndexOfEqual = mData.m_CharReader.findIndexOfNextMemoryValue('=', false);
	size_t iIndexOfNextSpace = mData.m_CharReader.findIndexOfNextMemoryValue(' ', false);
	
	KCString strAttributeName;
	if (iIndexOfEqual < iIndexOfNextSpace)
	{
		if (iIndexOfEqual > iIndexOfEndTag)
		{
			return true;	//no attributes
		}
		//lets copy that attribute name 
		KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfEqual, strAttributeName, true), false);
	}
	else
	{
		if (iIndexOfNextSpace > iIndexOfEndTag)
		{
			return true;	//no attributes
		}
		KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfNextSpace, strAttributeName, true), false);
	}
	KCEnsureAlwaysReturnVal(strAttributeName.size() > 0, false); //make sure we aren't empty
	size_t iIndexOfQuote = mData.m_CharReader.findIndexOfNextMemoryValue('\"', false);
	if (iIndexOfQuote == INVALID ||
		iIndexOfEndTag > iIndexOfEndTag)	//first quote is out of our array
	{
		mTag.m_Attributes[strAttributeName] = "true";
		return true; //we are going to assume it's a bool
	}
	mData.m_CharReader.seek(iIndexOfQuote + 1); //move to quote and move past by 1
	size_t iIndexOfNextQuote = mData.m_CharReader.findIndexOfNextMemoryValue('\"', false);	
	if (iIndexOfNextQuote == INVALID ||
		iIndexOfNextQuote > iIndexOfEndTag)
	{		
		KCEnsureAlwaysMsgReturnVal(false, "attribute in tag " + mTag.m_strTagName + " is missing it's end double quote", false); //make sure we aren't empty
	}
	KCString strAttributeValue;
	KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfNextQuote, strAttributeValue, true), false);
	mTag.m_Attributes[strAttributeName] = strAttributeValue;
	return _findAttributes(mData, mTag);
}

bool getNextTag(KCXMLParserData &mData, KCXMLTag &mTag)
{
	mTag.m_bValid = false;
	
	size_t iIndexOf = mData.m_CharReader.findIndexOfNextMemoryValue('<', true);
	if( iIndexOf == INVALID ){ return false; } //we are done
	mData.m_CharReader.next(); //read past the tag(<).
	size_t iIndexOfFirstSpace = mData.m_CharReader.findIndexOfNextMemoryValue(' ', false);

	KCEnsureAlwaysReturnVal( mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfFirstSpace, mTag.m_strTagName, true), false );
	mData.m_CharReader.next(); //read past the space
	mTag.m_Attributes.clear();
	if (_findAttributes(mData, mTag) == false)
	{
		return false;
	}
	mTag.m_bValid = true;
	return true;
}



bool KCDataGroupSimpleXMLReader::parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	KCTArray<uint8> mArray;
	KCEnsureAlwaysMsgReturnVal(KCFileUtilities::loadFile(strPathAndFile, mArray), "Unable to load file", false);
	KCString strStringStream((const char*)mArray.getMemory());
	return parseDataGroupFromString(strStringStream, mDataGroup);
	mDataGroup.setProperty("_FILE_", strPathAndFile);
	return true;
}

bool KCDataGroupSimpleXMLReader::parseDataGroupFromString(const KCString &strDataAsString, KCDataGroup &mDataGroup)
{
	KCXMLParserData mData;
	mData.m_CharReader.configureByArray(strDataAsString.c_str(), (uint32)strDataAsString.length());
	KCXMLTag mTag;
	getNextTag(mData, mTag);
	return true;
}
