#include "KCStats.h"
#include "Systems/Stats/KCStatManager.h"
#include "Systems/Stats/Private/KCStatDefinition.h"

void STATS::defineStats(KCStatManager *pManager)
{
	KCEnsureAlwaysReturn(pManager);
	STATS::RANK = pManager->getStatID("Rank");
}
