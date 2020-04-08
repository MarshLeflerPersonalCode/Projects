// CoreClasses.cpp : This file contains the 'main' function. Program execution begins and ends there.
//


#include "KCCore/KCDefines.h"
#include "KCCore/UnitTypes/KCUnitTypeManager.h"
#include "KCCore/UnitTypes/KCDefinedUnitTypes.h"
#include "KCCore/Containers/KCName.h"
#include "TestCases/KCDataGroupTestCase.h"
#include "KCCore/DataGroup/FileTypes/KCDataGroupSimpleXMLWriter.h"
#include "KCCore/DataGroup/FileTypes/KCDataGroupSimpleXMLReader.h"
#include <direct.h>
#include "TestCases/SerializeTest/KCIncludeTest.h"
#include <chrono>
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
	
	KCDataGroup mDataGroup;
	const KCName &strGroupname = mDataGroup.getGroupName();
	bool bIsEmpty = strGroupname == EMPTY_KCSTRING;
	KCIncludeTest mTest1( 12.5f, 13.5, 14.5f);
	KCByteWriter mWriter;
	mTest1.serialize(mWriter);
	mTest1.serialize(mDataGroup);
	KCIncludeTest mTest2;
	KCByteReader mReader(mWriter.getMemory(), mWriter.getArrayCount());
	mTest2.deserialize(mDataGroup);

	//KCString strData = KCDataGroupSimpleXMLWriter::writeDataGroupToString(mDataGroup);
	
	KCDataGroup mSecondGroup;
	KCDataGroupSimpleXMLReader::parseDataGroupFromFile(L"D:\\Personal\\Projects\\CoreClasses\\x64\\Intermediate\\CommandLineSerializer.cfg", mSecondGroup);
	std::chrono::high_resolution_clock::time_point t1 = std::chrono::high_resolution_clock::now();
	KCString strData = KCDataGroupSimpleXMLWriter::writeDataGroupToString(mSecondGroup);
	std::chrono::high_resolution_clock::time_point t2 = std::chrono::high_resolution_clock::now();
	std::chrono::duration<double> time_span = std::chrono::duration_cast<std::chrono::duration<double>>(t2 - t1);
	std::cout << strData << std::endl;
	std::cout << "time to parse: " << time_span.count() << "second(s)" << std::endl;
	KCDataGroupBinaryWriter::writeDataGroupToFile(L"D:\\Personal\\Projects\\CoreClasses\\x64\\Intermediate\\CommandLineSerializer.cfg.bin", mSecondGroup);
	
	bool bIsNumber1 = KCStringUtils::isNumber("-2323.0242f");
	bool bIsNumber2 = KCStringUtils::isNumber("-f");
	bool bIsNumber3 = KCStringUtils::isNumber("");
	bool bIsNumber4 = KCStringUtils::isNumber("0");
	bool bIsNumber5 = KCStringUtils::isNumber("3432");
	bool bIsNumber6 = KCStringUtils::isNumber("Hello - 000.003 2223");
	EDATATYPES eDataTypeString = DATATYPES_UTILS::getDataTypeRepresentingValue("testing");
	EDATATYPES eDataTypeInt32 = DATATYPES_UTILS::getDataTypeRepresentingValue("-23232");
	EDATATYPES eDataTypeInt32_2 = DATATYPES_UTILS::getDataTypeRepresentingValue("23232");
	EDATATYPES eDataTypeUInt32 = DATATYPES_UTILS::getDataTypeRepresentingValue("4294967295");
	EDATATYPES eDataTypeFloat = DATATYPES_UTILS::getDataTypeRepresentingValue("23232.0f");
	EDATATYPES eDataTypeFloat2 = DATATYPES_UTILS::getDataTypeRepresentingValue("-23232.0f");
	EDATATYPES eDataTypeInt64 = DATATYPES_UTILS::getDataTypeRepresentingValue("9223372036854775807");
	EDATATYPES eDataTypeInt642 = DATATYPES_UTILS::getDataTypeRepresentingValue("-9223372036854775808");
	EDATATYPES eDataTypeUInt64 = DATATYPES_UTILS::getDataTypeRepresentingValue("18446744073709551615");
	EDATATYPES eDataTypeUnknow = DATATYPES_UTILS::getDataTypeRepresentingValue("18446744073709551616");
	EDATATYPES eDataTypeUnknow2 = DATATYPES_UTILS::getDataTypeRepresentingValue("-9223372036854775809");
	std::cin.ignore();	//just ignores the next key press
	exit(0);

}


