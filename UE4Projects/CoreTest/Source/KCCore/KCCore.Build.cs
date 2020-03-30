// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.

namespace UnrealBuildTool.Rules
{
	public class KCCore : ModuleRules
	{
		public KCCore(ReadOnlyTargetRules Target) : base(Target)
		{
			PublicIncludePaths.Add("KCCore");
			PublicIncludePaths.Add("KCCore/CoreClasses/");


			PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

			PublicDependencyModuleNames.AddRange(
				new string[]
				{
					"Core",
					"CoreUObject",
					"Engine",
					"Json",
					"JsonUtilities",
				}
				);

			PublicDefinitions.Add("USING_UE4=1");
			
		}
	}
}
