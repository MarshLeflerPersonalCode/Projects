//////////////////////////////////////////////////////////////////////////
// PROPERTY OF RUNIC GAMES
// 1500 4th Ave Seattle WA 98101
//
//      Copyright (c) 2008.
//      All Rights Reserved.
//    
//    This Software may not be used/copied or disclosed to any person or persons
// that are not employed by Runic Games.
//
// http://www.runicgames.com
//
//
// TThradSafeArrayList<T> - Template for array lists
//////////////////////////////////////////////////////////////////////////
#pragma once
#ifndef _TThradSafeArrayList_H
#define _TThradSafeArrayList_H

#include <mutex>
// the linked list class
template <class T>
class TThreadSafeArrayList
{
public:
	TThreadSafeArrayList( size_t nGrowBy = 10 ) :
	  m_iGrowBy( nGrowBy ),
	  m_iMemSize( 0 ),
	  m_iCount( 0 ),
	  m_List( NULL )
	{
		//why use memory up if we don't need to.
		//ExpandList();	
	}

	~TThreadSafeArrayList( void )
	{
		delete [] m_List;
	}

	inline void lock( void )
	{
		m_Mutex.lock();
	}
	inline void unlock( void )
	{
		m_Mutex.unlock();
	}
	inline void set( size_t nIndex, T obj )
	{
		m_Mutex.lock();
		if( nIndex >= m_iMemSize ||
			nIndex < 0 )
		{
			nIndex = 0;
		}		
		m_List[nIndex] = obj;
		m_Mutex.unlock();
	}


	inline T operator[]( size_t nIndex ) 
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		T mReturnValue( m_List[0]);		
		if( nIndex >= 0 && nIndex < m_iMemSize )
		{
			mReturnValue = m_List[nIndex];
		}		
		return mReturnValue;
	}

	inline T get( size_t nIndex )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		T mReturnValue( m_List[0]);		
		if( nIndex >= 0 && nIndex < m_iCount )
		{
			mReturnValue = m_List[nIndex];
		}		
		return mReturnValue;
	}
	inline bool deleteAtIndex( size_t nIndex )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );		
		if( nIndex >= 0 && nIndex < m_iCount )
		{
			DELETE_SAFELY( m_List[nIndex] );
			return true;
		}		
		return false;
	}
	inline bool deleteAndRemoveAtIndex( size_t nIndex )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );		
		if( nIndex >= 0 && nIndex < m_iCount )
		{
			DELETE_SAFELY( m_List[nIndex] );
			m_iCount--;			
			m_List[ nIndex ] = m_List[ m_iCount ];	
			return true;
		}		
		return false;
	}

	inline bool attemptToRemoveAtIndex( size_t nIndex, T mItem )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		
		if( nIndex >= 0 && nIndex < m_iCount )
		{
			if( m_List[ nIndex ] == mItem )
			{
				m_iCount--;			
				m_List[ nIndex ] = m_List[ m_iCount ];						
				return true;
			}
		}
		for( size_t t = 0; t < m_iCount; t++ )
		{
			if( m_List[ t ] == mItem )
			{
				m_iCount--;
				m_List[ t ] = m_List[ m_iCount ];						
				return true;
			}
		}		
		return false;
	}
	inline bool attemptToDeleteAtIndex( size_t nIndex, T mItem )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		
		if( nIndex >= 0 && nIndex < m_iCount )
		{
			if( m_List[ nIndex ] == mObj )
			{
				DELETE_SAFELY( m_List[nIndex] );
				return true;
			}
		}
		for( size_t t = 0; t < m_iCount; t++ )
		{
			if( m_List[ t ] == mItem )
			{
				DELETE_SAFELY( m_List[t] );							
				return true;
			}
		}
		
		return false;
	}

	inline void setNullAtIndex( size_t nIndex )
	{
		
		m_Mutex.lock();
		if( nIndex >= 0 && nIndex < m_iMemSize )
		{
			m_List[nIndex] = NULL;			
		}
		m_Mutex.unlock();
		
	}

	inline void deleteAll( bool bDeleteUsedMemory = true )
	{
		m_Mutex.lock();
		for( size_t t = 0; t < m_iCount; t++ )
		{
			delete m_List[t];
		}
		m_iCount = 0;

		if( bDeleteUsedMemory )
		{
			m_iMemSize = 0;
			delete [] m_List;
			m_List = NULL;
		}
		else
		{
			ZeroMemory( m_List, sizeof( T ) * m_iMemSize );
		}
		m_Mutex.unlock();
	}

	inline void clear( bool bDeleteUsedMemory = true  )
	{
		m_Mutex.lock();
		m_iCount = 0;
		if( bDeleteUsedMemory )
		{
			m_iMemSize = 0;
			delete [] m_List;
			m_List = NULL;
		}
		m_Mutex.unlock();
	}
	


	inline size_t size()
	{ 
		std::lock_guard<std::mutex> mLock( m_Mutex );
		return m_iCount; 
	}

	inline size_t add( T item )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		if( m_iCount >= m_iMemSize )
		{
			expandList();
		}
		m_List[ m_iCount ] = item;
		m_iCount++;		
		return m_iCount;
	}

	inline size_t addUnique( T item )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		bool bAdd( false );
		for( size_t t = 0; t < m_iCount; t++ )
		{
			if( m_List[ t ] == item )
			{								
				return t; //found
			}
		}
		//not found we add it
		if( m_iCount >= m_iMemSize )
		{
			expandList();
		}
		m_List[ m_iCount ] = item;
		m_iCount++;		
		return m_iCount;
	}

	inline bool removeAtIndex( size_t nIndex )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		if( nIndex >= m_iCount )
		{						
			return false;
		}
		m_iCount--;
		m_List[ nIndex ] = m_List[ m_iCount ];		
		return true;
	}
	
	inline bool removeAtIndexShuffleDown( size_t nIndex )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		if( nIndex >= m_iCount)
		{
			
			return false;
		}		
		m_iCount--;
		for( size_t t = nIndex; t < m_iCount; t++ )
		{
			m_List[ t ] = m_List[ t + 1 ];
		}
		
		return true;
	}
	inline bool remove( T item )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		for( size_t t = 0; t < m_iCount; t++ )
		{
			if( m_List[ t ] == item )
			{
				m_iCount--;
				m_List[ t ] = m_List[ m_iCount ];			
				
				return true;
			}
		}
		
		return false;
	}

	inline size_t indexOf( T item )
	{
		std::lock_guard<std::mutex> mLock( m_Mutex );
		for( size_t t = 0; t < m_iCount; t++ )
		{
			if( m_List[ t ] == item )
			{
				
				return t;
			}
		}		
		return INVALID;
	}

	//!returns the T data array
	inline const T * getData(){ return m_List; }

	//!zero's out the memory
	inline void zeroMemory( void )
	{
		m_Mutex.lock();
		if( m_List == NULL )
		{
			expandList();
		}
		ZeroMemory( m_List, sizeof( T ) * m_iMemSize );
		m_Mutex.unlock();
	}

	//merges in an array of T data
	void merge( const T *pArray, size_t iSize )
	{
		
		m_Mutex.lock();
		for( size_t t = 0; t < iSize; t++ )
		{			
			if( m_iCount >= m_iMemSize )
			{
				expandList();
			}
			m_List[ m_iCount ] = pArray[t];
			m_iCount++;
		}
		m_Mutex.unlock();
	}

	//sets the grow by size dynamically
	void setGrowBy( size_t iGrowBy )
	{
		if( iGrowBy < 1 )
			iGrowBy = 1;
		m_iGrowBy = iGrowBy;
	}

	//!return the grow by size
	inline size_t getGrowBy()	{	return m_iGrowBy;	}

	//!swaps two values
	void swap( size_t iIndex1,
			   size_t iIndex2)
	{
		m_Mutex.lock();
		T value1 =  m_List[iIndex1];
		m_List[iIndex1] = m_List[iIndex2];
		m_List[iIndex2] = value1;
		m_Mutex.unlock();
	}

	//!inserts an item into the array and shifts the array up
	void insert( int32 iIndex, T item )
	{		
		
		m_Mutex.lock();
		if( iIndex >= (int32)m_iCount )
		{
			int32 iGrowBy = m_iGrowBy;
			m_iGrowBy = (iIndex - (int32)m_iCount ) + 1;
			expandList();
			m_iGrowBy = iGrowBy;			
		}				
		
		m_List[ m_iCount ] = item;	//puts it at the end
		m_iCount++;
		if( m_iCount == 1 )
		{
			m_Mutex.unlock();	
			return;
		}
		
		for( int32 t = (int32)m_iCount - 1; t >= (int32)iIndex; t-- )
		{
			if( t > 0 )
			{
				m_List[ t ] = m_List[ t - 1 ];
			}
		}
		m_List[iIndex] = item;
		m_Mutex.unlock();
	}

private:
	void operator=( const TThreadSafeArrayList<T> &ArrayList )
	{
		ASSERT_RETURN( false );
	}
	inline void expandList()
	{
		
		size_t newSize = m_iMemSize + m_iGrowBy;
		T *m_Tmp = new T[newSize];	

		for( size_t t = 0; t < m_iMemSize; t++ )
		{
			m_Tmp[t] = m_List[t];			
		}

		delete [] m_List;
		m_List = m_Tmp;
		m_iMemSize = newSize;
	}
	T							*m_List;
	size_t						 m_iCount;
	size_t						 m_iMemSize;
	size_t						 m_iGrowBy;

	std::mutex					m_Mutex;
}; 

#endif