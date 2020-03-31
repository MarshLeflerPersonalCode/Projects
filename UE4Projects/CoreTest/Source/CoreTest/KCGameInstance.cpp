// Fill out your copyright notice in the Description page of Project Settings.


#include "KCGameInstance.h"
#include "UnitTypes/UE4DefinedUnitTypes.h"

void UKCGameInstance::Init()
{
	Super::Init();
	UNITTYPE::KCUnitTypeManager mManager;
	if (mManager.parseUnitTypeFile(L"RawData/unittypes.bin"))
	{
		UNITTYPE::defineUnitTypes(&mManager);
		GEngine->AddOnScreenDebugMessage(-1, 60.f, FColor::Green, TEXT("Loaded unit types file successfully."));
		if (mManager.IsA("ITEMS", "NEW7", UNITTYPE_ITEMS::ANY))
		{
			GEngine->AddOnScreenDebugMessage(-1, 60.f, FColor::Green, TEXT("ISA is working."));
		}
		else
		{
			GEngine->AddOnScreenDebugMessage(-1, 60.f, FColor::Red, TEXT("ISA failed."));
		}
	}
	else
	{
		GEngine->AddOnScreenDebugMessage(-1, 60.f, FColor::Red, TEXT("Wasn't able to load unit types file."));
	}

}
