//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"
#include "Systems/UnitTypes/UnitTypes/KCUnitTypesItems.h"
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
namespace UNITTYPE
{

	class KCCORE_API KCUnitTypeManager
	{
	public:
		KCUnitTypeManager();
		~KCUnitTypeManager();

		//returns the singleton of this class
		static KCUnitTypeManager *		getSingleton();

		//returns the category Index by name
		uint32							getCategoryIndexByName(const KCString &strCategoryName) const
		{
			for (uint32 iCategoryIndex = 0; iCategoryIndex < m_Categories.getCount(); iCategoryIndex++)
			{
				if (m_Categories[iCategoryIndex].getCategoryName() == strCategoryName)
				{
					return iCategoryIndex;
				}
			}
			return INVALID;
		}

		//returns the category Index by name
		const KCUnitTypeCategory *		getCategoryByName(const KCString &strCategoryName) const
		{
			for (uint32 iCategoryIndex = 0; iCategoryIndex < m_Categories.getCount(); iCategoryIndex++)
			{
				if (m_Categories[iCategoryIndex].getCategoryName() == strCategoryName)
				{
					return &m_Categories[iCategoryIndex];
				}
			}
			return nullptr;
		}

		//returns the category Index by name
		const KCUnitTypeCategory *		getCategoryByIndex(uint32 iIndex) const
		{
			if (iIndex >= m_Categories.getCount())
			{
				return nullptr;
			}
			return &m_Categories[iIndex];
		}
		
		//parses the bytes of the unit type file
		bool							parseUnitTypeFile(const TCHAR *strPath);

		//returns the id of the unit type. Returns INVALID if not defined.
		uint32							getUnitTypeID(const KCString &strCategoryName, const KCString &strUnitTypeName)
		{
			const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
			if (pCategory)
			{
				return pCategory->getUnitTypeIDByName(strUnitTypeName);
			}
			return INVALID;
		}
		//does a check between two different unit types in the category passed in
		FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, uint32 iObjectsIsA, uint32 iSubChild) const
		{
			return mCategory.IsA(iObjectsIsA, iSubChild);
		}
		//does a check between two different unit types in the category passed in
		FORCEINLINE bool				IsA(const KCString &strCategoryName, uint32 iObjectsIsA, uint32 iSubChild) const
		{
			const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
			if (pCategory)
			{
				return pCategory->IsA(iObjectsIsA, iSubChild);
			}
			return false;
		}

		//does a check between two different unit types in this category
		FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, uint32 iObjectsIsA, const std::string &strSubChild) const
		{
			return mCategory.IsA(iObjectsIsA, strSubChild);
		}
		//does a check between two different unit types in this category
		FORCEINLINE bool				IsA(const KCString &strCategoryName, uint32 iObjectsIsA, const std::string &strSubChild) const
		{
			const KCUnitTypeCategory *pCategory = getCategoryByName(strCategoryName);
			if (pCategory)
			{
				return pCategory->IsA(iObjectsIsA, strSubChild);
			}
			return false;
		}
		//does a check between two different unit types in this category
		FORCEINLINE bool				IsA(const KCUnitTypeCategory &mCategory, const std::string &strObjectsIsA, uint32 iSubChild) const
		{
			return mCategory.IsA(strObjectsIsA, iSubChild);
		}
		//does a check between two different unit types in this category
		FORCEINLINE bool				IsA(const KCString &strCategoryName, const std::string &strObjectsIsA, uint32 iSubChild) const
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
		//NOTE - maybe in the future better to make this a map
		KCTArray<KCUnitTypeCategory>	m_Categories;
	};


}; //end namespace UNITTYPE

