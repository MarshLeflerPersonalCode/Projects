//copyright Marsh Lefler 2000-...
#pragma once
#include "Systems/Stats/Private/KCStats.h"
#include "Systems/Stats/Private/IKCStatModifier.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This is the glue that manages how the stats get manipulated.
//
//Stats go through a lot of calculations.
//
//
/////////////////////////////////////////////////////////

class KCDatabaseManager;
struct FKCStatDefinition;

class KCCORE_API KCStatHandler : public KCCoreObject, public IKCStatModifier
{
public:
	KCStatHandler(class KCCoreData *pCoreData, KCUnitType eOwnerUnitType);
	~KCStatHandler();

	//sets the raw value of a stat
	bool								setRawValue(KCStatID iStatID, int32 iValue);
	//sets the raw value of a stat
	bool								setRawValue(KCStatID iStatID, float fValue);
	//increments a stat
	bool								incrementRawValue(KCStatID iStatID, int32 iValueToAdd);
	//increments a stat
	bool								incrementRawValue(KCStatID iStatID, float fValueToAdd);

	//call this on weapons to get the stats calculated by the parent and the weapon, plus any sockets - but not other weapons equipped
	int32								getStatValueForWeapon(KCStatID iStatID, int32 iDefaultValue);	
	//call this on weapons to get the stats calculated by the parent and the weapon, plus any sockets - but not other weapons equipped
	float								getStatValueForWeapon(KCStatID iStatID, float fDefaultValue);
	//returns the stat value for only this stat handler. It won't include children
	int32								getStatValueSelfOnly(KCStatID iStatID, int32 iDefaultValue);
	//returns the stat value for only this stat handler. It won't include children
	float								getStatValueSelfOnly(KCStatID iStatID, float fDefaultValue);
	//returns the full hierarchy of a stat value. You can pass in a unittype to ignore
	int32								getStatValueFullHierarchy(KCStatID iStatID, int32 iDefaultValue, KCUnitType eUnitTypeToIgnore = INVALID);
	//returns the full hierarchy of a stat value. You can pass in a unittype to ignore
	float								getStatValueFullHierarchy(KCStatID iStatID, float fDefaultValue, KCUnitType eUnitTypeToIgnore = INVALID);

	//returns the stat value as an int. By default it includes children
	int32								getStatValue(KCStatID iStatID, int32 iDefaultValue, ESTAT_INHERIT_TYPES eInheritType = ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN, KCUnitType eUnitTypeToIgnore = INVALID);
	//returns the stat value as an int. By default it includes children
	uint32								getStatValue(KCStatID iStatID, uint32 iDefaultValue, ESTAT_INHERIT_TYPES eInheritType = ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN, KCUnitType eUnitTypeToIgnore = INVALID) { return (uint32)getStatValue(iStatID, (int32)iDefaultValue, eInheritType, eUnitTypeToIgnore); }
	//returns the stat value as an int. By default it includes children
	float								getStatValue(KCStatID iStatID, float fDefaultValue, ESTAT_INHERIT_TYPES eInheritType = ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN, KCUnitType eUnitTypeToIgnore = INVALID);

	//the getValueCalculated doesn't include graphs. If for some reason we need to use that function you need to call this to get the graph value.
	coreUnionData32Bit					getGraphValueForStatByValue(KCStatID iStatID, coreUnionData32Bit mValue);
	/////////////////////////////////////////////////////////////////////////////////
	//Stat Modifier Interface
	//returns the value of a stat fully calculated. This doesn't return the graph value. used the predefined functions above for that. Else call getGraphValueForStatByValue(that should never be used)
	virtual bool						getValueCalculated(FKCStatCalculationParams &mParams, coreUnionData32Bit &mValue) override;
	//returns the stat value. Returning false means it doesn't support that stat
	virtual bool						getRawStatValue(KCStatID iStatID, coreUnionData32Bit &mValue) override;
	//Should return the unit type the stat owner is
	virtual KCUnitType					getUnitType() const override { return m_eUnitTypeOfOwner; }
	//this gets called when a stat is relying on a stat that was just changed. For instance if a stat that relied on RANK to do a calculation and RANK was changed this function gets called with the stat relying on RANK
	virtual void						dirtyStat(KCStatID iStatID) override;

	//Stat Modifier Interface
	/////////////////////////////////////////////////////////////////////////////////

	//returns the stat definition
	const FKCStatDefinition *			getStatDefinition(KCStatID iStatID);
	//returns if it's a valid stat ID or not
	FORCEINLINE bool					isValidStatID(KCStatID iID) const { return (m_pStats && m_pStats->isValidStatID(iID)) ? true : false; }
	//returns if it's a valid stat ID or not
	FORCEINLINE bool					isValidStatID(const KCName &strName) const 
	{ 
		return (m_pStats && m_pStats->isValidStatID(strName)) ? true : false;
	}
protected:
	//sets the parent stat modifier
	virtual void						setStatModifierOwner(IKCStatModifier *pOwner) override;

private:
	//calculates the value based on the graph value and returns type the result value is
	ESTAT_PRIMITIVE_TYPES				_calculateGraphValue(const FKCStatDefinition *pStatDefinition, coreUnionData32Bit mCurrentValue, coreUnionData32Bit &mResultValue);
	//not this doesn't do any checks. Assumes checks have already been done.
	void								_dirtyStat(KCStatID iStat);
	void								_clean();
	KCUnitType							m_eUnitTypeOfOwner = 0;
	KCStats								*m_pStats = nullptr;	
	std::unordered_map<int32, coreUnionData32Bit>	m_CalculatedStats;
	
};



