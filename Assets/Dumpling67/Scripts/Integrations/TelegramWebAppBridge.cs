using System.Runtime.InteropServices;
using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Integrations
{
    /// <summary>
    /// Мост Unity WebGL ↔ Telegram WebApp SDK (Haptic, MainButton, Stars, Share).
    /// </summary>
    public class TelegramWebAppBridge : MonoBehaviour
    {
        public static TelegramWebAppBridge Instance { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void TriggerHapticFeedback(string style);
        [DllImport("__Internal")] private static extern void ShowTelegramMainButton(string text);
        [DllImport("__Internal")] private static extern void HideTelegramMainButton();
        [DllImport("__Internal")] private static extern void TelegramOpenInvoice(string url);
        [DllImport("__Internal")] private static extern void TelegramShareUrl(string url, string text);
        [DllImport("__Internal")] private static extern void TelegramExpand();
#endif

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void TriggerHaptic(string style = "medium")
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { TriggerHapticFeedback(style); } catch { }
#else
            Debug.Log($"[Telegram Mock] Haptic: {style}");
#endif
        }

        public static void SetMainButton(string text, bool visible)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                if (visible) ShowTelegramMainButton(text);
                else HideTelegramMainButton();
            }
            catch { }
#else
            Debug.Log($"[Telegram Mock] MainButton '{text}' visible={visible}");
#endif
        }

        public static void Expand()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { TelegramExpand(); } catch { }
#else
            Debug.Log("[Telegram Mock] Expand");
#endif
        }

        public void PurchaseStarsPack(int starsAmount, int coinReward)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try { TelegramOpenInvoice("https://t.me/$invoice_placeholder"); } catch { MockStars(coinReward); }
#else
            MockStars(coinReward);
#endif
        }

        public void ShareRun(int meters)
        {
            string text = $"Мой рекорд в Dumpling 67: {meters} м! Попробуй убежать из кастрюли.";
            string url = "https://t.me/Dumpling67Bot?start=ref_share";
#if UNITY_WEBGL && !UNITY_EDITOR
            try { TelegramShareUrl(url, text); } catch { }
#else
            Debug.Log($"[Telegram Mock] Share: {text}");
            GameEvents.TriggerToast("Ссылка скопирована (mock)", "info");
#endif
        }

        public void OnStarsPaymentSuccess(string payload)
        {
            int coins = 500;
            if (int.TryParse(payload, out int parsed)) coins = parsed;
            MockStars(coins);
        }

        public void OnMainButtonClick(string _)
        {
            GameEvents.TriggerToast("MainButton", "info");
        }

        private void MockStars(int coinReward)
        {
            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null) economy.AddCoins(coinReward);
            GameEvents.TriggerToast($"+{coinReward} за Stars (mock)", "success");
            TriggerHaptic("heavy");
        }
    }
}
