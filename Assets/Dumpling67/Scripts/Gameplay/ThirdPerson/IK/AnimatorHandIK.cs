using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson.IK
{
    /// <summary>
    /// Humanoid-only: OnAnimatorIK для рук и look-at.
    /// Требует Avatar = Humanoid и Animate Physics / IK Pass на слое.
    /// Для generic/пельменя без humanoid используйте TwoBoneIK + HandIkController.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorHandIK : MonoBehaviour
    {
        [Header("Weights")]
        [Range(0f, 1f)] public float leftHandWeight;
        [Range(0f, 1f)] public float rightHandWeight;
        [Range(0f, 1f)] public float leftRotationWeight = 1f;
        [Range(0f, 1f)] public float rightRotationWeight = 1f;
        [Range(0f, 1f)] public float lookWeight;

        [Header("Targets")]
        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Transform lookTarget;

        [Header("Elbow hints (optional)")]
        public Transform leftElbowHint;
        public Transform rightElbowHint;
        [Range(0f, 1f)] public float hintWeight = 0.5f;

        private Animator _anim;

        private void Awake() => _anim = GetComponent<Animator>();

        private void OnAnimatorIK(int layerIndex)
        {
            if (_anim == null || !_anim.isHuman) return;

            // Look
            if (lookTarget != null && lookWeight > 0f)
            {
                _anim.SetLookAtWeight(lookWeight);
                _anim.SetLookAtPosition(lookTarget.position);
            }
            else
            {
                _anim.SetLookAtWeight(0f);
            }

            ApplyHand(AvatarIKGoal.LeftHand, leftHandTarget, leftHandWeight, leftRotationWeight,
                leftElbowHint, AvatarIKHint.LeftElbow);
            ApplyHand(AvatarIKGoal.RightHand, rightHandTarget, rightHandWeight, rightRotationWeight,
                rightElbowHint, AvatarIKHint.RightElbow);
        }

        private void ApplyHand(
            AvatarIKGoal goal, Transform target, float posW, float rotW,
            Transform hint, AvatarIKHint hintId)
        {
            if (target != null && posW > 0f)
            {
                _anim.SetIKPositionWeight(goal, posW);
                _anim.SetIKRotationWeight(goal, rotW);
                _anim.SetIKPosition(goal, target.position);
                _anim.SetIKRotation(goal, target.rotation);
            }
            else
            {
                _anim.SetIKPositionWeight(goal, 0f);
                _anim.SetIKRotationWeight(goal, 0f);
            }

            if (hint != null && hintWeight > 0f)
            {
                _anim.SetIKHintPositionWeight(hintId, hintWeight * posW);
                _anim.SetIKHintPosition(hintId, hint.position);
            }
            else
            {
                _anim.SetIKHintPositionWeight(hintId, 0f);
            }
        }

        public void SetRightHand(Transform t, float w = 1f)
        {
            rightHandTarget = t;
            rightHandWeight = w;
        }

        public void SetLeftHand(Transform t, float w = 1f)
        {
            leftHandTarget = t;
            leftHandWeight = w;
        }
    }
}
