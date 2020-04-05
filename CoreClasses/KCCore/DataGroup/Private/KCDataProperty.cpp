#include "KCDataProperty.h"
#include <sstream>

static std::unordered_map<uint32, int64>		m_GetInt64FromLookupID;
static std::unordered_map<int64, uint32>		m_GetLookUpIDForInt64;

static uint32 _setint64(int64 iValue)
{

	auto mData = m_GetLookUpIDForInt64.find(iValue);
	if (mData == m_GetLookUpIDForInt64.end())
	{
		uint32 iIndexOf = (uint32)m_GetLookUpIDForInt64.size();
		m_GetLookUpIDForInt64[iValue] = iIndexOf;
		m_GetInt64FromLookupID[iIndexOf] = iValue;
		return iIndexOf;
	}
	return mData->second;
}

void KCDataProperty::operator<<(uint64 iValue)
{
	m_Data.m_uiValue32 = _setint64((int64)iValue);
	m_eType = EDATAGROUP_VARIABLE_TYPES::INT64;
}

void KCDataProperty::operator<<(int64 iValue)
{
	m_Data.m_uiValue32 = _setint64(iValue);
	m_eType = EDATAGROUP_VARIABLE_TYPES::INT64;
}

void KCDataProperty::operator<<(const KCString &strValue)
{
	m_Data.m_uiValue32 = KCName::getStringTable().getStringID(strValue);
	m_eType = EDATAGROUP_VARIABLE_TYPES::STRING;
}

 KCString KCDataProperty::getAsString()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:		
		return (m_Data.m_bValue[0])?"true":"false";
	case EDATAGROUP_VARIABLE_TYPES::CHAR:		
		return std::to_string( m_Data.m_cValue[0]);
	case EDATAGROUP_VARIABLE_TYPES::INT8:		
		return std::to_string(m_Data.m_iValue8[0]);
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return std::to_string(m_Data.m_uiValue8[0]);
	case EDATAGROUP_VARIABLE_TYPES::INT16:		
		return std::to_string(m_Data.m_iValue16[0]);
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return std::to_string(m_Data.m_uiValue16[0]);
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:		
		return std::to_string(m_Data.m_iValue32);
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return std::to_string(m_Data.m_uiValue32);
	case EDATAGROUP_VARIABLE_TYPES::INT64:		
		return std::to_string(getAsInt64());
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		return std::to_string((uint64)getAsInt64());
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:		
		return std::to_string(m_Data.m_fValue);
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return KCName::getStringTable().getStringByID(m_Data.m_uiValue32);
	}	
}

int64 KCDataProperty::getAsInt64()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (int64)m_Data.m_bValue[0];
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (int64)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return (int64)m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return (int64)m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return (int64)m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return (int64)m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return (int64)m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (int64)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		{
			auto mData = m_GetInt64FromLookupID.find(m_Data.m_uiValue32);
			if (mData == m_GetInt64FromLookupID.end())
			{
				return INVALID;
			}
			return mData->second;
		}		
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return (int64)m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "")?1:0;
	}
}

int32 KCDataProperty::getAsInt32()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (int32)m_Data.m_bValue[0];
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (int32)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return (int32)m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return (int32)m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return (int32)m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return (int32)m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (int32)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:			
		return (int32)getAsInt64();
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return (int32)m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "") ? 1 : 0;
	}
}

bool KCDataProperty::getAsBool()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return m_Data.m_bValue[0];
	default:
		return (m_Data.m_iValue32 != 0) ? true : false;
	}
}

int8 KCDataProperty::getAsInt8()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (int8)m_Data.m_bValue[0];
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (int8)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return (int8)m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return (int8)m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return (int8)m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return (int8)m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (int8)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		return (int8)getAsInt64();
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return (int8)m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "") ? 1 : 0;
	}
}

uint8 KCDataProperty::getAsUInt8()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (uint8)m_Data.m_bValue[0];
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (uint8)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return (uint8)m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return (uint8)m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return (uint8)m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return (uint8)m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (uint8)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		return (uint8)getAsInt64();
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return (uint8)m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "") ? 1 : 0;
	}
}

int16 KCDataProperty::getAsInt16()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (int16)m_Data.m_bValue[0];
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (int16)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return (int16)m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return (int16)m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return (int16)m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return (int16)m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (int16)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		return (int16)getAsInt64();
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return (int16)m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "") ? 1 : 0;
	}
}

uint16 KCDataProperty::getAsUInt16()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (uint16)m_Data.m_bValue[0];
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (uint16)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return (uint16)m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return (uint16)m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return (uint16)m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return (uint16)m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (uint16)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		return (uint16)getAsInt64();
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return (uint16)m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "") ? 1 : 0;
	}
}

float KCDataProperty::getAsFloat()
{
	switch (m_eType)
	{
	case EDATAGROUP_VARIABLE_TYPES::BOOL:
		return (float)(m_Data.m_bValue[0])?1.0f:0.0f;
	case EDATAGROUP_VARIABLE_TYPES::CHAR:
		return (float)m_Data.m_cValue[0];
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		return (float)m_Data.m_iValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		return (float)m_Data.m_uiValue8[0];
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		return (float)m_Data.m_iValue16[0];
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		return (float)m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		return (float)m_Data.m_iValue32;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		return (float)m_Data.m_uiValue32;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		return (float)getAsInt64();
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		return m_Data.m_fValue;
	case EDATAGROUP_VARIABLE_TYPES::STRING:
		return (getAsString() != "") ? 1.0f : 0.0f;
	}
}

bool KCDataProperty::setValueByString(const KCString &strString, EDATAGROUP_VARIABLE_TYPES eType)
{
	m_eType = eType;

	if (m_eType == EDATAGROUP_VARIABLE_TYPES::STRING)
	{
		m_Data.m_uiValue32 = KCName::getStringTable().getStringID(strString);
		m_eType = EDATAGROUP_VARIABLE_TYPES::STRING;
		return true;
	}
	else if (m_eType == EDATAGROUP_VARIABLE_TYPES::BOOL)
	{
		m_Data.m_bValue[0] = (strString.c_str()[0] == 'f' || strString.c_str()[0] == 'F') ? false : true;
		return true;
	}
	if (strString == "" ||
		eType == EDATAGROUP_VARIABLE_TYPES::COUNT)
	{
		return false;
	}
	//special case for char
	if (m_eType == EDATAGROUP_VARIABLE_TYPES::CHAR)
	{
		m_Data.m_cValue[0] = strString.c_str()[0];
		return true;
	}
	if (strString.size() == 1 &&
		strString.c_str()[0] == '0')
	{
		//everything is already set to 0
		return true;
	}

	//this is going to be so slow but it should take care of all memory issues.
	std::stringstream mParseStringStream(strString);
	switch (m_eType)
	{			
	case EDATAGROUP_VARIABLE_TYPES::INT8:
		mParseStringStream >> m_Data.m_iValue8[0];
		break;
	case EDATAGROUP_VARIABLE_TYPES::UINT8:
		mParseStringStream >> m_Data.m_uiValue8[0];
		break;		
	case EDATAGROUP_VARIABLE_TYPES::INT16:
		mParseStringStream >> m_Data.m_iValue16[0];		
	case EDATAGROUP_VARIABLE_TYPES::UINT16:
		mParseStringStream >> m_Data.m_uiValue16[0];
	default:
	case EDATAGROUP_VARIABLE_TYPES::INT32:
		mParseStringStream >> m_Data.m_iValue32;		
		break;
	case EDATAGROUP_VARIABLE_TYPES::UINT32:
		mParseStringStream >> m_Data.m_uiValue32;
		break;
	case EDATAGROUP_VARIABLE_TYPES::INT64:
		{
			int64 iValue(0);
			mParseStringStream >> iValue;
			if (iValue == 0)
			{
				return false;	//failed
			}
			m_Data.m_uiValue32 = _setint64(iValue);
		}
		break;
	case EDATAGROUP_VARIABLE_TYPES::UINT64:
		{
			uint64 iValue(0);
			mParseStringStream >> iValue;
			if (iValue == 0)
			{
				return false;	//failed
			}
			m_Data.m_uiValue32 = _setint64(iValue);
		}
		break;
	case EDATAGROUP_VARIABLE_TYPES::FLOAT:
		mParseStringStream >> m_Data.m_fValue;
		break;
	}
	if (m_Data.m_uiValue32 == 0)
	{
		return false;	//we already did a check for zero. So if we are still at zero then we failed to parse the string
	}
	return true;
}

bool KCDataProperty::setValueByString(const KCString &strString, const KCString &strType)
{
	return setValueByString(strString, getDataGroupTypeByString(strType));
}
