// =============================================================================
// Property of Runic Games
// 1500 4th Ave, Ste 500
// Seattle, WA
// 
// Copyright (c) 2008.
// All rights reserved.
//
// FILE:core/rand.h
//
// pseudorandom number generator
// =============================================================================
#if PRAGMA_ONCE
 #pragma once
#endif
#ifndef _RAND_H_
#define _RAND_H_





// -----------------------------------------------------------------------------
// thread safe Rand class
// -----------------------------------------------------------------------------

class Rand
{
public:
	

	Rand(uint32 seed)
	{ 
		setState(seed);
	}


	void setState(uint64 seed)
	{
		m_iState = seed;
	}

	inline uint64 getState() const
	{
		return m_iState;
	}

	inline void nextState()
	{
		uint64 iLastState( m_iState );
		uint64 tmp1 = m_iState & 0xffffffff;
		uint64 tmp2 = m_iState >> 32;
		m_iState = tmp1 * 695696193ULL + tmp2;
		if( m_iState == 0 )
		{
			printf("bad");
		}
	}

	inline uint32 UInt32(void)
	{
		
		nextState();
		return (uint32)m_iState;
	}

	inline uint64 UInt64(void)
	{
		uint32 result_hi, result_lo;
		{
			
			nextState();
			result_hi = uint32(m_iState);
			nextState();
			result_lo = uint32(m_iState);
		}
		return (uint64(result_hi) << uint64(32)) + uint64(result_lo);
	}

	inline int32 Int32(void)
	{
		return int32(UInt32());
	}

	inline int64 Int64(void)
	{
		return int64(UInt64());
	}

	// -------------------------------------------------------------------------
	// [0, max)
	// -------------------------------------------------------------------------
	inline uint32 UInt32(uint32 max)
	{
		assert(max != 0);
		return UInt32() % max;
	}

	// -------------------------------------------------------------------------
	// [0, max)
	// -------------------------------------------------------------------------
	inline uint64 UInt64(uint64 max)
	{
		assert(max != 0LL);
		return UInt64() % max;
	}

	// -------------------------------------------------------------------------
	// [min, max]
	// -------------------------------------------------------------------------
	inline uint32 UInt32(uint32 min, uint32 max)
	{
		uint32 divisor = max - min + 1;
		assert(divisor != 0);//, UInt32());
		return UInt32() % divisor + min;
	}

	// -------------------------------------------------------------------------
	// [min, max]
	// -------------------------------------------------------------------------
	inline uint32 UInt32X(uint32 min, uint32 max)
	{
		assert(min != max);
		uint32 divisor = max - min + 1;
		if (divisor == 0)
		{
			return UInt32();
		}
		return UInt32() % divisor + min;
	}

	// -------------------------------------------------------------------------
	// [min, max]
	// -------------------------------------------------------------------------
	inline uint64 UInt64(uint64 min, uint64 max)
	{
		uint64 divisor = max - min + 1;
		assert(divisor != 0LL);
		return UInt64() % divisor + min;
	}

	// -------------------------------------------------------------------------
	// [min, max]
	// -------------------------------------------------------------------------
	inline int32 Int32(int32 min, int32 max)
	{
		uint32 divisor = max - min + 1;
		assert(divisor != 0 );
		return int32(UInt32() % divisor + uint32(min));
	}

	// -------------------------------------------------------------------------
	// [min, max]
	// -------------------------------------------------------------------------
	inline int64 Int64(int64 min, int64 max)
	{
		uint64 divisor = max - min + 1;
		assert(divisor != 0LL );
		return int64(UInt64() % divisor + uint64(min));
	}

	// -------------------------------------------------------------------------
	// [0, 1.0f)
	// -------------------------------------------------------------------------
	inline float32 Float32(void)
	{
		float32 f;
		*((uint32 *)&f) = (UInt32() & 0x007fffff) | 0x3f800000;
		return f - 1.0f;
	}

	// -------------------------------------------------------------------------
	// [0, 1.0)
	// -------------------------------------------------------------------------
	inline float64 Float64(void)
	{
		float64 f;
		*((uint64 *)&f) = (UInt64() & 0x00fffffffffffffULL) | 0x3ff0000000000000ULL;
		return f - 1.0;
	}

	// -------------------------------------------------------------------------
	// [0, max)
	// -------------------------------------------------------------------------
	inline float32 Float32(float32 max)
	{
		return Float32() * max;
	}

	// -------------------------------------------------------------------------
	// [0, max)
	// -------------------------------------------------------------------------
	inline float64 Float64(float64 max)
	{
		return Float64() * max;
	}

	// -------------------------------------------------------------------------
	// [min, max)
	// -------------------------------------------------------------------------
	inline float32 Float32(float32 min, float32 max)
	{
		return Float32() * (max - min) + min;
	}

	// -------------------------------------------------------------------------
	// [min, max)
	// -------------------------------------------------------------------------
	inline float64 Float64(float64 min, float64 max)
	{
		return Float64() * (max - min) + min;
	}

	

protected:
	uint64 m_iState;
	
};


#endif // include guard
