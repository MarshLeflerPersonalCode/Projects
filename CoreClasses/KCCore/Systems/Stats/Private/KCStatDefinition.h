//copyright Marsh Lefler 2000-...
#pragma once
#include "KCStatInclude.h"
#include "Database/KCDBEntry.h"
#include "Systems/Stats/MathFunctions/IKCStatMathFunction.h"
#include "KCStatDefinition.serialize.inc"
namespace STATS
{ 

	struct FKCStatDefinition : public FKCDBEntry
	{
	public:
		KCSERIALIZE_CODE();

		FKCStatDefinition() { setDatabaseTable(DATABASE::EDATABASE_TABLES::STATS);}
		~FKCStatDefinition() {}

		//The stat type dictates how it will be used in game.
		UPROPERTY(Category = "GENERAL", DisplayName = "Type")
		ESTAT_PRIMITIVE_TYPES				m_eStatType = ESTAT_PRIMITIVE_TYPES::INT32;
		//The default value of the stat.
		UPROPERTY(Category = "GENERAL", DisplayName = "Default Value")
		KCString							m_strDefaultValue = "0";
		//The min value of the stat.
		UPROPERTY(Category = "GENERAL", DisplayName = "Min Value")
		KCString							m_strMinValue = "0";
		//The max value of the stat.
		UPROPERTY(Category = "GENERAL", DisplayName = "Max Value")
		KCString							m_strMaxValue = "0";
		//Will this stat exist on characters?
		UPROPERTY(Category = "APPLICABLE", DisplayName = "Characters")
		bool								m_bApplicableToCharacters = true;
		//Will this stat exist on items?
		UPROPERTY(Category = "APPLICABLE", DisplayName = "Items")
		bool								m_bApplicableToItems = true;
		//the graph that will be used to generate the final value. The graph stat should be a float
		UPROPERTY(Category = "GRAPH", DisplayName = "Graph Name", Meta = (List = "Graphs"))
		KCString							m_strGraph = "";
		//The stat which will be used in the graph. Most times it's the rank.
		UPROPERTY(Category = "GRAPH", DisplayName = "Graph Stat", Meta = (List = "Stats"))
		KCString							m_strGraphStat = "Rank";
		//functions that will do math on the stat. The functions get ran in order 0-to end.
		UPROPERTY(Category = "MISC", DisplayName = "Functions")
		KCTArray<IKCStatMathFunction>		m_MathFunctions;
		//Stats need to know what other stats are referencing them. This is the array of stat ids
		UPROPERTY(Category = "MISC", DisplayName = "Stat Refs", Meta = (Hidden))
		KCTArray<int32>						m_StatsReferencing;
		//When this stat changes all the stats will need to be recalculate
		UPROPERTY(Category = "MISC", DisplayName = "Dirty All Stats" )
		bool								m_bDirtyAllStats = false;
	};


}; //end namespace STATS

