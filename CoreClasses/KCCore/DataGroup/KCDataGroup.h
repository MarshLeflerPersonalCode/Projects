//copyright Marsh Lefler 2000-...
#pragma once
#include "Private/KCDataProperty.h"
#include <unordered_map>

class KCDataGroup
{
public:
	KCDataGroup();
	KCDataGroup(KCName strName);
	~KCDataGroup();

	//sets the group name
	void								setGroupName(const KCName &strName) { m_strGroupName = strName; }

	//attempts to add a group by name. If the group is already there it returns 
	KCDataGroup &						getOrCreateGroup(const KCName &strName);

	//attempts to add or get a property by name
	DEBUG_FORCEINLINE KCDataProperty &	getOrCreateProperty(const KCName &strName)
	{
		auto mData = m_Properties.find(strName);
		if (mData == m_Properties.end())
		{
			m_Properties[strName] = KCDataProperty(strName);
			mData = m_Properties.find(strName);
		}
		return mData->second;
	}

	//setters
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, bool bValue) { getOrCreateProperty(strName) << bValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, char cValue) { getOrCreateProperty(strName) << cValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, int8 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, uint8 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, int16 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, uint16 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, int32 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, uint32 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, int64 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, uint64 iValue) { getOrCreateProperty(strName) << iValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, float fValue) { getOrCreateProperty(strName) << fValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, const KCName &strValue) { getOrCreateProperty(strName) << strValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, const KCString &strValue) { getOrCreateProperty(strName) << strValue; }
	DEBUG_FORCEINLINE void				setProperty(const KCName &strName, const char *strValue) { getOrCreateProperty(strName) << KCString(strValue); }
	//getters
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, bool &bValue) { getOrCreateProperty(strName) >> bValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, char &cValue) { getOrCreateProperty(strName) >> cValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, int8 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, uint8 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, int16 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, uint16 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, int32 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, uint32 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, int64 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, uint64 &iValue) { getOrCreateProperty(strName) >> iValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, float &fValue) { getOrCreateProperty(strName) >> fValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, KCName &strValue) { getOrCreateProperty(strName) >> strValue; }
	DEBUG_FORCEINLINE void				getProperty(const KCName &strName, KCString &strValue) { getOrCreateProperty(strName) >> strValue; }
	
private:
	KCName														m_strGroupName;
	std::unordered_map<KCName, KCDataProperty, KCNameHasher>	m_Properties;
	std::unordered_map<KCName, KCDataGroup, KCNameHasher>		m_ChildGroups;
};