#pragma once
#include "KCDefines.h"
//copyright 2000-... Marsh Lefler

enum class ETARRAY_GROW_BY_TYPES
{
	DOUBLE,				//doubles everytime it reaches max
	PREDEFINED			//predefined grow by value
};

template<class T>
class KCTArray
{
public:
	KCTArray(uint32 iGrowBy = 0) { setGrowBy(iGrowBy);  }
	KCTArray(T errorValue = 0, uint32 iGrowBy = 0) { m_ErrorValue = errorValue;  setGrowBy(iGrowBy); }
	~KCTArray() { clean(); }

	//deletes all the contents by calling DELETE_SAFELY
	void						deleteContents()
	{
		for (uint32 iIndex = 0; iIndex < m_iCountOfItems; iIndex++)
		{
			KCDELETE_SAFELY(m_Memory[iIndex]);
		}
		clean();
	}

	//sets the error value
	void						setErrorValue(T errorValue) { m_ErrorValue = errorValue; }
	//sets the grow by number
	void						setGrowBy(uint32 iGrowBy) { m_iGrowBy = iGrowBy; m_eGrowType = ETARRAY_GROW_BY_TYPES::PREDEFINED; }
	//returns the grow by value
	uint32						getGrowBy() { return m_iGrowBy; }
	//returns the memory
	FORCEINLINE T*				getMemory() { return m_Memory; }
	//returns the memory
	uint32						getMemoryCount() { return m_iMemorySize; }
	//returns the amount of memory being taken up - does a sizeof(T) * getMemoryCount()
	uint32						getMemorySize() { return sizeof(T) * getMemoryCount(); }
	//returns the count
	FORCEINLINE uint32			getCount() { return m_iCountOfItems; }

	//resets the contents
	FORCEINLINE void			reset() { m_iCountOfItems = 0; }
	//removes all the memory
	FORCEINLINE void			clean() { delete[] m_Memory; m_iMemorySize = 0; m_iCountOfItems = 0; }

	//returns the item at the index
	FORCEINLINE T&			operator[](uint32 iIndex)
	{
		if (iIndex >= m_iCountOfItems)
		{
			return errorValue;
		}
		return m_Memory[m_iCountOfItems];
	}


	//returns the item at the index
	FORCEINLINE T&			get(uint32 iIndex)
	{
		if (iIndex >= m_iCountOfItems)
		{
			return errorValue;
		}
		return m_Memory[m_iCountOfItems];
	}

	//adds an item, returning the index
	FORCEINLINE uint32			add(T item)
	{
		if (m_iCountOfItems >= m_iMemorySize)
		{
			_expandMemory();
		}
		m_Memory[m_iCountOfItems] = item;
		m_iCountOfItems++;
		return m_iCountOfItems - 1;
	}
	//removes an item, replace the last item in the index with it
	FORCEINLINE bool			removeAndSwap(T item)
	{
		for (uint32 iIndex = 0; iIndex < m_iCountOfItems; iIndex++)
		{
			if (m_Memory[iIndex] == item)
			{
				m_iCountOfItems--;
				m_Memory[iIndex] = m_Memory[m_iCountOfItems];
				return true;
			}
		}
		return false;
	}
	//removes an item at an index, replace the last item in the index with it
	FORCEINLINE bool			removeAtIndexAndSwap(T item, uint32 iIndex)
	{
		if (iIndex < m_iCountOfItems)
		{
			m_iCountOfItems--;			
			m_Memory[iIndex] = m_Memory[m_iCountOfItems];
			return true;
		}
		return false;
	}
	//reserves the number of slots
	void						reserve(uint64 iSize)
	{
		_expandMemoryTo(iSize)
	}

private:
	void						 _expandMemory()
	{
		switch (m_eGrowType)
		{
		default:
		case ETARRAY_GROW_BY_TYPES::DOUBLE:
			_expandMemoryTo((m_iMemorySize == 0) ? 2 : m_iMemorySize + m_iMemorySize);
			break;
		case ETARRAY_GROW_BY_TYPES::PREDEFINED:
			_expandMemoryTo(m_iMemorySize + (m_iGrowBy <= 0) ? 10 : m_iGrowBy);
			break;
		}
	}
	//expands the memory to a given size
	void						 _expandMemoryTo(uint32 iNewMemory)
	{
		if (iNewMemory <= m_iMemorySize)
		{
			return;
		}
				
		T *mMemoryTemp = new T[iNewMemory];

		for (uint32 t = 0; t < m_iMemorySize; t++)
		{
			m_mMemoryTempTmp[t] = m_List[t];
		}

		delete[] m_Memory;
		m_Memory = mMemoryTemp;
		m_iMemorySize = iNewMemory;
	}


	T							m_ErrorValue;
	T							*m_Memory = nullptr;
	uint32						m_iMemorySize = 0;
	ETARRAY_GROW_BY_TYPES		m_eGrowType = ETARRAY_GROW_BY_TYPES::DOUBLE;
	uint32						m_iGrowBy = 0;
	uint32						m_iCountOfItems = 0;
};

