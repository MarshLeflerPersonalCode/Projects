//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDataGroupEnumsAndStructs.h"


struct KCDataProperty
{
	KCDataProperty() {}
	KCDataProperty(const KCName &strName) { m_strLookupName = strName; }
	EDATATYPES						m_eType = EDATATYPES::COUNT;
	coreUnionData32Bit				m_Data;
	KCName							m_strLookupName;
	KCString						m_strValueAsString;


	const KCString &				getNameAsString() const { return m_strLookupName.toString(); }

	void operator<<( bool bValue)
	{
		m_Data.m_bValue[0] = bValue;
		m_eType = EDATATYPES::BOOL;
	}
	void operator<<(char cValue)
	{
		m_Data.m_cValue[0] = cValue;
		m_eType = EDATATYPES::CHAR;
	}
	void operator<<(int8 iValue)
	{
		m_Data.m_iValue8[0] = iValue;
		m_eType = EDATATYPES::INT8;
	}
	void operator<<(uint8 iValue)
	{
		m_Data.m_uiValue8[0] = iValue;
		m_eType = EDATATYPES::UINT8;
	}
	void operator<<(int16 iValue)
	{
		m_Data.m_iValue16[0] = iValue;
		m_eType = EDATATYPES::INT16;
	}
	void operator<<(uint16 iValue)
	{
		m_Data.m_uiValue16[0] = iValue;
		m_eType = EDATATYPES::UINT16;
	}
	void operator<<(int32 iValue)
	{
		m_Data.m_iValue32 = iValue;
		m_eType = EDATATYPES::INT32;
	}
	void operator<<(uint32 iValue)
	{
		m_Data.m_uiValue32 = iValue;
		m_eType = EDATATYPES::UINT32;
	}
	void operator<<(float fValue)
	{
		m_Data.m_fValue = fValue;
		m_eType = EDATATYPES::FLOAT;
	}
	void operator<<(const KCName &strValue)
	{
		m_strValueAsString = strValue.toString();
		m_eType = EDATATYPES::STRING;
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
	FORCEINLINE void		operator>>(bool &bValue) const {	bValue = getAsBool();}
	FORCEINLINE void		operator>>(char &cValue) const {cValue = (char)getAsUInt8();}
	FORCEINLINE void		operator>>(int8 &iValue) const {iValue = getAsInt8();}
	FORCEINLINE void		operator>>(uint8 &iValue) const {iValue = getAsUInt8();}
	FORCEINLINE void		operator>>(int16 &iValue) const {iValue = getAsInt16();}
	FORCEINLINE void		operator>>(uint16 &iValue) const {iValue = getAsUInt16();}
	FORCEINLINE void		operator>>(int32 &iValue) const {iValue = getAsInt32();}
	FORCEINLINE void		operator>>(uint32 &iValue) const {iValue = getAsUInt32();}
	FORCEINLINE void		operator>>(float &fValue) const { fValue = getAsFloat();}
	FORCEINLINE void		operator>>(KCName &strValue) const { strValue = getAsString();}
	FORCEINLINE void		operator>>(KCString &strValue) const { strValue = getAsString(); }
	FORCEINLINE void		operator>>(int64 &iValue) const { iValue = getAsInt64(); }
	FORCEINLINE void		operator>>(uint64 &iValue) const { iValue = getAsUInt64(); }


	KCString 				getAsString() const;
	int64					getAsInt64() const;
	FORCEINLINE uint64		getAsUInt64() const { return (uint64)getAsInt64(); }
	int32					getAsInt32() const;
	uint32					getAsUInt32() const { return (uint32)getAsInt32(); }
	bool					getAsBool() const;
	int8					getAsInt8() const;
	uint8					getAsUInt8() const;
	int16					getAsInt16() const;
	uint16					getAsUInt16() const;
	float					getAsFloat() const;

	bool					setValueByString(const KCString &strString, const KCString &strType);
	bool					setValueByString(const KCString &strString, EDATATYPES eType);
};