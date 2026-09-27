using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dumpling67.Core;
using Dumpling67.Data;

namespace Dumpling67.Managers
{
    public class QuestManager : MonoBehaviour
    {
        private PlayerSaveData _save;
        private QuestDataSO[] _allQuests;

        public void Initialize(PlayerSaveData save, QuestDataSO[] quests)
        {
            _save = save;
            _allQuests = quests ?? new QuestDataSO[0];

            foreach (var q in _allQuests)
            {
                if (q == null) continue;
                if (!_save.questProgress.Any(p => p.questId == q.questId))
                {
                    _save.questProgress.Add(new QuestProgress
                    {
                        questId = q.questId,
                        currentValue = 0,
                        claimed = false
                    });
                }
            }
        }

        public void AddProgress(string questId, int amount = 1)
        {
            if (_save == null) return;

            var progress = _save.questProgress.FirstOrDefault(p => p.questId == questId);
            if (progress == null || progress.claimed) return;

            var questDef = _allQuests.FirstOrDefault(q => q != null && q.questId == questId);
            if (questDef == null) return;

            if (questDef.questType == QuestType.RunDistance)
                progress.currentValue = Mathf.Max(progress.currentValue, amount);
            else
                progress.currentValue += amount;

            GameEvents.TriggerQuestProgress(questId);

            if (progress.currentValue >= questDef.targetValue && !progress.claimed)
            {
                GameEvents.TriggerToast($"Квест выполнен: {questDef.title}!", "success");
            }

            GameEvents.TriggerSave();
        }

        public bool TryClaim(string questId)
        {
            if (_save == null) return false;

            var progress = _save.questProgress.FirstOrDefault(p => p.questId == questId);
            var questDef = _allQuests.FirstOrDefault(q => q != null && q.questId == questId);

            if (progress == null || questDef == null || progress.claimed) return false;
            if (progress.currentValue < questDef.targetValue) return false;

            progress.claimed = true;
            _save.completedQuestIds.Add(questId);

            var economy = GameManager.Instance?.economy;
            if (economy != null)
                economy.AddCoins(questDef.rewardCoins);

            GameEvents.TriggerQuestCompleted(questId);
            GameEvents.TriggerToast($"+{questDef.rewardCoins} 🥟 за квест!", "success");
            GameEvents.TriggerSave();
            return true;
        }

        public QuestProgress GetProgress(string questId)
        {
            return _save?.questProgress.FirstOrDefault(p => p.questId == questId);
        }

        public IEnumerable<(QuestDataSO def, QuestProgress prog)> GetAllQuests()
        {
            if (_allQuests == null) yield break;
            foreach (var q in _allQuests)
            {
                if (q == null) continue;
                var prog = GetProgress(q.questId) ?? new QuestProgress { questId = q.questId };
                yield return (q, prog);
            }
        }
    }
}
