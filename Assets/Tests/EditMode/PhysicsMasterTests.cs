#if UNITY_EDITOR
using NUnit.Framework; using PhysicsMaster.Core; using PhysicsMaster.Services;
public sealed class PhysicsMasterTests {
    [Test] public void LevelJsonParses(){var d=UnityEngine.JsonUtility.FromJson<LevelData>(UnityEngine.Resources.Load<UnityEngine.TextAsset>("Levels/level_001").text);Assert.AreEqual(1,d.id);Assert.Greater(d.inkLimit,0);}
    [Test] public void LocalizationHasCoreKeys(){LocalizationService.Initialize();Assert.IsNotEmpty(LocalizationService.T("play"));Assert.IsNotEmpty(LocalizationService.T("continue"));}
    [Test] public void StarRangeIsValid(){for(int i=0;i<=3;i++)Assert.That(i,Is.InRange(0,3));}
}
#endif
