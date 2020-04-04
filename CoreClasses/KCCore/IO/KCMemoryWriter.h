//copyright Marsh Lefler 2000-...
//simple byte reader.
#pragma once

#include "KCDefines.h"
#include "Containers/KCTArray.h"
#include "Containers/KCName.h"


template<class MemoryType>
class KCMemoryWriter
{
public:
	KCMemoryWriter(uint32 iGrowBy = 1000)
	{
		m_MemoryArray.setGrowBy(iGrowBy);				
		m_iCurrentByteIndex = 0;
	}
	FORCEINLINE uint32			tell() { return m_iCurrentByteIndex; }
	FORCEINLINE bool			seekBeginning() { m_iCurrentByteIndex = 0; }
	FORCEINLINE bool			seekEnd() { m_iCurrentByteIndex = m_MemoryArray.getCount(); }
	FORCEINLINE bool			seek(uint32 iLocation)
	{
		if (iLocation >= 0 && iLocation < m_MemoryArray.getCount())
		{
			m_iCurrentByteIndex = iLocation;
			return true;
		}
		return false;
	}	
	FORCEINLINE bool			reserveMemory(uint32 iMemoryReserve)
	{
		m_MemoryArray.reserve(iMemoryReserve);
	}

	//returns the memory
	const MemoryType *			getMemory() const { return m_MemoryArray.getMemory(); }
	//returns the memory size
	size_t						getMemorySize() { return m_MemoryArray.getMemorySize(); }
	//returns the memory size
	uint32						getArrayCount(){ return m_MemoryArray.getCount(); }


	FORCEINLINE bool			writeValue(const void *pValue, size_t iSize)
	{
		const uint8 *pByteArray = (uint8*)pValue;
		uint32 iIndex = m_iCurrentByteIndex;
		uint32 iFinalIndex = iIndex+(uint32)iSize;
		uint32 iArrayIndex(0);
		for (iIndex; iIndex < iFinalIndex; iIndex++)
		{
			uint8 iValue = pByteArray[iArrayIndex];
			iArrayIndex++;
			if (iIndex >= m_MemoryArray.getCount())
			{
				m_MemoryArray.add(iValue);
			}
			else
			{
				m_MemoryArray[iIndex] = iValue;
			}
		}
		m_iCurrentByteIndex += (int32)iSize;
		return true;
	}

	FORCEINLINE bool			operator<<(const bool &bValue)
	{
		if (writeValue(&bValue, sizeof(bool)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool			operator<<(const char &cValue)
	{
		if (writeValue(&cValue, sizeof(char)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool			operator<<(const int8 &iValue)
	{
		if (writeValue(&iValue, sizeof(int8)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const uint8 &iValue)
	{
		if (writeValue(&iValue, sizeof(uint8)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const int16 &iValue)
	{
		if (writeValue(&iValue, sizeof(int16)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const uint16 &iValue)
	{
		if (writeValue(&iValue, sizeof(uint16)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const int32 &iValue)
	{
		if (writeValue(&iValue, sizeof(int32)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const uint32 &iValue)
	{
		if (writeValue(&iValue, sizeof(uint32)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const int64 &iValue)
	{
		if (writeValue(&iValue, sizeof(int64)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const uint64 &iValue)
	{
		if (writeValue(&iValue, sizeof(uint64)))
		{
			return true;
		}
		return false;
	}

	FORCEINLINE bool			operator<<(const float &fValue)
	{
		if (writeValue(&fValue, sizeof(float)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool			operator<<(const double &dValue)
	{
		if (writeValue(&dValue, sizeof(double)))
		{
			return true;
		}
		return false;
	}
	FORCEINLINE bool			operator<<(const KCString &strValue)
	{
		return _saveString(strValue);
	}
	FORCEINLINE bool			operator<<(const KCName &strValue)
	{
		return _saveString(strValue.toString());
	
	}
	FORCEINLINE bool			operator<<(const char *strArray)
	{
		return _saveString(KCString(strArray));

	}
	
	FORCEINLINE bool			writeLine(const KCString &strString)
	{		
		return _saveStringWithLineBreak(strString);
	}
	FORCEINLINE bool			writeLine(const char *strArray)
	{
		return _saveStringWithLineBreak(KCString(strArray));
	}
	FORCEINLINE bool			write(const KCString &strString)
	{
		return _saveString(strString);
	}
	FORCEINLINE bool			write(const char *strArray)
	{
		return _saveString(KCString(strArray));
	}

private:

	bool						_saveString(const KCString strValue)
	{
		uint16 iNumberOfCharacters((uint16)strValue.size());
		writeValue(&iNumberOfCharacters, sizeof(uint16));
		writeValue(strValue.c_str(), sizeof(char) * strValue.size());
		return true;
	}
	bool						_saveStringWithLineBreak(const KCString strValue)
	{
		static char g_cEndLine( '\n' );
		uint16 iNumberOfCharacters((uint16)strValue.size() + 1);
		writeValue(&iNumberOfCharacters, sizeof(uint16));
		writeValue(strValue.c_str(), sizeof(char) * strValue.size());
		writeValue(&g_cEndLine, sizeof(char));
		return true;
	}
	KCTArray<MemoryType>	m_MemoryArray;
	uint32					m_iCurrentByteIndex = 0;

};

class KCCharWriter : public KCMemoryWriter<char>
{
public:
};