using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dumpling67.Core;
using Dumpling67.Data;

namespace Dumpling67.Managers
{
    public class GachaManager : MonoBehaviour
    {
        private PlayerSaveData _save;
        private PelmenDataSO[] _allPelmeni;
        private BlindBoxSO[] _allBoxes;

        public void Initialize(PlayerSaveData save, PelmenDataSO[] pelmeni, BlindBoxSO[] boxes)
        {
            _save = save;
            _allPelmeni = pelmeni ?? new PelmenDataSO[0];
            _allBoxes = boxes ?? new BlindBoxSO[0];
        }

        public bool TryOpenBox(BlindBoxSO box, out PelmenDataSO reward, out bool isNew)
        {
            reward = null;
            isNew = false;

            if (box == null || _save == null) return false;

            var economy = GameManager.Instance?.economy;
            if (economy == null || !economy.TrySpend(box.coinCost))
            {
                GameEvents.TriggerToast("Недостаточно монет!", "info");
                return false;
            }

            reward = box.GetRandomDrop();
            if (reward == null)
            {
                GameEvents.TriggerToast("Ошибка дропа!", "info");
                return false;
            }

            isNew = !_save.unlockedPelmeniIds.Contains(reward.pelmenId);
            _save.AddInventoryCount(reward.pelmenId, 1);

            if (isNew)
            {
                _save.unlockedPelmeniIds.Add(reward.pelmenId);
                GameEvents.TriggerPelmenUnlocked(reward);
            }
            else
            {
                economy.AddCoins(50);
            }

            _save.totalBoxesOpened++;
            GameEvents.TriggerBoxOpened(reward, isNew);
            GameEvents.TriggerQuestProgress("open_boxes");
            GameEvents.TriggerSave();

            return true;
        }

        public bool IsUnlocked(string pelmenId)
        {
            return _save != null && _save.unlockedPelmeniIds.Contains(pelmenId);
        }

        public int UnlockedCount => _save?.unlockedPelmeniIds.Count ?? 0;
        public int TotalPelmeni => _allPelmeni?.Length ?? 0;

        public PelmenDataSO GetPelmenById(string id)
        {
            return _allPelmeni?.FirstOrDefault(p => p != null && p.pelmenId == id);
        }

        public IEnumerable<PelmenDataSO> GetUnlockedPelmeni()
        {
            if (_save == null || _allPelmeni == null) yield break;
            foreach (var id in _save.unlockedPelmeniIds)
            {
                var p = GetPelmenById(id);
                if (p != null) yield return p;
            }
        }
    }
}
