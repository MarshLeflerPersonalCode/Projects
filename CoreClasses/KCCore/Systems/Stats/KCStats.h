//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"
#include "Systems/Stats/Private/KCStatDefinition.h"
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
// - this system has a lot of waisted space. the problem is that characters and items need to share stats. For instance
//HP_BONUS could be put on the player by an effect but items could also have the stat.
//
//One possible fix in the future would be to split out the stats, so that characters and items have completely
//different stats but yet could know about each other. So player HP could be calculated as HP = ITEM_BONUS_HP + CHARACTER_BONUS_HP.
//that would add a lot of complexity but would lower the memory by probably half.
/////////////////////////////////////////////////////////

namespace STATS
{

	enum class ESTAT_HANDLER_TYPE
	{
		CHARACTER,
		ITEM
	};

	class KCCORE_API KCStats
	{
	public:
		KCStats(class KCStatManager *pStatManager, ESTAT_HANDLER_TYPE eType);
		~KCStats();
		//returns if this is a character or item handler
		ESTAT_HANDLER_TYPE					getHandlerType() { return m_eType; }
		bool								getEnabled() const { return !hasFlag(ESTAT_HANDLER_FLAGS::DISABLED); }
		void								setEnabled(bool bEnabled) { setFlagByBool(ESTAT_HANDLER_FLAGS::DISABLED, !bEnabled); }
		//returns if it's a valid stat ID or not
		FORCEINLINE bool					isValidStatID(KCStatID iID) const
		{
			if (m_pStatManager != nullptr &&
				m_pStatDefintions != nullptr &&
				iID < m_Stats.Num() &&
				m_StatsOwned[iID])
			{
				return true;
			}
			return false;
		}
		//returns the id of a stat by its name
		KCStatID							getStatIDByName(const KCName &strStatName) const;
		//returns the default value of the stat
		coreUnionData32Bit 					getDefaultValue(KCStatID iStat);
		//calculates the value with all children
		int32								getCalculatedValueAsInt32(KCStatID iStat);
		//calculates the value with all children
		FORCEINLINE uint32					getCalculatedValueAsUInt32(KCStatID iStat) { return (uint32)getCalculatedValueAsInt32(iStat); }
		//calculates the value with all children
		float								getCalculatedValueAsFloat(KCStatID iStat);

		//returns the stat value as an int32
		FORCEINLINE int32					getValueAsInt32(KCStatID iStat)
		{
			KCEnsureAlwaysReturnVal(isValidStatID(iStat), 0);
			return m_Stats[iStat].m_iValue32;
		}
		//returns the stat value as an uint32
		FORCEINLINE uint32					getValueAsUInt32(KCStatID iStat)
		{
			KCEnsureAlwaysReturnVal(isValidStatID(iStat), 0);
			return m_Stats[iStat].m_uiValue32;
		}

		//returns the stat value as a float
		FORCEINLINE float					getValueAsFloat(KCStatID iStat)
		{
			KCEnsureAlwaysReturnVal(isValidStatID(iStat), 0);			
			return m_Stats[iStat].m_fValue;
		}
		//sets the value
		FORCEINLINE void					setValue(KCStatID iStat, int32 iValue) { setValue(iStat, (coreUnionData32Bit)iValue); }
		FORCEINLINE void					setValue(KCStatID iStat, uint32 iValue) { setValue(iStat, (coreUnionData32Bit)iValue); }
		FORCEINLINE void					setValue(KCStatID iStat, float fValue) { setValue(iStat, (coreUnionData32Bit)fValue); }
		FORCEINLINE void					setValue(KCStatID iStat, coreUnionData32Bit mValue)
		{
			KCEnsureAlwaysReturn(isValidStatID(iStat));
			m_Stats[iStat] = mValue;
			_dirtyStat(iStat);
		}
		FORCEINLINE void					dirtyStat(KCStatID iStat)
		{
			KCEnsureAlwaysReturn(isValidStatID(iStat));
			_dirtyStat(iStat);
		}
	private:
		void								_dirtyStat(KCStatID iStat);
		

		//////////////////////////////////////Not sure I should make this private or not//////////////////////////////////////
		enum class ESTAT_HANDLER_FLAGS
		{
			DISABLED
		};
		void								setFlag(ESTAT_HANDLER_FLAGS eFlag) { MASK_SET_BIT_32BIT(m_iFlags, eFlag); }
		bool								hasFlag(ESTAT_HANDLER_FLAGS eFlag) const { return MASK_TEST_BIT_32BIT(m_iFlags, eFlag); }
		bool								setFlagByBool(ESTAT_HANDLER_FLAGS eFlag, bool bSet) { return (bSet) ? MASK_SET_BIT_32BIT(m_iFlags, eFlag) : MASK_REMOVE_BIT_32BIT(m_iFlags, eFlag); }
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		void								_clean();
		KCBitArray							m_StatsOwned;
		KCBitArray							m_StatsCalculated;
		ESTAT_HANDLER_TYPE					m_eType = ESTAT_HANDLER_TYPE::ITEM;
		int32								m_iFlags = 0;
		class STATS::KCStatManager			*m_pStatManager = nullptr;
		KCTArray<coreUnionData32Bit>		m_Stats;
		KCTArray<coreUnionData32Bit>		m_StatsCalculatedValues;
		const KCTArray<FKCStatDefinition*>	*m_pStatDefintions = nullptr;

	};


}; //end namespace STATS
