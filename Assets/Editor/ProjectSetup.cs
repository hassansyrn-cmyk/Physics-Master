#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using PhysicsMaster.Core;

public static class ProjectSetup
{
    public const string ScenePath = "Assets/Scenes/Main.unity";

    [InitializeOnLoadMethod]
    private static void AutoSetup()
    {
        EditorApplication.delayCall += Ensure;
    }

    public static void Ensure()
    {
        EnsureMainScene();
        ConfigureBuildScenes();
        ConfigurePlayerSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void EnsureMainScene()
    {
        if (File.Exists(ScenePath))
        {
            return;
        }

        Directory.CreateDirectory("Assets/Scenes");

        var scene = EditorSceneManager.NewScene(
            NewSceneSetup.DefaultGameObjects,
            NewSceneMode.Single);

        var appControllerObject = new GameObject("AppController");
        appControllerObject.AddComponent<AppController>();

        EditorSceneManager.SaveScene(scene, ScenePath);
    }

    private static void ConfigureBuildScenes()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };
    }

    private static void ConfigurePlayerSettings()
    {
        NamedBuildTarget androidTarget = NamedBuildTarget.Android;

        PlayerSettings.companyName = "Afaq Games";
        PlayerSettings.productName = "Physics Master: Draw & Solve";
        PlayerSettings.bundleVersion = "0.2.0";

        PlayerSettings.SetApplicationIdentifier(
            androidTarget,
            "com.afaq.physicsmaster");

        PlayerSettings.Android.bundleVersionCode = 2;

        PlayerSettings.Android.minSdkVersion =
            AndroidSdkVersions.AndroidApiLevel26;

        PlayerSettings.Android.targetSdkVersion =
            (AndroidSdkVersions)36;

        PlayerSettings.SetScriptingBackend(
            androidTarget,
            ScriptingImplementation.IL2CPP);

        PlayerSettings.Android.targetArchitectures =
            AndroidArchitecture.ARM64;

        PlayerSettings.defaultInterfaceOrientation =
            UIOrientation.Portrait;

        PlayerSettings.SetApiCompatibilityLevel(
            androidTarget,
            ApiCompatibilityLevel.NET_Unity_4_8);
    }
}

#endif
