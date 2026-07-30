#if UNITY_EDITOR
using UnityEditor; using UnityEditor.Build.Reporting; using UnityEngine; using System; using System.IO;
public static class AndroidBuild {
    public static void BuildApk(){Build(false);}
    public static void BuildAab(){Build(true);}
    static void Build(bool aab){ProjectSetup.Ensure();Directory.CreateDirectory("build/Android");EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);EditorUserBuildSettings.buildAppBundle=aab;string path=aab?"build/Android/PhysicsMaster.aab":"build/Android/PhysicsMaster.apk";var report=BuildPipeline.BuildPlayer(new[]{ProjectSetup.ScenePath},path,BuildTarget.Android,BuildOptions.None);if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Android build failed: "+report.summary.result);Debug.Log("BUILD_OK "+path);}
}
#endif
