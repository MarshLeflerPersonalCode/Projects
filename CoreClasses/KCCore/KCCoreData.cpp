#include "KCCoreData.h"

KCCoreData::KCCoreData()
{

}

KCCoreData::~KCCoreData()
{

}

void KCCoreData::_initialize(const KCWString &mContentPath)
{	
	
	 
	
	m_strContentPath = mContentPath;

	m_DataGroupManager._configureCoreObject(this);
	m_DataGroupManager.loadLooseFiles(m_strContentPath.c_str());

	m_DatabaseManager._configureCoreObject(this);
	m_DatabaseManager.reload();	

	m_StatManager._configureCoreObject(this);
	m_StatManager.initialize();

	m_UnitTypeManager._configureCoreObject(this);
	m_UnitTypeManager.configureUnitTypeByConfigFile( (m_strContentPath + L"unittypes.bin").c_str());
}
