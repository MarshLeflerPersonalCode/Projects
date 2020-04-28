#pragma once
#include "KCIncludes.h"
#include "KCUnitTypeDef.h"




class KCCORE_API KCUnitTypeCategory : public KCCoreObject
{
public:
	KCUnitTypeCategory(KCCoreData *pCoreData);
	~KCUnitTypeCategory();

	//returns the category name
	const KCString &					getCategoryName() const { return m_strName; }

	//returns the unit type ID
	DEBUG_FORCEINLINE KCUnitType		getUnitTypeIDByName(const KCString &strString) const
	{
			auto mPair = m_MapLookUp.find(strString);
			if (mPair != m_MapLookUp.end())
			{
				return mPair->second;
			}
			return INVALID;
	}
	//returns the unit type ID
	DEBUG_FORCEINLINE const KCString &	getUnitTypeNameByID(KCUnitType iID) const
	{
		if (iID < m_iNumberOfUnitTypes)
		{
			return m_UnitTypes[iID].m_strString;
		}
		return EMPTY_KCSTRING;
	}

	//does a check between two different unit types in this category
	DEBUG_FORCEINLINE bool				IsA(KCUnitType iObjectsIsA, KCUnitType iSubChild) const
	{
		if (iObjectsIsA < m_iNumberOfUnitTypes &&
			iSubChild < m_iNumberOfUnitTypes)
		{
			return m_UnitTypes[iObjectsIsA].bitCheck((uint32)iSubChild);
		}
		return false;
	}
	//does a check between two different unit types in this category
	DEBUG_FORCEINLINE bool				IsA(KCUnitType iObjectsIsA, const std::string &strSubChild) const
	{
		return IsA(iObjectsIsA, getUnitTypeIDByName(strSubChild));
	}
	//does a check between two different unit types in this category
	DEBUG_FORCEINLINE bool				IsA(const std::string &strObjectsIsA, KCUnitType iSubChild) const
	{
		return IsA(getUnitTypeIDByName(strObjectsIsA), iSubChild);
	}
	//does a check between two different unit types in this category
	DEBUG_FORCEINLINE bool				IsA(const std::string &strObjectsIsA, const std::string &strSubChild) const
	{
		return IsA(getUnitTypeIDByName(strObjectsIsA), getUnitTypeIDByName(strSubChild));
	}

	//parses the category
	bool								_parse(KCByteReader &mByteReader);

private:
	uint32							m_iNumberOfUnitTypes = 0;
	uint32							m_iNumberOfBitLookIndexs = 0;
	KCUnitTypeDef					*m_UnitTypes = nullptr;
	std::map<KCString, KCUnitType >	m_MapLookUp;
	KCString						m_strName;

};



