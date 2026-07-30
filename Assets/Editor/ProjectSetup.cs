#if UNITY_EDITOR
using UnityEditor; using UnityEditor.SceneManagement; using UnityEngine; using PhysicsMaster.Core;
public static class ProjectSetup {
    public const string ScenePath="Assets/Scenes/Main.unity";
    [InitializeOnLoadMethod] static void Auto(){EditorApplication.delayCall+=Ensure;}
    public static void Ensure(){
        if(!System.IO.File.Exists(ScenePath)){System.IO.Directory.CreateDirectory("Assets/Scenes");var s=EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects,NewSceneMode.Single);new GameObject("AppController").AddComponent<AppController>();EditorSceneManager.SaveScene(s,ScenePath);}
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        PlayerSettings.companyName="Afaq Games";PlayerSettings.productName="Physics Master: Draw & Solve";PlayerSettings.bundleVersion="0.2.0";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.afaq.physicsmaster");
        PlayerSettings.Android.bundleVersionCode=2;PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.Android.targetSdkVersion=(AndroidSdkVersions)36;PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;PlayerSettings.defaultInterfaceOrientation=UIOrientation.Portrait;
        PlayerSettings.SetApiCompatibilityLevel(UnityEditor.Build.NamedBuildTarget.Android,ApiCompatibilityLevel.NET_Standard_2_1);
        AssetDatabase.SaveAssets();
    }
}
#endif
