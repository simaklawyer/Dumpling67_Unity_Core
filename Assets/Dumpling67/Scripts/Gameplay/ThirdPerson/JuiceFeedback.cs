using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Экранный «сок»: shake камеры, импульс scale, простые particles.
    /// </summary>
    public class JuiceFeedback : MonoBehaviour
    {
        [Header("Camera shake")]
        public Transform cameraTransform;
        public float landShake = 0.12f;
        public float collectShake = 0.08f;
        public float shakeTime = 0.12f;

        [Header("Player punch")]
        public Transform playerVisual;
        public float jumpSquash = 0.12f;

        [Header("VFX (optional prefabs)")]
        public ParticleSystem landDust;
        public ParticleSystem collectBurst;
        public ParticleSystem jumpPuff;

        private Vector3 _shakeOffset;
        private float _shakeTimer;
        private float _shakeAmp;
        private Vector3 _visualBaseScale = Vector3.one;

        private void Awake()
        {
            if (playerVisual != null) _visualBaseScale = playerVisual.localScale;
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (cameraTransform == null) return;
            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                float t = _shakeTimer / shakeTime;
                _shakeOffset = Random.insideUnitSphere * _shakeAmp * t;
                cameraTransform.position += _shakeOffset;
            }
        }

        public void OnJump()
        {
            if (jumpPuff != null) jumpPuff.Play();
            PunchScale(new Vector3(0.9f, 1.12f, 0.9f), 0.1f);
        }

        public void OnLand(float impactSpeed)
        {
            float amp = landShake * Mathf.Clamp01(impactSpeed / 16f);
            Shake(amp);
            if (landDust != null) landDust.Play();
            PunchScale(new Vector3(1.15f, 0.85f, 1.15f), 0.12f);
        }

        public void OnCollect()
        {
            Shake(collectShake);
            if (collectBurst != null)
            {
                collectBurst.transform.position = transform.position + Vector3.up;
                collectBurst.Play();
            }
            PunchScale(new Vector3(1.2f, 1.2f, 1.2f), 0.08f);
        }

        public void Shake(float amplitude)
        {
            _shakeAmp = amplitude;
            _shakeTimer = shakeTime;
        }

        private void PunchScale(Vector3 target, float duration)
        {
            if (playerVisual == null) return;
            StopAllCoroutines();
            StartCoroutine(ScalePunchCo(target, duration));
        }

        private System.Collections.IEnumerator ScalePunchCo(Vector3 target, float duration)
        {
            float half = duration * 0.5f;
            float t = 0f;
            while (t < half)
            {
                t += Time.deltaTime;
                playerVisual.localScale = Vector3.Lerp(_visualBaseScale, Vector3.Scale(_visualBaseScale, target), t / half);
                yield return null;
            }
            t = 0f;
            Vector3 from = playerVisual.localScale;
            while (t < half)
            {
                t += Time.deltaTime;
                playerVisual.localScale = Vector3.Lerp(from, _visualBaseScale, t / half);
                yield return null;
            }
            playerVisual.localScale = _visualBaseScale;
        }
    }
}
