using System.Collections.Generic;
using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// تبسيط المسارات المرسومة يدوياً باستخدام خوارزمية Douglas-Peucker.
    /// يحوّل مئات النقاط الخام الناتجة عن إصبع اللاعب إلى عدد قليل من النقاط
    /// الأساسية التي تحافظ على شكل الرسمة، وهو ضروري لأداء الفيزياء ونظافة الشكل.
    /// </summary>
    public static class PathSimplifier
    {
        public static List<Vector2> DouglasPeucker(List<Vector2> points, float epsilon)
        {
            if (points == null || points.Count < 3)
                return points != null ? new List<Vector2>(points) : new List<Vector2>();

            float maxDist = 0f;
            int index = 0;
            int end = points.Count - 1;

            for (int i = 1; i < end; i++)
            {
                float dist = PerpendicularDistance(points[i], points[0], points[end]);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    index = i;
                }
            }

            if (maxDist > epsilon)
            {
                List<Vector2> left = DouglasPeucker(points.GetRange(0, index + 1), epsilon);
                List<Vector2> right = DouglasPeucker(points.GetRange(index, points.Count - index), epsilon);

                left.RemoveAt(left.Count - 1); // تفادي تكرار نقطة الالتقاء
                left.AddRange(right);
                return left;
            }

            return new List<Vector2> { points[0], points[end] };
        }

        private static float PerpendicularDistance(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
        {
            float dx = lineEnd.x - lineStart.x;
            float dy = lineEnd.y - lineStart.y;

            if (dx == 0f && dy == 0f)
                return Vector2.Distance(point, lineStart);

            float t = ((point.x - lineStart.x) * dx + (point.y - lineStart.y) * dy) / (dx * dx + dy * dy);
            t = Mathf.Clamp01(t);

            Vector2 projection = new Vector2(lineStart.x + t * dx, lineStart.y + t * dy);
            return Vector2.Distance(point, projection);
        }
    }
}
