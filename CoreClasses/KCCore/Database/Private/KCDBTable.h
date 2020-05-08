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
	KCDBTable(const KCDataGroupManager *pDatagroupManager, DATABASE::EDATABASE_TABLES eTable, const KCString &strDatabaseFolder)
	{
		m_eDatatable = eTable;
		TArray<const KCDataGroup*> mDataGroups;
		mDataGroups.Reserve(200);
		pDatagroupManager->getDataGroupsInDirectory(strDatabaseFolder, mDataGroups);
		for (int32 iIndex = 0; iIndex < mDataGroups.Num(); iIndex++)
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
	const TArray< T * > &			getEntries() const { return m_Entries; }

	//returns the table type
	DATABASE::EDATABASE_TABLES		getTableType() const { return m_EntriesByGuid;}

	//returns the entry
	const T *						getEntry(KCDatabaseGuid iGuid) const
	{
		auto mEntry = m_EntriesByGuid.find(iGuid);
		if (mEntry != m_EntriesByGuid.end())
		{
			return getEntryByIndex(mEntry->second);
		}
		return nullptr;
	}

	//returns the entry
	const T *						getEntry(KCName strName) const
	{
		auto mEntry = m_EntriesByName.find(strName);
		if (mEntry != m_EntriesByName.end())
		{
			return getEntryByIndex(mEntry->second);
		}
		return nullptr;
	}
	//returns the count of entries
	uint32							getCountOfEntries() const { return m_Entries.Num(); }
	//returns the entry by index
	FORCEINLINE const T	*			getEntryByIndex(uint32 iIndex) const
	{
		KCEnsureAlwaysReturnVal(iIndex >= 0 && iIndex < (uint32)m_Entries.Num(), nullptr);
		return m_Entries[iIndex];
	}

protected:

	void							_addEntry(FKCDBEntry *pEntry)
	{
		KCEnsureAlwaysMsgReturn(pEntry, "Entry must not be null");
		m_EntriesByGuid[pEntry->m_DatabaseGuid] = m_Entries.Num();
		m_EntriesByName[pEntry->m_strName] = m_Entries.Num();
		m_Entries.Add((T *)pEntry);
		
	}

	//deletes all the entries and cleans up the table
	void							_clean()
	{
		for (int32 iIndex = 0; iIndex < m_Entries.Num(); iIndex++)
		{
			DELETE_SAFELY(m_Entries[iIndex]);
		}
		m_Entries.Empty();
		m_EntriesByGuid.clear();
		m_EntriesByName.clear();
	}
	DATABASE::EDATABASE_TABLES		m_eDatatable = DATABASE::EDATABASE_TABLES::UNDEFINED;
	TArray< T * >					m_Entries;
	std::unordered_map<KCDatabaseGuid, uint32>			m_EntriesByGuid;
	std::unordered_map<KCName, uint32, KCNameHasher>	m_EntriesByName;
};


