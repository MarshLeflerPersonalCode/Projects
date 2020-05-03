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
		//std::cout << __LINE__ << std::endl;
		m_fX = x; m_fY = y; m_fZ = z;
		//m_pSerializeChildTest = new KCSerializeChild();
		//m_pSerializeChildTest->m_strTest ="Working!";
		m_eEnumTest = ETEST::FOUR;
		for (int32 i = 0; i < 10; i++)
		{
			/*switch (i % 2)
			{
			default:
				m_Array.add(ETEST::FOUR);
				break;
			case 0:
				m_Array.Add(ETEST::ONE);
				break;
			case 1:
				m_Array.Add(ETEST::TWO);
				break;
			}*/
			//m_Array.add( rand());
			//m_Array.add(std::to_string(rand()));
			m_Array.Add(KC_NEW KCSerializeChild());
			m_Array.Last()->m_strTest = std::to_string(rand());
		}
	}
	~KCIncludeTest()
	{
		DELETE_SAFELY(m_pSerializeChildTest);
		m_Array.deleteContents();
		m_Array.clean();
	}



	void set(float x, float y, float z ){ m_fX = x; m_fY = y; m_fZ = z;}
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