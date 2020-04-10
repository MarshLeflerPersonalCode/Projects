#include "KCDataGroupStringWriter.h"
#include "KCIncludes.h"
#include "Systems/DataGroup/KCDataGroup.h"
#include <sstream>



void _writeDataGroup(KCDataGroup &mDataGroup, std::ostringstream  &mStringStream, KCString strIndent)
{
	mStringStream << strIndent << "[" + mDataGroup.getGroupNameAsString() << "]" << std::endl;
	
	KCString strIndentChildren = strIndent + "     ";
	for (auto mPropertyIter : mDataGroup.getProperties())
	{
		KCDataProperty &mProperty = mPropertyIter.second;
		mStringStream << strIndentChildren << "<" << DATATYPES_UTILS::getDataTypeName(mProperty.m_eType) << ">" << mProperty.getNameAsString() << ":" << mProperty.getAsString() << std::endl;
	}

	for (auto mChildIter : mDataGroup.getChildGroups()) 
	{ 
		_writeDataGroup(mChildIter.second, mStringStream, strIndentChildren);

	}
	mStringStream << strIndent << "[/" + mDataGroup.getGroupNameAsString() << "]" << std::endl;
	
}




bool KCDataGroupStringWriter::writeDataGroupToFile(const WCHAR *strPathAndFile, KCDataGroup &mDataGroup)
{
	std::ostringstream  mStringStream;
	mDataGroup.setProperty("_FILE_", strPathAndFile);
	_writeDataGroup(mDataGroup, mStringStream, "");		
	KCString strFile = mStringStream.str();
	KCFileUtilities::saveToFile(strPathAndFile, strFile.c_str(), strFile.size());
	

	return true;
}

KCString KCDataGroupStringWriter::writeDataGroupToString(KCDataGroup &mDataGroup)
{
	std::ostringstream  mStringStream;	
	_writeDataGroup(mDataGroup, mStringStream, "");
	return mStringStream.str();
}
