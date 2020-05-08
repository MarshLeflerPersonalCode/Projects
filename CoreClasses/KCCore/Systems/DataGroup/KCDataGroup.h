//copyright Marsh Lefler 2000-...
//this is a data group that stores properties in a child parent relationship.
#pragma once
#include "Systems/DataGroup/Private/KCDataProperty.h"
#include <unordered_map>

class KCDataGroup
{
public:	
	KCDataGroup();
	KCDataGroup(KCName strName);
	~KCDataGroup();

	//sets the group name
	void								setGroupName(const KCName &strName) { m_strGroupName = strName; }
	const KCName &						getGroupName() const { return m_strGroupName; }
	const KCString &					getGroupNameAsString() const { return m_strGroupName.toString(); }
	//attempts to add a group by name. If the group is already there it returns 
	KCDataGroup &						getOrCreateChildGroup(const KCName &strName);

	//removes a child group
	bool								removeChildGroup(KCDataGroup &mGroup);
	//returns true if there are no child groups or properties
	bool								isEmpty() const;
	//attempts to add a group. if it doesn't have a name or the name is already taken false will be returned. 
	bool								addChildGroup(KCDataGroup &mGroup);
	//grabs a child by name. Returns null if not found
	KCDataGroup *						getChildGroup(const KCName &strName);
	//grabs a child by name. Returns null if not found
	const KCDataGroup *					getChildGroup(const KCName &strName) const;
	//returns if the child group exists
	bool								childGroupExists(const KCName &strName){ return (getChildGroup(strName))?true:false; }
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
	//attempts to get a property by name, if not found return null
	DEBUG_FORCEINLINE KCDataProperty *	getProperty(const KCName &strName)
	{
		auto mData = m_Properties.find(strName);
		if (mData == m_Properties.end())
		{
			return nullptr;
		}
		return &mData->second;
	}
	//attempts to get a property by name, if not found return null
	DEBUG_FORCEINLINE const KCDataProperty *	getProperty(const KCName &strName) const
	{
		auto mData = m_Properties.find(strName);
		if (mData == m_Properties.end())
		{
			return nullptr;
		}
		return &mData->second;
	}
	//returns a string representing the data group
	KCString							getStringRepresentingDataGroup();
	/////////////////////////////////PROPERTIES/////////////////////////////////////////////////////
	//returns the number of properties
	size_t								getCountOfProperties() { return m_Properties.size(); }
	//returns the child group map
	//for (auto element : getChildGroups())
	//{
	//	element.first; //name
	//	element.second; //property
	//}
	std::unordered_map<KCName, KCDataProperty, KCNameHasher> & getProperties() { return m_Properties; }
	//returns the being iterator for the properties
	//auto iter = getPropertiesBegin();
	//while( iter != getPropertiesEnd()){ iter->first(name); iter->second(property); iter++; ... }
	auto								getPropertiesBegin() { return m_Properties.begin(); }
	//returns the end iterator for the properties
	//auto iter = getPropertiesBegin();
	//while( iter != getPropertiesEnd()){ iter->first(name); iter->second(property); iter++; ... }
	auto								getPropertiesEnd() { return m_Properties.end(); }
	/////////////////////////////////PROPERTIES/////////////////////////////////////////////////////
	///////////////////////////////////GROUPS//////////////////////////////////////////////////////
	//returns the number of groups
	size_t								getCountOfChildGroups() { return m_ChildGroups.size(); }
	//returns the child group map
	//for (auto element : getChildGroups()){element.first(name); element.second(group); }
	std::unordered_map<KCName, KCDataGroup, KCNameHasher> & getChildGroups() { return m_ChildGroups; }
	//returns the being iterator for the properties
	//auto iter = getChildGroupBegin();
	//while( iter != getChildGroupEnd()){ iter->first(name); iter->second(group); iter++; ... }
	auto								getChildGroupBegin() { return m_ChildGroups.begin(); }
	//returns the end iterator for the properties
	//auto iter = getChildGroupBegin();
	//while( iter != getChildGroupEnd()){ iter->first(name); iter->second(group); iter++; ... }
	auto								getChildGroupEnd() { return m_ChildGroups.end(); }
	///////////////////////////////////GROUPS//////////////////////////////////////////////////////
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
	void								setProperty(const KCName &strName, const WCHAR *strValue);
	//getters - by ref is faster for objects
	DEBUG_FORCEINLINE bool				getPropertyByRef(const KCName &strName, bool &bValue) { getOrCreateProperty(strName) >> bValue; return bValue;}
	DEBUG_FORCEINLINE char				getPropertyByRef(const KCName &strName, char &cValue) { getOrCreateProperty(strName) >> cValue; return cValue;}
	DEBUG_FORCEINLINE int8				getPropertyByRef(const KCName &strName, int8 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue; }
	DEBUG_FORCEINLINE uint8				getPropertyByRef(const KCName &strName, uint8 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue;}
	DEBUG_FORCEINLINE int16				getPropertyByRef(const KCName &strName, int16 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue; }
	DEBUG_FORCEINLINE uint16			getPropertyByRef(const KCName &strName, uint16 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue; }
	DEBUG_FORCEINLINE int32				getPropertyByRef(const KCName &strName, int32 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue; }
	DEBUG_FORCEINLINE uint32			getPropertyByRef(const KCName &strName, uint32 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue; }
	DEBUG_FORCEINLINE int64				getPropertyByRef(const KCName &strName, int64 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue;}
	DEBUG_FORCEINLINE uint64			getPropertyByRef(const KCName &strName, uint64 &iValue) { getOrCreateProperty(strName) >> iValue; return iValue; }
	DEBUG_FORCEINLINE float				getPropertyByRef(const KCName &strName, float &fValue) { getOrCreateProperty(strName) >> fValue; return fValue; }
	DEBUG_FORCEINLINE KCName &			getPropertyByRef(const KCName &strName, KCName &strValue) { getOrCreateProperty(strName) >> strValue; return strValue; }
	DEBUG_FORCEINLINE KCString &		getPropertyByRef(const KCName &strName, KCString &strValue) { getOrCreateProperty(strName) >> strValue; return strValue; }
	DEBUG_FORCEINLINE bool				getProperty(const KCName &strName, bool bValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> bValue; } return bValue; }
	DEBUG_FORCEINLINE char				getProperty(const KCName &strName, char cValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> cValue; } return cValue; }
	DEBUG_FORCEINLINE int8				getProperty(const KCName &strName, int8 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE uint8				getProperty(const KCName &strName, uint8 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE int16				getProperty(const KCName &strName, int16 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE uint16			getProperty(const KCName &strName, uint16 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE int32				getProperty(const KCName &strName, int32 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE uint32			getProperty(const KCName &strName, uint32 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE int64				getProperty(const KCName &strName, int64 iValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE uint64			getProperty(const KCName &strName, uint64 iValue)  const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> iValue; } return iValue; }
	DEBUG_FORCEINLINE float				getProperty(const KCName &strName, float fValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> fValue; } return fValue; }
	DEBUG_FORCEINLINE KCName 			getPropertyAsName(const KCName &strName, KCName &strValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> strValue; } return strValue; }
	DEBUG_FORCEINLINE KCName 			getProperty(const KCName &strName, const KCName &strValue) const { KCName mReturnValue = strValue; const KCDataProperty *pProperty = getProperty(strName); if (pProperty) {  (*pProperty) >> mReturnValue; } return mReturnValue; }
	DEBUG_FORCEINLINE KCString			getPropertyAsString(const KCName &strName, KCString &strValue) const { const KCDataProperty *pProperty = getProperty(strName); if (pProperty) { (*pProperty) >> strValue; } return strValue; }
	DEBUG_FORCEINLINE KCString			getProperty(const KCName &strName, const KCString &strValue) const { KCString mReturnValue = strValue; const KCDataProperty *pProperty = getProperty(strName);  if (pProperty) {  (*pProperty) >> mReturnValue; } return mReturnValue; }
	
	DEBUG_FORCEINLINE KCString			getProperty(const KCName &strName, const char *pArray) const 
	{ 
		KCString strValue = pArray;
		return getPropertyAsString(strName, strValue);
	}

private:
	KCName														m_strGroupName;
	std::unordered_map<KCName, KCDataProperty, KCNameHasher>	m_Properties;
	std::unordered_map<KCName, KCDataGroup, KCNameHasher>		m_ChildGroups;
};