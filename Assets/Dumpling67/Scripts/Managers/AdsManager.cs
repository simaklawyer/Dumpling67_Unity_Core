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
                    else
                        MockReward();
                    break;
                case Platform.Vk:
                    if (VkGamesBridge.Instance != null)
                        VkGamesBridge.Instance.ShowRewarded();
                    else
                        MockReward();
                    break;
                default:
                    MockReward();
                    break;
            }
        }

        public void ShowInterstitial()
        {
            switch (Resolve())
            {
                case Platform.Yandex:
                    YandexGamesBridge.Instance?.ShowInterstitial();
                    break;
                case Platform.Vk:
                    VkGamesBridge.Instance?.ShowInterstitial();
                    break;
                default:
                    Debug.Log("[Ads] Mock interstitial");
                    break;
            }
        }

        private Platform Resolve()
        {
            if (preferred != Platform.Auto) return preferred;
            if (YandexGamesBridge.Instance != null) return Platform.Yandex;
            if (VkGamesBridge.Instance != null) return Platform.Vk;
            return Platform.Mock;
        }

        private void MockReward()
        {
            Debug.Log("[Ads] Mock rewarded → +coins");
            // EconomyManager.Instance?.AddSoft(50);
        }
    }
}
