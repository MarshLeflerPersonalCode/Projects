//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once

#include "KCDefines.h"



public class KCByteReader
{
public:
	KCByteReader(const int8 *pArray, const int32 iCount = 0)
	{
		m_pByteArray = pArray;
		m_iArraySize = iCount;
		m_iCurrentByteIndex = 0;
	}

	FORCEINLINE int32 tell() { return m_iCurrentByteIndex; }
	FORCEINLINE bool seek(int32 iLocation)
	{
		if (iLocation >= 0 && iLocation < m_iArraySize )
		{
			m_iCurrentByteIndex = iLocation;
			return true;
		}
		return false;
	}
	
	FORCEINLINE bool eof() { return m_iCurrentByteIndex >= m_iArraySize; }

	FORCEINLINE bool readValue(void *pValue, size_t iSize)
	{
		if (eof() == false)
		{
			memcpy_s(pValue, iSize, &m_pByteArray[m_iCurrentByteIndex], iSize);
			m_iCurrentByteIndex += iSize;
			return true;
		}
		return false;
	}

	FORCEINLINE bool readBool(bool bDefaultValue = false)
	{
		bool bValue(false);
		if (readValue(&bValue, sizeof(bool)))
		{
			return bValue;
		}
		return bDefaultValue;
	}
	FORCEINLINE char readChar(char iDefaultValue = 0)
	{
		char iValue(0);
		if (readValue(&iValue, sizeof(char)))
		{
			return iValue;
		}
		return iDefaultValue;
	}
	FORCEINLINE int8 readInt8(int8 iDefaultValue = INVALID)
	{
		int8 iValue(0);
		if (readValue(&iValue, sizeof(int8)))
		{
			return iValue;
		}
		return iDefaultValue;
	}
	FORCEINLINE int16 readInt16(int16 iDefaultValue = INVALID)
	{
		int16 iValue(0);
		if (readValue(&iValue, sizeof(int16)))
		{
			return iValue;
		}
		return iDefaultValue;
	}
	FORCEINLINE int32 readInt32(int32 iDefaultValue = INVALID)
	{
		int32 iValue(0);
		if (readValue(&iValue, sizeof(int32)))
		{
			return iValue;
		}
		return iDefaultValue;
	}

	FORCEINLINE int64 readInt64(int64 iDefaultValue = INVALID)
	{
		int64 iValue(0);
		if (readValue(&iValue, sizeof(int64)))
		{
			return iValue;
		}
		return iDefaultValue;
	}

	FORCEINLINE float readFloat(float fDefaultValue = INVALID)
	{
		float fValue(0);
		if (readValue(&fValue, sizeof(float)))
		{
			return fValue;
		}
		return fDefaultValue;
	}

	FORCEINLINE double readDouble(double dDefaultValue = INVALID)
	{
		double dValue(0);
		if (readValue(&dValue, sizeof(double)))
		{
			return dValue;
		}
		return dDefaultValue;
	}

	
	FORCEINLINE KCString readString(KCString strDefaultValue = EMPTY_KCSTRING)
	{
		uint16 iNumberOfCharacters = (uint16)readInt16(0);
		if (iNumberOfCharacters == 0)
		{
			return strDefaultValue;
		}
		if ((m_iCurrentByteIndex + iNumberOfCharacters) - 1 >= m_iArraySize)
		{
			return strDefaultValue;
		}
		std::string strReturnValue = std::string((char*)&m_pByteArray[m_iCurrentByteIndex], sizeof(char) * (size_t)iNumberOfCharacters);
		m_iCurrentByteIndex += iNumberOfCharacters;
		if (strReturnValue != EMPTY_KCSTRING)
		{
			return strReturnValue;
		}
		return strDefaultValue;
	}

	//TODO - how to read a string in

private:
	const int8			*m_pByteArray = nullptr;
	int32				m_iArraySize = 0;
	int32				m_iCurrentByteIndex = 0;

};
