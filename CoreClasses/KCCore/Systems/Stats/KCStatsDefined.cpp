#include "KCStatsDefined.h"
#include "Systems/Stats/KCStatManager.h"


KCStatID			KCSTATS::RANK = INVALID;

void _defineStats(KCStatManager *pManager)
{
	KCEnsureAlwaysReturn(pManager);
	KCSTATS::RANK = pManager->getStatID("Rank");
}
