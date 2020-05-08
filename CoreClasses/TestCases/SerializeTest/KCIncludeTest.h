//copyright Marsh Lefler 2000-...
#pragma once
#include "Systems/DataGroup/KCDataGroup.h"
#include "Database/KCDBEntry.h"
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

 
class KCIncludeTest : public FKCDBEntry
{
public:
	KCSERIALIZE_CODE();

	KCIncludeTest() 
	{
		m_pSerializeChildTest = new KCSerializeChild();
		m_pSerializeChildTest->m_strTest = "Working!";

	}
	KCIncludeTest(float x, float y, float z)
	{
		m_SerializeChild.m_strTest = "WORKS!";
		m_fX = x; m_fY = y; m_fZ = z;
		m_eEnumTest = ETEST::FOUR;
			
			
		m_Array.Add(KC_NEW KCSerializeChild());
		m_Array.Last()->m_strTest = std::to_string(rand());
		
	}
	~KCIncludeTest()
	{
		DELETE_SAFELY(m_pSerializeChildTest);
		m_Array.deleteContents();
		m_Array.clean();
	}



	void set(float x, float y, float z) {}// m_fX = x; m_fY = y; m_fZ = z;
protected:
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
	
	UPROPERTY(DisplayName="Child Serialized Objects")
	KCTArray<KCSerializeChild *>	m_Array;
	UPROPERTY(DisplayName = "Array of Values")
	KCTArray<float>					m_ArrayOfValues;
	//UPROPERTY(DisplayName = "Child REf Objects")
	//KCTArray<KCSerializeChild >	m_ArrayRef;
	//The stat which will be used in the graph. Most times it's the rank.
	UPROPERTY(Category = "TESTING", DisplayName = "Unit Type Test", Meta = (UnitTypeCategory = "Items", UnitTypeFilter = "New4"))
	KCString							m_strUnitType = "";
	UPROPERTY(Category = "TESTING", DisplayName = "Folder Path", Meta = (FolderPath = "Content"))
	KCString							m_strFolderTest = "";
	UPROPERTY(Category = "TESTING", DisplayName = "File", Meta = (FilePath = "Content", FileFilter = "*.dat"))
	KCString							m_strFileTest = "";
	UPROPERTY(Category = "TESTING", DisplayName = "Parent", Meta=(List="Testing"))
	KCString							m_strParent = "";
};