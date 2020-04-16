#pragma once
#include "Utils/TypeDefinitions/KCPrimitiveTypesDefinition.h"
#include "Utils/Helpers/KCBitAndMaskUtils.h"
//copyright Marsh Lefler 2000-...
//The database guid is a 32bit value that is broken up into multiple bits to tell what
//type of database it is as well as a unique ID.

typedef uint32	KCDatabaseGuid;


namespace DATABASE
{

//this is 0000 0000 0000 0000 1111 1111 1111 1111
#define DB_GUID_MASK_ID						0x0000FFFF
//this is 0111 1000 0000 0000 0000 0000 0000 0000
#define DB_GUID_MASK_TABLES					0x78000000
#define DB_GUID_MASK_TABLES_SHIFT			27
//this is 1000 0000 0000 0000 0000 0000 0000 0000
#define DB_GUID_MASK_INVALID				0x80000000
#define DB_GUID_MASK_INVALID_SHIFT			31

	static FORCEINLINE bool isDatabaseGuidValid(KCDatabaseGuid iDatabaseGuid)
	{
		return ((iDatabaseGuid & DB_GUID_MASK_INVALID) == 0 && (iDatabaseGuid & DB_GUID_MASK_TABLES) != 0 && (iDatabaseGuid & DB_GUID_MASK_ID) != 0) ? true : false;
	}
	//gets the table mask an then shifts it to fit in a uint8
	static FORCEINLINE uint8 getDatabaseGuidTableMaskAsUInt8(KCDatabaseGuid iDatabaseGuid)
	{
		return (uint8)((iDatabaseGuid & DB_GUID_MASK_TABLES) >> 27);
	}

	//sets the database guid table by ID - return false if the datatable isn't valid
	static FORCEINLINE bool setDatabaseGuidTable(KCDatabaseGuid &iDatabaseGuid, uint8 iTableID)
	{
		iDatabaseGuid = MASK_REMOVE_SECOND_MASK(iDatabaseGuid, DB_GUID_MASK_TABLES);
		MASK_ADD_SECOND_MASK(iDatabaseGuid, ((KCDatabaseGuid)iTableID << DB_GUID_MASK_TABLES_SHIFT));
		return (getDatabaseGuidTableMaskAsUInt8(iDatabaseGuid) != 0)?true:false;
	}

};//end namespace


#define UNINITIALIZED_DATABASE_GUID		0

//returns if the database guid is valid or not
#define isValidDatabaseGuid(iGuid)		DATABASE::isDatabaseGuidValid((KCDatabaseGuid)iGuid)
