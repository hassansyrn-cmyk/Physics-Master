using UnityEngine;
namespace PhysicsMaster.Gameplay {
    public static class Effects {
        public static void Burst(Vector2 at, Color color, int count=18) {
            var root=new GameObject("Burst"); root.transform.position=at;
            for(int i=0;i<count;i++){var g=GameObject.CreatePrimitive(PrimitiveType.Quad);g.name="Particle";g.transform.SetParent(root.transform);g.transform.localScale=Vector3.one*.08f;var r=g.GetComponent<Renderer>();r.material=new Material(Shader.Find("Sprites/Default"));r.material.color=color;Object.Destroy(g.GetComponent<Collider>());var b=g.AddComponent<Rigidbody2D>();float a=i*Mathf.PI*2/count;b.linearVelocity=new Vector2(Mathf.Cos(a),Mathf.Sin(a))*Random.Range(1.5f,4f);b.gravityScale=.5f;Object.Destroy(g,1.2f);} Object.Destroy(root,1.3f);
        }
    }
}
