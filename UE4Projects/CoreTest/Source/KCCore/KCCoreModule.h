#pragma once

#include "Modules/ModuleManager.h"
#include "CoreClasses/KCCoreData.h"

/**
* The public interface to this module.  In most cases, this interface is only public to sibling modules
* within this plugin.
*/
class KCCoreModule : public IModuleInterface
{
public:
	/**
	* Singleton-like access to this module's interface.  This is just for convenience!
	* Beware of calling this during the shutdown phase, though.  Your module might have been unloaded already.
	*
	* @return Returns singleton instance, loading the module on demand if needed
	*/
	static FORCEINLINE KCCoreModule& get()
	{
		return FModuleManager::LoadModuleChecked< KCCoreModule >("KCCore");
	}

	/**
	* Checks to see if this module is loaded and ready.  It is only valid to call Get() if IsAvailable() returns true.
	*
	* @return True if the module is loaded and ready to use
	*/
	static inline bool isAvailable()
	{
		return FModuleManager::Get().IsModuleLoaded("KCCore");
	}
	void StartupModule() override;
	void ShutdownModule() override;

	//returns the unit type manager
	static const KCUnitTypeManager *	getUnitTypeManager();
	//returns the core data
	static KCCoreData *					getCoreData();
private:
	KCCoreData							m_CoreData;	
};
