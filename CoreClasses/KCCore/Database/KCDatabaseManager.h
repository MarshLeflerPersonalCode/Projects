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

	//returns the table by the enum
	const KCDBTable<FKCDBEntry> *	getTable(DATABASE::EDATABASE_TABLES eTable);

	//reloads the databases
	void							reload();

private:
	void							_clean();
	//initializes the DB Manager and creates the databases
	void							_initialize();

	KCDBTable<FKCDBEntry>			*m_Tables[(uint32)DATABASE::EDATABASE_TABLES::COUNT];
};
