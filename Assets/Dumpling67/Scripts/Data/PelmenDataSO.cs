using UnityEngine;

namespace Dumpling67.Data
{
    public enum RarityTier
    {
        Common,
        Rare,
        Epic,
        Mythic
    }

    [CreateAssetMenu(fileName = "NewPelmenData", menuName = "Dumpling67/Pelmen Data", order = 1)]
    public class PelmenDataSO : ScriptableObject
    {
        [Header("Основная информация")]
        public string pelmenId = "p1";
        public string pelmenName = "Классический Пельмень";
        [TextArea(2, 4)]
        public string description = "Обычный, но любимый.";
        public RarityTier rarity = RarityTier.Common;

        [Header("Визуальное оформление")]
        public Sprite mainSprite;
        public Color skinColor = Color.white;
        public Color eyeColor = Color.black;
        public Color shadowColor = new Color(0.6f, 0f, 1f, 0.4f);
        public string emojiFallback = "🥟";

        [Header("Экономика и бонусы")]
        public int baseCoinReward = 10;
        public float runnerSpeedMultiplier = 1.0f;
        public float squishForceMultiplier = 1.0f;
    }
}
