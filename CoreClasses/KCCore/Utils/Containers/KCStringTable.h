//copyright Marsh Lefler 2000-...
//string look up by either id or string - thread safe!
#pragma once
#include "KCDefines.h"
#include <algorithm>

class KCStringTable
{
public:
	KCStringTable() { getStringIDFast(""); }
	~KCStringTable() {}

	//returns the id of the string
	uint32				getStringID(KCString strString)
	{
		transform(strString.begin(), strString.end(), strString.begin(), ::toupper);
		return getStringIDFast(strString);
	}
	//returns the id of the string but the string must be upper
	uint32				getStringIDFast(const KCString &strStringUpper)
	{
		
		auto mReturnValue =  m_IDLookUp.find(strStringUpper);
		if (mReturnValue != m_IDLookUp.end())
		{
			return mReturnValue->second;
		}
		m_Mutex.lock();
		m_IDLookUp[strStringUpper] = m_iIncrementor;
		m_StringLookUp[m_iIncrementor] = strStringUpper;
		m_iIncrementor++;
		m_Mutex.unlock();
		return m_iIncrementor - 1;
	}

	//returns the string by ID
	const KCString &	getStringByID(uint32 iStringID)
	{
		
		auto mReturnValue = m_StringLookUp.find(iStringID);
		if (mReturnValue != m_StringLookUp.end())
		{
			return mReturnValue->second;
		}		
		return EMPTY_KCSTRING;
	}

	//returns the count of the string table
	uint32				getCount() { return m_iIncrementor; }

private:
	//the hash map to store all the strings
	std::unordered_map<KCString, uint32 >	m_IDLookUp;
	std::unordered_map<uint32, KCString >	m_StringLookUp;
	uint32									m_iIncrementor = 0;
	std::mutex								m_Mutex;
};