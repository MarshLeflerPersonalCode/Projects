// CoreClasses.cpp : This file contains the 'main' function. Program execution begins and ends there.
//


#include "KCCore/KCDefines.h"
#include "KCCore/UnitTypes/KCUnitTypeManager.h"
#include "KCCore/UnitTypes/KCDefinedUnitTypes.h"
#include "KCCore/Containers/KCName.h"
#include "TestCases/KCDataGroupTestCase.h"
#include <direct.h>
#include <assert.h>
#define GetCurrentDir _getcwd

static void funTest()
{
	static int32 iCount(1);
	//KCEnsureOnce(false);
	std::cout << "True" << iCount << std::endl;
	iCount++;
}

int main()
{
	char cCurrentPath[FILENAME_MAX];
	if (!GetCurrentDir(cCurrentPath, sizeof(cCurrentPath)))
	{
		return errno;
	}

	cCurrentPath[sizeof(cCurrentPath) - 1] = '\0'; /* not really required */

	printf("The current working directory is %s\n", cCurrentPath);


	UNITTYPE::KCUnitTypeManager mManager;
	if (mManager.parseUnitTypeFile(L"..\\..\\UE4Projects\\CoreTest\\Content\\RawData\\unittypes.bin"))
	{
		bool bIsA = mManager.getCategoryByIndex(0)->IsA(1, CORE_UNITTYPE_ITEMS::ANY);
		if (bIsA && mManager.IsA("ITEMS", "NEW7", "NEW7"))
		{
			std::cout << "True\n";
		}
		else
		{
			std::cout << "False\n";
		}
	}
	funTest();
	funTest();
	funTest();
	//KCEnsureOnce(false);
	testDataGroupSavingAndLoad(L".\\content\\DataGroupTestCast.dat");
	KCString strInput;
	std::getline(std::cin, strInput);
	exit(0);

}


