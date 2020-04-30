#include "KCCoreModule.h"
#include "Misc/Paths.h"

static KCCoreModule *g_pCoreModule(nullptr);

void KCCoreModule::StartupModule()
{
	m_CoreData._initialize(*(FPaths::ProjectContentDir() + TEXT("RawData/")));
}


void KCCoreModule::ShutdownModule()
{

}
IMPLEMENT_MODULE(KCCoreModule, KCCore)

const KCUnitTypeManager * KCCoreModule::getUnitTypeManager()
{

	return g_pCoreModule->m_CoreData.getUnitTypeManager();
}
