//copyright Marsh Lefler 2000-...
#pragma once
#include "IKCStatMathFunction.h"
#include "Systems/Stats/KCStats.h"
#include "KCStatMathFunction.serialize.inc"

///////////////////////////////////////////////////////////
//HOW TO USE
//
//Interface for doing math on stats
/////////////////////////////////////////////////////////

namespace STATS
{
	
	//this math function simply adds another stat onto the current stat
	//Current = Current + Stat
	class KCStatMathFunctionAdd : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{			
			return iValue + pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue + pStats->getCalculatedValueAsFloat(pStats->getStatIDByName(m_strStat));
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta=(List="Stats"))
		KCName				m_strStat;
	};
	//this math function simply subtracts another stat from the current stat
	//Current = Current - Stat
	class KCStatMathFunctionSubtract : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{
			return iValue - pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue - pStats->getCalculatedValueAsFloat(pStats->getStatIDByName(m_strStat));
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta = (List = "Stats"))
		KCName				m_strStat;
	};
	//this math function divides the current stat by the second stat.
	//Current = Current / Stat
	class KCStatMathFunctionDivide : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{
			return iValue / pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue / pStats->getCalculatedValueAsFloat(pStats->getStatIDByName(m_strStat));
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta = (List = "Stats"))
		KCName				m_strStat;
	};
	//this math function multiplies the stat specified by the current stat
	//Current = Current * Stat
	class KCStatMathFunctionMultiply : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{
			return iValue * pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue * pStats->getCalculatedValueAsFloat(pStats->getStatIDByName(m_strStat));
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta = (List = "Stats"))
		KCName				m_strStat;
	};

	//this math function divides the stat specified by the denominator and then multiplies it by the current stat. Current Stat
	//Current = Current * ( Stat/Denominator )
	class KCStatMathFunctionDivideAndMultiply : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{
			return (int32)((double)iValue * ((double)pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat))/ (double)m_fDenominator));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue * (pStats->getCalculatedValueAsFloat(pStats->getStatIDByName(m_strStat)) / m_fDenominator);
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta = (List = "Stats"))
		KCName				m_strStat;
		//the denominator
		UPROPERTY(Category = "GENERAL", DisplayName = "Denominator")
		float				m_fDenominator = 100.0f;
	};

	//this math function is meant to be used as a percent value. Where stat could be from -100 to 100. So it could look like this: Current * ( 1 + ( Stat/100)). If Stat was 0 then there would be no bonus to current stat. Great for bonus values
	//Current = Current * (Value +( Stat/Denominator ))
	class KCStatMathFunctionDivideAddAndMultiply : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{
			return (int32)((double)iValue * ((double)m_fValue + ((double)pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat)) / (double)m_fDenominator)));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue * (m_fValue + (pStats->getCalculatedValueAsFloat(pStats->getStatIDByName(m_strStat)) / m_fDenominator));
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta = (List = "Stats"))
		KCName				m_strStat;
		//the denominator
		UPROPERTY(Category = "GENERAL", DisplayName = "Denominator")
		float				m_fDenominator = 100.0f;
		//the value
		UPROPERTY(Category = "GENERAL", DisplayName = "Value")
		float				m_fValue = 1.0f;
	};

	//this math function divides the stat by the denominator, multiplies it by the current stat and adds that back onto the stat
	//Current = Current + ( Current * ( Stat/Denominator ))
	class KCStatMathFunctionDivideAddAndMultiply : public IKCStatMathFunction
	{
	public:
		KCSERIALIZE_CODE();
		virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const
		{
			return iValue + (int32)((double)iValue * ((double)pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat)) / (double)m_fDenominator));
		}
		virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
		{
			return fValue + (fValue * (pStats->getCalculatedValueAsInt32(pStats->getStatIDByName(m_strStat)) / m_fDenominator));
		}
		//the stat that will be added onto the current stat
		UPROPERTY(Category = "GENERAL", DisplayName = "Stat", Meta = (List = "Stats"))
		KCName				m_strStat;
		//the denominator
		UPROPERTY(Category = "GENERAL", DisplayName = "Denominator")
		float				m_fDenominator = 100.0f;

	};

}; //end namespace STATS
