#pragma once
#include "Database/Private/KCDBInclude.h"
#include "KCDBEntry.serialize.inc"
//copyright Marsh Lefler 2000-...
//For our tools to work all our database entries must extend this base class.
//Not much in here except for a guid and name



struct FKCDBEntry
{
	KCSERIALIZE_CODE();
	FKCDBEntry() {}
	~FKCDBEntry() {}

	//returns if the guid and name are valid
	bool							isValid() const  { return DATABASE::isDatabaseGuidValid(m_DatabaseGuid) && m_strName.isEmpty() == false; }

	//returns if the entry is equal to another entry
	bool							operator==(const FKCDBEntry &mEntry) const { return (m_strName == mEntry.m_strName && m_DatabaseGuid == mEntry.m_DatabaseGuid)?true:false; }

	//returns if the entries are not equal to another entry
	bool							operator!=(const FKCDBEntry &mEntry) const { return (m_strName != mEntry.m_strName || m_DatabaseGuid != mEntry.m_DatabaseGuid) ? true : false; }

	//returns the table the entry is in.
	DATABASE::EDATABASE_TABLES		getDatabaseTable() const
	{
		uint8 iTableValue = DATABASE::getDatabaseGuidTableMaskAsUInt8(m_DatabaseGuid);
		if( iTableValue < (uint8)DATABASE::EDATABASE_TABLES::COUNT){ return (DATABASE::EDATABASE_TABLES)iTableValue; }
		return DATABASE::EDATABASE_TABLES::UNDEFINED;
	}

	//sets the database table
	void							setDatabaseTable(DATABASE::EDATABASE_TABLES eTable)
	{
		DATABASE::setDatabaseGuidTable(m_DatabaseGuid, (uint8)eTable);
	}
	
	//the database guid. Must be unique
	KCPROPERTY(Category = "DATABASE", DisplayName = "Database Guid", ReadOnly)
	KCDatabaseGuid					m_DatabaseGuid = UNINITIALIZED_DATABASE_GUID;


	//the name of the entry. Must be unique
	KCPROPERTY(Category = "DATABASE", DisplayName = "Name", ReadOnly)
	KCName							m_strName;
	//The filename of this entry
	KCPROPERTY(Category = "DATABASE", DisplayName = "Name", ReadOnly)
	KCString						m_strFileName;
};
