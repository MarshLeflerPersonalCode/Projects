//copyright Marsh Lefler 2000-...
#pragma once
#include "Systems/Stats/Private/KCStatInclude.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//This loads/creates/ manages the global stat definitions.
//
//There is a stat editor that saves out the stats for this editor to load and manage.
//
//
/////////////////////////////////////////////////////////

class KCDatabaseManager;

namespace STATS
{

	class KCCORE_API KCStatManager
	{
	public:
		KCStatManager();
		~KCStatManager();

		//returns the singleton of this class
		static KCStatManager *		getSingleton();

		//initializes the stat manager. NOTE the database manager must be initialized first
		bool						initialize(KCDatabaseManager *pDatabaseManager);

		//returns the stat index
		FORCEINLINE KCStatID		getStatID(const KCName &strName) const
		{
			std::unordered_map<KCName, KCStatID, KCNameHasher>::const_iterator iter = m_StatsByName.find(strName);
			KCEnsureAlwaysReturnVal(iter != m_StatsByName.end(), (KCStatID)0xFFFF);
			return iter->second;
		}

		
		
		//returns the stat definition by ID
		const struct FKCStatDefinition *	getStatDefinitionByID(KCStatID iID) const;
		//returns the stat definition by name
		const struct FKCStatDefinition *	getStatDefinitionByName(KCName strName) const;

	private:
		void						_clean();
		std::unordered_map<KCName, KCStatID, KCNameHasher>		m_StatsByName;
		KCTArray<KCName>										m_StatsByID;
	};


}; //end namespace UNITTYPE

