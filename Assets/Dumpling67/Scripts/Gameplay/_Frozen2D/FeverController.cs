// ⚠ FROZEN (2026-09-26): вторичная 2D-ветка (раннер/softbody), не используется third-person ядром.
// См. HANDOFF.md / _Frozen2D/README.md.
using UnityEngine;
using Dumpling67.Core;

namespace Dumpling67.Gameplay
{
    public class FeverController : MonoBehaviour
    {
        [SerializeField] int streakNeed = 5;
        int _streak;
        bool _fever;

        public bool IsFever => _fever;
        public int CoinMultiplier => _fever ? 2 : 1;

        public void ResetFever()
        {
            _streak = 0;
            _fever = false;
        }

        public void OnCoinCollected()
        {
            _streak++;
            if (!_fever && _streak >= streakNeed)
            {
                _fever = true;
                GameEvents.TriggerToast("FEVER x2", "info");
            }
        }

        public void OnHit()
        {
            _streak = 0;
        }
    }
}
