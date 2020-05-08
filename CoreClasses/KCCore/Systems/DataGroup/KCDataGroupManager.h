//copyright Marsh Lefler 2000-...
//this is a data group that stores properties in a child parent relationship.
#pragma once
#include "KCIncludes.h"
#include "Systems/DataGroup/KCDataGroup.h"
#include <unordered_map>

class KCCORE_API KCDataGroupManager : public KCCoreObject
{
public:	
	KCDataGroupManager();	
	~KCDataGroupManager();

	

	//loads all the loose files in a directory
	int32						loadLooseFiles( const WCHAR *pPath);

	//loads all the data groups and puts them in 1 master file
	bool						createMasterFile(const WCHAR *pPathToLooseFiles, const WCHAR *pPathToMasterFile);

	//returns all the data groups in the directory passed in
	int32						getDataGroupsInDirectory(const KCString &strPath, TArray<const KCDataGroup *> &mDataGroups) const;


	//returns a datagroup by file path and file name
	const KCDataGroup *			getDataGroupByFileName(const KCString &strPath) const;

	//when data groups load, some objects want to specify parents by a tag inside the datagroup. For instance DB Entries all have a name property and the inheritance tag named m_strParent looks for that tag to know what to inherit from
	void						addInhertianceByPropertyName(const TArray<const KCDataGroup *> &mDataGroups, const KCName &strPropertyName, const KCName &strInheritanceProperty) const;
private:
	void						_configureFileInheritance();
	void						_clean();
	
	std::unordered_map<KCString, KCDataGroup *>		m_DataGroups;	
	
};