using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson.IK
{
    /// <summary>
    /// Аналитический two-bone IK: boneA (плечо) → boneB (локоть) → boneC (кисть).
    /// Humanoid не нужен. Вызывать из LateUpdate или из HandIkController.
    /// </summary>
    public class TwoBoneIK : MonoBehaviour
    {
        [Header("Bones")]
        public Transform boneA;
        public Transform boneB;
        public Transform boneC;

        [Header("Target")]
        public Transform target;
        public Transform pole;

        [Header("Weights")]
        [Range(0f, 1f)] public float weight = 1f;
        [Range(0f, 1f)] public float rotationWeight = 1f;

        [Header("Reach")]
        public bool enforceReach = true;
        [Range(0.5f, 1f)] public float maxReach = 0.98f;

        [Header("Runtime")]
        public bool autoSolve = true;

        private float _lenAB;
        private float _lenBC;

        private void Awake() => MeasureLengths();

        public void MeasureLengths()
        {
            if (boneA == null || boneB == null || boneC == null) return;
            _lenAB = Vector3.Distance(boneA.position, boneB.position);
            _lenBC = Vector3.Distance(boneB.position, boneC.position);
        }

        private void LateUpdate()
        {
            if (!autoSolve) return;
            if (weight <= 0.001f || target == null) return;
            Solve(target.position, target.rotation, weight);
        }

        public void Solve(Vector3 targetPos, Quaternion targetRot, float w)
        {
            if (boneA == null || boneB == null || boneC == null) return;
            if (_lenAB < 1e-5f || _lenBC < 1e-5f) MeasureLengths();
            if (_lenAB < 1e-5f || _lenBC < 1e-5f) return;

            w = Mathf.Clamp01(w);
            if (w <= 0f) return;

            Vector3 aPos = boneA.position;
            Vector3 toTarget = targetPos - aPos;
            float dist = toTarget.magnitude;
            float maxDist = (_lenAB + _lenBC) * maxReach;
            float minDist = Mathf.Abs(_lenAB - _lenBC) * 0.05f + 0.001f;

            if (enforceReach)
                dist = Mathf.Clamp(dist, minDist, maxDist);
            else
                dist = Mathf.Max(dist, minDist);

            if (toTarget.sqrMagnitude < 1e-8f) return;
            Vector3 dir = toTarget.normalized;
            Vector3 goal = aPos + dir * dist;

            Vector3 poleVec = pole != null ? pole.position - aPos : Vector3.up;

            Vector3 normal = Vector3.Cross(dir, poleVec);
            if (normal.sqrMagnitude < 1e-6f)
                normal = Vector3.Cross(dir, Vector3.right);
            normal.Normalize();
            Vector3 bendDir = Vector3.Cross(normal, dir).normalized;

            float cosA = Mathf.Clamp(
                (_lenAB * _lenAB + dist * dist - _lenBC * _lenBC) / (2f * _lenAB * dist),
                -1f, 1f);
            float angA = Mathf.Acos(cosA) * Mathf.Rad2Deg;

            Vector3 elbowDir = (Quaternion.AngleAxis(angA, normal) * dir).normalized;
            if (Vector3.Dot(elbowDir, bendDir) < 0f)
                elbowDir = (Quaternion.AngleAxis(-angA, normal) * dir).normalized;

            Vector3 elbowPos = aPos + elbowDir * _lenAB;

            RotateBoneToward(boneA, boneB, elbowPos, w);
            RotateBoneToward(boneB, boneC, goal, w);

            if (rotationWeight > 0f)
                boneC.rotation = Quaternion.Slerp(boneC.rotation, targetRot, w * rotationWeight);
        }

        private static void RotateBoneToward(Transform bone, Transform child, Vector3 worldTarget, float w)
        {
            Vector3 from = child.position - bone.position;
            Vector3 to = worldTarget - bone.position;
            if (from.sqrMagnitude < 1e-8f || to.sqrMagnitude < 1e-8f) return;
            Quaternion delta = Quaternion.FromToRotation(from, to);
            bone.rotation = Quaternion.Slerp(bone.rotation, delta * bone.rotation, w);
        }
    }
}
