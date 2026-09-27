// ⚠ FROZEN (2026-09-26): вторичная 2D-ветка (раннер/softbody), не используется third-person ядром.
// См. HANDOFF.md / _Frozen2D/README.md.
using UnityEngine;
using System.Collections.Generic;

namespace Dumpling67.Gameplay
{
    /// <summary>
    /// Пул препятствий, монет и паверапов. Спавн с бюджетом реакции.
    /// </summary>
    public class RunnerSpawner : MonoBehaviour
    {
        [SerializeField] float baseInterval = 1.2f;
        [SerializeField] float minInterval = 0.7f;
        [SerializeField] float coinInterval = 0.45f;
        [SerializeField] float powerupInterval = 5f;

        readonly List<Transform> _pool = new List<Transform>();
        float _obs;
        float _coin;
        float _power;
        float _speed = 6f;

        public void ResetSpawner()
        {
            _obs = 0.8f;
            _coin = 0.3f;
            _power = 4f;
            _speed = 6f;
            foreach (var t in _pool)
            {
                if (t) t.gameObject.SetActive(false);
            }
        }

        public void Tick(float dt, float speed, float spawnX, float groundY)
        {
            _speed = speed;
            _obs -= dt;
            _coin -= dt;
            _power -= dt;

            float gap = Mathf.Max(minInterval, baseInterval - speed * 0.04f);

            if (_obs <= 0f)
            {
                bool chili = Random.value > 0.5f;
                Spawn(chili ? "chili" : "fork", spawnX, chili ? groundY + 1.4f : groundY);
                _obs = gap;
            }

            if (_coin <= 0f)
            {
                Spawn("coin", spawnX, groundY + Random.Range(0.6f, 2.2f));
                _coin = coinInterval;
            }

            if (_power <= 0f)
            {
                Spawn(Random.value > 0.5f ? "shield" : "magnet", spawnX, groundY + 1.1f);
                _power = powerupInterval;
            }
        }

        void Spawn(string kind, float x, float y)
        {
            Transform item = null;
            foreach (var t in _pool)
            {
                if (t && !t.gameObject.activeSelf)
                {
                    item = t;
                    break;
                }
            }

            if (item == null)
            {
                var go = new GameObject("Pooled_" + kind);
                item = go.transform;
                item.SetParent(transform);
                _pool.Add(item);
            }

            item.name = kind;
            item.position = new Vector3(x, y, 0f);
            item.gameObject.SetActive(true);
        }
    }
}
