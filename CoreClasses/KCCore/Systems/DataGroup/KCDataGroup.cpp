#include "KCDataGroup.h"
#include "Systems/DataGroup/FileTypes/KCDataGroupStringWriter.h"


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

bool KCDataGroup::removeChildGroup(KCDataGroup &mGroup)
{
	auto mData = m_ChildGroups.find(mGroup.m_strGroupName);
	if (mData != m_ChildGroups.end())
	{
		m_ChildGroups.erase(mData);
		return true;
	}
	return false;
}

bool KCDataGroup::isEmpty() const
{
	return ( m_ChildGroups.size() == 0 && m_Properties.size() == 0)?true:false;
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

