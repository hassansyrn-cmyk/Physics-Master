using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PhysicsMaster.Gameplay;
using PhysicsMaster.Services;
using PhysicsMaster.UI;

namespace PhysicsMaster.Core
{
    public sealed class AppController : MonoBehaviour
    {
        private Canvas canvas;
        private Camera cam;
        private DrawingController drawing;
        private LevelData level;
        private Text hudInk;
        private Text hudTitle;
        private float elapsed;
        private bool completed;
        private bool sandbox;
        private GameObject world;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            LocalizationService.Initialize();

            cam = Camera.main;

            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.backgroundColor = Theme.Background;
            }

            if (EventSystem.current == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            canvas = UiFactory.Canvas();
            ShowMainMenu();
        }

        private void Update()
        {
            if (drawing != null && hudInk != null)
            {
                hudInk.text =
                    $"{LocalizationService.T("ink")} {drawing.RemainingInk:0.0}";

                if (drawing.SimulationRunning && !completed)
                {
                    elapsed += Time.deltaTime;
                }
            }

            if (drawing != null &&
                drawing.SimulationRunning &&
                !completed)
            {
                GameObject ball = FindBall();

                if (ball != null && ball.transform.position.y < -6f)
                {
                    ShowFailure();
                }
            }
        }

        private void ResetScreen()
        {
            Time.timeScale = 1f;

            if (drawing != null)
            {
                Destroy(drawing);
            }

            if (world != null)
            {
                Destroy(world);
            }

            world = null;
            drawing = null;
            hudInk = null;
            hudTitle = null;

            if (canvas != null)
            {
                UiFactory.Clear(canvas.transform);
            }
        }

        public void ShowMainMenu()
        {
            sandbox = false;

            ResetScreen();
            CreateLabBackdrop();

            Text title = UiFactory.Label(
                canvas.transform,
                "PHYSICS MASTER",
                new Vector2(0f, 620f),
                new Vector2(900f, 120f),
                72,
                Theme.Navy);

            title.fontStyle = FontStyle.Bold;

            UiFactory.Label(
                canvas.transform,
                "DRAW • TEST • SOLVE",
                new Vector2(0f, 530f),
                new Vector2(700f, 70f),
                31,
                Theme.Purple);

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("continue"),
                new Vector2(0f, 300f),
                new Vector2(680f, 120f),
                Theme.Mint,
                () => StartLevel(SaveService.CurrentLevel));

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("levels"),
                new Vector2(0f, 140f),
                new Vector2(680f, 105f),
                Theme.Blue,
                ShowLevels);

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("sandbox"),
                new Vector2(0f, 0f),
                new Vector2(680f, 105f),
                Theme.Purple,
                StartSandbox);

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("daily"),
                new Vector2(0f, -140f),
                new Vector2(680f, 105f),
                Theme.Gold,
                ClaimDaily);

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("shop"),
                new Vector2(-180f, -290f),
                new Vector2(320f, 90f),
                Theme.Coral,
                ShowShop);

            UiFactory.Button(
                canvas.transform,
                "AR / EN",
                new Vector2(180f, -290f),
                new Vector2(320f, 90f),
                Theme.Navy,
                () =>
                {
                    LocalizationService.Toggle();
                    ShowMainMenu();
                });

            UiFactory.Label(
                canvas.transform,
                $"★ {TotalStars()}     ● {SaveService.Coins}",
                new Vector2(0f, -470f),
                new Vector2(650f, 80f),
                38,
                Theme.Navy);
        }

        private void CreateLabBackdrop()
        {
            world = new GameObject("MenuLab");

            CreateCircle(
                "DecorBall",
                new Vector2(-6f, -3f),
                0.55f,
                Theme.Gold,
                false);

            CreateBox(
                "DecorPlatform",
                new Vector2(-5.4f, -3.7f),
                new Vector2(3f, 0.25f),
                Theme.Navy,
                false);

            CreateCircle(
                "DecorMagnet",
                new Vector2(6f, 2.5f),
                0.7f,
                Theme.Coral,
                false);
        }

        private int TotalStars()
        {
            int total = 0;

            for (int i = 1; i <= 50; i++)
            {
                total += SaveService.Stars(i);
            }

            return total;
        }

        private void ClaimDaily()
        {
            bool claimed = SaveService.ClaimDaily();

            string message = claimed
                ? LocalizationService.T("claimed")
                : "COME BACK TOMORROW";

            ShowMessage(message, ShowMainMenu);
        }

        private void ShowMessage(
            string message,
            System.Action back)
        {
            ResetScreen();

            UiFactory.Panel(
                canvas.transform,
                "Card",
                Vector2.zero,
                new Vector2(850f, 520f),
                Theme.Panel);

            UiFactory.Label(
                canvas.transform,
                message,
                new Vector2(0f, 80f),
                new Vector2(760f, 150f),
                52,
                Theme.Navy);

            UiFactory.Button(
                canvas.transform,
                "OK",
                new Vector2(0f, -110f),
                new Vector2(350f, 95f),
                Theme.Mint,
                () => back());
        }

        private void ShowShop()
        {
            ResetScreen();

            UiFactory.Label(
                canvas.transform,
                LocalizationService.T("shop"),
                new Vector2(0f, 700f),
                new Vector2(800f, 100f),
                65,
                Theme.Navy);

            string[] items =
            {
                "NEON INK • 500 COINS",
                "MIDNIGHT LAB • 10 GEMS",
                "NO ADS • PLAY BILLING",
                "HINT PACK • PLAY BILLING"
            };

            for (int i = 0; i < items.Length; i++)
            {
                int itemIndex = i;

                UiFactory.Button(
                    canvas.transform,
                    items[i],
                    new Vector2(0f, 440f - i * 150f),
                    new Vector2(800f, 110f),
                    i < 2 ? Theme.Purple : Theme.Blue,
                    () => Debug.Log("Shop item " + itemIndex));
            }

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("menu"),
                new Vector2(0f, -550f),
                new Vector2(500f, 100f),
                Theme.Navy,
                ShowMainMenu);
        }

        public void ShowLevels()
        {
            ResetScreen();

            UiFactory.Label(
                canvas.transform,
                LocalizationService.T("levels"),
                new Vector2(0f, 770f),
                new Vector2(800f, 100f),
                65,
                Theme.Navy);

            for (int i = 1; i <= 50; i++)
            {
                int levelId = i;
                int row = (i - 1) / 5;
                int column = (i - 1) % 5;

                float x = -360f + column * 180f;
                float y = 610f - row * 130f;

                bool isOpen = i <= SaveService.Unlocked;

                string label = isOpen
                    ? $"{i}\n{new string('★', SaveService.Stars(i))}"
                    : LocalizationService.T("locked");

                Color buttonColor = isOpen
                    ? ChapterColor((i - 1) / 10)
                    : new Color(0.55f, 0.58f, 0.6f);

                UiFactory.Button(
                    canvas.transform,
                    label,
                    new Vector2(x, y),
                    new Vector2(145f, 105f),
                    buttonColor,
                    () =>
                    {
                        if (isOpen)
                        {
                            StartLevel(levelId);
                        }
                    });
            }

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("menu"),
                new Vector2(0f, -760f),
                new Vector2(460f, 90f),
                Theme.Navy,
                ShowMainMenu);
        }

        private Color ChapterColor(int chapter)
        {
            Color[] colors =
            {
                Theme.Blue,
                Theme.Mint,
                Theme.Gold,
                Theme.Purple,
                Theme.Coral
            };

            int index = Mathf.Clamp(chapter, 0, colors.Length - 1);
            return colors[index];
        }

        public void StartSandbox()
        {
            sandbox = true;
            StartLevel(1);
        }

        public void StartLevel(int id)
        {
            ResetScreen();

            completed = false;
            elapsed = 0f;

            TextAsset levelAsset = Resources.Load<TextAsset>(
                $"Levels/level_{id:000}");

            if (levelAsset == null)
            {
                Debug.LogError(
                    $"Level file was not found: Levels/level_{id:000}");

                ShowMainMenu();
                return;
            }

            level = JsonUtility.FromJson<LevelData>(
                levelAsset.text);

            if (level == null)
            {
                Debug.LogError(
                    $"Level data could not be loaded for level {id}.");

                ShowMainMenu();
                return;
            }

            SaveService.CurrentLevel = id;

            float gravityDirection = level.reverseGravity
                ? level.gravity
                : -level.gravity;

            Physics2D.gravity = new Vector2(
                0f,
                gravityDirection);

            world = new GameObject("World");

            BuildWorld();
            BuildHud();
        }

        private void BuildWorld()
        {
            CreateBox(
                "Floor",
                new Vector2(0f, -4.55f),
                new Vector2(18f, 0.45f),
                Theme.Navy,
                false);

            CreateBox(
                "Shelf",
                new Vector2(0f, -1.2f),
                new Vector2(3.4f, 0.22f),
                new Color32(115, 140, 145, 255),
                false);

            GameObject ball = CreateCircle(
                "DynamicBall",
                new Vector2(level.ballX, level.ballY),
                0.42f,
                Theme.Gold,
                true);

            GameObject face = CreateCircle(
                "BallEye",
                ball.transform.position +
                new Vector3(0.12f, 0.08f, -0.1f),
                0.055f,
                Theme.Navy,
                false);

            CircleCollider2D faceCollider =
                face.GetComponent<CircleCollider2D>();

            if (faceCollider != null)
            {
                Destroy(faceCollider);
            }

            face.transform.SetParent(ball.transform);
            face.transform.localPosition =
                new Vector3(0.12f, 0.08f, -0.1f);

            GameObject goal = CreateBox(
                "Goal",
                new Vector2(level.goalX, level.goalY),
                new Vector2(1.7f, 0.24f),
                Theme.Mint,
                false);

            BoxCollider2D goalTrigger =
                goal.GetComponent<BoxCollider2D>();

            goalTrigger.isTrigger = true;

            GoalZone goalZone =
                goal.AddComponent<GoalZone>();

            goalZone.Entered = CompleteLevel;

            if (level.movingObstacle)
            {
                CreateBox(
                    "Obstacle",
                    new Vector2(1.1f, 0.7f),
                    new Vector2(0.35f, 2.2f),
                    ChapterColor(level.chapter - 1),
                    false);
            }

            if (level.hazard == "Spikes" ||
                level.hazard == "Lava")
            {
                CreateBox(
                    "Hazard",
                    new Vector2(-0.5f, -4.15f),
                    new Vector2(3.2f, 0.25f),
                    Theme.Coral,
                    false);
            }

            drawing =
                gameObject.AddComponent<DrawingController>();

            drawing.MaxInk = sandbox
                ? 999f
                : level.inkLimit;
        }

        private GameObject CreateBox(
            string objectName,
            Vector2 position,
            Vector2 size,
            Color color,
            bool dynamicBody)
        {
            GameObject gameObjectInstance =
                new GameObject(objectName);

            if (world != null)
            {
                gameObjectInstance.transform.SetParent(
                    world.transform);
            }

            gameObjectInstance.transform.position = position;
            gameObjectInstance.transform.localScale = size;

            SpriteRenderer spriteRenderer =
                gameObjectInstance.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = WhiteSprite();
            spriteRenderer.color = color;

            BoxCollider2D boxCollider =
                gameObjectInstance.AddComponent<BoxCollider2D>();

            if (dynamicBody)
            {
                Rigidbody2D rigidbody =
                    gameObjectInstance.AddComponent<Rigidbody2D>();

                rigidbody.simulated = false;
                rigidbody.collisionDetectionMode =
                    CollisionDetectionMode2D.Continuous;

                boxCollider.sharedMaterial = CreatePhysicsMaterial(
                    level != null ? level.friction : 0.3f,
                    level != null ? level.bounciness : 0.1f);
            }

            return gameObjectInstance;
        }

        private GameObject CreateCircle(
            string objectName,
            Vector2 position,
            float radius,
            Color color,
            bool dynamicBody)
        {
            GameObject gameObjectInstance =
                new GameObject(objectName);

            if (world != null)
            {
                gameObjectInstance.transform.SetParent(
                    world.transform);
            }

            gameObjectInstance.transform.position = position;
            gameObjectInstance.transform.localScale =
                Vector3.one * radius * 2f;

            SpriteRenderer spriteRenderer =
                gameObjectInstance.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = CircleSprite();
            spriteRenderer.color = color;

            CircleCollider2D circleCollider =
                gameObjectInstance.AddComponent<CircleCollider2D>();

            if (dynamicBody)
            {
                Rigidbody2D rigidbody =
                    gameObjectInstance.AddComponent<Rigidbody2D>();

                rigidbody.simulated = false;
                rigidbody.collisionDetectionMode =
                    CollisionDetectionMode2D.Continuous;

                circleCollider.sharedMaterial = CreatePhysicsMaterial(
                    level != null ? level.friction : 0.3f,
                    level != null ? level.bounciness : 0.1f);
            }

            return gameObjectInstance;
        }

        private PhysicsMaterial2D CreatePhysicsMaterial(
            float friction,
            float bounciness)
        {
            PhysicsMaterial2D physicsMaterial =
                new PhysicsMaterial2D("LevelMaterial");

            physicsMaterial.friction = friction;
            physicsMaterial.bounciness = bounciness;

            return physicsMaterial;
        }

        private Sprite WhiteSprite()
        {
            Texture2D texture = Texture2D.whiteTexture;

            return Sprite.Create(
                texture,
                new Rect(
                    0f,
                    0f,
                    texture.width,
                    texture.height),
                new Vector2(0.5f, 0.5f),
                1f);
        }

        private Sprite CircleSprite()
        {
            const int textureSize = 64;

            Texture2D texture = new Texture2D(
                textureSize,
                textureSize,
                TextureFormat.RGBA32,
                false);

            Color[] pixels =
                new Color[textureSize * textureSize];

            float center = textureSize / 2f;
            float radius = textureSize * 0.48f;
            float radiusSquared = radius * radius;

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float deltaX = x - center;
                    float deltaY = y - center;

                    bool insideCircle =
                        deltaX * deltaX + deltaY * deltaY <
                        radiusSquared;

                    pixels[y * textureSize + x] =
                        insideCircle
                            ? Color.white
                            : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(
                    0f,
                    0f,
                    textureSize,
                    textureSize),
                new Vector2(0.5f, 0.5f),
                textureSize);
        }

        private void BuildHud()
        {
            UiFactory.Panel(
                canvas.transform,
                "Top",
                new Vector2(0f, 810f),
                new Vector2(1040f, 190f),
                new Color32(255, 255, 255, 236));

            string objectiveText = sandbox
                ? LocalizationService.T("sandboxInfo")
                : $"{level.id} • {LocalizationService.T("objective")}";

            hudTitle = UiFactory.Label(
                canvas.transform,
                objectiveText,
                new Vector2(0f, 850f),
                new Vector2(780f, 75f),
                35,
                Theme.Navy);

            hudInk = UiFactory.Label(
                canvas.transform,
                string.Empty,
                new Vector2(-390f, 770f),
                new Vector2(230f, 65f),
                30,
                Theme.Purple);

            UiFactory.Button(
                canvas.transform,
                LocalizationService.T("hint"),
                new Vector2(390f, 770f),
                new Vector2(210f, 70f),
                Theme.Gold,
                Hint);

            GameObject toolbar = UiFactory.Panel(
                canvas.transform,
                "ToolBar",
                new Vector2(0f, -800f),
                new Vector2(1050f, 250f),
                new Color32(255, 255, 255, 243));

            string[] toolNames =
            {
                "FREE",
                "LINE",
                "CIRCLE",
                "BOX",
                "PIN"
            };

            for (int i = 0; i < toolNames.Length; i++)
            {
                int toolIndex = i;

                UiFactory.Button(
                    toolbar.transform,
                    toolNames[i],
                    new Vector2(
                        -390f + i * 195f,
                        55f),
                    new Vector2(175f, 70f),
                    toolIndex == 0
                        ? Theme.Blue
                        : Theme.Purple,
                    () =>
                    {
                        if (drawing != null)
                        {
                            drawing.Tool =
                                (DrawingTool)toolIndex;
                        }
                    });
            }

            UiFactory.Button(
                toolbar.transform,
                LocalizationService.T("undo"),
                new Vector2(-330f, -55f),
                new Vector2(190f, 70f),
                Theme.Navy,
                () =>
                {
                    if (drawing != null)
                    {
                        drawing.Undo();
                    }
                });

            UiFactory.Button(
                toolbar.transform,
                LocalizationService.T("clear"),
                new Vector2(-110f, -55f),
                new Vector2(190f, 70f),
                Theme.Coral,
                () =>
                {
                    if (drawing != null)
                    {
                        drawing.Clear();
                    }
                });

            UiFactory.Button(
                toolbar.transform,
                LocalizationService.T("play"),
                new Vector2(185f, -55f),
                new Vector2(330f, 80f),
                Theme.Mint,
                () =>
                {
                    if (drawing != null)
                    {
                        drawing.StartSimulation();
                    }
                });

            UiFactory.Button(
                toolbar.transform,
                "☰",
                new Vector2(430f, -55f),
                new Vector2(120f, 75f),
                Theme.Navy,
                Pause);
        }

        private void Hint()
        {
            ShowOverlay(
                "DRAW A RAMP OR BRIDGE\nارسم منحدراً أو جسراً",
                () => { });
        }

        private void Pause()
        {
            Time.timeScale = 0f;

            ShowOverlay(
                LocalizationService.T("pause"),
                () => Time.timeScale = 1f,
                true);
        }

        private void ShowOverlay(
            string title,
            System.Action close,
            bool isPauseOverlay = false)
        {
            GameObject panel = UiFactory.Panel(
                canvas.transform,
                "Overlay",
                Vector2.zero,
                new Vector2(850f, 650f),
                new Color32(255, 255, 255, 250));

            UiFactory.Label(
                panel.transform,
                title,
                new Vector2(0f, 170f),
                new Vector2(730f, 180f),
                48,
                Theme.Navy);

            UiFactory.Button(
                panel.transform,
                LocalizationService.T("resume"),
                new Vector2(0f, 20f),
                new Vector2(520f, 90f),
                Theme.Mint,
                () =>
                {
                    if (isPauseOverlay)
                    {
                        Time.timeScale = 1f;
                    }

                    Destroy(panel);
                    close?.Invoke();
                });

            UiFactory.Button(
                panel.transform,
                LocalizationService.T("restart"),
                new Vector2(0f, -100f),
                new Vector2(520f, 90f),
                Theme.Blue,
                () =>
                {
                    if (level != null)
                    {
                        StartLevel(level.id);
                    }
                });

            UiFactory.Button(
                panel.transform,
                LocalizationService.T("menu"),
                new Vector2(0f, -220f),
                new Vector2(520f, 90f),
                Theme.Navy,
                ShowMainMenu);
        }

        private void CompleteLevel()
        {
            if (completed || drawing == null || level == null)
            {
                return;
            }

            completed = true;

            int stars = sandbox
                ? 0
                : 1 +
                  (elapsed < level.timeLimit ? 1 : 0) +
                  (drawing.StrokeCount <= level.parStrokes ? 1 : 0);

            stars = Mathf.Clamp(stars, 0, 3);

            if (!sandbox)
            {
                SaveService.Complete(level.id, stars);
            }

            Effects.Burst(
                new Vector2(level.goalX, level.goalY),
                Theme.Gold,
                24);

            GameObject resultPanel = UiFactory.Panel(
                canvas.transform,
                "Result",
                Vector2.zero,
                new Vector2(880f, 720f),
                new Color32(255, 255, 255, 250));

            UiFactory.Label(
                resultPanel.transform,
                LocalizationService.T("solved"),
                new Vector2(0f, 230f),
                new Vector2(760f, 100f),
                52,
                Theme.Navy);

            UiFactory.Label(
                resultPanel.transform,
                new string('★', stars),
                new Vector2(0f, 110f),
                new Vector2(650f, 100f),
                76,
                Theme.Gold);

            UiFactory.Label(
                resultPanel.transform,
                $"{elapsed:0.0}s   •   {drawing.StrokeCount} strokes",
                new Vector2(0f, 20f),
                new Vector2(650f, 70f),
                30,
                Theme.Purple);

            UiFactory.Button(
                resultPanel.transform,
                LocalizationService.T("next"),
                new Vector2(0f, -100f),
                new Vector2(560f, 90f),
                Theme.Mint,
                () =>
                {
                    if (sandbox)
                    {
                        StartSandbox();
                    }
                    else
                    {
                        StartLevel(
                            Mathf.Min(50, level.id + 1));
                    }
                });

            UiFactory.Button(
                resultPanel.transform,
                LocalizationService.T("retry"),
                new Vector2(0f, -215f),
                new Vector2(560f, 85f),
                Theme.Blue,
                () => StartLevel(level.id));

            UiFactory.Button(
                resultPanel.transform,
                LocalizationService.T("menu"),
                new Vector2(0f, -320f),
                new Vector2(560f, 80f),
                Theme.Navy,
                ShowMainMenu);
        }

        private void ShowFailure()
        {
            if (completed)
            {
                return;
            }

            completed = true;

            ShowOverlay(
                "TRY A DIFFERENT SHAPE\nجرّب شكلاً مختلفاً",
                () => { });
        }

        private GameObject FindBall()
        {
            return GameObject.Find("DynamicBall");
        }
    }
}
