using UnityEngine;

namespace Dumpling67.Data
{
    /// <summary>
    /// Централизованный баланс экономики (крутилка без перекомпиляции логики).
    /// </summary>
    [CreateAssetMenu(fileName = "EconomyBalance", menuName = "Dumpling67/Economy Balance", order = 10)]
    public class EconomyBalanceSO : ScriptableObject
    {
        [Header("Старт")]
        public int startingCoins = 1250;

        [Header("Гача — компенсация дубликата")]
        public int duplicateCoinRefund = 50;

        [Header("Раннер")]
        public int coinsPerTenMeters = 1;
        public float startSpeed = 6f;
        public float maxSpeed = 14f;
        public float accelPerSecond = 0.15f;

        [Header("Реклама (mock / fallback)")]
        public int rewardedCoinsYandex = 150;
        public int rewardedCoinsVk = 120;

        [Header("Рекомендуемые цены коробок")]
        public int studentBoxCost = 100;
        public int memeBoxCost = 300;
        public int goldBoxCost = 750;
        public int royalBoxCost = 1500;

        [Header("Целевые drop-rate (сумма ≈ 1)")]
        [Range(0f, 1f)] public float studentCommon = 0.70f;
        [Range(0f, 1f)] public float studentRare = 0.25f;
        [Range(0f, 1f)] public float studentEpic = 0.05f;
        [Range(0f, 1f)] public float memeCommon = 0.30f;
        [Range(0f, 1f)] public float memeRare = 0.50f;
        [Range(0f, 1f)] public float memeEpic = 0.18f;
        [Range(0f, 1f)] public float memeMythic = 0.02f;
    }
}
