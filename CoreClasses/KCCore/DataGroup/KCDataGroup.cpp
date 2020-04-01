#include "KCDataGroup.h"
#include "FileTypes/KCDataGroupStringWriter.h"
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

	//probably a better way to do this..
	static char g_StringMemoryPool[1000];
	std::wstring strString(strValue);
	KCEnsureAlwaysMsgReturn(strString.size() * sizeof(WCHAR) < 1000, "Increase the memory pool value.");
	size_t iSizeOutput(0);
	wcstombs_s(&iSizeOutput, g_StringMemoryPool, 1000, strString.c_str(), strString.size());
	getOrCreateProperty(strName) << KCString(g_StringMemoryPool);
}

