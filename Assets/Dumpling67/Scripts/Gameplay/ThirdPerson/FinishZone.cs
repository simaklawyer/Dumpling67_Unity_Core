using UnityEngine;
using Dumpling67.Core;
using Dumpling67.Analytics;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Финиш уровня: игрок забегает в триггер — победа + награда.
    /// </summary>
    public class FinishZone : MonoBehaviour
    {
        public int bonusCoins = 100;
        public string winMessage = "Сбежал из кастрюли!";
        public bool once = true;

        private bool _done;

        private void OnTriggerEnter(Collider other)
        {
            if (_done && once) return;
            if (other.GetComponent<ThirdPersonPlayerController>() == null) return;

            _done = true;
            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null) economy.AddCoins(bonusCoins);

            GameAudio.Play("collect");
            GameEvents.TriggerToast($"{winMessage} +{bonusCoins}", "success");

            int hs = GameManager.Instance != null && GameManager.Instance.SaveData != null
                ? GameManager.Instance.SaveData.highScore : 0;
            Metrics.RunFinished(0, bonusCoins, hs);

            var juice = other.GetComponent<JuiceFeedback>();
            if (juice != null) { juice.OnCollect(); juice.Shake(0.18f); }
        }
    }
}
