using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dumpling67.Core
{
    [Serializable]
    public class PlayerSaveData
    {
        public int coins = 1250;
        public int highScore = 0;
        public string activeSkinId = "p1";
        public List<string> unlockedPelmeniIds = new List<string> { "p1", "p2", "p3" };
        // FIX: Dictionary не сериализуется JsonUtility (молча теряется при Save/Load).
        // Инвентарь хранится как List<InventoryEntry>, доступ — через методы ниже.
        public List<InventoryEntry> inventory = new List<InventoryEntry>();
        public List<string> completedQuestIds = new List<string>();
        public List<QuestProgress> questProgress = new List<QuestProgress>();
        public int totalBoxesOpened = 0;
        public int totalSquishes = 0;
        public long lastDailyResetUnix = 0;

        // --- Inventory helpers (замена Dictionary) ---

        public int GetInventoryCount(string pelmenId)
        {
            var e = inventory.Find(x => x.pelmenId == pelmenId);
            return e?.count ?? 0;
        }

        public void SetInventoryCount(string pelmenId, int count)
        {
            var e = inventory.Find(x => x.pelmenId == pelmenId);
            if (e != null)
            {
                e.count = count;
            }
            else if (count != 0)
            {
                inventory.Add(new InventoryEntry { pelmenId = pelmenId, count = count });
            }
        }

        public int AddInventoryCount(string pelmenId, int delta)
        {
            int newCount = GetInventoryCount(pelmenId) + delta;
            SetInventoryCount(pelmenId, newCount);
            return newCount;
        }
    }

    [Serializable]
    public class InventoryEntry
    {
        public string pelmenId;
        public int count;
    }

    [Serializable]
    public class QuestProgress
    {
        public string questId;
        public int currentValue;
        public bool claimed;
    }

    public static class SaveSystem
    {
        private const string SAVE_KEY = "dumpling67_save_v2";

        public static void Save(PlayerSaveData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                PlayerPrefs.SetString(SAVE_KEY, json);
                PlayerPrefs.Save();
                Debug.Log("[SaveSystem] Прогресс сохранён.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Ошибка сохранения: {ex.Message}");
            }
        }

        public static PlayerSaveData Load()
        {
            if (!PlayerPrefs.HasKey(SAVE_KEY))
            {
                var fresh = CreateDefaultSave();
                Save(fresh);
                return fresh;
            }

            try
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                var data = JsonUtility.FromJson<PlayerSaveData>(json);
                if (data == null) return CreateDefaultSave();
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Ошибка чтения: {ex.Message}");
                return CreateDefaultSave();
            }
        }

        public static void ResetProgress()
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            PlayerPrefs.Save();
            Debug.Log("[SaveSystem] Прогресс сброшен.");
        }

        private static PlayerSaveData CreateDefaultSave()
        {
            return new PlayerSaveData
            {
                coins = 1250,
                highScore = 0,
                activeSkinId = "p1",
                unlockedPelmeniIds = new List<string> { "p1", "p2", "p3" },
                completedQuestIds = new List<string>(),
                questProgress = new List<QuestProgress>(),
                totalBoxesOpened = 0,
                totalSquishes = 0,
                lastDailyResetUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
    }
}
