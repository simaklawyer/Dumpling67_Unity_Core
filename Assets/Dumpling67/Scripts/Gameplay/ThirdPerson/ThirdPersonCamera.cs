using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Орбитальная камера как в Roblox: мышь / правый стик / touch look-pad.
    /// </summary>
    public class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Follow")]
        public Transform target;
        public Vector3 targetOffset = new Vector3(0f, 1.4f, 0f);
        public float distance = 6.5f;
        public float minDistance = 2.5f;
        public float maxDistance = 12f;
        public float followSmooth = 12f;

        [Header("Orbit")]
        public float mouseSensitivity = 2.4f;
        public float touchSensitivity = 0.15f;
        public float minPitch = -25f;
        public float maxPitch = 55f;
        public bool invertY;

        [Header("Collision")]
        public float collisionRadius = 0.25f;
        public LayerMask collisionMask = ~0;

        [Header("Input")]
        public LookTouchPad lookPad;
        public bool requireRightMouse = false;

        private float _yaw;
        private float _pitch = 18f;

        private void Start()
        {
            if (target != null)
            {
                Vector3 e = transform.eulerAngles;
                _yaw = e.y;
                _pitch = e.x;
            }
            Cursor.lockState = CursorLockMode.None;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            ReadLook(out float dx, out float dy);
            _yaw += dx;
            _pitch += invertY ? dy : -dy;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
                distance = Mathf.Clamp(distance - scroll * 0.8f, minDistance, maxDistance);

            Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 pivot = target.position + targetOffset;
            Vector3 desired = pivot - rot * Vector3.forward * distance;

            if (Physics.SphereCast(pivot, collisionRadius, (desired - pivot).normalized,
                    out RaycastHit hit, distance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                desired = pivot + (desired - pivot).normalized * Mathf.Max(0.4f, hit.distance - 0.15f);
            }

            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-followSmooth * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1f - Mathf.Exp(-followSmooth * Time.deltaTime));
        }

        private void ReadLook(out float dx, out float dy)
        {
            dx = 0f; dy = 0f;

            if (lookPad != null && lookPad.IsDragging)
            {
                dx = lookPad.Delta.x * touchSensitivity;
                dy = lookPad.Delta.y * touchSensitivity;
                return;
            }

            bool allowMouse = !requireRightMouse || Input.GetMouseButton(1) || Input.GetMouseButton(0);
            // На десктопе: зажать ПКМ или просто двигать, если курсор захвачен
            if (Cursor.lockState == CursorLockMode.Locked || Input.GetMouseButton(1))
            {
                dx = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
                dy = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            }
            else if (allowMouse && Input.GetMouseButton(1))
            {
                dx = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
                dy = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            }

            // Q/E лёгкий поворот без мыши
            if (Input.GetKey(KeyCode.Q)) dx -= 60f * Time.deltaTime;
            if (Input.GetKey(KeyCode.E)) dx += 60f * Time.deltaTime;
        }

        public void SetTarget(Transform t) => target = t;
    }
}
