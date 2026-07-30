using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// المنسّق المركزي لنظام الرسم: يستقبل الرسمة الخام من DrawInputController،
    /// يمرّرها عبر خط الأنابيب (تحقق من الطول → تبسيط → بناء الشريط → تحقق من المساحة)
    /// ثم يُنشئ جسماً فيزيائياً فعلياً. يطلق أحداثاً ليستفيد منها بقية الأنظمة
    /// (عدّاد الأدوات المتبقية، تتبّع هدف المرحلة، تشغيل صوت الرسم...) دون أي اعتماد مباشر بينها.
    /// </summary>
    public class DrawingManager : MonoBehaviour
    {
        [SerializeField] private DrawInputController inputController;
        [SerializeField] private DrawSettings settings;
        [SerializeField] private PhysicsMaterial2D physicsMaterial;
        [SerializeField] private Transform shapesParent;

        /// <summary>يُطلق عند إنشاء شكل فيزيائي بنجاح.</summary>
        public event Action<DrawnPhysicsObject> OnShapeCreated;

        /// <summary>يُطلق عند رفض رسمة اللاعب مع سبب الرفض (لعرضه كرسالة قصيرة للاعب).</summary>
        public event Action<string> OnDrawRejected;

        private bool _subscribed;

        /// <summary>
        /// يسمح بربط المراجع برمجياً (مثلاً من SceneBootstrapper) كبديل آمن
        /// عن الربط اليدوي من الـ Inspector. يقوم بالاشتراك في الأحداث فوراً.
        /// </summary>
        public void Configure(DrawInputController controller, DrawSettings drawSettings, Transform parent,
            PhysicsMaterial2D material = null)
        {
            Unsubscribe();

            inputController = controller;
            settings = drawSettings;
            shapesParent = parent;
            physicsMaterial = material;

            Subscribe();
        }

        private void OnEnable() => Subscribe();

        private void OnDisable() => Unsubscribe();

        private void Subscribe()
        {
            if (_subscribed || inputController == null) return;
            inputController.OnStrokeCompleted += HandleStrokeCompleted;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed || inputController == null) return;
            inputController.OnStrokeCompleted -= HandleStrokeCompleted;
            _subscribed = false;
        }

        private void HandleStrokeCompleted(List<Vector2> rawPoints)
        {
            float rawLength = RibbonMeshBuilder.PathLength(rawPoints);
            if (rawLength > settings.maxDrawLength)
            {
                OnDrawRejected?.Invoke("الرسمة أطول من المسموح");
                return;
            }

            List<Vector2> simplified = PathSimplifier.DouglasPeucker(rawPoints, settings.simplificationTolerance);
            if (simplified.Count < 2)
            {
                OnDrawRejected?.Invoke("رسمة غير صالحة");
                return;
            }

            List<Vector2> ribbon = RibbonMeshBuilder.BuildRibbon(simplified, settings.lineThickness);
            if (ribbon == null || ribbon.Count < 3)
            {
                OnDrawRejected?.Invoke("تعذر تحويل الرسمة إلى شكل فيزيائي");
                return;
            }

            float area = RibbonMeshBuilder.PolygonArea(ribbon);
            if (area < settings.minValidArea)
            {
                OnDrawRejected?.Invoke("الشكل صغير جداً");
                return;
            }

            CreatePhysicsObject(simplified, ribbon);
        }

        private void CreatePhysicsObject(List<Vector2> centerline, List<Vector2> ribbon)
        {
            var go = new GameObject("DrawnShape");
            if (shapesParent != null) go.transform.SetParent(shapesParent);

            var drawnObject = go.AddComponent<DrawnPhysicsObject>();
            drawnObject.Initialize(centerline, ribbon, settings, physicsMaterial);

            OnShapeCreated?.Invoke(drawnObject);
        }
    }
}
