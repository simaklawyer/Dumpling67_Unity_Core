using UnityEngine;
using Dumpling67.Integrations;

namespace Dumpling67.Managers
{
    /// <summary>
    /// Единая точка rewarded/interstitial: Яндекс → VK → mock.
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        public enum Platform { Auto, Yandex, Vk, Mock }

        [SerializeField] Platform preferred = Platform.Auto;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ShowRewardedForDoubleCoins()
        {
            switch (Resolve())
            {
                case Platform.Yandex:
                    if (YandexGamesBridge.Instance != null)
                        YandexGamesBridge.Instance.ShowRewardedAd();
                    break;
                case Platform.Vk:
                    if (VkGamesBridge.Instance != null)
                        VkGamesBridge.Instance.ShowRewardedAd();
                    break;
                default:
                    if (YandexGamesBridge.Instance != null)
                        YandexGamesBridge.Instance.ShowRewardedAd();
                    break;
            }
        }

        public void ShowInterstitialSoft()
        {
            if (Resolve() == Platform.Yandex && YandexGamesBridge.Instance != null)
                YandexGamesBridge.Instance.ShowFullscreenAd();
        }

        private Platform Resolve()
        {
            if (preferred != Platform.Auto) return preferred;
#if UNITY_WEBGL
            return Platform.Yandex;
#else
            return Platform.Mock;
#endif
        }
    }
}
