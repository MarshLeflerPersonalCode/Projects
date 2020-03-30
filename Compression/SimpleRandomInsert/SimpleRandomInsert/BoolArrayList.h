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
// CBoolArrayList<T> - Template for array lists of bools
//////////////////////////////////////////////////////////////////////////
#pragma once
#ifndef _CBoolArrayList_H
#define _CBoolArrayList_H



// the linked list class
static size_t BITS_IN_A_BOOL( sizeof( bool ) * 8 );
class CBoolArrayList
{
public:
	CBoolArrayList( size_t nGrowBy = 10 ) :
	  m_iGrowBy( (nGrowBy/BITS_IN_A_BOOL) + 1 ),
	  m_iMemSize( 0 ),
	  m_iCount( 0 ),
	  m_List( NULL )
	{
		//why use memory up if we don't need to.
		//ExpandList();	
	}

	~CBoolArrayList( void )
	{
		if( m_List != NULL )
		{
			DELETE_ARRAY_SAFELY( m_List);
			m_List = NULL;
		}
		
	}

	inline void preAllocate( size_t iCount )
	{
		if( m_List != NULL )
		{
			return;
		}
		size_t iOldGrowBy( m_iGrowBy );
		m_iGrowBy = max( m_iGrowBy, (iCount/BITS_IN_A_BOOL) + 1 );
		expandList();
		m_iGrowBy = iOldGrowBy;		
	}

	inline void set( size_t nIndex, bool bValue )
	{
		if( nIndex >= m_iCount ){ return; }
		size_t iBitIndex = getBitIndex( nIndex );
		uint8 iBitOffset = getBitOffset( nIndex );
		if( bValue )
		{
			m_List[ iBitIndex ] = m_List[ iBitIndex ] |  ( 1 << iBitOffset );
		}
		else
		{
			m_List[ iBitIndex ] = m_List[ iBitIndex ] & ~( 1 << iBitOffset );			
		}
		
		
	}

	inline bool operator[]( size_t nIndex ) const
	{	
		if( nIndex >= m_iCount ){ return false; }
		size_t iBitIndex = getBitIndex( nIndex );
		size_t iBitOffset = getBitOffset( nIndex );		
		bool bValue = ( m_List[ iBitIndex ] & ( 1 << iBitOffset ) )?true:false;
		return bValue;
	}

	inline bool get( size_t nIndex ) const
	{				
		if( nIndex >= m_iCount ){ return false; }
		size_t iBitIndex = getBitIndex( nIndex );
		uint8 iBitOffset = getBitOffset( nIndex );		
		bool bValue = ( m_List[ iBitIndex ] & ( 1 << iBitOffset ) )?true:false;
		return bValue;
	}


	inline void clear( bool bDeleteUsedMemory = true  )
	{
		m_iCount = 0;
		if( bDeleteUsedMemory )
		{
			m_iMemSize = 0;
			DELETE_ARRAY_SAFELY( m_List);
			m_List = NULL;
		}
		else
		{
			for( size_t t = 0; t < m_iMemSize; t++ )
			{
				m_List[ t ] = 0;
			}
		}
	}

	inline size_t size() const{ return m_iCount; }

	inline size_t add( bool bValue )
	{
		if( m_iCount >= m_iMemSize * BITS_IN_A_BOOL )
		{
			expandList();
		}
		m_iCount++;
		set( m_iCount - 1, bValue );		
		return m_iCount;
	}

	size_t getCount( bool bCountOfTrue, size_t iToIndex = INVALID )
	{
		if( iToIndex == INVALID )
		{
			iToIndex = m_iCount;
		}
		size_t iCount( 0 );
		for( size_t t = 0; t < iToIndex; t++ )
		{
			if( bCountOfTrue == get( t ) )
			{
				iCount++;
			}
		}
		return iCount;
	}

	bool insertBitShiftUp( size_t nIndex, bool bValue )
	{
		if( nIndex > m_iCount ||
			nIndex < 0 )
		{			
			return false;
		}
		if( m_iCount >= m_iMemSize )
		{
			expandList();
		}
		for( int32 t = (int32)m_iCount;
			t > (int32)nIndex;
			t-- )
		{
			set( t, get( t - 1 ) );
		}
		m_iCount++;
		set( nIndex, bValue );
		return true;
	}
	bool removeBitShiftDown( size_t nIndex )
	{
		if( nIndex >= m_iCount ||
			nIndex < 0 )
		{			
			return false;
		}
		for( size_t t = nIndex;
			t < m_iCount - 1;
			t++ )
		{
			set( t, get( t + 1 ) );
		}
		m_iCount--;
		return true;
	}

	inline bool removeAtIndex( size_t nIndex )
	{
		if( nIndex >= m_iCount ||
			nIndex < 0 )
		{			
			return false;
		}
		
		m_iCount--;
		bool bitAtLocation = get( m_iCount );
		set( nIndex, bitAtLocation );		
		return true;
	}
	
	

	//returns the T data array
	inline uint8 * getData(){ return m_List; }


	//sets the grow by size dynamically
	void setGrowBy( size_t iGrowBy )
	{
		if( iGrowBy < 1 )
			iGrowBy = 1;
		m_iGrowBy = (iGrowBy/BITS_IN_A_BOOL) + 1;
	}

	//!return the grow by size
	inline size_t getGrowBy()	{	return m_iGrowBy;	}

	
	void zeroOut( size_t iSize = INVALID )
	{
		m_iCount = iSize;
		if( iSize != INVALID )
		{
			iSize = ((iSize/BITS_IN_A_BOOL) + 1 ) + 1;
			if( m_iMemSize < iSize )
			{
				DELETE_ARRAY_SAFELY(m_List);
				m_List = new uint8[iSize];
				m_iMemSize = iSize;
			}
		}
		ZeroMemory( m_List, m_iMemSize );
		
	}

	void copyArrayList( CBoolArrayList &mBoolArrayList )
	{
		while( m_iMemSize < mBoolArrayList.m_iMemSize )
		{
			expandList();
		}
		m_iCount = mBoolArrayList.m_iCount;
		_memccpy( m_List, mBoolArrayList.m_List, NULL, sizeof( byte ) * m_iMemSize );
	}

	size_t getSizeInBytes( void ) 
	{ 
		size_t iNewSize = (m_iCount/BITS_IN_A_BOOL);
		return iNewSize + ((( m_iCount % 8 ) != 0 )?1:0); 
	}

	size_t getMemorySize( void ) { return m_iMemSize; }

	void	setData( uint8 *pData, size_t iMemorySize, size_t iCount, bool bDeleteFirst = true )
	{
		if( bDeleteFirst &&
			m_List != NULL )
		{
			DELETE_ARRAY_SAFELY( m_List);
			m_List = NULL;
		}
		m_List = pData;
		m_iMemSize = iMemorySize;
		m_iCount = iCount;
	}
private:
	inline void expandList()
	{
		if( m_List == NULL )
		{
			m_iMemSize = m_iGrowBy;
			m_List = new uint8[m_iGrowBy];
			for( size_t t = 0; t < m_iGrowBy; t++ )
			{
				m_List[t] = 0;
			}
			return;
		}
		size_t newSize = m_iMemSize + m_iGrowBy;
		uint8 *m_Tmp = new uint8[newSize];	
		for( size_t t = 0; t < newSize; t++ )
		{
			if( t < m_iMemSize )
			{
				m_Tmp[t] = m_List[t];			
			}
			else
			{
				m_Tmp[t] = 0;
			}
		}
		DELETE_ARRAY_SAFELY(m_List);
		m_List = NULL;
		m_List = m_Tmp;
		m_iMemSize = newSize;
	}

	inline size_t getBitIndex( size_t iIndex ) const
	{
		size_t iBitIndex = iIndex/BITS_IN_A_BOOL;
		return iBitIndex;
	}
	inline uint8 getBitOffset( size_t iIndex ) const 
	{
		uint8 iBitOffset = (uint8)(iIndex%BITS_IN_A_BOOL);
		return iBitOffset;
	}
	uint8						*m_List;
	size_t						 m_iCount;
	size_t						 m_iMemSize;
	size_t						 m_iGrowBy;
}; 

#endif