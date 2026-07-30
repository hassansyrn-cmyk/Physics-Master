#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using PhysicsMaster.Core;
using PhysicsMaster.Services;
using PhysicsMaster.Gameplay;
using PhysicsMaster.UI;

public sealed class PhysicsMasterTests {

    [SetUp]
    public void Setup()
    {
        // Destroy any leftover objects before each test starts to ensure a clean slate
        Teardown();

        // Setup a dummy Screen size / PlayableWorldRect if not already initialized
        // This ensures the MapCoordinates work correctly in edit mode tests.
        System.Reflection.MethodInfo method = typeof(AppController).GetMethod("RecalculatePlayableBounds",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (method != null)
        {
            GameObject go = new GameObject("DummyAppController");
            AppController app = go.AddComponent<AppController>();
            method.Invoke(app, null);
            if (Application.isPlaying) Object.Destroy(go);
            else Object.DestroyImmediate(go);
        }
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up all GameObjects in the scene to prevent leaking across tests
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (go != null)
            {
                if (Application.isPlaying) Object.Destroy(go);
                else Object.DestroyImmediate(go);
            }
        }
    }

    [Test]
    public void All50LevelsHaveValidJson()
    {
        for (int i = 1; i <= 50; i++)
        {
            TextAsset asset = Resources.Load<TextAsset>($"Levels/level_{i:03d}");
            Assert.IsNotNull(asset, $"Level {i} asset was not found.");
            LevelData data = JsonUtility.FromJson<LevelData>(asset.text);
            Assert.IsNotNull(data, $"Level {i} JSON failed to parse.");
            Assert.AreEqual(i, data.id, $"Level {i} ID mismatch.");
            Assert.Greater(data.inkLimit, 0, $"Level {i} inkLimit should be > 0");
        }
    }

    [Test]
    public void All50LevelsCriticalObjectsInsidePlayableWorldRect()
    {
        Rect rect = AppController.PlayableWorldRect;
        Assert.AreNotEqual(0f, rect.width, "PlayableWorldRect width should be non-zero");

        for (int i = 1; i <= 50; i++)
        {
            TextAsset asset = Resources.Load<TextAsset>($"Levels/level_{i:03d}");
            LevelData data = JsonUtility.FromJson<LevelData>(asset.text);

            Vector2 ballPos = AppController.MapCoordinates(data.ballX, data.ballY);
            Vector2 goalPos = AppController.MapCoordinates(data.goalX, data.goalY);

            Assert.IsTrue(rect.Contains(ballPos), $"Level {i}: mapped Ball {ballPos} is outside PlayableWorldRect {rect}");
            Assert.IsTrue(rect.Contains(goalPos), $"Level {i}: mapped Goal {goalPos} is outside PlayableWorldRect {rect}");
        }
    }

    [Test]
    public void MainMenuCreationWithoutGameplayObjects()
    {
        GameObject appObj = new GameObject("AppController");
        AppController app = appObj.AddComponent<AppController>();
        app.ShowMainMenu();

        GameObject gameplayWorld = GameObject.Find("World");
        Assert.IsNull(gameplayWorld, "Main menu should not have gameplay world 'World' active.");

        GameObject dynamicBall = GameObject.Find("DynamicBall");
        Assert.IsNull(dynamicBall, "Main menu should not contain any gameplay 'DynamicBall'.");
    }

    [Test]
    public void NoDuplicateCanvasEventSystemOrDrawingController()
    {
        GameObject appObj = new GameObject("AppController");
        AppController app = appObj.AddComponent<AppController>();

        // Simulate creation of secondary elements
        GameObject secEventSystem = new GameObject("EventSystem");
        secEventSystem.AddComponent<EventSystem>();

        // Trigger AppController initialization again on a new instance
        GameObject appObj2 = new GameObject("AppController2");
        AppController app2 = appObj2.AddComponent<AppController>();

        // Check duplicates are cleaned up
        Assert.AreEqual(1, Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length, "Should only have one EventSystem.");
        Assert.AreEqual(1, Object.FindObjectsByType<AppController>(FindObjectsSortMode.None).Length, "Should only have one active AppController.");
    }

    [Test]
    public void ArabicLocalizationReturnsShapedRtlText()
    {
        LocalizationService.Initialize();
        if (!LocalizationService.Arabic)
        {
            LocalizationService.Toggle();
        }

        string word = "مرحباً";
        string shaped = LocalizationService.ShapeText(word);

        Assert.AreNotEqual(word, shaped, "Arabic text should be shaped/connected into RTL.");
    }

    [Test]
    public void EnglishLocalizationRemainsUnchanged()
    {
        LocalizationService.Initialize();
        if (LocalizationService.Arabic)
        {
            LocalizationService.Toggle();
        }

        string key = "continue";
        string translated = LocalizationService.T(key);
        Assert.AreEqual("CONTINUE", translated);
    }

    [Test]
    public void MixedArabicAndEnglishLinesProcessedSeparately()
    {
        string mixed = "English Line\nمرحباً";
        string processed = LocalizationService.ShapeText(mixed);

        string[] lines = processed.Split('\n');
        Assert.AreEqual("English Line", lines[0], "English line should remain completely unchanged.");
        Assert.AreNotEqual("مرحباً", lines[1], "Arabic line should be shaped correctly.");
    }

    [Test]
    public void Level1ContainsVisibleBallAndGoal()
    {
        GameObject appObj = new GameObject("AppController");
        AppController app = appObj.AddComponent<AppController>();
        app.StartLevel(1);

        GameObject ball = GameObject.Find("DynamicBall");
        Assert.IsNotNull(ball, "Level 1 must contain a 'DynamicBall'.");

        GameObject goal = GameObject.Find("Goal");
        Assert.IsNotNull(goal, "Level 1 must contain a 'Goal'.");

        Rect playableRect = AppController.PlayableWorldRect;
        Assert.IsTrue(playableRect.Contains(ball.transform.position), "Ball in level 1 must be within PlayableWorldRect.");
        Assert.IsTrue(playableRect.Contains(goal.transform.position), "Goal in level 1 must be within PlayableWorldRect.");
    }

    [Test]
    public void ReturnToMenuDestroysAllGameplayObjectsAndStrokes()
    {
        GameObject appObj = new GameObject("AppController");
        AppController app = appObj.AddComponent<AppController>();
        app.StartLevel(1);

        GameObject stroke = new GameObject("PlayerStroke");
        stroke.transform.SetParent(GameObject.Find("World").transform);

        app.ShowMainMenu();

        GameObject world = GameObject.Find("World");
        Assert.IsNull(world, "Gameplay world should be completely destroyed on returning to Main Menu.");

        GameObject strokeCheck = GameObject.Find("PlayerStroke");
        Assert.IsNull(strokeCheck, "All player drawn strokes must be destroyed on returning to Main Menu.");
    }

    [Test]
    public void AndroidBuildUsesNoneForRelease()
    {
        string buildScriptPath = "Assets/Editor/AndroidBuild.cs";
        Assert.IsTrue(File.Exists(buildScriptPath), "Build script must exist.");

        string content = File.ReadAllText(buildScriptPath);
        Assert.IsFalse(content.Contains("BuildOptions.Development"), "Build script must not use BuildOptions.Development!");
        Assert.IsTrue(content.Contains("BuildOptions.None"), "Build script must use BuildOptions.None!");
    }
}
#endif
