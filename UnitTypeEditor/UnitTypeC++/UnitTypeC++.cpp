// UnitTypeC++.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include "KCDefines.h"
#include "UnitTypes/KCUnitTypeManager.h"
#include "Interfaces/KCByteReader.h"
#define WINDOWS

#ifdef WINDOWS
#include <direct.h>
#define GetCurrentDir _getcwd
#else
#include <unistd.h>
#define GetCurrentDir getcwd
#endif


int main()
{
	UNITTYPE::KCUnitTypeManager mManager;
	if (UNITTYPE::ISA(EUNITTYPES_ITEMS::NEW6, EUNITTYPES_ITEMS::ANY))
	{
		std::cout << "True\n";
	}
	else
	{
		std::cout << "False\n";
	}
	

	char cCurrentPath[FILENAME_MAX];

	if (!GetCurrentDir(cCurrentPath, sizeof(cCurrentPath)))
	{
		return errno;
	}

	cCurrentPath[sizeof(cCurrentPath) - 1] = '\0'; /* not really required */

	printf("The current working directory is %s\n", cCurrentPath);

	
	size_t iBufferSize(1000);
	int8 mBuffer[1000];
	size_t iFileSize = 0;
	FILE *pFileReader = null;
	if (fopen_s(&pFileReader, "..\\executable\\unittypes.bin", "r") == 0)
	{
		fseek(pFileReader, 0, SEEK_END);
		iFileSize = ftell(pFileReader);
		fseek(pFileReader, 0, SEEK_SET);
		fread_s(mBuffer, iBufferSize, sizeof(int8), iFileSize, pFileReader);
		fclose(pFileReader);
	}

	//KCByteReader mByteReader(mBuffer, (int32)iFileSize);
	mManager.parseUnitTypeFile(mBuffer, (int32)iFileSize);


	std::cout << UNITTYPE::getUnitTypeName(EUNITTYPES_ITEMS::NEW4);
    std::cout << "\nPress Enter To Exit\n";
	std::cin.get();
	return 0;
} 
