//copyright Marsh Lefler 2000-...
#pragma once
#include "Database/KCDBEntry.h"
#include "Database/Private/KCDBTable.h"
///////////////////////////////////////////////////////////////////////////
//The database manager holds all the tables for storing the games data.
//
///////////////////////////////////////////////////////////////////////////



class KCDatabaseManager
{
public:
	KCDatabaseManager();
	~KCDatabaseManager();

	static KCDatabaseManager *						getSingleton();

	//returns the table by the enum
	FORCEINLINE const KCDBTable<FKCDBEntry> *		getTable(DATABASE::EDATABASE_TABLES eTable) const
	{
		KCEnsureAlwaysReturnVal(eTable != DATABASE::EDATABASE_TABLES::UNDEFINED && eTable != DATABASE::EDATABASE_TABLES::COUNT, nullptr);
		return m_Tables[(int32)eTable];
	}

	//reloads the databases
	void											reload();

	
	

	///////////////////////////////////////////////////////////////////////////////////////
	//									BY NAME											 //
	static const struct STATS::FKCStatDefinition *	getStatDefinitionByName(const KCName &strName);


	//									BY NAME											 //
	///////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//									BY ID											 //
	static const struct STATS::FKCStatDefinition *	getStatDefinitionByGuid(KCDatabaseGuid iGuid);

	//									BY ID											 //
	///////////////////////////////////////////////////////////////////////////////////////
private:
	void											_clean();
	//initializes the DB Manager and creates the databases
	void											_initialize();

	KCDBTable<FKCDBEntry>							*m_Tables[(uint32)DATABASE::EDATABASE_TABLES::COUNT];
};
