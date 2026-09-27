// ⚠ FROZEN (2026-09-26): вторичная 2D-ветка.
// См. HANDOFF.md / _Frozen2D/README.md.
using UnityEngine;
using Dumpling67.Core;
using Dumpling67.Integrations;

namespace Dumpling67.Gameplay
{
    /// <summary>
    /// Раннер: прыжок, coyote, double-jump, связка со спавнером и fever.
    /// </summary>
    public class RunnerController : MonoBehaviour
    {
        [Header("Movement")]
        public float jumpForce = 12f;
        public float doubleJumpForce = 10f;
        public float gravity = 28f;
        public float groundY = 0f;
        public float runSpeed = 6f;
        public float maxSpeed = 14f;
        public float accelPerSecond = 0.15f;

        [Header("Forgiveness")]
        public float coyoteTime = 0.08f;
        public float jumpBuffer = 0.12f;

        [Header("Refs")]
        public RunnerSpawner spawner;
        public FeverController fever;

        [Header("State")]
        public bool isRunning;
        public float distance;
        public int coinsCollected;

        private float _vy;
        private bool _isGrounded = true;
        private bool _canDoubleJump = true;
        private float _coyote;
        private float _buffer;
        private float _speed;
        private Transform _player;

        private void Awake()
        {
            _player = transform;
            if (spawner == null) spawner = GetComponent<RunnerSpawner>();
            if (fever == null) fever = GetComponent<FeverController>();
        }

        private void Update()
        {
            if (!isRunning) return;

            float dt = Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                _buffer = jumpBuffer;

            _buffer = Mathf.Max(0f, _buffer - dt);
            if (!_isGrounded) _coyote = Mathf.Max(0f, _coyote - dt);

            bool wantJump = _buffer > 0f;
            if (wantJump && (_isGrounded || _coyote > 0f))
            {
                _vy = jumpForce;
                _isGrounded = false;
                _canDoubleJump = true;
                _coyote = 0f;
                _buffer = 0f;
                TelegramWebAppBridge.TriggerHaptic("light");
            }
            else if (wantJump && _canDoubleJump && !_isGrounded)
            {
                _vy = doubleJumpForce;
                _canDoubleJump = false;
                _buffer = 0f;
                TelegramWebAppBridge.TriggerHaptic("light");
            }

            _vy -= gravity * dt;
            if (_vy > 28f) _vy = 28f;

            Vector3 pos = _player.position;
            pos.y += _vy * dt;

            if (pos.y <= groundY)
            {
                pos.y = groundY;
                _vy = 0f;
                if (!_isGrounded) _coyote = coyoteTime;
                _isGrounded = true;
                _canDoubleJump = true;
            }
            else
            {
                _isGrounded = false;
            }

            _player.position = pos;

            _speed = Mathf.Min(maxSpeed, _speed + accelPerSecond * dt);
            distance += _speed * dt;
            GameEvents.TriggerRunnerScore(Mathf.FloorToInt(distance));

            if (spawner != null)
                spawner.Tick(dt, _speed, _player.position.x + 18f, groundY);
        }

        public void StartRun()
        {
            isRunning = true;
            distance = 0f;
            coinsCollected = 0;
            _vy = 0f;
            _speed = runSpeed;
            _isGrounded = true;
            _canDoubleJump = true;
            _coyote = 0f;
            _buffer = 0f;
            Vector3 pos = _player.position;
            pos.y = groundY;
            _player.position = pos;
            if (spawner != null) spawner.ResetSpawner();
            if (fever != null) fever.ResetFever();
        }

        public void StopRun()
        {
            isRunning = false;
            GameEvents.TriggerRunnerGameOver();
            TelegramWebAppBridge.TriggerHaptic("heavy");

            int meters = Mathf.FloorToInt(distance);
            int earned = coinsCollected + meters / 10;

            var gm = GameManager.Instance;
            if (gm != null && gm.economy != null)
            {
                gm.economy.AddCoins(earned);
                gm.economy.UpdateHighScore(meters);
            }

            if (YandexGamesBridge.Instance != null)
                YandexGamesBridge.Instance.SubmitScore("dumpling_runner", meters);
        }

        public void CollectCoin(int amount = 1)
        {
            int mult = fever != null ? fever.CoinMultiplier : 1;
            coinsCollected += amount * mult;
            if (fever != null) fever.OnCoinCollected();
        }

        public void OnObstacleHit()
        {
            if (fever != null) fever.OnHit();
            StopRun();
        }
    }
}
