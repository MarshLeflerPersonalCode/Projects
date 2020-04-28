#pragma once
#include "KCDefines.h"
#include "Utils/KCAsserts.h"
//copyright 2000-... Marsh Lefler

class BitReference
{
public:

	BitReference(uint32 &iReference, uint32 m_iMask)
		: m_iReference(iReference)
		, m_iMask(m_iMask)
	{}

	FORCEINLINE operator bool() const
	{
		return (m_iReference & m_iMask) != 0;
	}
	FORCEINLINE void operator=(const bool NewValue)
	{
		if (NewValue)
		{
			m_iReference |= m_iMask;
		}
		else
		{
			m_iReference &= ~m_iMask;
		}
	}

private:

	uint32 &m_iReference;
	uint32 m_iMask;
};

class KCBitArray
{
public:	
	KCBitArray(){}
	KCBitArray(const KCBitArray &mBitArray) 
	{
		*this = mBitArray;
	}
	~KCBitArray() { clean(); }

	void						operator=(const KCBitArray &mBitArray)
	{
		clean();
		_expandMemoryTo(mBitArray.m_iMemorySize);
		memcpy(m_Memory, mBitArray.m_Memory, sizeof(uint32) * mBitArray.m_iMemorySize);
		m_iCountOfBits = mBitArray.m_iCountOfBits;
	}
	//returns the memory
	const uint32 *				getMemory() const { return m_Memory; }
	//returns the memory
	uint32						getMemoryCount() const { return m_iMemorySize; }
	//returns the amount of memory being taken up - does a sizeof(T) * getMemoryCount()
	size_t						getMemorySize() const { return getMemoryCount() * sizeof(int32); }
	//help support UE4
	FORCEINLINE uint32			Num() const { return m_iCountOfBits;}
	//resets the contents but doesn't clear the memory - upper case to support UE4
	FORCEINLINE void			Reset()
	{ 
		m_iCountOfBits = 0; 
		for (uint32 iMemoryIndex = 0; iMemoryIndex < m_iMemorySize; iMemoryIndex++)
		{
			m_Memory[iMemoryIndex] = 0;
		}
	}
	//removes all the memory and resets the count - NOTE this will not delete the objects in it.
	FORCEINLINE void			clean() { DELETE_ARRAY_SAFELY(m_Memory); m_iMemorySize = 0; m_iCountOfBits = 0; }
	//removes all the memory and resets the count - NOTE this will not delete the objects in it. - UE4 support
	FORCEINLINE void			Empty(){ clean(); }
	//returns the bit by index
	FORCEINLINE bool			operator[](uint32 iBitIndex) const {return _getBit(iBitIndex); }
	//returns the bit by index
	FORCEINLINE BitReference	operator[](uint32 iBitIndex)
	{ 
		bool bValue = _getBit(iBitIndex);	//makes sure we have the memory
		return BitReference(m_Memory[iBitIndex / 32], MASK_CREATE_32BIT( iBitIndex % 32 ));		
	}
	//sets a bit
	FORCEINLINE void			setBit(uint32 iBitIndex, bool bValue){ _setBit(iBitIndex, bValue); }
	void						setBits(uint32 iStartBit, uint32 iEndBit, bool bValue)
	{ 
		KCEnsureAlwaysReturn(iStartBit < iEndBit);		
		for (uint32 iBit = iStartBit; iBit < iEndBit; iBit++)
		{
			_setBit(iBit, bValue);
		}		
	}

	//returns the item at the index
	FORCEINLINE bool			get(uint32 iBitIndex) const { return _getBit(iBitIndex); }
	FORCEINLINE bool			getBit(uint32 iBitIndex) const { return _getBit(iBitIndex); }

	FORCEINLINE bool removeLast()
	{
		if (m_iCountOfBits > 0)
		{
			m_iCountOfBits--;
			return true;
		}
		return false;
	}

	//returns the last item in the list
	FORCEINLINE const bool last() const
	{
		
		return get(m_iCountOfBits - 1);
	}
	//returns the last item in the list - ue4 helper
	FORCEINLINE const bool & Last() const { return last(); }

	
	//reserves the number of bits.
	void						reserve(uint32 iNumberOfBits)
	{
		clean();		
		uint32 iNewMemorySize = (iNumberOfBits / 32) + (iNumberOfBits % 32 > 0)?1:0;
		_expandMemoryTo(iNewMemorySize);
	}
	//reserves the number of bits.
	void						Reserve(uint32 iNumberOfBits){reserve(iNumberOfBits);}

private:
	bool 						_getBit(uint32 iBitIndex) const
	{
		
		uint32 iIndex = iBitIndex / 32;
		if (iIndex < m_iMemorySize)
		{
			uint32 iBit = iBitIndex % 32;
			return MASK_TEST_BIT_32BIT(m_Memory[iIndex], iBit);
		}
		return false;
		
	}
	void						_setBit(uint32 iBitIndex, bool bValue)
	{
		m_iCountOfBits = (iBitIndex > m_iCountOfBits) ? iBitIndex : m_iCountOfBits;
		uint32 iBit = iBitIndex % 32;
		uint32 iIndex = iBitIndex / 32;
		if ((uint32)(iIndex + (iBit > 0)?1:0) >= m_iMemorySize ||
			m_iMemorySize == 0)
		{
			_expandMemoryTo((iIndex + (iBit > 0) ? 1 : 0));
		}		
		MASK_SET_BIT_32BIT(m_Memory[iIndex], iBit);
	}

	void						 _expandMemoryTo(uint32 iNewMemorySize)
	{			
		if (iNewMemorySize < 0)
		{
			clean();
			return;
		}
		if (iNewMemorySize == 0)
		{
			iNewMemorySize = 1;
		}
		uint32 *mMemoryTemp = KC_NEW uint32[iNewMemorySize];
		if (m_iMemorySize > 0)
		{
			memcpy(mMemoryTemp, m_Memory, ((iNewMemorySize > m_iMemorySize)?m_iMemorySize: iNewMemorySize) * sizeof(uint32));
		}
		DELETE_ARRAY_SAFELY(m_Memory);
		if (m_iMemorySize < iNewMemorySize)
		{
			mMemoryTemp[iNewMemorySize - 1] = 0;
		}
		m_Memory = mMemoryTemp;
		m_iMemorySize = iNewMemorySize;
	}


	
	uint32						*m_Memory = nullptr;
	uint32						m_iMemorySize = 0;		
	uint32						m_iCountOfBits = 0;
};

