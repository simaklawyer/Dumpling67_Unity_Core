using UnityEngine;
using Dumpling67.Data;
using Dumpling67.Managers;
using Dumpling67.Integrations;
using Dumpling67.Analytics;

namespace Dumpling67.Core
{
    /// <summary>
    /// Точка входа. Сейв, менеджеры, сигнал готовности площадкам (Яндекс/TMA).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public EconomyManager economy;
        public GachaManager gacha;
        public QuestManager quests;
        public AdsManager ads;

        [Header("Catalog (assign in Inspector)")]
        public PelmenDataSO[] allPelmeni;
        public BlindBoxSO[] allBoxes;
        public QuestDataSO[] allQuests;

        public PlayerSaveData SaveData { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();

            EnsureBridge<Integrations.TelegramWebAppBridge>("TelegramWebAppBridge");
            EnsureBridge<Integrations.YandexGamesBridge>("YandexGamesBridge");
            EnsureBridge<Integrations.VkGamesBridge>("VkGamesBridge");

            ApplyRuntimePerfDefaults();
        }

        private static void ApplyRuntimePerfDefaults()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private static T EnsureBridge<T>(string exactName) where T : Component
        {
            var go = GameObject.Find(exactName);
            if (go == null)
            {
                go = new GameObject(exactName);
                DontDestroyOnLoad(go);
            }
            var comp = go.GetComponent<T>();
            return comp != null ? comp : go.AddComponent<T>();
        }

        private void Start()
        {
            if (economy != null) economy.Initialize(SaveData);
            if (gacha != null) gacha.Initialize(SaveData, allPelmeni, allBoxes);
            if (quests != null) quests.Initialize(SaveData, allQuests);

            GameEvents.OnSaveRequested += SaveProgress;

            TelegramWebAppBridge.Expand();
            if (YandexGamesBridge.Instance != null)
                YandexGamesBridge.Instance.GameReady();

            Metrics.SessionStart();
        }

        private void OnDestroy()
        {
            GameEvents.OnSaveRequested -= SaveProgress;
        }

        public void LoadProgress()
        {
            SaveData = SaveSystem.Load();
            Debug.Log($"[GameManager] Загружено. Монеты: {SaveData.coins}, Рекорд: {SaveData.highScore}");
        }

        public void SaveProgress()
        {
            if (SaveData == null) return;
            SaveSystem.Save(SaveData);
        }

        public void ResetAllProgress()
        {
            SaveSystem.ResetProgress();
            LoadProgress();
            if (economy != null) economy.Initialize(SaveData);
            if (gacha != null) gacha.Initialize(SaveData, allPelmeni, allBoxes);
            if (quests != null) quests.Initialize(SaveData, allQuests);
            GameEvents.TriggerToast("Прогресс сброшен!", "info");
        }

        private void OnApplicationQuit() { SaveProgress(); }
        private void OnApplicationPause(bool pauseStatus) { if (pauseStatus) SaveProgress(); }
    }
}
