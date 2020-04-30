// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

#include "CoreTest.h"
#include "Modules/ModuleManager.h"
#include "KCCoreModule.h"


IMPLEMENT_PRIMARY_GAME_MODULE(FCoreTest, CoreTest, "CoreTest" );

void FCoreTest::StartupModule()
{

	KCCoreModule &mModule = KCCoreModule::get();
	//UNITTYPE::KCUnitTypeManager *pManager = UNITTYPE::KCUnitTypeManager::getSingleton();// mModule.getUnitTypeManager();
}

void FCoreTest::ShutdownModule()
{

}