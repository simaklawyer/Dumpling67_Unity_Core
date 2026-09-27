using System;
using System.Collections.Generic;
using UnityEngine;
using Dumpling67.Core;
using Dumpling67.Analytics;

namespace Dumpling67.Managers
{
    /// <summary>
    /// Заготовка IAP. В Editor — mock. На мобилках подключите Unity IAP / RevenueCat.
    /// Telegram Stars обрабатывается отдельно через TelegramWebAppBridge.
    /// </summary>
    public class IapManager : MonoBehaviour
    {
        public static IapManager Instance { get; private set; }

        [Serializable]
        public struct ProductDef
        {
            public string productId;
            public string title;
            public int coinReward;
            public string priceLabel; // для UI, реальная цена — из стора
        }

        [SerializeField]
        private List<ProductDef> products = new List<ProductDef>
        {
            new ProductDef { productId = "coins_500",  title = "Горсть монет", coinReward = 500,  priceLabel = "99 ₽" },
            new ProductDef { productId = "coins_1500", title = "Миска монет",  coinReward = 1500, priceLabel = "249 ₽" },
            new ProductDef { productId = "coins_5000", title = "Казан монет",  coinReward = 5000, priceLabel = "699 ₽" },
        };

        public IReadOnlyList<ProductDef> Products => products;

        public event Action<string, bool> OnPurchaseFinished;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Покупка. Сейчас — мгновенный mock. Замените тело на UnityPurchasing.
        /// </summary>
        public void Purchase(string productId)
        {
            var def = products.Find(p => p.productId == productId);
            if (string.IsNullOrEmpty(def.productId))
            {
                Debug.LogWarning($"[IAP] Unknown product {productId}");
                OnPurchaseFinished?.Invoke(productId, false);
                Metrics.IapPurchase(productId, false);
                return;
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Grant(def);
            OnPurchaseFinished?.Invoke(productId, true);
            Metrics.IapPurchase(productId, true);
            GameEvents.TriggerToast($"+{def.coinReward} (IAP mock)", "success");
#else
            // TODO: UnityPurchasing / Google Play Billing / StoreKit
            Debug.Log($"[IAP] Purchase requested: {productId} — wire Unity IAP here");
            Grant(def);
            OnPurchaseFinished?.Invoke(productId, true);
            Metrics.IapPurchase(productId, true);
#endif
        }

        private void Grant(ProductDef def)
        {
            var economy = GameManager.Instance != null ? GameManager.Instance.economy : null;
            if (economy != null) economy.AddCoins(def.coinReward);
        }
    }
}
