//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"
#include "IKCStatMathFunction.serialize.inc"

///////////////////////////////////////////////////////////
//HOW TO USE
//
//Interface for doing math on stats
/////////////////////////////////////////////////////////


class IKCStatMathFunction
{
public:
	KCSERIALIZE_CODE();
	IKCStatMathFunction() {}
	virtual ~IKCStatMathFunction() {}
	//Does the actual math function		
	virtual int32 calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, int32 iValue) const 
	{
		return iValue;
	}
	virtual float calculateStat(class KCStats *pStats, const struct FKCStatDefinition *pStatDef, float fValue) const
	{
		return fValue;
	}
};

