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

	const KCString &				getNameAsString(){ return m_strLookupName.toString(); }

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
	//per type of variable. These call the getAs functions. NOTE that uint64 and uint32
	//just recast while the other don't.  This is because of casting of smaller number to larger
	//numbers can lose their sign. If the memory doesn't change, you can cast between 4294967295(max uint32(0xFFFFFFFF)) and -1(int32 0xFFFFFFFFF)
	/////////////////////////////////////
	FORCEINLINE void		operator>>(bool &bValue) {	bValue = getAsBool();}
	FORCEINLINE void		operator>>(char &cValue){cValue = (char)getAsUInt8();}
	FORCEINLINE void		operator>>(int8 &iValue){iValue = getAsInt8();}
	FORCEINLINE void		operator>>(uint8 &iValue){iValue = getAsUInt8();}
	FORCEINLINE void		operator>>(int16 &iValue){iValue = getAsInt16();}
	FORCEINLINE void		operator>>(uint16 &iValue){iValue = getAsUInt16();}
	FORCEINLINE void		operator>>(int32 &iValue){iValue = getAsInt32();}
	FORCEINLINE void		operator>>(uint32 &iValue){iValue = getAsUInt32();}
	FORCEINLINE void		operator>>(float &fValue) { fValue = getAsFloat();}
	FORCEINLINE void		operator>>(KCName &strValue) { strValue = getAsString();}
	FORCEINLINE void		operator>>(KCString &strValue) { strValue = getAsString(); }
	FORCEINLINE void		operator>>(int64 &iValue) { iValue = getAsInt64(); }
	FORCEINLINE void		operator>>(uint64 &iValue) { iValue = getAsUInt64(); }


	KCString 				getAsString();
	int64					getAsInt64();
	FORCEINLINE uint64		getAsUInt64() { return (uint64)getAsInt64(); }
	int32					getAsInt32();
	uint32					getAsUInt32() { return (uint32)getAsInt32(); }
	bool					getAsBool();
	int8					getAsInt8();
	uint8					getAsUInt8();
	int16					getAsInt16();
	uint16					getAsUInt16();
	float					getAsFloat();

	bool					setValueByString(const KCString &strString, EDATAGROUP_VARIABLE_TYPES eType);
};