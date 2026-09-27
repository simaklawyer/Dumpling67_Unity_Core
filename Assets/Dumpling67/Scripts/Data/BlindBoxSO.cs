using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dumpling67.Data
{
    [Serializable]
    public struct DropProbability
    {
        public RarityTier rarity;
        [Range(0f, 1f)]
        public float chance;
    }

    [CreateAssetMenu(fileName = "NewBlindBox", menuName = "Dumpling67/Blind Box Data", order = 2)]
    public class BlindBoxSO : ScriptableObject
    {
        public string boxId = "box_common";
        public string boxName = "Обычная Коробка";
        public int coinCost = 100;
        public Sprite boxSprite;
        public Color boxGlowColor = Color.magenta;

        [Header("Шансы выпадения")]
        public List<DropProbability> dropTable = new List<DropProbability>
        {
            new DropProbability { rarity = RarityTier.Common, chance = 0.70f },
            new DropProbability { rarity = RarityTier.Rare,   chance = 0.22f },
            new DropProbability { rarity = RarityTier.Epic,   chance = 0.07f },
            new DropProbability { rarity = RarityTier.Mythic, chance = 0.01f }
        };

        [Header("Пул пельменей")]
        public List<PelmenDataSO> availablePelmeni = new List<PelmenDataSO>();

        public PelmenDataSO GetRandomDrop()
        {
            if (availablePelmeni == null || availablePelmeni.Count == 0)
            {
                Debug.LogWarning($"[BlindBoxSO] {boxName}: пул пельменей пуст!");
                return null;
            }

            float rand = UnityEngine.Random.value;
            float cumulative = 0f;
            RarityTier selectedRarity = RarityTier.Common;

            foreach (var drop in dropTable)
            {
                cumulative += drop.chance;
                if (rand <= cumulative)
                {
                    selectedRarity = drop.rarity;
                    break;
                }
            }

            var matchingPool = availablePelmeni.FindAll(p => p != null && p.rarity == selectedRarity);

            if (matchingPool.Count > 0)
                return matchingPool[UnityEngine.Random.Range(0, matchingPool.Count)];

            Debug.LogWarning($"[BlindBoxSO] {boxName}: нет пельменей редкости {selectedRarity} — fallback из всего пула.");
            return availablePelmeni[UnityEngine.Random.Range(0, availablePelmeni.Count)];
        }
    }
}
