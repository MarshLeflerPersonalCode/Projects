//copyright Marsh Lefler 2000-...
#pragma once
#include "Database/Private/KCDBInclude.h"
#include "utils/Containers/KCName.h"

///////////////////////////////////////////////////////////////////////////
//The database manager holds all the tables for storing the games data.
//
///////////////////////////////////////////////////////////////////////////
template<class T>
class KCDBTable
{
public:
	KCDBTable(DATABASE::EDATABASE_TABLES eTable)
	{
		m_eDatatable = eTable;
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

	//deletes all the entries and cleans up the table
	void							_clean()
	{
		m_Entries.deleteContents();
		m_EntriesByGuid.clear();
		m_EntriesByName.clear();
	}
	DATABASE::EDATABASE_TABLES		m_eDatatable = DATABASE::EDATABASE_TABLES::UNDEFINED;
	KCTArray< T * >					m_Entries;
	std::unordered_map<KCDatabaseGuid, int32, KCNameHasher>		m_EntriesByGuid;
	std::unordered_map<KCName, int32, KCNameHasher>				m_EntriesByName;
};


