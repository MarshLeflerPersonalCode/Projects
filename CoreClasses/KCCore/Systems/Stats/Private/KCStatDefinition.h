//copyright Marsh Lefler 2000-...
#pragma once
#include "KCStatInclude.h"
#include "Database/KCDBEntry.h"
#include "KCStatDefinition.serialize.inc"
namespace STATS
{

	class FKCStatDefinition : public FKCDBEntry
	{
	public:
		KCSERIALIZE_CODE();

		FKCStatDefinition() { setDatabaseTable(DATABASE::EDATABASE_TABLES::STATS);}
		~FKCStatDefinition() {}

		UPROPERTY(Category = "GENERAL", DisplayName = "TYPE")
		ESTAT_PRIMITIVE_TYPES				m_eStatType = ESTAT_PRIMITIVE_TYPES::INT32;

		UPROPERTY(Category = "GENERAL", DisplayName = "Default Value")
		KCString							m_strDefaultValue = "0";

		UPROPERTY(Category = "GENERAL", DisplayName = "Min Value")
		KCString							m_strMinValue = "0";

		UPROPERTY(Category = "GENERAL", DisplayName = "Max Value")
		KCString							m_strMaxValue = "0";

		UPROPERTY(Category = "STAT MODIFICATION", DisplayName = "Code Name")
		KCString							m_strCodeName = "";
	};


}; //end namespace STATS

