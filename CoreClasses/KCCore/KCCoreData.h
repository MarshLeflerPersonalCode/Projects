//copyright Marsh Lefler 2000-...
#pragma once
#include "Systems/UnitTypes/KCUnitTypeManager.h"
#include "Systems/Stats/KCStatManager.h"
#include "Systems/DataGroup/KCDataGroupManager.h"
#include "Database/KCDatabaseManager.h"
///////////////////////////////////////////////////////////
//HOW TO USE
//
//Simple wrapper that gets passed along to most objects. From this
//class you should be able to access everything in the game.
//
/////////////////////////////////////////////////////////



class KCCORE_API KCCoreData
{
public:
	KCCoreData();	
	~KCCoreData();
	
	//initializes the core data
	void								_initialize();
	//returns the data group manager
	KCDataGroupManager *				getDataGroupManager() { return &m_DataGroupManager; }
	const KCDataGroupManager *			getDataGroupManager() const { return &m_DataGroupManager; }
	//returns the data base manager	
	const KCDatabaseManager *			getDatabaseManager() const { return &m_DatabaseManager; }
	//returns the stat manager	
	const KCStatManager *				getStatManager() const { return &m_StatManager; }
	//returns the stat manager	
	const KCUnitTypeManager *			getUnitTypeManager() const { return &m_UnitTypeManager; }

private:
	KCDataGroupManager					m_DataGroupManager;
	KCDatabaseManager					m_DatabaseManager;
	KCStatManager						m_StatManager;
	KCUnitTypeManager					m_UnitTypeManager;
};

