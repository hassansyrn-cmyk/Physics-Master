using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// إعدادات نظام الرسم الحر. يُنشأ كأصل (Asset) واحد ويُمرَّر لكل الأنظمة
    /// حتى يسهل ضبط اللعبة بالكامل من مكان واحد دون تعديل الكود.
    /// </summary>
    [CreateAssetMenu(fileName = "DrawSettings", menuName = "PhysicsMaster/Draw Settings")]
    public class DrawSettings : ScriptableObject
    {
        [Header("الرسم")]
        [Tooltip("سمك الخط المرسوم بوحدات World Space")]
        public float lineThickness = 0.15f;

        [Tooltip("أقصى طول مسموح للرسمة الواحدة (قبل التبسيط)")]
        public float maxDrawLength = 15f;

        [Tooltip("أقصى عدد نقاط خام تُلتقط أثناء السحب الواحد")]
        public int maxRawPoints = 400;

        [Tooltip("قيمة التبسيط (خوارزمية Douglas-Peucker) - كل ما زادت قلّت التفاصيل والتعرجات")]
        [Range(0.005f, 0.2f)]
        public float simplificationTolerance = 0.03f;

        [Tooltip("أقل مسافة بين نقطتين متتاليتين قبل تسجيل نقطة جديدة أثناء السحب")]
        public float minPointDistance = 0.05f;

        [Header("الفيزياء")]
        public float density = 1f;
        public float friction = 0.4f;
        public float bounciness = 0.1f;
        public bool startsAsStatic = false;

        [Header("التحقق من صحة الرسمة")]
        [Tooltip("أقل مساحة مقبولة للشكل الناتج (لمنع الأشكال شبه المنعدمة أو النقاط الوهمية)")]
        public float minValidArea = 0.02f;
    }
}
