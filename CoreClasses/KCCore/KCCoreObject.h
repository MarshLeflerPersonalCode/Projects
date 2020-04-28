//copyright Marsh Lefler 2000-...
#pragma once
#include "KCDefines.h"
#include "Utils/KCAsserts.h"
///////////////////////////////////////////////////////////
//This class helps initialize all the objects in Core.
//Can do memory track in the future
/////////////////////////////////////////////////////////


#ifdef WITH_UE4
class KCCORE_API KCCoreObject : public UObject
#else
class KCCORE_API KCCoreObject
#endif
{
public:
	KCCoreObject(class KCCoreData *pCoreData = nullptr);	
	virtual ~KCCoreObject();
	
	//configures the CoreObject
	void										_configureCoreObject(class KCCoreData *pCoreData);
	
	//returns the core data
	class KCCoreData *							getCoreData() { return m_pCoreData; }

	//returns the unit type manager
	FORCEINLINE const class KCUnitTypeManager *	getUnitTypeManager() 
	{ 
		KCEnsureAlwaysReturnVal(m_pCoreData, nullptr);
		return m_pUnitTypeManager; 
	}
	//returns the database manager
	const class KCDatabaseManager *				getDatabaseManager() const;
	//returns the datagroup manager
	const class KCDataGroupManager *			getDatagroupManager() const;
	//retyrns the stat manager
	const class KCStatManager *					getStatManager() const;
protected:
	class KCCoreData							*m_pCoreData = nullptr;
	const class KCUnitTypeManager				*m_pUnitTypeManager = nullptr;

};

