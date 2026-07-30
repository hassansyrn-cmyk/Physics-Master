using UnityEngine;
using PhysicsMaster.Drawing;

namespace PhysicsMaster.Bootstrap
{
    /// <summary>
    /// نقطة الدخول الوحيدة للمشهد. يبني كل ما يلزم لتشغيل تجربة قابلة للّعب فوراً:
    /// كاميرا، أرضية فيزيائية، منطقة رسم محدودة، ونظام الرسم الحر بالكامل.
    /// هذا يعني أن مشهد Unity نفسه يبقى فارغاً تقريباً (كائن واحد فقط يحمل هذا
    /// السكربت) وكل التركيب يحدث بالكود — ما يقلل جداً من احتمال وجود مراجع
    /// مفقودة أو غير متطابقة داخل ملف المشهد (.unity) بحد ذاته.
    /// </summary>
    public class SceneBootstrapper : MonoBehaviour
    {
        [Header("حجم منطقة اللعب (وحدات World Space)")]
        [SerializeField] private Vector2 playAreaSize = new Vector2(9f, 16f);

        [Header("إعدادات الرسم (اختياري - إن ترك فارغاً سيُنشأ افتراضي وقت التشغيل)")]
        [SerializeField] private DrawSettings drawSettingsOverride;

        private void Awake()
        {
            Camera cam = BuildCamera();
            PlayAreaBounds playArea = BuildPlayArea();
            BuildGround(playArea);

            DrawSettings settings = drawSettingsOverride != null
                ? drawSettingsOverride
                : CreateDefaultDrawSettings();

            BuildDrawingSystem(cam, playArea, settings);
        }

        private Camera BuildCamera()
        {
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = playAreaSize.y * 0.5f + 1f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            cam.backgroundColor = new Color(0.09f, 0.1f, 0.14f);
            camGO.AddComponent<AudioListener>();
            return cam;
        }

        private PlayAreaBounds BuildPlayArea()
        {
            var areaGO = new GameObject("PlayArea");
            var box = areaGO.AddComponent<BoxCollider2D>();
            box.size = playAreaSize;
            box.isTrigger = true;
            return areaGO.AddComponent<PlayAreaBounds>();
        }

        private void BuildGround(PlayAreaBounds playArea)
        {
            var groundGO = new GameObject("Ground");
            groundGO.transform.position = new Vector3(0f, -playAreaSize.y * 0.5f, 0f);

            var box = groundGO.AddComponent<BoxCollider2D>();
            box.size = new Vector2(playAreaSize.x, 0.5f);

            var rb = groundGO.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            var lr = groundGO.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.widthMultiplier = 0.5f;
            lr.useWorldSpace = false;
            lr.SetPosition(0, new Vector3(-playAreaSize.x * 0.5f, 0f, 0f));
            lr.SetPosition(1, new Vector3(playAreaSize.x * 0.5f, 0f, 0f));
        }

        private void BuildDrawingSystem(Camera cam, PlayAreaBounds playArea, DrawSettings settings)
        {
            var shapesParent = new GameObject("DrawnShapes").transform;

            var systemGO = new GameObject("DrawingSystem");
            var input = systemGO.AddComponent<DrawInputController>();
            var manager = systemGO.AddComponent<DrawingManager>();

            // ربط آمن بالكامل (بدون Reflection) عبر دوال Configure العامة.
            input.Configure(cam, playArea, settings);
            manager.Configure(input, settings, shapesParent);

            manager.OnDrawRejected += reason => Debug.Log($"[Drawing] رُفضت الرسمة: {reason}");
            manager.OnShapeCreated += shape => Debug.Log($"[Drawing] تم إنشاء شكل جديد: {shape.name}");
        }

        private static DrawSettings CreateDefaultDrawSettings()
        {
            return ScriptableObject.CreateInstance<DrawSettings>(); // يستخدم القيم الافتراضية المعرّفة داخل DrawSettings.cs
        }
    }
}
