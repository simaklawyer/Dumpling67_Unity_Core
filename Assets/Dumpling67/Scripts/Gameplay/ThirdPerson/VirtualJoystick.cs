using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Экранный стик (левая половина / UI Image).
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 60f;

        public Vector2 Value { get; private set; }
        public bool IsActive { get; private set; }

        private Vector2 _origin;
        private Canvas _canvas;
        private Camera _uiCam;

        /// <summary>Для случаев, когда UI собирается в рантайме (см. SceneBootstrap) — Inspector недоступен.</summary>
        public void Configure(RectTransform bg, RectTransform handleRect, float range)
        {
            background = bg;
            handle = handleRect;
            handleRange = range;
        }

        private void Awake()
        {
            if (background == null) background = transform as RectTransform;
            _canvas = GetComponentInParent<Canvas>();
            _uiCam = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _canvas.worldCamera
                : null;
            _origin = background.anchoredPosition;
            if (handle != null) handle.anchoredPosition = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

        public void OnDrag(PointerEventData eventData)
        {
            IsActive = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, _uiCam, out Vector2 local);

            Vector2 clamped = Vector2.ClampMagnitude(local, handleRange);
            if (handle != null) handle.anchoredPosition = clamped;
            Value = clamped / handleRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsActive = false;
            Value = Vector2.zero;
            if (handle != null) handle.anchoredPosition = Vector2.zero;
        }
    }
}
