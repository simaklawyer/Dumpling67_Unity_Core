using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Подбираемый пельмень в мире. Крутится, подпрыгивает, при касании игрока — награда.
    /// </summary>
    public class CollectiblePelmen : MonoBehaviour
    {
        // Для HandIkController auto-reach удобно повесить tag Interactable

        public int coinReward = 15;
        public float rotateSpeed = 90f;
        public float bobAmplitude = 0.25f;
        public float bobSpeed = 2.5f;
        public GameObject collectVfxPrefab;
        public AudioClip collectClip;

        private Vector3 _origin;
        private bool _taken;

        private void Start()
        {
            _origin = transform.position;
        }

        private void Update()
        {
            if (_taken) return;
            transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, SpaceSpace.World);
            float y = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            transform.position = _origin + Vector3.up * y;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_taken) return;
            if (!other.CompareTag("Player") && other.GetComponent<ThirdPersonPlayerController>() == null)
                return;

            _taken = true;
            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null) economy.AddCoins(coinReward);

            GameAudio.Play("collect");
            GameEvents.TriggerToast($"+{coinReward}", "success");

            var juice = other.GetComponent<JuiceFeedback>();
            if (juice != null) juice.OnCollect();

            if (collectVfxPrefab != null)
            {
                var vfx = Instantiate(collectVfxPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 2f);
            }

            Destroy(gameObject);
        }
    }
}
