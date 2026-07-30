using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsMaster.Drawing
{
    /// <summary>
    /// يلتقط إدخال اللمس (على الجهاز) أو الماوس (في المحرر) أثناء عملية الرسم،
    /// ويحوّله إلى نقاط World Space. لا يقوم بأي معالجة فيزيائية أو تبسيط —
    /// مسؤوليته الوحيدة هي جمع البيانات الخام وبثّها عبر الأحداث (فصل المسؤوليات).
    /// </summary>
    public class DrawInputController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;
        [SerializeField] private PlayAreaBounds playArea;
        [SerializeField] private DrawSettings settings;

        public event Action OnStrokeStarted;
        public event Action<Vector2> OnPointAdded;
        public event Action<List<Vector2>> OnStrokeCompleted;
        public event Action OnStrokeCancelled;

        private readonly List<Vector2> _rawPoints = new List<Vector2>();
        private bool _isDrawing;
        private bool _drawingEnabled = true;

        /// <summary>
        /// يسمح بربط المراجع برمجياً (مثلاً من SceneBootstrapper) كبديل آمن
        /// عن الربط اليدوي من الـ Inspector، دون الحاجة لأي Reflection.
        /// </summary>
        public void Configure(Camera camera, PlayAreaBounds area, DrawSettings drawSettings)
        {
            targetCamera = camera;
            playArea = area;
            settings = drawSettings;
        }

        public void SetDrawingEnabled(bool enabled)
        {
            _drawingEnabled = enabled;
            if (!enabled && _isDrawing) CancelStroke();
        }

        private void Update()
        {
            if (!_drawingEnabled) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0)) BeginStroke(Input.mousePosition);
            else if (Input.GetMouseButton(0) && _isDrawing) UpdateStroke(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0) && _isDrawing) EndStroke();
        }

        private void HandleTouch()
        {
            if (Input.touchCount == 0) return;
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    BeginStroke(touch.position);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (_isDrawing) UpdateStroke(touch.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_isDrawing) EndStroke();
                    break;
            }
        }

        private void BeginStroke(Vector2 screenPos)
        {
            Vector2 worldPos = targetCamera.ScreenToWorldPoint(screenPos);

            if (playArea != null && !playArea.Contains(worldPos))
                return; // لا يبدأ الرسم إن كانت أول لمسة خارج المنطقة المسموحة

            _rawPoints.Clear();
            _rawPoints.Add(worldPos);
            _isDrawing = true;
            OnStrokeStarted?.Invoke();
            OnPointAdded?.Invoke(worldPos);
        }

        private void UpdateStroke(Vector2 screenPos)
        {
            Vector2 worldPos = targetCamera.ScreenToWorldPoint(screenPos);

            if (playArea != null && !playArea.Contains(worldPos))
            {
                // نوقف الرسم عند خروج الإصبع من المنطقة بدل السماح برسم خارجها
                EndStroke();
                return;
            }

            Vector2 lastPoint = _rawPoints[_rawPoints.Count - 1];
            if (Vector2.Distance(lastPoint, worldPos) < settings.minPointDistance)
                return;

            if (_rawPoints.Count >= settings.maxRawPoints)
            {
                EndStroke();
                return;
            }

            _rawPoints.Add(worldPos);
            OnPointAdded?.Invoke(worldPos);
        }

        private void EndStroke()
        {
            _isDrawing = false;

            if (_rawPoints.Count < 2)
            {
                OnStrokeCancelled?.Invoke();
                return;
            }

            OnStrokeCompleted?.Invoke(new List<Vector2>(_rawPoints));
            _rawPoints.Clear();
        }

        private void CancelStroke()
        {
            _isDrawing = false;
            _rawPoints.Clear();
            OnStrokeCancelled?.Invoke();
        }
    }
}
