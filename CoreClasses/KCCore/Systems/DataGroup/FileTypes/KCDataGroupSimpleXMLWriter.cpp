#include "KCDataGroupSimpleXMLWriter.h"
#include "KCDataGroupSimpleXMLInclude.h"
#include "KCIncludes.h"
#include "Systems/DataGroup/KCDataGroup.h"
#include <sstream>



void _writeDataGroupToXML(KCDataGroup &mDataGroup, std::ostringstream  &mStringStream, KCString strIndent)
{
	KCString strGroupNameCleaned = mDataGroup.getGroupNameAsString();	
	KCStringUtils::chopEndOffOfString(strGroupNameCleaned, SAME_GROUP_TAG_NAME_SPLIT );
	mStringStream << strIndent << "<" + strGroupNameCleaned << " Type=\"" << GROUP_TYPE_NAME << "\">" << std::endl;
	
	KCString strIndentChildren = strIndent + "     ";
	for (auto mPropertyIter : mDataGroup.getProperties())
	{
		KCDataProperty &mProperty = mPropertyIter.second;
		mStringStream << strIndentChildren << "<" << mProperty.getNameAsString() << " Type=\""  << DATATYPES_UTILS::getDataTypeName(mProperty.m_eType) << "\">" <<  mProperty.getAsString() << "</" << mProperty.getNameAsString() << ">" << std::endl;
	}

	for (auto mChildIter : mDataGroup.getChildGroups()) 
	{ 
		_writeDataGroupToXML(mChildIter.second, mStringStream, strIndentChildren);
	}
	mStringStream << strIndent << "</" + strGroupNameCleaned << ">" << std::endl;
	
}




bool KCDataGroupSimpleXMLWriter::writeDataGroupToFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	std::ostringstream  mStringStream;
	mDataGroup.setProperty("_FILE_", strPathAndFile);
	_writeDataGroupToXML(mDataGroup, mStringStream, "");
	KCString strFile = mStringStream.str();
	KCFileUtilities::saveToFile(strPathAndFile, strFile.c_str(), strFile.size());
	

	return true;
}

KCString KCDataGroupSimpleXMLWriter::writeDataGroupToString(KCDataGroup &mDataGroup)
{
	std::ostringstream  mStringStream;	
	_writeDataGroupToXML(mDataGroup, mStringStream, "");
	return mStringStream.str();
}
