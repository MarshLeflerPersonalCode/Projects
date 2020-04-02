//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once

#include "KCDefines.h"
#include "Containers/KCTArray.h"



class KCByteReader
{
public:
	KCByteReader() {}
	KCByteReader(const KCTArray<uint8> &mArray)
	{
		configureByTArray(mArray);
	}
	KCByteReader(const uint8 *pArray, const int32 iCount = 0)
	{
		configureByByteArray( pArray, iCount);
	}
	void configureByTArray(const KCTArray<uint8> &mArray)
	{
		m_pByteArray = mArray.getMemory();
		m_iArraySize = mArray.getCount();
		m_iCurrentByteIndex = 0;
	}

	void configureByByteArray(const uint8 *pArray, const int32 iCount = 0)
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
			m_iCurrentByteIndex += (int32)iSize;
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(bool &bValue)
	{
		if (readValue(&bValue, sizeof(bool)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool operator<<(char &cValue)
	{
		if (readValue(&cValue, sizeof(char)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool operator<<(int8 &iValue)
	{
		if (readValue(&iValue, sizeof(int8)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(uint8 &iValue)
	{
		if (readValue(&iValue, sizeof(uint8)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(int16 &iValue)
	{
		if (readValue(&iValue, sizeof(int16)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(uint16 &iValue)
	{
		if (readValue(&iValue, sizeof(uint16)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(int32 &iValue)
	{
		if (readValue(&iValue, sizeof(int32)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(uint32 &iValue)
	{
		if (readValue(&iValue, sizeof(uint32)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(int64 &iValue)
	{
		if (readValue(&iValue, sizeof(int64)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(uint64 &iValue)
	{
		if (readValue(&iValue, sizeof(uint64)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool operator<<(float &fValue)
	{
		if (readValue(&fValue, sizeof(float)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool operator<<(double &dValue)
	{
		if (readValue(&dValue, sizeof(double)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool operator<<(KCString &strValue)
	{
		uint16 iNumberOfCharacters(0);
		if (readValue(&iNumberOfCharacters, sizeof(uint16)))
		{
			if (iNumberOfCharacters == 0)
			{
				return true;
			}
			if ((m_iCurrentByteIndex + iNumberOfCharacters) - 1 >= m_iArraySize)
			{
				return false;
			}
			strValue = KCString((char*)&m_pByteArray[m_iCurrentByteIndex], sizeof(char) * (size_t)iNumberOfCharacters);
			m_iCurrentByteIndex += iNumberOfCharacters;
			return true;
		}		
		return false;
	}

	FORCEINLINE bool operator<<(KCName &strValue)
	{
		uint16 iNumberOfCharacters(0);
		if (readValue(&iNumberOfCharacters, sizeof(uint16)))
		{
			if (iNumberOfCharacters == 0)
			{
				return true;
			}
			if ((m_iCurrentByteIndex + iNumberOfCharacters) - 1 >= m_iArraySize)
			{
				return false;
			}
			strValue = KCString((char*)&m_pByteArray[m_iCurrentByteIndex], sizeof(char) * (size_t)iNumberOfCharacters);
			m_iCurrentByteIndex += iNumberOfCharacters;
			return true;
		}
		return false;
	}
	
	//TODO - how to read a string in

private:
	const uint8			*m_pByteArray = nullptr;
	int32				m_iArraySize = 0;
	int32				m_iCurrentByteIndex = 0;

};
