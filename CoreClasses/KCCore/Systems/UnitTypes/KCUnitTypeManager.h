//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"
#include "Systems/UnitTypes/Private/KCUnitTypeCategory.h"

///////////////////////////////////////////////////////////
//HOW TO USE
//
//This is the matching c++ part of the unittypes from the c# editor.
//
//The c# editor can save unit types into two different types of formats. 
//
//Format 1: header files. Selecting the category in the editor will allow you to specify where to save the header.
//
//Format 2: Binary Lookup File. The tool also exports a unittype.bin file that can be loaded by the UnitTypeManager. The
//			advantages of this format is that it doesn't require a new build for adding unit types or inheritances.
//
/////////////////////////////////////////////////////////

class KCCORE_API KCUnitTypeManager : public KCCoreObject
{
public:
	KCUnitTypeManager();
	~KCUnitTypeManager();

	//returns the singleton of this class
	static KCUnitTypeManager *		getSingleton();

	//returns the category Index by name
	uint32							getCategoryIndexByName(const KCString &strCategoryName) const
	{
		for (int32 iCategoryIndex = 0; iCategoryIndex < m_Categories.Num(); iCategoryIndex++)
		{
			if (m_Categories[iCategoryIndex]->getCategoryName() == strCategoryName)
			{
				return iCategoryIndex;
			}
		}
		return INVALID;
	}

	//returns the category Index by name
	const KCUnitTypeCategory *		getCategoryByName(const KCString &strCategoryName) const
	{
		for (int32 iCategoryIndex = 0; iCategoryIndex < m_Categories.Num(); iCategoryIndex++)
		{
			if (m_Categories[iCategoryIndex]->getCategoryName() == strCategoryName)
			{
				return m_Categories[iCategoryIndex];
			}
		}
		return nullptr;
	}

	//returns the category Index by name
	const KCUnitTypeCategory *		getCategoryByIndex(uint32 iIndex) const
	{
		if (iIndex >= (uint32)m_Categories.Num())
		{
			return nullptr;
		}
		return m_Categories[iIndex];
	}
		
	//parses the bytes of the unit type file
	bool							configureUnitTypeByConfigFile(const TCHAR *strPath);

	//returns the id of the unit type. Returns INVALID if not defined.
	KCUnitType						getUnitTypeID(const KCString &strCategoryName, const KCString &strUnitTypeName)
	{
		return getUnitTypeID( getCategoryByName(strCategoryName), strUnitTypeName);
	}
	//returns the id of the unit type. Returns INVALID if not defined.
	KCUnitType						getUnitTypeID(const KCUnitTypeCategory *pCategory, const KCString &strUnitTypeName)
	{
		KCEnsureAlwaysReturnVal(pCategory, INVALID);
		return pCategory->getUnitTypeIDByName(strUnitTypeName);
	}

	//does a check between two different unit types in the category passed in
	FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, KCUnitType iObjectsIsA, KCUnitType iSubChild) const
	{
		return mCategory.IsA(iObjectsIsA, iSubChild);
	}
	//does a check between two different unit types in the category passed in
	FORCEINLINE bool				IsA(const KCString &strCategoryName, KCUnitType iObjectsIsA, KCUnitType iSubChild) const
	{
		const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
		if (pCategory)
		{
			return pCategory->IsA(iObjectsIsA, iSubChild);
		}
		return false;
	}

	//does a check between two different unit types in this category
	FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, KCUnitType iObjectsIsA, const std::string &strSubChild) const
	{
		return mCategory.IsA(iObjectsIsA, strSubChild);
	}
	//does a check between two different unit types in this category
	FORCEINLINE bool				IsA(const KCString &strCategoryName, KCUnitType iObjectsIsA, const std::string &strSubChild) const
	{
		const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
		if (pCategory)
		{
			return pCategory->IsA(iObjectsIsA, strSubChild);
		}
		return false;
	}
	//does a check between two different unit types in this category
	FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, const std::string &strObjectsIsA, KCUnitType iSubChild) const
	{
		return mCategory.IsA(strObjectsIsA, iSubChild);
	}
	//does a check between two different unit types in this category
	FORCEINLINE bool				IsA(const KCString &strCategoryName, const std::string &strObjectsIsA, KCUnitType iSubChild) const
	{
		const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
		if (pCategory)
		{
			return pCategory->IsA(strObjectsIsA, iSubChild);
		}
		return false;
	}

	//does a check between two different unit types in this category
	FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, const std::string &strObjectsIsA, const std::string &strSubChild) const
	{
		return mCategory.IsA(strObjectsIsA, strSubChild);
	}
	//does a check between two different unit types in this category
	FORCEINLINE bool				IsA(const KCString &strCategoryName, const std::string &strObjectsIsA, const std::string &strSubChild) const
	{
		const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
		if (pCategory)
		{
			return pCategory->IsA(strObjectsIsA, strSubChild);
		}
		return false;
	}


private:
	void							_clean();
	//NOTE - maybe in the future better to make this a map
	TArray<KCUnitTypeCategory *>	m_Categories;
};



