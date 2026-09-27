using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Integrations
{
    /// <summary>
    /// Мост к SDK Яндекс Игр (реклама, лидерборд, данные игрока).
    /// В Editor — mock. На WebGL — jslib + YaGames.
    /// </summary>
    public class YandexGamesBridge : MonoBehaviour
    {
        public static YandexGamesBridge Instance { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void YandexShowRewarded();
        [DllImport("__Internal")] private static extern void YandexShowFullscreen();
        [DllImport("__Internal")] private static extern void YandexSetLeaderboardScore(string boardId, int score);
        [DllImport("__Internal")] private static extern void YandexReady();
#endif

        public event Action OnRewardedSuccess;
        public event Action OnRewardedClosed;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void GameReady()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { YandexReady(); } catch { }
#else
            Debug.Log("[Yandex Mock] GameReady");
#endif
        }

        public void ShowRewardedAd()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { YandexShowRewarded(); } catch { OnRewardedMock(); }
#else
            OnRewardedMock();
#endif
        }

        public void ShowFullscreenAd()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { YandexShowFullscreen(); } catch { }
#else
            Debug.Log("[Yandex Mock] Fullscreen ad");
#endif
        }

        public void SubmitScore(string boardId, int score)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { YandexSetLeaderboardScore(boardId, score); } catch { }
#else
            Debug.Log($"[Yandex Mock] Leaderboard {boardId} = {score}");
#endif
        }

        // Вызывается из jslib через SendMessage
        public void OnYandexRewarded()
        {
            OnRewardedSuccess?.Invoke();
            GameEvents.TriggerToast("Награда за рекламу получена", "success");
        }

        public void OnYandexRewardedClose()
        {
            OnRewardedClosed?.Invoke();
        }

        private void OnRewardedMock()
        {
            Debug.Log("[Yandex Mock] Rewarded success (+150 coins)");
            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null) economy.AddCoins(150);
            OnRewardedSuccess?.Invoke();
            GameEvents.TriggerToast("+150 за рекламу (mock)", "success");
        }
    }
}
