using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FHG
{
	public class BuildScripts
	{
		static string[] SharedInfrastructure = { 
				"Assets/AdamLighting/Scenes/Area light.unity",
				/*"Assets/Scenes/Content/MainMenu_Content.unity",
				"Assets/Scenes/UI/MainMenu_UI.unity",
				"Assets/Scenes/Content/TestLevel_Content.unity",
				"Assets/Scenes/UI/TestLevel_UI.unity",
				"Assets/Scenes/Content/Lobby_Content.unity",
				"Assets/Scenes/UI/Lobby_UI.unity",
				"Assets/Scenes/Content/StartGame_Content.unity",
				"Assets/Scenes/UI/StartGame_UI.unity",*/
			};

		static void Build_Spark()
		{
			CommandLineArguments arguments = new CommandLineArguments();
			ProcessCommandLineArguments(arguments);

			Build_Spark_Internal(arguments);
		}

		private static void Build_Spark_Internal(CommandLineArguments arguments)
		{
			SetVersion(arguments);

			PerformBuildTargets(arguments);
		}

		[MenuItem("Build/Build Central Win32 (Development)", false, 1)]
		static void Build_Central_Win32_Development()
		{
			CommandLineArguments arguments = new CommandLineArguments();
			arguments.winBuild = true;
			arguments.releaseBuild = false;
			arguments.bumpVersion = true;

			Build_Spark_Internal(arguments);
		}

		[MenuItem("Build/Build Central Win32 (Release)", false, 2)]
		static void Build_Central_Win32_Release()
		{
			CommandLineArguments arguments = new CommandLineArguments();
			arguments.winBuild = true;
			arguments.releaseBuild = true;
			arguments.bumpVersion = true;

			Build_Spark_Internal(arguments);
		}

		static private void Build(string[] levels, string locationPath, BuildTarget buildTarget, BuildOptions options)
		{
			// Already building, bail
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}

			string error = BuildPipeline.BuildPlayer(levels, locationPath, buildTarget, options);
			if (error != null && error != "" && UnityEditorInternal.InternalEditorUtility.inBatchMode)
			{
				EditorApplication.Exit(-1);
			}
		}

		private static void SetVersion(CommandLineArguments arguments)
		{
			/*if (arguments.bumpVersion)
			{
				BuildVersion buildVersion = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Tools/Version.prefab", typeof(BuildVersion)) as BuildVersion;
				EonAssert.AssertUnityObjectNotNull(buildVersion, "Build could not find BuildVersion prefab at Assets/Prefabs/Tools/Version.prefab. Did it get moved?");

				buildVersion.GenerateBuildVersion();
				EditorUtility.SetDirty(buildVersion);

				AssetDatabase.SaveAssets();
			}*/
		}

		private static string[] GetLevels()
		{
			return SharedInfrastructure;
		}

		private static void PerformBuildTargets(CommandLineArguments arguments)
		{
			BuildOptions buildOptions = BuildOptions.None;

			if (arguments.winBuild)
			{
				SetSharedCentralBuildSettings();
				string buildPath = arguments.buildPath;
				if (buildPath == null)
				{
					buildPath = arguments.releaseBuild ? GetBaseBuildPath("/../Build/Central/Windows/Release/Build_Test.exe") : GetBaseBuildPath("/../Build/Central/Windows/Development/Build_Test.exe");
				}

				GenerateBuildPathFolders(buildPath);
				Build(GetLevels(), buildPath, BuildTarget.StandaloneWindows, buildOptions);
			}
			
			if (arguments.osxBuild)
			{
				SetSharedCentralBuildSettings();
				string buildPath = arguments.buildPath;
				if (buildPath == null)
				{
					buildPath = arguments.releaseBuild ? GetBaseBuildPath("/../Build/Central/OSX/Release/Build_Test.exe") : GetBaseBuildPath("/../Build/Central/OSX/Development/Build_Test.exe");
				}

				GenerateBuildPathFolders(buildPath);
				Build(GetLevels(), buildPath, BuildTarget.StandaloneOSXIntel, buildOptions);
			}
		}

		private static void SetSharedCentralBuildSettings()
		{
			PlayerSettings.productName = "Build_Test";
			PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_2_0);
			UnityEditor.EditorUserBuildSettings.SetPlatformSettings("Standalone", "CopyPDBFiles", "true");
		}

		private static void GenerateBuildPathFolders(string buildPath)
		{
			// axe the trailing file
			string directoryName = System.IO.Path.GetDirectoryName(buildPath);
			System.IO.Directory.CreateDirectory(directoryName);
		}

		private class CommandLineArguments
		{
			public bool bumpVersion = false;									// --bumpversion; --bv
			public bool releaseBuild = false;                                   // --release; --r

			public bool winBuild = false;										// --win
			public bool osxBuild = false;										// --osx

			public string buildPath = null;                                     // --path "path/To/Build"

			public bool buildDLC = false;										// --builddlc
		}

		private static void ProcessCommandLineArguments(CommandLineArguments arguments)
		{
			string[] commands = System.Environment.GetCommandLineArgs();
			
			for (int i = 0; i < commands.Length; i++)
			{
				string command = commands[i];

				if (command == "--bumpversion" || command == "--bv")
				{
					arguments.bumpVersion = true;
				}
				else if (command == "--release" || command == "--r")
				{
					arguments.releaseBuild = true;
				}
				else if (command == "--win")
				{
					arguments.winBuild = true;
				}
				else if (command == "--osx")
				{
					arguments.osxBuild = true;
				}
				else if (command == "--path")
				{
					// We have at least one more argument
					if (i < commands.Length)
					{
						arguments.buildPath = commands[i + 1];
					}
				}
				else if (command == "--builddlc")
				{
					arguments.buildDLC = true;
				}
			}
		}

		private static string GetBaseBuildPath(string extraPath)
		{
			string path = Application.dataPath + extraPath;
			return System.IO.Path.GetFullPath(path);
		}
	}
}
