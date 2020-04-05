//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once

#include "KCDefines.h"
#include "Utils/KCAsserts.h"
#include "Containers/KCTArray.h"
#include "Containers/KCName.h"

template<class MemoryType>
class KCMemoryReader
{
public:
	KCMemoryReader() {}
	KCMemoryReader(const KCTArray<MemoryType> &mArray)
	{
		configureByTArray(mArray);
	}
	KCMemoryReader(const MemoryType *pArray, const int32 iCount = 0)
	{
		configureByByteArray( pArray, iCount);
	}
	void configureByTArray(const KCTArray<MemoryType> &mArray)
	{
		m_pMemoryArray = mArray.getMemory();
		m_iArraySize = mArray.getCount();
		m_iCurrentByteIndex = 0;
	}

	void configureByArray(const MemoryType *pArray, const int32 iCount = 0)
	{
		m_pMemoryArray = pArray;
		m_iArraySize = iCount;
		m_iCurrentByteIndex = 0;
	}

	FORCEINLINE size_t tell() { return m_iCurrentByteIndex; }
	FORCEINLINE bool seek(size_t iLocation)
	{
		if (iLocation >= 0 && iLocation < m_iArraySize )
		{
			m_iCurrentByteIndex = iLocation;
			return true;
		}
		return false;
	}
	//returns the next memory index
	FORCEINLINE MemoryType next(){ seek(m_iCurrentByteIndex + 1); return m_pMemoryArray[m_iCurrentByteIndex];}
	//tells what the value is at a specific memory location
	FORCEINLINE MemoryType memoryValueAtLocation(size_t iLocationInMemory){ KCEnsureAlwaysReturnVal(( iLocationInMemory < m_iArraySize ), (MemoryType)INVALID); return m_pMemoryArray[iLocationInMemory]; }
	//is the current carrot at the end of memory?
	FORCEINLINE bool eof() { return m_iCurrentByteIndex >= m_iArraySize; }

	FORCEINLINE bool readValue(void *pValue, size_t iSize)
	{
		if (eof() == false)
		{
			memcpy_s(pValue, iSize, &m_pMemoryArray[m_iCurrentByteIndex], iSize);
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
			strValue = KCString((char*)&m_pMemoryArray[m_iCurrentByteIndex], sizeof(char) * (size_t)iNumberOfCharacters);
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
			strValue = KCString((char*)&m_pMemoryArray[m_iCurrentByteIndex], sizeof(char) * (size_t)iNumberOfCharacters);
			m_iCurrentByteIndex += iNumberOfCharacters;
			return true;
		}
		return false;
	}

	//searches the memory from the current location for a specific memory value. Move carrot will move the selected index.
	size_t			findIndexOfNextMemoryValue(MemoryType mValue, bool bMoveCarrot = true)
	{
		for (size_t iIndex = m_iCurrentByteIndex; iIndex < m_iArraySize; iIndex++)
		{
			if (m_pMemoryArray[iIndex] == mValue)
			{
				if (bMoveCarrot)
				{
					m_iCurrentByteIndex = iIndex;
				}
				return iIndex;
			}
		}
		return INVALID;
	}
	//searches the memory from the current location for a specific memory value that doesn't match the value passed in. Move carrot will move the selected index.
	size_t			findIndexOfNextMemoryValueNotMatching(MemoryType mValueNotToFind, bool bMoveCarrot = true)
	{
		for (size_t iIndex = m_iCurrentByteIndex; iIndex < m_iArraySize; iIndex++)
		{
			if (m_pMemoryArray[iIndex] != mValueNotToFind)
			{
				if (bMoveCarrot)
				{
					m_iCurrentByteIndex = iIndex;
				}
				return iIndex;
			}
		}
		return INVALID;
	}
	

protected:
	const MemoryType	*m_pMemoryArray = nullptr;
	size_t				m_iArraySize = 0;
	size_t				m_iCurrentByteIndex = 0;

};


class KCCharReader : public KCMemoryReader<char>
{
public:
	//copies the memory into a string. REturns true if successful
	bool				copyMemoryIntoString(size_t iCount, KCString &strString, bool bMoveCarrot = true)
	{
		if (iCount + m_iCurrentByteIndex >= m_iArraySize)
		{
			return false;
		}
		strString = KCString(&m_pMemoryArray[m_iCurrentByteIndex],(uint32)iCount);
		if (bMoveCarrot)
		{
			m_iCurrentByteIndex += iCount;
		}
		return true;
	}
	//copies the memory into a string. REturns true if successful
	bool				copyMemoryIntoStringToMemoryLocation(size_t iMemoryLocation, KCString &strString, bool bMoveCarrot = true)
	{
		if (iMemoryLocation >= m_iArraySize)
		{
			return false;
		}
		strString = KCString(&m_pMemoryArray[m_iCurrentByteIndex], (uint32)(iMemoryLocation -m_iCurrentByteIndex));
		if (bMoveCarrot)
		{
			m_iCurrentByteIndex = iMemoryLocation;
		}
		return true;
	}
};