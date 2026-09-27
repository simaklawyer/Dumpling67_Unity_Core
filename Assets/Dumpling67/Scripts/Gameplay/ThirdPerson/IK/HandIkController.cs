using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson.IK
{
    /// <summary>
    /// Управляет reach-IK рук к interactable (пельмени и т.п.).
    /// Работает и с Humanoid (AnimatorHandIK), и с generic (TwoBoneIK).
    /// </summary>
    public class HandIkController : MonoBehaviour
    {
        [Header("Mode")]
        public bool useHumanoidIk = true;
        public AnimatorHandIK humanoidIk;
        public TwoBoneIK leftTwoBone;
        public TwoBoneIK rightTwoBone;

        [Header("Auto reach")]
        public float reachRadius = 1.8f;
        public LayerMask interactableMask = ~0;
        public string interactableTag = "Interactable";
        public float blendSpeed = 8f;

        [Header("Idle")]
        public Transform leftIdleTarget;
        public Transform rightIdleTarget;

        private float _leftW, _rightW;
        private Transform _leftTarget, _rightTarget;

        private void Awake()
        {
            if (humanoidIk == null) humanoidIk = GetComponent<AnimatorHandIK>();
            if (leftTwoBone == null || rightTwoBone == null)
            {
                var bones = GetComponentsInChildren<TwoBoneIK>();
                if (bones.Length >= 2) { leftTwoBone = bones[0]; rightTwoBone = bones[1]; }
            }
        }

        private void LateUpdate()
        {
            FindNearestTargets(out Transform left, out Transform right);
            BlendToward(ref _leftW, ref _leftTarget, left, leftIdleTarget);
            BlendToward(ref _rightW, ref _rightTarget, right, rightIdleTarget);

            if (useHumanoidIk && humanoidIk != null)
            {
                humanoidIk.SetLeftHand(_leftTarget, _leftW);
                humanoidIk.SetRightHand(_rightTarget, _rightW);
            }
            else
            {
                if (leftTwoBone != null)
                {
                    leftTwoBone.target = _leftTarget;
                    leftTwoBone.weight = _leftW;
                }
                if (rightTwoBone != null)
                {
                    rightTwoBone.target = _rightTarget;
                    rightTwoBone.weight = _rightW;
                }
            }
        }

        private void BlendToward(ref float w, ref Transform cur, Transform desired, Transform idle)
        {
            Transform goal = desired != null ? desired : idle;
            float targetW = desired != null ? 1f : 0f;
            w = Mathf.MoveTowards(w, targetW, blendSpeed * Time.deltaTime);
            if (goal != null) cur = goal;
        }

        private void FindNearestTargets(out Transform left, out Transform right)
        {
            left = right = null;
            var cols = Physics.OverlapSphere(transform.position + Vector3.up, reachRadius, interactableMask);
            float bestL = float.MaxValue, bestR = float.MaxValue;
            foreach (var c in cols)
            {
                if (!string.IsNullOrEmpty(interactableTag) && !c.CompareTag(interactableTag))
                    continue;
                Vector3 local = transform.InverseTransformPoint(c.transform.position);
                float d = local.magnitude;
                if (local.x < 0f && d < bestL) { bestL = d; left = c.transform; }
                if (local.x >= 0f && d < bestR) { bestR = d; right = c.transform; }
            }
        }
    }
}
