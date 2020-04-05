#include "KCDataGroupSimpleXMLReader.h"
#include "KCDataGroupSimpleXMLInclude.h"
#include "IO/KCMemoryReader.h"
#include "IO/KCFileUtilities.h"
#include "DataGroup/KCDataGroup.h"
#include <sstream>
#include <iostream>


struct KCXMLParserData
{
	KCCharReader					m_CharReader;
	KCDataGroup						*m_pOriginalDataGroup = null;
	std::map<KCString, KCString>	m_Attributes;
	KCString						m_strTagName;
	KCString						m_strInnterText;
	KCString						m_strFileName;
};






bool _findAttributesOfTag(KCXMLParserData &mData, KCDataGroup &mDatagroup)
{
	
	//the end of the tag I'm in
	size_t iIndexOfEndTag = mData.m_CharReader.findIndexOfNextMemoryValue('>', false);
	if( iIndexOfEndTag == INVALID){ return false;}
	size_t iIndexOfFirstNoneSpace = mData.m_CharReader.findIndexOfNextMemoryValueNotMatching(' ', false);
	if (iIndexOfFirstNoneSpace == INVALID ||
		iIndexOfFirstNoneSpace > iIndexOfEndTag )	//there is a space but it's further past the tag end
	{
		mData.m_CharReader.seek(iIndexOfEndTag + 1);	//jump to the end of the tag
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
			mData.m_CharReader.seek(iIndexOfEndTag + 1);	//jump to the end of the tag
			return true;	//no attributes
		}
		//lets copy that attribute name 
		KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfEqual, strAttributeName, true), false);
	}
	else
	{
		if (iIndexOfNextSpace > iIndexOfEndTag)
		{
			mData.m_CharReader.seek(iIndexOfEndTag + 1);	//jump to the end of the tag
			return true;	//no attributes
		}
		KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfNextSpace, strAttributeName, true), false);
	}
	KCEnsureAlwaysReturnVal(strAttributeName.size() > 0, false); //make sure we aren't empty
	size_t iIndexOfQuote = mData.m_CharReader.findIndexOfNextMemoryValue('\"', false);
	if (iIndexOfQuote == INVALID ||
		iIndexOfEndTag > iIndexOfEndTag)	//first quote is out of our array
	{
		mData.m_Attributes[KCStringUtils::toUpper(strAttributeName)] = "true";
		return _findAttributesOfTag(mData, mDatagroup); //we are going to assume it's a bool
	}
	mData.m_CharReader.seek(iIndexOfQuote + 1); //move to quote and move past by 1
	size_t iIndexOfNextQuote(INVALID);
FIND_NEXT_DOUBLE_QUOTE:
	iIndexOfNextQuote = mData.m_CharReader.findIndexOfNextMemoryValue('\"', false);	
	if (iIndexOfNextQuote == INVALID ||
		iIndexOfNextQuote > iIndexOfEndTag)
	{				
		KCEnsureAlwaysMsgReturnVal(false, "attribute in tag " + mDatagroup.getGroupNameAsString() + " is missing it's end double quote in file: " + mData.m_strFileName, false); //make sure we aren't empty
	}
	//lets check for \".
	if (mData.m_CharReader.memoryValueAtLocation(iIndexOfNextQuote - 1) == '\\')
	{
		goto FIND_NEXT_DOUBLE_QUOTE;
	}
	KCString strAttributeValue;
	KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfNextQuote, strAttributeValue, true), false);
	mData.m_Attributes[KCStringUtils::toUpper(strAttributeName)] = strAttributeValue;
	return _findAttributesOfTag(mData, mDatagroup);
}


//the mTagNames is weird - XML can have the same tag names. We can not because of inheritance. So we keep a list per child, and that list increments with each use.
bool parseXMLTagAndChildren(KCXMLParserData &mData, KCDataGroup &mDataGroup, std::map<KCString, int32> &mTagNames)
{


	size_t iIndexOf = mData.m_CharReader.findIndexOfNextMemoryValue('<', true);
	if (iIndexOf == INVALID) { return false; } //we are done
	mData.m_CharReader.next(); //read past the tag(<).
	size_t iIndexOfFirstSpace = mData.m_CharReader.findIndexOfNextMemoryValue(' ', false);

	KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfFirstSpace, mData.m_strTagName, true), false);
	mData.m_CharReader.next(); //read past the space
	/*mData.m_Attributes.clear();
	if (_findAttributesOfTag(mData, mDataGroup) == false)
	{
		return false;
	}*/

	std::map<KCString, int32>::iterator mTagNameIterator = mTagNames.find(mData.m_strTagName);
	if (mTagNameIterator != mTagNames.end())
	{
		mTagNameIterator->second++;
		mData.m_strTagName = mData.m_strTagName + SAME_GROUP_TAG_NAME_SPLIT + std::to_string(mTagNameIterator->second);
	}
	else
	{
		mTagNames[mData.m_strTagName] = 0;
	}
	//std::map<KCString, int32> mTmp;
	//std::map<KCString, int32> *pTagNamesToPass = &mTagNames;
	//KCDataGroup *pWorkingDataGroup = nullptr;
	//KCDataProperty *pWorkingDatProperty = nullptr;
	/*std::map<KCString, KCString>::iterator mAttributeSearch = mData.m_Attributes.find(ATTRIBUTE_TYPE);
	if (mAttributeSearch != mData.m_Attributes.end())
	{		
		if (mAttributeSearch->second == GROUP_TYPE_NAME)
		{
			if (mDataGroup.getGroupName().isEmpty())
			{
				mDataGroup.setGroupName(mData.m_strTagName);
				pWorkingDataGroup = &mDataGroup;
			}
			else
			{
				pWorkingDataGroup = &mDataGroup.getOrCreateChildGroup(mData.m_strTagName);
				pTagNamesToPass = &mTmp;
			}
		}
		else //it's a property
		{
			pWorkingDatProperty = &mDataGroup.getOrCreateProperty(mData.m_strTagName);
			pWorkingDatProperty->m_eType = getDataGroupTypeByString(mAttributeSearch->second);
		}
	}*/

	//lets see if the next tag is open or closed
	size_t iIndexOfNextTag = mData.m_CharReader.findIndexOfNextMemoryValue('<', false);
	if (iIndexOfNextTag == INVALID)
	{
		return false;
	}
	if (iIndexOfNextTag != mData.m_CharReader.tell())
	{	
		//copy the inner text
		KCEnsureAlwaysReturnVal(mData.m_CharReader.copyMemoryIntoStringToMemoryLocation(iIndexOfNextTag, mData.m_strInnterText, true), false);
	}
	if (mData.m_CharReader.memoryValueAtLocation(iIndexOfNextTag + 1) == '/')
	{
		
		EDATAGROUP_VARIABLE_TYPES eDataGroupType = configureDataGroupTypeFromStringValue(mData.m_strInnterText);
		if (eDataGroupType == EDATAGROUP_VARIABLE_TYPES::COUNT)
		{
			eDataGroupType = EDATAGROUP_VARIABLE_TYPES::INT32;
		}
		//if it gets here it's a property
		mDataGroup.getOrCreateProperty(mData.m_strTagName).setValueByString(mData.m_strInnterText, eDataGroupType);
		//lets move to the end of this tag
		mData.m_CharReader.seek(iIndexOfNextTag + 1);
		mData.m_CharReader.findIndexOfNextMemoryValue('>', true);
		mData.m_CharReader.next();	//and we go past it by one		
		return true;
	}
	else //it's a child!
	{
		KCDataGroup &mChild = (mDataGroup.getGroupName().isEmpty())?mDataGroup:mDataGroup.getOrCreateChildGroup(mData.m_strTagName);
		if (&mChild == &mDataGroup)
		{
			mDataGroup.setGroupName(mData.m_strTagName);
		}
		std::map<KCString, int32> mChildTagNames;		
		//if it gets here it's a child group. 
		do 
		{		
			if (parseXMLTagAndChildren(mData, mChild, mChildTagNames) == false)
			{				
				return false;
			}
			size_t iIndexOfNextTag = mData.m_CharReader.findIndexOfNextMemoryValue('<', false);
			if (iIndexOfNextTag == INVALID)
			{
				return true;
			}
			if (mData.m_CharReader.memoryValueAtLocation(iIndexOfNextTag + 1) == '/')
			{
				//we are at the end of the tag. We need to return
				mData.m_CharReader.findIndexOfNextMemoryValue('>', true);
				mData.m_CharReader.next();
				return true;
			}
		} while (mData.m_CharReader.eof() == false);
	}
	
		
	
	return true;
}



bool KCDataGroupSimpleXMLReader::parseDataGroupFromFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	KCTArray<uint8> mArray;
	KCEnsureAlwaysMsgReturnVal(KCFileUtilities::loadFile(strPathAndFile, mArray), "Unable to load file", false);
	KCString strStringStream((const char*)mArray.getMemory());
	KCXMLParserData mData;
	mData.m_pOriginalDataGroup = &mDataGroup;
	mData.m_strFileName = KCStringUtils::toNarrowUtf8(strPathAndFile);
	KCString strCleaned = KCString((const char *)mArray.getMemory(), mArray.getCount());
	strCleaned.erase(std::remove(strCleaned.begin(), strCleaned.end(), '\n'), strCleaned.end());
	strCleaned.erase(std::remove(strCleaned.begin(), strCleaned.end(), '\r'), strCleaned.end());
	strCleaned.erase(std::remove(strCleaned.begin(), strCleaned.end(), '\t'), strCleaned.end());
	mData.m_CharReader.configureByArray(strCleaned.c_str(), (uint32)strCleaned.length());
	std::map<KCString, int32> mTagNames;
	parseXMLTagAndChildren(mData, mDataGroup, mTagNames);
	mDataGroup.setProperty("_FILE_", strPathAndFile);
	
	return true;
}

bool KCDataGroupSimpleXMLReader::parseDataGroupFromString(const KCString &strDataAsString, KCDataGroup &mDataGroup)
{
	KCXMLParserData mData;
	mData.m_pOriginalDataGroup = &mDataGroup;
	mData.m_strFileName = "Text Parsing";
	KCString strCleaned = strDataAsString;
	strCleaned.erase(std::remove(strCleaned.begin(), strCleaned.end(), '\n'), strCleaned.end());
	strCleaned.erase(std::remove(strCleaned.begin(), strCleaned.end(), '\r'), strCleaned.end());
	strCleaned.erase(std::remove(strCleaned.begin(), strCleaned.end(), '\t'), strCleaned.end());
	mData.m_CharReader.configureByArray(strCleaned.c_str(), (uint32)strCleaned.length());	
	std::map<KCString, int32> mTagNames;
	parseXMLTagAndChildren(mData, mDataGroup, mTagNames);
	return true;
}
