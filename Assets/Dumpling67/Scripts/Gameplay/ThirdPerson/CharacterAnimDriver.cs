using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Анимация движения:
    /// 1) Animator (Speed, Grounded, VerticalVelocity, Jump, Land, MotionSpeed)
    /// 2) Процедурный fallback на visual — bob, lean, tilt в воздухе
    ///    (видно сразу, даже без клипов).
    /// </summary>
    public class CharacterAnimDriver : MonoBehaviour
    {
        [Header("Sources")]
        public ThirdPersonPlayerController motor;
        public Animator animator;
        public Transform visual;

        [Header("Animator param names")]
        public string speedParam = "Speed";
        public string motionSpeedParam = "MotionSpeed";
        public string groundedParam = "Grounded";
        public string verticalParam = "VerticalVelocity";
        public string jumpTrigger = "Jump";
        public string landTrigger = "Land";
        public string isMovingParam = "IsMoving";
        public string isSprintingParam = "IsSprinting";

        [Header("Smoothing")]
        public float speedDamp = 0.12f;
        public float verticalDamp = 0.08f;

        [Header("Procedural — walk bob")]
        public bool useProcedural = true;
        public float bobAmplitude = 0.06f;
        public float bobFrequency = 8f;
        public float sprintBobMul = 1.35f;

        [Header("Procedural — lean / tilt")]
        public float leanAngle = 12f;
        public float airTiltAngle = 18f;
        public float leanSmooth = 10f;

        [Header("Thresholds")]
        public float moveThreshold = 0.15f;
        public float sprintThreshold = 0.85f;

        private float _speedAnim;
        private float _vertAnim;
        private float _bobPhase;
        private Vector3 _visualBasePos;
        private Quaternion _visualBaseRot;
        private Vector3 _visualBaseScale;
        private int _hSpeed, _hMotion, _hGrounded, _hVert, _hJump, _hLand, _hMoving, _hSprint;
        private bool _hasAnimator;

        private void Awake()
        {
            if (motor == null) motor = GetComponent<ThirdPersonPlayerController>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (visual == null && transform.childCount > 0)
                visual = transform.GetChild(0);

            CacheAnimatorHashes();
            CacheVisualBase();
        }

        private void OnEnable()
        {
            if (motor != null)
            {
                motor.Jumped += HandleJump;
                motor.Landed += HandleLand;
            }
        }

        private void OnDisable()
        {
            if (motor != null)
            {
                motor.Jumped -= HandleJump;
                motor.Landed -= HandleLand;
            }
        }

        private void CacheAnimatorHashes()
        {
            _hasAnimator = animator != null;
            if (!_hasAnimator) return;
            _hSpeed = Animator.StringToHash(speedParam);
            _hMotion = Animator.StringToHash(motionSpeedParam);
            _hGrounded = Animator.StringToHash(groundedParam);
            _hVert = Animator.StringToHash(verticalParam);
            _hJump = Animator.StringToHash(jumpTrigger);
            _hLand = Animator.StringToHash(landTrigger);
            _hMoving = Animator.StringToHash(isMovingParam);
            _hSprint = Animator.StringToHash(isSprintingParam);
        }

        private void CacheVisualBase()
        {
            if (visual == null) return;
            _visualBasePos = visual.localPosition;
            _visualBaseRot = visual.localRotation;
            _visualBaseScale = visual.localScale;
        }

        private void Update()
        {
            if (motor == null) return;
            float dt = Time.deltaTime;
            float normSpeed = motor.PlanarSpeed / Mathf.Max(0.1f, motor.sprintSpeed);
            bool grounded = motor.IsGrounded;
            float vy = motor.VerticalVelocity;
            bool moving = normSpeed > moveThreshold;
            bool sprinting = normSpeed > sprintThreshold;

            if (_hasAnimator) DriveAnimator(normSpeed, grounded, vy, moving, sprinting);
            if (useProcedural && visual != null) DriveProcedural(dt, normSpeed, grounded, vy, moving, sprinting);
        }

        private void DriveAnimator(float normSpeed, bool grounded, float vy, bool moving, bool sprinting)
        {
            _speedAnim = Mathf.Lerp(_speedAnim, normSpeed, 1f - Mathf.Exp(-1f / speedDamp * Time.deltaTime));
            _vertAnim = Mathf.Lerp(_vertAnim, vy, 1f - Mathf.Exp(-1f / verticalDamp * Time.deltaTime));

            animator.SetFloat(_hSpeed, _speedAnim);
            animator.SetFloat(_hMotion, normSpeed);
            animator.SetBool(_hGrounded, grounded);
            animator.SetFloat(_hVert, _vertAnim);
            animator.SetBool(_hMoving, moving);
            animator.SetBool(_hSprint, sprinting);
        }

        private void DriveProcedural(float dt, float normSpeed, bool grounded, float vy, bool moving, bool sprinting)
        {
            Vector3 pos = _visualBasePos;
            if (grounded && moving)
            {
                float freq = bobFrequency * (sprinting ? sprintBobMul : 1f);
                _bobPhase += dt * freq * Mathf.Clamp01(normSpeed);
                float bob = Mathf.Sin(_bobPhase * Mathf.PI * 2f) * bobAmplitude * normSpeed;
                pos.y += Mathf.Abs(bob);
            }
            else
            {
                _bobPhase = 0f;
            }

            Vector3 localVel = transform.InverseTransformDirection(new Vector3(motor.PlanarVelocity.x, 0f, motor.PlanarVelocity.z));
            float leanX = 0f;
            float leanZ = 0f;
            if (grounded)
            {
                leanX = Mathf.Clamp(localVel.z / Mathf.Max(0.1f, motor.sprintSpeed), -1f, 1f) * leanAngle;
                leanZ = -Mathf.Clamp(localVel.x / Mathf.Max(0.1f, motor.sprintSpeed), -1f, 1f) * leanAngle;
            }
            else
            {
                leanX = Mathf.Clamp(vy / 12f, -1f, 1f) * -airTiltAngle;
            }

            Quaternion targetLean = _visualBaseRot * Quaternion.Euler(leanX, 0f, leanZ);
            visual.localPosition = Vector3.Lerp(visual.localPosition, pos, 1f - Mathf.Exp(-leanSmooth * dt));
            visual.localRotation = Quaternion.Slerp(visual.localRotation, targetLean, 1f - Mathf.Exp(-leanSmooth * dt));
        }

        private void HandleJump(bool isDouble)
        {
            if (_hasAnimator) animator.SetTrigger(_hJump);
        }

        private void HandleLand(float impact)
        {
            if (_hasAnimator && impact > 4f) animator.SetTrigger(_hLand);
        }

        public void ResetVisual()
        {
            if (visual == null) return;
            visual.localPosition = _visualBasePos;
            visual.localRotation = _visualBaseRot;
            visual.localScale = _visualBaseScale;
            _bobPhase = 0f;
        }
    }
}
