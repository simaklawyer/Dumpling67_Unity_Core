using System.Runtime.InteropServices;
using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Integrations
{
    /// <summary>
    /// Мост к VK Bridge / VK Games (реклама, шаринг).
    /// </summary>
    public class VkGamesBridge : MonoBehaviour
    {
        public static VkGamesBridge Instance { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void VkShowRewarded();
        [DllImport("__Internal")] private static extern void VkShare(string link, string text);
#endif

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowRewardedAd()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { VkShowRewarded(); } catch { MockReward(); }
#else
            MockReward();
#endif
        }

        public void Share(string link, string text)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { VkShare(link, text); } catch { }
#else
            Debug.Log($"[VK Mock] Share: {text} | {link}");
            GameEvents.TriggerToast("Пост VK (mock)", "info");
#endif
        }

        public void OnVkRewarded()
        {
            MockReward();
        }

        private void MockReward()
        {
            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null) economy.AddCoins(120);
            GameEvents.TriggerToast("+120 за VK-рекламу (mock)", "success");
        }
    }
}
