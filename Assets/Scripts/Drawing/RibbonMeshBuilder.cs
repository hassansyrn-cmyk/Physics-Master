using System.Collections.Generic;
using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// يحوّل خطاً مركزياً (centerline) — أي مساراً من نقاط بلا سُمك —
    /// إلى شكل "شريط" مغلق (ribbon polygon) بسُمك معيّن، جاهز للاستخدام
    /// مباشرة كنقاط PolygonCollider2D. هذا هو جوهر تحويل "رسمة" إلى "جسم فيزيائي".
    /// </summary>
    public static class RibbonMeshBuilder
    {
        public static List<Vector2> BuildRibbon(List<Vector2> centerline, float thickness)
        {
            if (centerline == null || centerline.Count < 2) return null;

            float half = thickness * 0.5f;
            var left = new List<Vector2>();
            var right = new List<Vector2>();

            for (int i = 0; i < centerline.Count; i++)
            {
                Vector2 dir;
                if (i == 0)
                    dir = SafeDir(centerline[1] - centerline[0]);
                else if (i == centerline.Count - 1)
                    dir = SafeDir(centerline[i] - centerline[i - 1]);
                else
                    dir = SafeDir(centerline[i + 1] - centerline[i - 1]);

                Vector2 normal = new Vector2(-dir.y, dir.x);

                left.Add(centerline[i] + normal * half);
                right.Add(centerline[i] - normal * half);
            }

            // أغطية نصف دائرية مبسطة عند الطرفين لتفادي حواف حادة قد تُصعّب على الفيزياء
            List<Vector2> endCap = BuildCap(centerline[centerline.Count - 1],
                right[right.Count - 1], left[left.Count - 1]);
            List<Vector2> startCap = BuildCap(centerline[0], left[0], right[0]);

            var polygon = new List<Vector2>();
            polygon.AddRange(left);
            polygon.AddRange(endCap);
            right.Reverse();
            polygon.AddRange(right);
            polygon.AddRange(startCap);

            return polygon;
        }

        private static Vector2 SafeDir(Vector2 v)
        {
            return v.sqrMagnitude > 0.0001f ? v.normalized : Vector2.right;
        }

        private static List<Vector2> BuildCap(Vector2 center, Vector2 from, Vector2 to, int segments = 6)
        {
            var points = new List<Vector2>();
            float radius = Vector2.Distance(center, from);
            if (radius < 0.0001f) return points;

            float angleFrom = Mathf.Atan2(from.y - center.y, from.x - center.x) * Mathf.Rad2Deg;
            float angleTo = Mathf.Atan2(to.y - center.y, to.x - center.x) * Mathf.Rad2Deg;
            float delta = Mathf.DeltaAngle(angleFrom, angleTo);

            for (int i = 1; i < segments; i++)
            {
                float t = (float)i / segments;
                float angle = (angleFrom + delta * t) * Mathf.Deg2Rad;
                points.Add(center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);
            }

            return points;
        }

        /// <summary>مساحة الشكل المضلع (صيغة Shoelace) — تُستخدم لحساب الكتلة والتحقق من صحة الرسمة.</summary>
        public static float PolygonArea(List<Vector2> polygon)
        {
            float area = 0f;
            int n = polygon.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 a = polygon[i];
                Vector2 b = polygon[(i + 1) % n];
                area += a.x * b.y - b.x * a.y;
            }
            return Mathf.Abs(area) * 0.5f;
        }

        /// <summary>الطول الكلي لمسار من النقاط — يُستخدم للتحقق من حد أقصى طول الرسمة.</summary>
        public static float PathLength(List<Vector2> path)
        {
            float length = 0f;
            for (int i = 1; i < path.Count; i++)
                length += Vector2.Distance(path[i - 1], path[i]);
            return length;
        }
    }
}
