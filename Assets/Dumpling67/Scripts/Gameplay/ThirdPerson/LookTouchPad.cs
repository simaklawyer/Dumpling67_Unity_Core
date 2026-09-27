using UnityEngine;
using UnityEngine.EventSystems;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Правая зона экрана: drag = поворот камеры.
    /// </summary>
    public class LookTouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public Vector2 Delta { get; private set; }
        public bool IsDragging { get; private set; }

        private Vector2 _last;

        public void OnPointerDown(PointerEventData eventData)
        {
            IsDragging = true;
            _last = eventData.position;
            Delta = Vector2.zero;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Delta = eventData.position - _last;
            _last = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsDragging = false;
            Delta = Vector2.zero;
        }

        private void LateUpdate()
        {
            // Delta читается один кадр камерой, потом гасим
            if (!IsDragging) Delta = Vector2.zero;
            else Delta *= 0.85f; // лёгкое сглаживание шума
        }
    }
}
