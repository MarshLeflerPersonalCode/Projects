//copyright Marsh Lefler 2000-...
#pragma once
#include "KCIncludes.h"
#include "KCStatDefinition.serialize.inc"
namespace STATS
{

	class KCStatDefinition
	{
	public:
		KCSERIALIZE_CODE();
		KCStatDefinition() {}
		~KCStatDefinition() {}

		UPROPERTY(Category = "GENERAL", DisplayName = "Code Name")
		KCString							m_strCodeName = "";


	};


}; //end namespace STATS

