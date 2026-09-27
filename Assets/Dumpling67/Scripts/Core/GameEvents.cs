using System;
using Dumpling67.Data;

namespace Dumpling67.Core
{
    /// <summary>
    /// Глобальная шина событий. Декуплирует системы.
    /// Подписывайся в OnEnable / отписывайся в OnDisable.
    /// </summary>
    public static class GameEvents
    {
        // Economy
        public static Action<int> OnCoinsChanged;
        public static Action<int> OnHighScoreUpdated;

        // Collection / Gacha
        public static Action<PelmenDataSO> OnPelmenUnlocked;
        public static Action<PelmenDataSO, bool> OnBoxOpened; // (reward, isNew)

        // Quests
        public static Action<string> OnQuestProgress;     // questId
        public static Action<string> OnQuestCompleted;    // questId

        // UI / Feedback
        public static Action<string, string> OnToastTriggered; // (message, type)
        public static Action OnSaveRequested;

        // Runner
        public static Action<int> OnRunnerScoreChanged;
        public static Action OnRunnerGameOver;

        // Helpers
        public static void TriggerCoinsChanged(int amount) => OnCoinsChanged?.Invoke(amount);
        public static void TriggerHighScoreUpdated(int score) => OnHighScoreUpdated?.Invoke(score);
        public static void TriggerPelmenUnlocked(PelmenDataSO pelmen) => OnPelmenUnlocked?.Invoke(pelmen);
        public static void TriggerBoxOpened(PelmenDataSO reward, bool isNew) => OnBoxOpened?.Invoke(reward, isNew);
        public static void TriggerQuestProgress(string questId) => OnQuestProgress?.Invoke(questId);
        public static void TriggerQuestCompleted(string questId) => OnQuestCompleted?.Invoke(questId);
        public static void TriggerToast(string msg, string type = "info") => OnToastTriggered?.Invoke(msg, type);
        public static void TriggerSave() => OnSaveRequested?.Invoke();
        public static void TriggerRunnerScore(int score) => OnRunnerScoreChanged?.Invoke(score);
        public static void TriggerRunnerGameOver() => OnRunnerGameOver?.Invoke();
    }
}
