#include "KCCoreData.h"

KCCoreData::KCCoreData()
{

}

KCCoreData::~KCCoreData()
{

}

void KCCoreData::_initialize()
{	
	
	
	
	
	
	m_DataGroupManager._configureCoreObject(this);
	m_DataGroupManager.loadLooseFiles(L"Content\\");	

	m_DatabaseManager._configureCoreObject(this);
	m_DatabaseManager.reload();	

	m_StatManager._configureCoreObject(this);
	m_StatManager.initialize();

	m_UnitTypeManager._configureCoreObject(this);
	m_UnitTypeManager.configureUnitTypeByConfigFile( L"..\\UE4Projects\\CoreTest\\Content\\RawData\\unittypes.bin");
}
