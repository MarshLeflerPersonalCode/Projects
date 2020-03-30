// Copyright 1998-2019 Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class CoreTest : ModuleRules
{
	public CoreTest(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
		bUseUnity = true;
		PublicDependencyModuleNames.AddRange(new string[] { "Core", 
															"CoreUObject", 
															"Engine", 
															"InputCore",
															"Json",
															"JsonUtilities",															
															"KCCore"});

		//PublicIncludePaths.AddRange(new string[] { "CoreTest", "KCCore/Source/" });
		//PublicIncludePathModuleNames.AddRange(new string[] { "KCCore" });

		PrivateDependencyModuleNames.AddRange(new string[] {  });

		// Uncomment if you are using Slate UI
		PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });
		
		// Uncomment if you are using online features
		// PrivateDependencyModuleNames.Add("OnlineSubsystem");

		// To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
	}
}
