// Copyright 1998-2018 Epic Games, Inc. All Rights Reserved.
using UnrealBuildTool;
using System.IO;
namespace UnrealBuildTool.Rules
{
	public class KCCore : ModuleRules
	{
		public KCCore(ReadOnlyTargetRules Target) : base(Target)
		{            
            runKCHeaderCompiler(Target);
            bUseUnity = true;
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

        public void runKCHeaderCompiler(ReadOnlyTargetRules Target)
        {
            System.Diagnostics.Stopwatch m_StopWatch = System.Diagnostics.Stopwatch.StartNew();
            System.Console.WriteLine("NAME:\t\t\t\t" + Target.Name);
            System.Console.WriteLine("VERSION:\t\t\t" + Target.Version.BuildVersionString);
            System.Console.WriteLine("TYPE:\t\t\t\t" + Target.Type);            
            System.Console.WriteLine("BUILD_EDITOR:\t\t" + Target.bBuildEditor);
            System.Console.WriteLine("PLATFORM:\t\t\t" + Target.Platform);
            System.Console.WriteLine("CONFIGURATION:\t" + Target.Configuration);
            System.Console.WriteLine("ARCHITECTURE:\t" + Target.Architecture);
            System.Console.WriteLine("PROJECTFILE:\t\t" + Target.ProjectFile);
            System.Console.WriteLine("SOLUTIONDIR:\t\t" + Target.SolutionDirectory);
            System.Console.WriteLine("BUILD_PLUGINS:\t" + Target.bPrecompile);
            System.Console.WriteLine("ADD_PLUGINS:\t\t" + Target.AdditionalPlugins);
            System.Console.WriteLine("PAK_SIGNING:\t\t" + Target.PakSigningKeysFile);
            //string mPlatform = Target.Platform.ToString();
            string strProjectPathAndFile = Target.ProjectFile.ToString();
            string strProjectFile = Path.GetFileName(strProjectPathAndFile);
            string strProjectPath = strProjectPathAndFile.Substring(0, strProjectPathAndFile.Length - strProjectFile.Length);
            string strCommandlineSerializer = Path.GetFullPath( Path.Combine(strProjectPath, "Binaries\\CommandLineSerializer.exe") );
            string strIntermediateDirectory = Path.GetFullPath(Path.Combine(strProjectPath, "Intermediate\\Build\\" + Target.Platform + "\\" + ((Target.Type.ToString() == "Game")?"UE4":"UE4Editor") + "\\CommandlineSerializer\\" + Target.Name + "\\"));
            PublicIncludePaths.Add(strIntermediateDirectory);
            Directory.CreateDirectory(strIntermediateDirectory);
            string strArguments = "-SourceDir '" + strProjectPath + "Source\\KCCore\\'";
            strArguments = strArguments + " -IntermediateDir '" + strIntermediateDirectory + "'";
            strArguments = strArguments + " -TypeDefs 'KCDatabaseGuid=int32,StatID=int16'";
            strArguments = strArguments + " -ForceRecompile";
            //strArguments = strArguments + " -Debug";
            System.Console.WriteLine("Compiling Headers: " + strProjectPath);
            System.Console.WriteLine("Running Command line Serializer at: " + strCommandlineSerializer);
            System.Console.WriteLine("Intermediate Folder at: " + strIntermediateDirectory);
            System.Console.WriteLine("Arguments: " + strArguments);
            System.Diagnostics.ProcessStartInfo cmdsi = new System.Diagnostics.ProcessStartInfo(strCommandlineSerializer);
            cmdsi.Arguments = strArguments;
            cmdsi.UseShellExecute = false;
            cmdsi.RedirectStandardOutput = true;
            cmdsi.RedirectStandardError = true;
            cmdsi.CreateNoWindow = true;
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(cmdsi);
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine();
                System.Console.WriteLine(line);
                // do something with line
            }
            System.Console.WriteLine("Done running command line Serailizer.");// Total time: " + m_StopWatch.Elapsed.TotalSeconds.ToString());
        }
    }
}
