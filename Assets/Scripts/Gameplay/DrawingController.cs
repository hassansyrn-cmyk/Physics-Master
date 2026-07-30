using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace PhysicsMaster.Gameplay {
    public sealed class DrawingController : MonoBehaviour {
        public float MaxInk { get; set; } = 16f;
        public float RemainingInk => Mathf.Max(0, MaxInk - usedInk);
        public int StrokeCount => strokes.Count;
        public bool SimulationRunning { get; private set; }
        public DrawingTool Tool { get; set; } = DrawingTool.Freehand;
        readonly List<GameObject> strokes = new(); readonly List<Vector2> points = new();
        Camera worldCamera; LineRenderer activeLine; float usedInk, activeInk; Vector2 start;
        static readonly Color InkColor = new Color32(38, 66, 78, 255);
        void Awake() { worldCamera = Camera.main; }
        void Update() {
            if (SimulationRunning || worldCamera == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
            if (Input.GetMouseButtonDown(0)) Begin(WorldPoint());
            if (Input.GetMouseButton(0)) Drag(WorldPoint());
            if (Input.GetMouseButtonUp(0)) End(WorldPoint());
        }
        Vector2 WorldPoint() { Vector2 p = worldCamera.ScreenToWorldPoint(Input.mousePosition); return new Vector2(Mathf.Clamp(p.x,-8.6f,8.6f),Mathf.Clamp(p.y,-4.25f,3.6f)); }
        void Begin(Vector2 p) { if (RemainingInk <= .05f) return; start=p; points.Clear(); points.Add(p); activeInk=0; activeLine=CreateLine("DrawingPreview"); activeLine.positionCount=1; activeLine.SetPosition(0,p); }
        void Drag(Vector2 p) {
            if (activeLine==null) return;
            if (Tool==DrawingTool.Line) { SetPreview(new[]{start,p}); return; }
            if (Tool==DrawingTool.Circle) { SetPreview(CirclePoints(start,Mathf.Min(Vector2.Distance(start,p),RemainingInk/(2*Mathf.PI)))); return; }
            if (Tool==DrawingTool.Box) { SetPreview(BoxPoints(start,p)); return; }
            if (Vector2.Distance(points[^1],p)<.10f) return;
            float segment=Vector2.Distance(points[^1],p); if (usedInk+activeInk+segment>MaxInk) return;
            activeInk+=segment; points.Add(p); activeLine.positionCount=points.Count; activeLine.SetPosition(points.Count-1,p);
        }
        void SetPreview(IReadOnlyList<Vector2> pts) { points.Clear(); for(int i=0;i<pts.Count;i++) points.Add(pts[i]); activeLine.positionCount=points.Count; for(int i=0;i<points.Count;i++) activeLine.SetPosition(i,points[i]); activeInk=Length(points); }
        void End(Vector2 p) {
            if(activeLine==null) return; if(Tool==DrawingTool.Line) SetPreview(new[]{start,p});
            if(points.Count<2 || activeInk>.01f && usedInk+activeInk>MaxInk+.01f) { Destroy(activeLine.gameObject); activeLine=null; return; }
            var clean=Simplify(points,.07f); if(clean.Count<2){Destroy(activeLine.gameObject);activeLine=null;return;}
            activeLine.gameObject.name="PlayerStroke"; var edge=activeLine.gameObject.AddComponent<EdgeCollider2D>(); edge.points=clean.ToArray(); edge.edgeRadius=.08f;
            var body=activeLine.gameObject.AddComponent<Rigidbody2D>(); body.bodyType=RigidbodyType2D.Dynamic; body.simulated=false; body.mass=Mathf.Clamp(activeInk*.18f,.2f,4f);
            if(Tool==DrawingTool.Pin){body.bodyType=RigidbodyType2D.Static; activeLine.startColor=activeLine.endColor=PhysicsMaster.Core.Theme.Purple;}
            strokes.Add(activeLine.gameObject); usedInk+=activeInk; activeLine=null; points.Clear();
        }
        LineRenderer CreateLine(string n){var g=new GameObject(n);var l=g.AddComponent<LineRenderer>();l.material=new Material(Shader.Find("Sprites/Default"));l.startColor=l.endColor=InkColor;l.startWidth=l.endWidth=.18f;l.numCapVertices=6;l.numCornerVertices=5;l.useWorldSpace=true;return l;}
        static List<Vector2> Simplify(List<Vector2> src,float min){var o=new List<Vector2>{src[0]};for(int i=1;i<src.Count-1;i++)if(Vector2.Distance(o[^1],src[i])>=min)o.Add(src[i]);o.Add(src[^1]);return o;}
        static float Length(IReadOnlyList<Vector2> p){float n=0;for(int i=1;i<p.Count;i++)n+=Vector2.Distance(p[i-1],p[i]);return n;}
        static Vector2[] CirclePoints(Vector2 c,float r){int n=28;var p=new Vector2[n+1];for(int i=0;i<=n;i++){float a=i*Mathf.PI*2/n;p[i]=c+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r;}return p;}
        static Vector2[] BoxPoints(Vector2 a,Vector2 b)=>new[]{a,new Vector2(b.x,a.y),b,new Vector2(a.x,b.y),a};
        public void Undo(){if(SimulationRunning||strokes.Count==0)return;var g=strokes[^1];var line=g.GetComponent<LineRenderer>();usedInk=Mathf.Max(0,usedInk+-.01f-LengthFromLine(line));strokes.RemoveAt(strokes.Count-1);Destroy(g);}
        float LengthFromLine(LineRenderer l){float n=0;for(int i=1;i<l.positionCount;i++)n+=Vector3.Distance(l.GetPosition(i-1),l.GetPosition(i));return n;}
        public void Clear(){if(SimulationRunning)return;foreach(var g in strokes)if(g)Destroy(g);strokes.Clear();usedInk=0;}
        public void StartSimulation(){SimulationRunning=true;foreach(var g in strokes){var b=g?g.GetComponent<Rigidbody2D>():null;if(b&&b.bodyType==RigidbodyType2D.Dynamic)b.simulated=true;}foreach(var b in FindObjectsByType<Rigidbody2D>(FindObjectsSortMode.None))if(b.gameObject.name.StartsWith("Dynamic"))b.simulated=true;}
        public void StopSimulation(){SimulationRunning=false;Time.timeScale=1;}
    }
}
