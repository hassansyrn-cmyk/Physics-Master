using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// يمثل الجسم الفيزيائي الناتج عن رسمة اللاعب. يُبنى بالكامل بالكود:
    /// Rigidbody2D + PolygonCollider2D لسلوك الفيزياء، وLineRenderer للعرض المرئي
    /// (يُطابق سُمك الخط تماماً مع سُمك الكولايدر لتفادي أي فجوة بصرية).
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class DrawnPhysicsObject : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private PolygonCollider2D _collider;
        private LineRenderer _lineRenderer;

        public Rigidbody2D Body => _rb;

        public void Initialize(List<Vector2> centerline, List<Vector2> ribbonPolygon,
            DrawSettings settings, PhysicsMaterial2D material)
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<PolygonCollider2D>();

            _collider.points = ribbonPolygon.ToArray();
            _collider.sharedMaterial = material;

            _rb.bodyType = settings.startsAsStatic ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic;
            _rb.gravityScale = 1f;

            float area = RibbonMeshBuilder.PolygonArea(ribbonPolygon);
            _rb.mass = Mathf.Max(0.05f, area * settings.density);

            SetupVisual(centerline, settings.lineThickness);
        }

        private void SetupVisual(List<Vector2> centerline, float thickness)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = centerline.Count;
            _lineRenderer.widthMultiplier = thickness;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.numCapVertices = 8;
            _lineRenderer.numCornerVertices = 4;
            _lineRenderer.sortingLayerName = "Default";

            for (int i = 0; i < centerline.Count; i++)
                _lineRenderer.SetPosition(i, centerline[i]);
        }
    }
}
