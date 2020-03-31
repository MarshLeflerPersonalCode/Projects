#include "KCDataGroup.h"

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

KCDataGroup & KCDataGroup::getOrCreateGroup(const KCName &strName)
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{
		m_ChildGroups[strName] = KCDataGroup(strName);
		mData = m_ChildGroups.find(strName);
	}
	return mData->second;
}




