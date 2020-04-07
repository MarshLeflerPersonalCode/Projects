//copyright Marsh Lefler 2000-...
#pragma once
#include "IO/KCByteReader.h"
#include "IO/KCByteWriter.h"
#include "DataGroup/KCDataGroup.h"
#include "Serialization/KCSerializationDefines.h"
#include "KCIncludeTest.serialize.inc"



enum class ETEST
{
	ONE	= 4	UMETA(DISPLAYNAME="TEST 1"),
	TWO	=8	UMETA(DISPLAYNAME = "TEST 2"),
	THREE=12	UMETA(DISPLAYNAME = "TEST 3"),
	FOUR= 44	UMETA(DISPLAYNAME = "TEST 4"),
	COUNT = 123	UMETA(DISPLAYNAME = "TEST 5")
};

struct KCSerializeChild
{	
public:
	KCSERIALIZE_CODE();

	UPROPERTY()
	KCString		m_strTest;
};

 
class KCIncludeTest
{
public:
	KCSERIALIZE_CODE();

	KCIncludeTest() {}
	KCIncludeTest(float x, float y, float z)
	{
		m_SerializeChild.m_strTest = "WORKS!";
		std::cout << __LINE__ << std::endl;
		m_fX = x; m_fY = y; m_fZ = z;
		m_pSerializeChildTest = new KCSerializeChild();
		m_eEnumTest = ETEST::FOUR;
	}
	~KCIncludeTest()
	{
		DELETE_SAFELY(m_pSerializeChildTest);
	}



	void set(float x, float y, float z ){ m_fX = x; m_fY = y; m_fZ = z;}
private:
	UPROPERTY()
	float				m_fX = 0;
	UPROPERTY()
	float				m_fY = 0;
	UPROPERTY()
	float				m_fZ = 0;
	UPROPERTY()
	KCSerializeChild	m_SerializeChild;
	UPROPERTY()
	KCSerializeChild	*m_pSerializeChildTest = nullptr;
	UPROPERTY()
	ETEST				m_eEnumTest = ETEST::COUNT;
};