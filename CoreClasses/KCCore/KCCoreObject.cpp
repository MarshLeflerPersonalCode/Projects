#include "KCCoreObject.h"
#include "KCCoreData.h"

static uint64 g_iObjectCount(0);

KCCoreObject::KCCoreObject(KCCoreData *pCoreData)
{
	g_iObjectCount++;
	if (pCoreData != null)
	{
		_configureCoreObject(pCoreData);
	}
}

KCCoreObject::~KCCoreObject()
{
	g_iObjectCount--;
}

void KCCoreObject::_configureCoreObject(class KCCoreData *pCoreData)
{
	m_pCoreData = pCoreData;
	m_pUnitTypeManager = pCoreData->getUnitTypeManager();
}

const KCDatabaseManager * KCCoreObject::getDatabaseManager() const
{
	KCEnsureAlwaysReturnVal(m_pCoreData, nullptr);
	return m_pCoreData->getDatabaseManager();
}

const KCDataGroupManager * KCCoreObject::getDatagroupManager() const
{
	KCEnsureAlwaysReturnVal(m_pCoreData, nullptr);
	return m_pCoreData->getDataGroupManager();
}

const KCStatManager * KCCoreObject::getStatManager() const
{
	KCEnsureAlwaysReturnVal(m_pCoreData, nullptr);
	return m_pCoreData->getStatManager();
}

