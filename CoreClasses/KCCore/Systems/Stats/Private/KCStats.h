//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"
#include "KCStatDefinition.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This is the work horse of the stat system. Any object can have 
//a stats handler. Every character or object should.
//
//
//
//Some hard numbers. Each stat is 32bit, 8 bytes
//a Megabyte has 1000000 bytes so we can in theory hold 125,000 stats per megabyte - or...
//If we end up with 100 stats we can get 1250 stat systems to fit into a megabyte. 
//A single level might have 500 units active at any 1 time. 
//each of those units will have a weapon(s) or armor with stats - say 2.
//each player will have 200 items(max); 4 players in a level
//total number of stats will be (500(monsters) * ~2(items)) + (4(players) * 200(items)) = 1800 stat systems
//1800 stat systems, every 1250 stat systems is a megabyte = 1.44 megabytes of stats(could fit on a 3.5 inch disk!)
//PS5 has 16 gigs of ram, or 12 gigs to use - so 1.44MB/((1MB * 100MB)GB * 12GB)) = .0012% will be stats
//
//UPDATE I now have two lists in each stat class so it's twice the memory.
//
// - this system has a lot of waisted space. the problem is that characters and items need to share stats. For instance
//HP_BONUS could be put on the player by an effect but items could also have the stat.
//
//One possible fix in the future would be to split out the stats, so that characters and items have completely
//different stats but yet could know about each other. So player HP could be calculated as HP = ITEM_BONUS_HP + CHARACTER_BONUS_HP.
//that would add a lot of complexity but would lower the memory by probably half.
/////////////////////////////////////////////////////////



class KCCORE_API KCStats : public KCCoreObject 
{
public:		
	KCStats(class KCCoreData *pCoreData, ESTAT_HANDLER_TYPE eType);
	~KCStats();
	//returns if this is a character or item handler
	ESTAT_HANDLER_TYPE					getHandlerType() { return m_eType; }		
	//returns if it's a valid stat ID or not
	DEBUG_FORCEINLINE bool				isValidStatID(KCStatID iStatID) const
	{
		if (getStatManager() &&
			m_pStatDefintions != nullptr &&
			iStatID < m_Stats.Num() &&
			m_StatsOwned[iStatID])
		{
			return true;
		}
		return false;
	}
	//returns if it's a valid stat ID or not
	bool								isValidStatID(const KCName &strName) const;

	//returns the id of a stat by its name
	KCStatID							getStatIDByName(const KCName &strStatName) const;
	//returns the default value of the stat
	coreUnionData32Bit 					getDefaultValue(KCStatID iStat);
	//calculates the value with all children
	int32								getValue(KCStatID iStat, int32 iDefaultValue);
	//calculates the value with all children
	FORCEINLINE uint32					getValue(KCStatID iStat, uint32 iDefaultValue) { return (uint32)getValue(iStat, (int32)iDefaultValue); }
	//calculates the value with all children
	float								getValue(KCStatID iStat, float fDefaultValue);

	//returns the stat value as an int32
	FORCEINLINE int32					getRawValue(KCStatID iStat, int32 iDefaultValue)
	{
		KCEnsureAlwaysReturnVal(isValidStatID(iStat), iDefaultValue);
		return m_Stats[iStat].m_iValue32;
	}
	//returns the stat value as an uint32
	FORCEINLINE uint32					getRawValue(KCStatID iStat, uint32 iDefaultValue)
	{
		KCEnsureAlwaysReturnVal(isValidStatID(iStat), iDefaultValue);
		return m_Stats[iStat].m_uiValue32;
	}

	//returns the stat value as a float
	FORCEINLINE float					getRawValue(KCStatID iStat, float fDefaultValue)
	{
		KCEnsureAlwaysReturnVal(isValidStatID(iStat), fDefaultValue);
		return m_Stats[iStat].m_fValue;
	}
	//returns the stat value as a float
	FORCEINLINE coreUnionData32Bit		getRawValue(KCStatID iStat)
	{
		KCEnsureAlwaysReturnVal(isValidStatID(iStat), (coreUnionData32Bit)0);
		return m_Stats[iStat];
	}



	//sets the value
	FORCEINLINE void					addRawValue(KCStatID iStat, int32 iValue) { setRawValue(iStat, (coreUnionData32Bit)(getRawValue(iStat, 0) + iValue)); }
	FORCEINLINE void					subtractRawValue(KCStatID iStat, int32 iValue) { setRawValue(iStat, (coreUnionData32Bit)(getRawValue(iStat, 0) - iValue)); }
	FORCEINLINE void					setRawValue(KCStatID iStat, int32 iValue) { setRawValue(iStat, (coreUnionData32Bit)iValue); }

	FORCEINLINE void					addRawValue(KCStatID iStat, uint32 iValue) { setRawValue(iStat, (coreUnionData32Bit)(getRawValue(iStat, (uint32)0) + iValue)); }
	FORCEINLINE void					subtractRawValue(KCStatID iStat, uint32 iValue) { setRawValue(iStat, (coreUnionData32Bit)(getRawValue(iStat, (uint32)0) - iValue)); }
	FORCEINLINE void					setRawValue(KCStatID iStat, uint32 iValue) { setRawValue(iStat, (coreUnionData32Bit)iValue); }

	FORCEINLINE void					addRawValue(KCStatID iStat, float fValue) { setRawValue(iStat, (coreUnionData32Bit)(getRawValue(iStat, 0.0f) + fValue)); }
	FORCEINLINE void					subtractRawValue(KCStatID iStat, float fValue) { setRawValue(iStat, (coreUnionData32Bit)(getRawValue(iStat, 0.0f) - fValue)); }
	FORCEINLINE void					setRawValue(KCStatID iStat, float fValue) { setRawValue(iStat, (coreUnionData32Bit)fValue); }

	FORCEINLINE void					setRawValue(KCStatID iStat, coreUnionData32Bit mValue)
	{
		KCEnsureAlwaysReturn(isValidStatID(iStat));
		m_Stats[iStat] = mValue;			
	}

private:
	//configures the stats
	void								_configure(class KCCoreData *pCoreData, ESTAT_HANDLER_TYPE eType);
		
	void								_clean();		
	KCBitArray							m_StatsOwned;		
	ESTAT_HANDLER_TYPE					m_eType = ESTAT_HANDLER_TYPE::ITEM;		
	TArray<coreUnionData32Bit>		m_Stats;		
	const TArray<FKCStatDefinition*>	*m_pStatDefintions = nullptr;

};


