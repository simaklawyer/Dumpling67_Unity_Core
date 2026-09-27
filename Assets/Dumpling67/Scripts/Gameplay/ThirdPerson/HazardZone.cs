using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Опасная зона (кипяток / вилка): отбрасывает игрока и снимает монеты.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        public int coinPenalty = 10;
        public float knockback = 8f;
        public float upwardBoost = 4f;
        public float invulnSeconds = 1.2f;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponent<ThirdPersonPlayerController>();
            if (player == null) return;

            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null && coinPenalty > 0)
                economy.TrySpend(coinPenalty);

            Vector3 dir = (other.transform.position - transform.position);
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.01f) dir = -other.transform.forward;
            dir.Normalize();

            var cc = other.GetComponent<CharacterController>();
            if (cc != null)
                cc.Move((dir * knockback + Vector3.up * upwardBoost) * 0.15f);

            GameAudio.Play("hit");
            GameEvents.TriggerToast($"Ой! -{coinPenalty}", "danger");

            var juice = other.GetComponent<JuiceFeedback>();
            if (juice != null) juice.Shake(0.2f);
        }
    }
}
