//copyright Marsh Lefler 2000-...
//this is a data group that stores properties in a child parent relationship.
#pragma once
#include "Systems/DataGroup/KCDataGroup.h"
#include <unordered_map>

class KCDataGroupManager
{
public:	
	KCDataGroupManager();	
	~KCDataGroupManager();

	static KCDataGroupManager * getSingleton();

	//loads all the loose files in a directory
	int32						loadLooseFiles(const WCHAR *pPath);

	//loads all the data groups and puts them in 1 master file
	bool						createMasterFile(const WCHAR *pPathToLooseFiles, const WCHAR *pPathToMasterFile);

	//returns all the data groups in the directory passed in
	int32						getDataGroupsInDirectory(const KCString &strPath, KCTArray<const KCDataGroup *> &mDataGroups);

private:

	void						_clean();
	
	std::unordered_map<KCString, KCDataGroup *>					m_DataGroups;	
};