//copyright Marsh Lefler 2000-...
//Like the UE4 FName this is my own version. Does quick look ups by keeping the ID of the name for comparison
#pragma once
#include "KCDefines.h"


class KCName
{
public:
	KCName() { _setString(EMPTY_KCSTRING); }
	KCName(const char *strString) { _setString((strString == null)?EMPTY_KCSTRING:KCString(strString)); }
	KCName(const KCString strString) { _setString(strString); }
	~KCName() {}
	//returns if its empty
	FORCEINLINE bool		isEmpty() const { return (m_iUniqueID == INVALID || m_iUniqueID == 0 )?true:false; }
	//returns the string table used for KCName
	static class KCStringTable &	getStringTable();
	//returns the KCName by ID
	static KCName			getNameFromID(uint32 iStringID);
	//returns the ID of the string
	FORCEINLINE uint32		getValue() const{ return m_iUniqueID; }

	//returns the KCName as an int
							operator int() const{ return m_iUniqueID; }

	//sets the Name
	KCName &				operator=(const KCName &mName)
	{
		m_iUniqueID = mName.m_iUniqueID;
#ifdef _DEBUG
		m_strString = mName.m_strString;
#endif
		return *this;
	}

	//sets the Name
	KCName &				operator=(const KCString &strString)
	{
		_setString(strString);
		return *this;
	}

	//returns the string
	const KCString &		toString() const;


	//returns if the names are the same
	bool					operator==(const KCName &mName) const
	{
		return (m_iUniqueID == mName.m_iUniqueID) ? true : false;
	}
	//returns if the names are not the same
	bool					operator!=(const KCName &mName) const
	{
		return (m_iUniqueID != mName.m_iUniqueID) ? true : false;
	}
	//returns if the names are the same
	bool					operator==(const KCString &strString) const
	{
		return (m_iUniqueID == _getIDFromString(strString)) ? true : false;
	}
	//returns if the names are not the same
	bool					operator!=(const KCString &strString) const
	{
		return (m_iUniqueID != _getIDFromString(strString)) ? true : false;
	}

private:
	uint32					_getIDFromString(const KCString &strString) const;
	void					_setString(const KCString &strString);

	uint32					m_iUniqueID = INVALID;
#ifdef _DEBUG
	KCString				m_strString;
#endif
};

//used for maps
struct KCNameHasher
{
	size_t operator()(const KCName& strName) const
	{
		return (size_t)strName.getValue();
	}
};