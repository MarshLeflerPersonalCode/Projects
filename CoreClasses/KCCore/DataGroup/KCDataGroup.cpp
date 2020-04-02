#include "KCDataGroup.h"
#include "FileTypes/KCDataGroupStringWriter.h"
#include "utils/KCAsserts.h"

KCDataGroup::KCDataGroup()
{

}

KCDataGroup::KCDataGroup(KCName strName)
{
	m_strGroupName = strName;
}

KCDataGroup::~KCDataGroup()
{

}



KCDataGroup & KCDataGroup::getOrCreateChildGroup(const KCName &strName)
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{
		m_ChildGroups[strName] = KCDataGroup(strName);
		mData = m_ChildGroups.find(strName);
	}
	return mData->second;
}

bool KCDataGroup::addChildGroup(KCDataGroup &mGroup)
{
	KCEnsureAlwaysMsgReturnVal(!mGroup.getGroupName().isEmpty(), "No name specified in group.", false );
	KCDataGroup *pChildWithName = getChildGroup(mGroup.getGroupName());
	KCEnsureAlwaysMsgReturnVal(pChildWithName == nullptr, "Child with name: " + mGroup.getGroupName().toString() + " already exists.", false);
	m_ChildGroups[mGroup.getGroupName()] = mGroup;	//copies
	return true;
}

KCDataGroup * KCDataGroup::getChildGroup(const KCName &strName)
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{
		return nullptr;
	}
	return &mData->second;
}

KCString KCDataGroup::getStringRepresentingDataGroup()
{
	return KCDataGroupStringWriter::writeDataGroupToString(*this);
}

void KCDataGroup::setProperty(const KCName &strName, const WCHAR *strValue)
{	
	getOrCreateProperty(strName) << KCStringUtils::converWideToUtf8(strValue);
}

