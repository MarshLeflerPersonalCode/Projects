#include "Systems/Stats/KCStatManager.h"


KCStatID			STATS::RANK = INVALID;

void _defineStats(KCStatManager *pManager)
{
	KCEnsureAlwaysReturn(pManager);
	STATS::RANK = pManager->getStatID("Rank");
}
