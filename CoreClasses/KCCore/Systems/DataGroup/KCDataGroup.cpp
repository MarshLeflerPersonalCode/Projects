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



void KCDataGroup::setParentDataGroup(const KCDataGroup *pDataGroupParent)
{
	KCEnsureAlwaysReturn(pDataGroupParent != this);
	m_pInheritingParent = pDataGroupParent;
	for (std::unordered_map<KCName, KCDataGroup, KCNameHasher>::iterator iter = m_ChildGroups.begin(); iter != m_ChildGroups.end(); iter++)
	{
		KCDataGroup &mChild = iter->second;
		if (m_pInheritingParent == nullptr)
		{
			mChild.setParentDataGroup(nullptr);
		}
		else
		{
			const KCDataGroup *pChildParentGroup = m_pInheritingParent->getChildGroupWithInhertance(iter->first);
			mChild.setParentDataGroup(pChildParentGroup);
		}
	}
}

KCDataGroup & KCDataGroup::getOrCreateChildGroup(const KCName &strName)
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{
		m_ChildGroups[strName] = KCDataGroup(strName);
		mData = m_ChildGroups.find(strName);
		if (m_pInheritingParent != null)
		{
			const KCDataGroup *pChildParentGroup = m_pInheritingParent->getChildGroupWithInhertance(strName);
			if (pChildParentGroup != nullptr)
			{
				mData->second.setParentDataGroup(pChildParentGroup);
			}
		}
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
	if (m_pInheritingParent != nullptr &&
		m_pInheritingParent->isEmpty() == false)
	{
		return false;
	}
	return ( m_ChildGroups.size() == 0 && m_Properties.size() == 0)?true:false;
}

bool KCDataGroup::addChildGroup(KCDataGroup &mGroup)
{
	KCEnsureAlwaysMsgReturnVal(!mGroup.getGroupName().isEmpty(), "No name specified in group.", false );
	KCDataGroup *pChildWithName = getChildGroupNoInhertance(mGroup.getGroupName());
	KCEnsureAlwaysMsgReturnVal(pChildWithName == nullptr, "Child with name: " + mGroup.getGroupName().toString() + " already exists.", false);
	m_ChildGroups[mGroup.getGroupName()] = mGroup;	//copies
	if (m_pInheritingParent != null)
	{
		const KCDataGroup *pChildParentGroup = m_pInheritingParent->getChildGroupWithInhertance(mGroup.getGroupName());
		if (pChildParentGroup != nullptr)
		{
			m_ChildGroups[mGroup.getGroupName()].setParentDataGroup(pChildParentGroup);
		}
	}
	return true;
}

bool KCDataGroup::hasPropertyNotInherited(const KCName &strPropertyName)
{
	return (getDataPropertyNoInheritance(strPropertyName) != null) ? true : false;
}

bool KCDataGroup::hasPropertyInherited(const KCName &strPropertyName)
{
	return (getDataPropertyWithInheritance(strPropertyName) != null) ? true : false;
}

KCDataGroup * KCDataGroup::getChildGroupNoInhertance(const KCName &strName)
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{


		return nullptr;
	}
	return &mData->second;
}

const KCDataGroup * KCDataGroup::getChildGroupNoInhertance(const KCName &strName) const
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{
		return nullptr;
	}
	return &mData->second;
}

const KCDataGroup * KCDataGroup::getChildGroupWithInhertance(const KCName &strName) const
{
	auto mData = m_ChildGroups.find(strName);
	if (mData == m_ChildGroups.end())
	{
		if (m_pInheritingParent != null)
		{
			return m_pInheritingParent->getChildGroupWithInhertance(strName);
		}
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
	getOrCreateProperty(strName) << KCStringUtils::convertWideToUtf8(strValue);
}

void KCDataGroup::_cacheParents(std::unordered_map<KCDataGroup *, const KCDataGroup *> &mCacheLookup)
{
	mCacheLookup[this] = m_pInheritingParent;
	m_pInheritingParent = nullptr;
	for (std::unordered_map<KCName, KCDataGroup, KCNameHasher>::iterator iter = m_ChildGroups.begin(); iter != m_ChildGroups.end(); iter++)
	{
		KCDataGroup &mChild = iter->second;
		mChild._cacheParents(mCacheLookup);
	}
}

void KCDataGroup::_reinitializeFromCache(std::unordered_map<KCDataGroup *, const KCDataGroup *> &mCacheLookup)
{
	auto mIterator = mCacheLookup.find(this);
	if (mIterator != mCacheLookup.end())
	{
		m_pInheritingParent = mIterator->second;
	}
	else
	{
		KCEnsureAlwaysMsg(false, "Every data group should have an entry. If your getting this something went terribly wrong.");
	}
	
	for (std::unordered_map<KCName, KCDataGroup, KCNameHasher>::iterator iter = m_ChildGroups.begin(); iter != m_ChildGroups.end(); iter++)
	{
		KCDataGroup &mChild = iter->second;
		mChild._reinitializeFromCache(mCacheLookup);
	}
}

