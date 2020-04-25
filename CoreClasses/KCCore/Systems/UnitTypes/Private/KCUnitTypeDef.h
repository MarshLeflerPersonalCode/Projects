#pragma once
#include "KCIncludes.h"

struct KCUnitTypeDef
{
public:
	~KCUnitTypeDef()
	{
		DELETE_ARRAY_SAFELY(m_BitLookUpArray);
	}
	KCString		m_strString;
	uint16			m_iIndex = 0;
	int32			*m_BitLookUpArray = nullptr;

	FORCEINLINE bool bitCheck(int32 iBitLookUp)
	{
		int32 iBitOffsetIntoInt = iBitLookUp % 32;
		int32 iIndexIntoArray = iBitLookUp / 32;
		return (m_BitLookUpArray[iIndexIntoArray] & (1 << iBitOffsetIntoInt)) ? true : false;
	}
};