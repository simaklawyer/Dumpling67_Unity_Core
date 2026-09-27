using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Dumpling67.Core;
using Dumpling67.Integrations;

using Dumpling67.Managers;

namespace Dumpling67.UI
{
    /// <summary>
    /// Карточка «Мой рекорд» + шаринг в Telegram / VK.
    /// </summary>
    public class ShareCardUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scoreLabel;
        [SerializeField] TextMeshProUGUI coinsLabel;
        [SerializeField] GameObject root;

        public void Show(int meters, int coinsEarned)
        {
            if (root != null) root.SetActive(true);
            if (scoreLabel != null) scoreLabel.text = $"{meters} м";
            if (coinsLabel != null) coinsLabel.text = $"+{coinsEarned}";
        }

        public void Hide()
        {
            if (root != null) root.SetActive(false);
        }

        public void OnShareTelegram()
        {
            int hs = GameManager.Instance != null && GameManager.Instance.SaveData != null
                ? GameManager.Instance.SaveData.highScore
                : 0;
            if (TelegramWebAppBridge.Instance != null)
                TelegramWebAppBridge.Instance.ShareRun(hs);
            else
                TelegramWebAppBridge.TriggerHaptic("medium");
        }

        public void OnShareVk()
        {
            int hs = GameManager.Instance != null && GameManager.Instance.SaveData != null
                ? GameManager.Instance.SaveData.highScore
                : 0;
            if (VkGamesBridge.Instance != null)
                VkGamesBridge.Instance.Share(
                    "https://vk.com/app_dumpling67",
                    $"Рекорд в Dumpling 67: {hs} м!");
        }

        public void OnWatchAdDouble()
        {
            if (AdsManager.Instance != null)
                AdsManager.Instance.ShowRewardedForDoubleCoins();
        }
    }
}
