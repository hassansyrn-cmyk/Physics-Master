using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// يحدد المنطقة المسموح بالرسم داخلها في المرحلة الحالية.
    /// يوضع على GameObject يحتوي BoxCollider2D يمثل حدود منطقة اللعب المرئية.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayAreaBounds : MonoBehaviour
    {
        private BoxCollider2D _bounds;

        private void Awake()
        {
            _bounds = GetComponent<BoxCollider2D>();
            _bounds.isTrigger = true;
        }

        public bool Contains(Vector2 worldPoint)
        {
            return _bounds.bounds.Contains(worldPoint);
        }

        public Vector2 Clamp(Vector2 worldPoint)
        {
            Bounds b = _bounds.bounds;
            return new Vector2(
                Mathf.Clamp(worldPoint.x, b.min.x, b.max.x),
                Mathf.Clamp(worldPoint.y, b.min.y, b.max.y)
            );
        }
    }
}
