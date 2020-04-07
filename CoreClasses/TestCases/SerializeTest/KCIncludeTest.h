//copyright Marsh Lefler 2000-...
#pragma once
#include "IO/KCByteReader.h"
#include "IO/KCByteWriter.h"
#include "DataGroup/KCDataGroup.h"
#include "KCIncludeTest.serialized.h"

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
		std::cout << __FILE__ << std::endl;
		m_fX = x; m_fY = y; m_fZ = z;
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
};