using UnityEngine; using UnityEngine.UI; using UnityEngine.Events; using PhysicsMaster.Core;
namespace PhysicsMaster.UI {
    public static class UiFactory {
        static Font font;
        public static Font Font => font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        public static Canvas Canvas(){var g=new GameObject("Canvas");var c=g.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;g.AddComponent<GraphicRaycaster>();var s=g.AddComponent<CanvasScaler>();s.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;s.referenceResolution=new Vector2(1080,1920);s.matchWidthOrHeight=.5f;return c;}
        public static GameObject Panel(Transform p,string n,Vector2 pos,Vector2 size,Color color){var g=new GameObject(n);g.transform.SetParent(p,false);var i=g.AddComponent<Image>();i.color=color;var r=i.rectTransform;r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;return g;}
        public static Text Label(Transform p,string text,Vector2 pos,Vector2 size,int fontSize,Color color,TextAnchor align=TextAnchor.MiddleCenter){var g=new GameObject("Label");g.transform.SetParent(p,false);var t=g.AddComponent<Text>();t.font=Font;t.text=text;t.fontSize=fontSize;t.color=color;t.alignment=align;t.resizeTextForBestFit=true;t.resizeTextMinSize=14;t.resizeTextMaxSize=fontSize;var r=t.rectTransform;r.anchorMin=r.anchorMax=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;return t;}
        public static Button Button(Transform p,string text,Vector2 pos,Vector2 size,Color color,UnityAction click){var g=Panel(p,"Button_"+text,pos,size,color);var b=g.AddComponent<Button>();b.targetGraphic=g.GetComponent<Image>();b.onClick.AddListener(click);var t=Label(g.transform,text,Vector2.zero,size-(Vector2.one*18),32,Color.white);t.fontStyle=FontStyle.Bold;return b;}
        public static void Clear(Transform root){for(int i=root.childCount-1;i>=0;i--)Object.Destroy(root.GetChild(i).gameObject);}
    }
}
