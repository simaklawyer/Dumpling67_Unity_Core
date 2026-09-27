using System;
using UnityEngine;
using Dumpling67.Integrations;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Third-person (Roblox-like): WASD/stick относительно камеры, прыжок с нормальной физикой.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonPlayerController : MonoBehaviour
    {
        [Header("Move")]
        public float walkSpeed = 6f;
        public float sprintSpeed = 10f;
        public float acceleration = 48f;
        public float airAcceleration = 22f;
        public float rotationSpeed = 14f;
        [Range(0f, 1f)] public float airControl = 0.72f;

        [Header("Jump — высота и тайминги")]
        public float jumpHeight = 1.85f;
        public float coyoteTime = 0.12f;
        public float jumpBuffer = 0.15f;
        public bool allowDoubleJump = true;

        [Header("Jump — гравитация (кривая)")]
        public float jumpUpGravity = 28f;
        public float fallGravity = 48f;
        public float apexGravityScale = 0.55f;
        public float apexThreshold = 1.2f;
        [Range(0.1f, 1f)] public float jumpCutMultiplier = 0.4f;
        public float maxFallSpeed = 26f;
        public float groundedStick = -2.5f;

        [Header("Ground check")]
        public float groundCheckRadius = 0.28f;
        public float groundCheckExtra = 0.12f;
        public LayerMask groundMask = ~0;

        [Header("Refs")]
        public Transform cameraTarget;
        public Animator animator;
        public JuiceFeedback juice;
        public CharacterAnimDriver animDriver;

        [Header("Mobile")]
        public VirtualJoystick moveStick;
        public LookTouchPad lookPad;

        /// <summary>isDouble</summary>
        public event Action<bool> Jumped;
        /// <summary>impact speed</summary>
        public event Action<float> Landed;

        public float VerticalSpeed => _velocity.y;
        public float HorizontalSpeed => _currentSpeed;
        public float NormalizedSpeed => _currentSpeed / Mathf.Max(0.01f, sprintSpeed);
        public bool IsGroundedState => _wasGrounded;
        public bool IsSprinting { get; private set; }
        public Vector3 PlanarVelocity => new Vector3(_velocity.x, 0f, _velocity.z);
        public float PlanarSpeed => new Vector3(_velocity.x, 0f, _velocity.z).magnitude;
        public float VerticalVelocity => _velocity.y;
        public bool IsGrounded => _wasGrounded;

        private CharacterController _cc;
        private Vector3 _velocity;
        private float _currentSpeed;
        private float _coyoteCounter;
        private float _jumpBufferCounter;
        private int _jumpsLeft;
        private bool _wasGrounded;
        private bool _jumpCut;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            if (cameraTarget == null && Camera.main != null)
                cameraTarget = Camera.main.transform;
            if (animDriver == null) animDriver = GetComponent<CharacterAnimDriver>();
            if (juice == null) juice = GetComponent<JuiceFeedback>();
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            bool grounded = CheckGrounded();

            if (grounded && !_wasGrounded)
            {
                float impact = Mathf.Abs(_velocity.y);
                Landed?.Invoke(impact);
                juice?.OnLand(impact);
                _jumpsLeft = allowDoubleJump ? 2 : 1;
                _jumpCut = false;
            }
            _wasGrounded = grounded;

            if (grounded) _coyoteCounter = coyoteTime;
            else _coyoteCounter -= dt;

            _jumpBufferCounter -= dt;
            if (ReadJumpPressed()) _jumpBufferCounter = jumpBuffer;

            if (_jumpBufferCounter > 0f && (_coyoteCounter > 0f || _jumpsLeft > 0))
            {
                bool isDouble = !grounded && _jumpsLeft < (allowDoubleJump ? 2 : 1);
                DoJump(isDouble);
                _jumpBufferCounter = 0f;
                _coyoteCounter = 0f;
            }

            if (ReadJumpReleased() && _velocity.y > 0f && !_jumpCut)
            {
                _velocity.y *= jumpCutMultiplier;
                _jumpCut = true;
            }

            Vector2 input = ReadMoveInput();
            Vector3 wish = GetCameraRelative(input);
            float targetSpeed = (Input.GetKey(KeyCode.LeftShift) || (moveStick != null && moveStick.Value.magnitude > 0.85f))
                ? sprintSpeed : walkSpeed;
            IsSprinting = targetSpeed > walkSpeed + 0.1f && input.sqrMagnitude > 0.1f;

            float accel = grounded ? acceleration : airAcceleration * airControl;
            Vector3 planar = new Vector3(_velocity.x, 0f, _velocity.z);
            Vector3 desired = wish * targetSpeed;
            planar = Vector3.MoveTowards(planar, desired, accel * dt);
            _velocity.x = planar.x;
            _velocity.z = planar.z;
            _currentSpeed = planar.magnitude;

            // Gravity curve
            if (grounded && _velocity.y < 0f)
                _velocity.y = groundedStick;
            else
            {
                float g = _velocity.y > apexThreshold ? jumpUpGravity * apexGravityScale
                    : (_velocity.y > 0f ? jumpUpGravity : fallGravity);
                _velocity.y -= g * dt;
                if (_velocity.y < -maxFallSpeed) _velocity.y = -maxFallSpeed;
            }

            // Rotate toward move
            if (wish.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(wish, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f - Mathf.Exp(-rotationSpeed * dt));
            }

            _cc.Move(_velocity * dt);
        }

        private void DoJump(bool isDouble)
        {
            float jumpVel = Mathf.Sqrt(2f * jumpUpGravity * jumpHeight);
            _velocity.y = jumpVel;
            _jumpsLeft--;
            _jumpCut = false;
            Jumped?.Invoke(isDouble);
            juice?.OnJump();
            GameAudio.Play("jump");
            TelegramWebAppBridge.TriggerHaptic("light");
        }

        private bool CheckGrounded()
        {
            Vector3 origin = transform.position + Vector3.up * (groundCheckRadius + 0.05f);
            float dist = groundCheckRadius + groundCheckExtra;
            return Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out _, dist, groundMask, QueryTriggerInteraction.Ignore)
                || _cc.isGrounded;
        }

        private Vector2 ReadMoveInput()
        {
            if (moveStick != null && moveStick.IsActive)
                return moveStick.Value;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            return new Vector2(h, v);
        }

        private bool ReadJumpPressed()
        {
            return Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space);
        }

        private bool ReadJumpReleased()
        {
            return Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.Space);
        }

        private Vector3 GetCameraRelative(Vector2 input)
        {
            if (input.sqrMagnitude < 0.01f) return Vector3.zero;
            Transform cam = cameraTarget != null ? cameraTarget : (Camera.main != null ? Camera.main.transform : null);
            if (cam == null) return new Vector3(input.x, 0f, input.y).normalized;
            Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
            Vector3 right = cam.right; right.y = 0f; right.Normalize();
            Vector3 dir = (forward * input.y + right * input.x);
            float mag = Mathf.Clamp01(input.magnitude);
            return dir.sqrMagnitude > 0.01f ? dir.normalized * mag : Vector3.zero;
        }

        public void Teleport(Vector3 worldPos)
        {
            _cc.enabled = false;
            transform.position = worldPos;
            _cc.enabled = true;
            _velocity = Vector3.zero;
            animDriver?.ResetVisual();
        }
    }
}
