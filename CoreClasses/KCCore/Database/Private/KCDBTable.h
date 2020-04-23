//copyright Marsh Lefler 2000-...
#pragma once
#include "Database/Private/KCDBInclude.h"
#include "utils/Containers/KCName.h"
#include "Systems/DataGroup/KCDataGroupManager.h"
#include "Systems/Stats/Private/KCStatDefinition.h"

///////////////////////////////////////////////////////////////////////////
//The database manager holds all the tables for storing the games data.
//
///////////////////////////////////////////////////////////////////////////
template<class T>
class KCDBTable
{
public:
	KCDBTable(DATABASE::EDATABASE_TABLES eTable, const KCString &strDatabaseFolder)
	{
		m_eDatatable = eTable;
		KCTArray<const KCDataGroup*> mDataGroups(200);
		KCDataGroupManager::getSingleton()->getDataGroupsInDirectory(strDatabaseFolder, mDataGroups);
		for (uint32 iIndex = 0; iIndex < mDataGroups.Num(); iIndex++)
		{
			T *pEntryObject = KC_NEW T();
			FKCDBEntry *pEntry = (FKCDBEntry *)pEntryObject;
			pEntryObject->deserialize(*mDataGroups[iIndex]);
			_addEntry(pEntry);
		}
	}
	~KCDBTable()
	{
		_clean();
	}

	//returns all the DB entries
	const KCTArray< T * > &			getEntries() const { return m_Entries; }

	//returns the table type
	DATABASE::EDATABASE_TABLES		getTableType() const { return m_EntriesByGuid;}

	//returns the entry
	const T *						getEntry(KCDatabaseGuid iGuid) const
	{
		auto mEntry = m_EntriesByGuid.find(iGuid);
		if (mEntry != m_EntriesByGuid.end())
		{
			return _getEntryByIndex(mEntry.second);
		}
		return nullptr;
	}

	//returns the entry
	const T *						getEntry(KCName strName) const
	{
		auto mEntry = m_EntriesByName.find(iGuid);
		if (mEntry != m_EntriesByName.end())
		{
			return _getEntryByIndex(mEntry.second);
		}
		return nullptr;
	}

protected:
	//returns the entry by ID
	FORCEINLINE T	*				_getEntryByIndex(int32 iIndex)
	{
		KCEnsureAlwaysReturnVal( iIndex >= 0 && iIndex < m_Entries.Num(), nullptr);
		return m_Entries[iIndex];
	}

	void							_addEntry(FKCDBEntry *pEntry)
	{
		KCEnsureAlwaysMsgReturn(pEntry, "Entry must not be null");
		m_EntriesByGuid[pEntry->m_DatabaseGuid] = m_Entries.Num();
		m_EntriesByGuid[pEntry->m_strName] = m_Entries.Num();
		m_Entries.Add((T *)pEntry);
		
	}

	//deletes all the entries and cleans up the table
	void							_clean()
	{
		m_Entries.deleteContents();
		m_EntriesByGuid.clear();
		m_EntriesByName.clear();
	}
	DATABASE::EDATABASE_TABLES		m_eDatatable = DATABASE::EDATABASE_TABLES::UNDEFINED;
	KCTArray< T * >					m_Entries;
	std::unordered_map<KCDatabaseGuid, int32>					m_EntriesByGuid;
	std::unordered_map<KCName, int32, KCNameHasher>				m_EntriesByName;
};


