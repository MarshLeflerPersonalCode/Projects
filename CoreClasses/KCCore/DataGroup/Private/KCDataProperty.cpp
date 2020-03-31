#include "KCDataProperty.h"

static std::unordered_map<uint32, int64>		m_GetInt64FromLookupID;
static std::unordered_map<int64, uint32>		m_GetLookUpIDForInt64;

void KCDataProperty::operator<<(uint64 iValue)
{
	auto mData = m_GetLookUpIDForInt64.find(iValue);
	if (mData == m_GetLookUpIDForInt64.end())
	{
		m_Data.m_uiValue32 = (uint32)m_GetLookUpIDForInt64.size();
		m_GetLookUpIDForInt64[(int64)iValue] = m_Data.m_uiValue32;
		m_GetInt64FromLookupID[m_Data.m_uiValue32] = (int64)iValue;
	}
	m_eType = EDATAGROUP_VARIABLE_TYPES::INT64;
}

void KCDataProperty::operator<<(int64 iValue)
{
	auto mData = m_GetLookUpIDForInt64.find(iValue);
	if (mData == m_GetLookUpIDForInt64.end())
	{
		m_Data.m_uiValue32 = (uint32)m_GetLookUpIDForInt64.size();
		m_GetLookUpIDForInt64[iValue] = m_Data.m_uiValue32;
		m_GetInt64FromLookupID[m_Data.m_uiValue32] = iValue;
	}
	m_eType = EDATAGROUP_VARIABLE_TYPES::INT64;
}

void KCDataProperty::operator<<(const KCString &strValue)
{
	m_Data.m_uiValue32 = KCName::getStringTable().getStringID(strValue);
	m_eType = EDATAGROUP_VARIABLE_TYPES::STRING;
}

void KCDataProperty::operator>>(int64 &iValue)
{
	auto mData = m_GetInt64FromLookupID.find(m_Data.m_uiValue32);
	if (mData == m_GetInt64FromLookupID.end())
	{
		return;
	}
	iValue = mData->second;
}

void KCDataProperty::operator>>(uint64 &iValue)
{
	auto mData = m_GetInt64FromLookupID.find(m_Data.m_uiValue32);
	if (mData == m_GetInt64FromLookupID.end())
	{
		return;
	}
	iValue = (uint64)mData->second;
}

void KCDataProperty::operator>>(KCString &strValue)
{
	strValue = KCName::getStringTable().getStringByID(m_Data.m_uiValue32);
}
