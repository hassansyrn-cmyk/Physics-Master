using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using PhysicsMaster.Core;

namespace PhysicsMaster.UI {
    public static class UiFactory {
        static Font font;
        public static Font Font
        {
            get
            {
                if (font == null)
                {
                    font = Resources.Load<Font>("Amiri-Regular");
                }
                if (font == null)
                {
                    font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }
                return font;
            }
        }

        public static Canvas Canvas()
        {
            var existing = Object.FindFirstObjectByType<Canvas>();
            if (existing != null) return existing;

            var g = new GameObject("Canvas");
            var c = g.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            g.AddComponent<GraphicRaycaster>();
            var s = g.AddComponent<CanvasScaler>();
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = new Vector2(1080, 1920);
            s.matchWidthOrHeight = .5f;
            return c;
        }

        public static RectTransform GetSafeAreaContainer(Transform canvas)
        {
            Transform container = canvas.Find("SafeAreaContainer");
            if (container != null) return container.GetComponent<RectTransform>();

            GameObject go = new GameObject("SafeAreaContainer");
            go.transform.SetParent(canvas, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;

            ApplySafeArea(rt);
            return rt;
        }

        public static void ApplySafeArea(RectTransform rt)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            float sw = Screen.width > 0 ? Screen.width : 1080f;
            float sh = Screen.height > 0 ? Screen.height : 1920f;

            anchorMin.x /= sw;
            anchorMin.y /= sh;
            anchorMax.x /= sw;
            anchorMax.y /= sh;

            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static GameObject Panel(Transform p, string n, Vector2 pos, Vector2 size, Color color)
        {
            var g = new GameObject(n);
            g.transform.SetParent(p, false);
            var i = g.AddComponent<Image>();
            i.color = color;
            var r = i.rectTransform;
            r.anchorMin = r.anchorMax = new Vector2(.5f, .5f);
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            return g;
        }

        public static Text Label(Transform p, string text, Vector2 pos, Vector2 size, int fontSize, Color color, TextAnchor align = TextAnchor.MiddleCenter)
        {
            var g = new GameObject("Label");
            g.transform.SetParent(p, false);
            var t = g.AddComponent<Text>();
            t.font = Font;
            t.text = text;
            t.fontSize = fontSize;
            t.color = color;
            t.alignment = align;
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = 14;
            t.resizeTextMaxSize = fontSize;
            var r = t.rectTransform;
            r.anchorMin = r.anchorMax = new Vector2(.5f, .5f);
            r.anchoredPosition = pos;
            r.sizeDelta = size;
            return t;
        }

        public static Button Button(Transform p, string text, Vector2 pos, Vector2 size, Color color, UnityAction click)
        {
            var g = Panel(p, "Button_" + text, pos, size, color);
            var b = g.AddComponent<Button>();
            b.targetGraphic = g.GetComponent<Image>();
            b.onClick.AddListener(click);
            var t = Label(g.transform, text, Vector2.zero, size - (Vector2.one * 18), 32, Color.white);
            t.fontStyle = FontStyle.Bold;
            return b;
        }

        public static void SafeDestroy(UnityEngine.Object target, float delay = 0f)
        {
            if (target == null) return;
            if (Application.isPlaying)
            {
                Object.Destroy(target, delay);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }

        public static void Clear(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                SafeDestroy(root.GetChild(i).gameObject);
            }
        }
    }
}
