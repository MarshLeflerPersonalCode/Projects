//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDataGroupEnumsAndStructs.h"


struct KCDataProperty
{
	KCDataProperty() {}
	KCDataProperty(const KCName &strName) { m_strLookupName = strName; }
	EDATAGROUP_VARIABLE_TYPES		m_eType = EDATAGROUP_VARIABLE_TYPES::COUNT;
	coreUnionData32Bit				m_Data;
	KCName							m_strLookupName;

	void operator<<( bool bValue)
	{
		m_Data.m_bValue[0] = bValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::BOOL;
	}
	void operator<<(char cValue)
	{
		m_Data.m_cValue[0] = cValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::CHAR;
	}
	void operator<<(int8 iValue)
	{
		m_Data.m_iValue8[0] = iValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::INT8;
	}
	void operator<<(uint8 iValue)
	{
		m_Data.m_uiValue8[0] = iValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::UINT8;
	}
	void operator<<(int16 iValue)
	{
		m_Data.m_iValue16[0] = iValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::INT16;
	}
	void operator<<(uint16 iValue)
	{
		m_Data.m_uiValue16[0] = iValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::UINT16;
	}
	void operator<<(int32 iValue)
	{
		m_Data.m_iValue32 = iValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::INT32;
	}
	void operator<<(uint32 iValue)
	{
		m_Data.m_uiValue32 = iValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::UINT32;
	}
	void operator<<(float fValue)
	{
		m_Data.m_fValue = fValue;
		m_eType = EDATAGROUP_VARIABLE_TYPES::FLOAT;
	}
	void operator<<(const KCName &strValue)
	{
		m_Data.m_uiValue32 = strValue.getValue();
		m_eType = EDATAGROUP_VARIABLE_TYPES::STRING;
	}
	void operator<<(const KCString &strValue);
	void operator<<(int64 iValue);
	void operator<<(uint64 iValue);

	///////////////////////////////////////
	//this is the harder one - we should probably try to cast
	//per type of variable.
	/////////////////////////////////////
	void operator>>(bool &bValue)
	{		
		bValue = m_Data.m_bValue[0];
	}
	void operator>>(char &cValue)
	{
		cValue = m_Data.m_cValue[0];
	}
	void operator>>(int8 &iValue)
	{
		iValue = m_Data.m_iValue8[0];
	}
	void operator>>(uint8 &iValue)
	{
		iValue = m_Data.m_uiValue8[0];
	}
	void operator>>(int16 &iValue)
	{
		iValue = m_Data.m_iValue16[0];
	}
	void operator>>(uint16 &iValue)
	{
		iValue = m_Data.m_uiValue16[0];
	}
	void operator>>(int32 &iValue)
	{
		iValue = m_Data.m_iValue32;
	}
	void operator>>(uint32 &iValue)
	{
		iValue = m_Data.m_uiValue32;
	}
	void operator>>(float &fValue)
	{
		fValue = m_Data.m_fValue;
	}
	void operator>>(KCName &strValue)
	{
		strValue = KCName::getNameFromID(m_Data.m_uiValue32);
	}
	void operator>>(KCString &strValue);
	void operator>>(int64 &iValue);
	void operator>>(uint64 &iValue);
};