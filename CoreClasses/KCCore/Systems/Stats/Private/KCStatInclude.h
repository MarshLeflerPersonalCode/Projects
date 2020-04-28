//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"




enum class ESTAT_PRIMITIVE_TYPES
{
	INT32,
	FLOAT
};


enum class ESTAT_HANDLER_TYPE
{
	CHARACTER,
	ITEM
};

enum class ESTAT_AGGREGATE_TYPES
{
	ADD,
	SUBTRACT,
	MULTIPLE
};

enum class ESTAT_INHERIT_TYPES
{
	SELF									UMETA(DisplayName = "Self Only", ToolTip = "This won't get any stats from any other children or parents"),
	SELF_AND_CHILDREN						UMETA(DisplayName = "Self And Children", ToolTip = "This gets all the stats from the current stat handler and it's children"),
	FULL_HIERARCHY							UMETA(DisplayName = "Full Hierarchy", ToolTip = "This will get all the stats accumalated by walking up the parent and then getting all the stats including children"),
};

//all the params for calculating a stat
struct FKCStatCalculationParams
{
	//if you copy and change anything you'll get the same has hash code, but shouldn't. So we need a way to reset it.
	void							resetHashCode() { m_iStatHashCode = 0; }
	//the original stat modifier
	class IKCStatModifier			*m_pOriginalParent = nullptr;
	//the stat manager
	const class KCStatManager		*m_pStatManager = nullptr;
	//the stat definition	
	const struct FKCStatDefinition	*m_pStatDefinition = nullptr;
	//the stat ID to calculate
	KCStatID						m_iStatID = INVALID;
	//unit type to ignore
	KCUnitType						m_eUnitTypeToIgnore = INVALID;
	//what type of inheritance we are doing
	ESTAT_INHERIT_TYPES				m_eInheritType = ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN;
	//is this a valid request
	bool							isValid() { return (m_iStatID != INVALID && m_pStatDefinition != null && m_pStatManager != nullptr) ? true : false; }
	//returns the hash code.
	int32							getStatHashCode()
	{
		if (m_iStatHashCode == 0)
		{
			if (isValid() == false)
			{
				return false;
			}
			//this combines the stat id(0xFFF or 4095 stats allowed)
			//with the unit type 0xFFF << 12(4095 unittypes allowed)
			//with the InheritTYpe << 28
			//this leave bits 24-27 open for the future!
			m_iStatHashCode = m_iStatID & 0xFFF;
			m_iStatHashCode = m_iStatHashCode | ((m_eUnitTypeToIgnore & 0xFFF) << 12);
			m_iStatHashCode = m_iStatHashCode | ((uint8)m_eInheritType << 28);
		}
		return m_iStatHashCode;		
	}

private:
	int32					m_iStatHashCode = 0;
};


