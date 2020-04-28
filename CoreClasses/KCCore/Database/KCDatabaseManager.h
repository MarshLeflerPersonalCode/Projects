//copyright Marsh Lefler 2000-...
#pragma once
#include "Database/KCDBEntry.h"
#include "Database/Private/KCDBTable.h"

///////////////////////////////////////////////////////////////////////////
//The database manager holds all the tables for storing the games data.
//
///////////////////////////////////////////////////////////////////////////



class KCDatabaseManager : public KCCoreObject
{
public:
	KCDatabaseManager();
	~KCDatabaseManager();

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
	const struct FKCStatDefinition *				getStatDefinitionByName(const KCName &strName) const;


	//									BY NAME											 //
	///////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////
	//									BY ID											 //
	const struct FKCStatDefinition *				getStatDefinitionByGuid(KCDatabaseGuid iGuid) const;

	//									BY ID											 //
	///////////////////////////////////////////////////////////////////////////////////////
private:
	void											_clean();
	//initializes the DB Manager and creates the databases
	void											_initialize();	
	KCDBTable<FKCDBEntry>							*m_Tables[(uint32)DATABASE::EDATABASE_TABLES::COUNT];
};
