// ⚠ FROZEN (2026-09-26): вторичная 2D-ветка (раннер/softbody).
// См. HANDOFF.md / _Frozen2D/README.md.
using UnityEngine;
using Dumpling67.Integrations;

namespace Dumpling67.Gameplay
{
    /// <summary>
    /// Простая 2D soft-body деформация на основе пружин.
    /// </summary>
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SoftBodyPelmen2D : MonoBehaviour
    {
        [Header("Настройки пружинной физики")]
        [Range(10f, 400f)] public float stiffness = 140f;
        [Range(1f, 25f)] public float damping = 7f;
        [Range(8, 32)] public int pointCount = 16;
        public float radius = 1.8f;

        [Header("Взаимодействие")]
        public float dragRadius = 2.2f;
        public float squishBurstForce = 8f;

        private Vector3[] _baseVertices;
        private Vector3[] _currentVertices;
        private Vector3[] _velocities;
        private Mesh _mesh;
        private Camera _cam;
        private bool _isDragging;
        private int _selectedIndex = -1;

        private void Awake()
        {
            _cam = Camera.main;
            InitializeMesh();
        }

        private void InitializeMesh()
        {
            _mesh = new Mesh { name = "SoftBodyPelmen" };
            _baseVertices = new Vector3[pointCount + 1];
            _currentVertices = new Vector3[pointCount + 1];
            _velocities = new Vector3[pointCount + 1];

            _baseVertices[0] = Vector3.zero;
            _currentVertices[0] = Vector3.zero;

            int[] triangles = new int[pointCount * 3];
            for (int i = 0; i < pointCount; i++)
            {
                float angle = (i / (float)pointCount) * Mathf.PI * 2f;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
                _baseVertices[i + 1] = pos;
                _currentVertices[i + 1] = pos;
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % pointCount + 1;
            }

            _mesh.vertices = _currentVertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        private void Update()
        {
            HandleInput();
            SimulatePhysics();
        }

        private void HandleInput()
        {
            if (_cam == null) return;
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0f;
                float minDist = float.MaxValue;
                int best = -1;
                for (int i = 1; i <= pointCount; i++)
                {
                    float dist = Vector3.Distance(transform.TransformPoint(_currentVertices[i]), mouseWorld);
                    if (dist < minDist) { minDist = dist; best = i; }
                }
                if (minDist < dragRadius)
                {
                    _isDragging = true;
                    _selectedIndex = best;
                    TelegramWebAppBridge.TriggerHaptic("impact");
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
                _selectedIndex = -1;
            }
        }

        private void SimulatePhysics()
        {
            float dt = Time.deltaTime;
            for (int i = 1; i <= pointCount; i++)
            {
                if (_isDragging && i == _selectedIndex)
                {
                    Vector3 mouseWorld = _cam.ScreenToWorldPoint(Input.mousePosition);
                    mouseWorld.z = 0f;
                    _currentVertices[i] = transform.InverseTransformPoint(mouseWorld);
                    _velocities[i] = Vector3.zero;
                    continue;
                }
                Vector3 force = (_baseVertices[i] - _currentVertices[i]) * stiffness;
                Vector3 dampForce = -_velocities[i] * damping;
                _velocities[i] += (force + dampForce) * dt;
                _currentVertices[i] += _velocities[i] * dt;
            }
            _currentVertices[0] = Vector3.zero;
            _mesh.vertices = _currentVertices;
            _mesh.RecalculateBounds();
        }

        public void ApplySquishBurst(float forceMagnitude = -1f)
        {
            float force = forceMagnitude > 0 ? forceMagnitude : squishBurstForce;
            for (int i = 1; i <= pointCount; i++)
            {
                Vector3 dir = (_baseVertices[i] - _baseVertices[0]).normalized;
                _velocities[i] += dir * force;
            }
            TelegramWebAppBridge.TriggerHaptic("heavy");
        }

        public void ResetShape()
        {
            for (int i = 0; i <= pointCount; i++)
            {
                _currentVertices[i] = _baseVertices[i];
                _velocities[i] = Vector3.zero;
            }
            _mesh.vertices = _currentVertices;
        }
    }
}
