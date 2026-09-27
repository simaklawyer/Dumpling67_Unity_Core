using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Managers
{
    public class EconomyManager : MonoBehaviour
    {
        private PlayerSaveData _save;

        public int Coins => _save?.coins ?? 0;
        public int HighScore => _save?.highScore ?? 0;

        public void Initialize(PlayerSaveData save)
        {
            _save = save;
            GameEvents.TriggerCoinsChanged(_save.coins);
            GameEvents.TriggerHighScoreUpdated(_save.highScore);
        }

        public bool TrySpend(int amount)
        {
            if (_save == null || _save.coins < amount) return false;
            _save.coins -= amount;
            GameEvents.TriggerCoinsChanged(_save.coins);
            GameEvents.TriggerSave();
            return true;
        }

        public void AddCoins(int amount)
        {
            if (_save == null || amount <= 0) return;
            _save.coins += amount;
            GameEvents.TriggerCoinsChanged(_save.coins);
            GameEvents.TriggerSave();
        }

        public void UpdateHighScore(int score)
        {
            if (_save == null) return;
            if (score > _save.highScore)
            {
                _save.highScore = score;
                GameEvents.TriggerHighScoreUpdated(_save.highScore);
                GameEvents.TriggerSave();
            }
        }
    }
}
