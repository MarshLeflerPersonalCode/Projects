//copyright Marsh Lefler 2000-...
#pragma once
#include "Systems/Stats/Private/KCStatInclude.h"
#include "KCCoreObject.h"
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


class KCCORE_API KCStatManager : public KCCoreObject
{
public:
	KCStatManager();
	~KCStatManager();


	//initializes the stat manager. NOTE the database manager must be initialized first
	bool									initialize();

	//returns the stat index
	FORCEINLINE KCStatID					getStatID(const KCName &strName) const
	{
		std::unordered_map<KCName, KCStatID, KCNameHasher>::const_iterator iter = m_StatsByName.find(strName);
		KCEnsureAlwaysReturnVal(iter != m_StatsByName.end(), (KCStatID)0xFFFF);
		return iter->second;
	}
	//returns a bit array that tells if a stat is on a character or not.
	FORCEINLINE const KCBitArray &			getCharacterHasStatBitArray() const { return m_CharacterStatsByID; }
	//returns a bit array that tells if a stat is on an item or not.
	FORCEINLINE const KCBitArray &			getItemHasStatBitArray() const { return m_ItemsStatsByID; }
	//returns an array of all the default values
	FORCEINLINE const TArray< coreUnionData32Bit> & getDefaultValues() const { return m_DefaultValues; }
		
	//returns the stat definition by ID
	const struct FKCStatDefinition *		getStatDefinitionByID(KCStatID iID) const;
	//returns the stat definition by name
	const struct FKCStatDefinition *		getStatDefinitionByName(KCName strName) const;
	//returns the array of stat definitions
	const TArray<FKCStatDefinition * > *	getStatDefinitions() const;
		

private:
	void									_clean();
	std::unordered_map<KCName, KCStatID, KCNameHasher>		m_StatsByName;
	TArray<KCName>						m_StatsByID;
	KCBitArray								m_CharacterStatsByID;
	KCBitArray								m_ItemsStatsByID;
	TArray< coreUnionData32Bit>			m_DefaultValues;		
	TArray< const FKCStatDefinition * >	m_StatDefinitions;
};

