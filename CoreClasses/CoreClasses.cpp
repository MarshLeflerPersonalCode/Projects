// CoreClasses.cpp : This file contains the 'main' function. Program execution begins and ends there.
//


#include "KCCore/KCDefines.h"
#include "KCCore/UnitTypes/KCUnitTypeManager.h"
#include "KCCore/IO/KCByteReader.h"

#include <direct.h>
#define GetCurrentDir _getcwd


int main()
{
	char cCurrentPath[FILENAME_MAX];
	if (!GetCurrentDir(cCurrentPath, sizeof(cCurrentPath)))
	{
		return errno;
	}

	cCurrentPath[sizeof(cCurrentPath) - 1] = '\0'; /* not really required */

	printf("The current working directory is %s\n", cCurrentPath);


	size_t iBufferSize(10000);
	int8 mBuffer[10000];
	size_t iFileSize = 0;
	FILE *pFileReader = null;
	if (fopen_s(&pFileReader, "..\\UE4Projects\\CoreTest\\Content\\RawData\\unittypes.bin", "r") == 0)
	{
		fseek(pFileReader, 0, SEEK_END);
		iFileSize = ftell(pFileReader);
		fseek(pFileReader, 0, SEEK_SET);
		fread_s(mBuffer, iBufferSize, sizeof(int8), iFileSize, pFileReader);
		fclose(pFileReader);
	}

	//KCByteReader mByteReader(mBuffer, (int32)iFileSize);
	UNITTYPE::KCUnitTypeManager mManager;
	mManager.parseUnitTypeFile(mBuffer, (int32)iFileSize);
	
	bool bIsA = mManager.getCategoryByIndex(0)->IsA(1, 0);
	if (mManager.IsA("ITEMS","NEW7", "NEW7"))
	{
		std::cout << "True\n";
	}
	else
	{
		std::cout << "False\n";
	}

}


