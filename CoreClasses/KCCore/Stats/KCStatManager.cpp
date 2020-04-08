#include "KCStatManager.h"







static STATS::KCStatManager *g_pStatManager(nullptr);

STATS::KCStatManager::KCStatManager()
{
	if (g_pStatManager == nullptr)
	{
		g_pStatManager = this;
	}
	
}

STATS::KCStatManager::~KCStatManager()
{
	
	
}



STATS::KCStatManager * STATS::KCStatManager::getSingleton()
{
	if (g_pStatManager == nullptr)
	{
		KC_NEW KCStatManager();
	}
	return g_pStatManager;
}
