using System.Collections.Generic;
using UnityEngine;

namespace Dumpling67.Gameplay.ThirdPerson
{
    /// <summary>
    /// Простой пул AudioSource + именованные клипы.
    /// Повесь на DontDestroy объект, заполни клипы в инспекторе.
    /// Если клип не задан — короткий procedural beep (чтобы «звук был» сразу).
    /// </summary>
    public class GameAudio : MonoBehaviour
    {
        public static GameAudio Instance { get; private set; }

        [System.Serializable]
        public struct NamedClip
        {
            public string id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume;
            public float pitchJitter;
        }

        [SerializeField] private NamedClip[] clips = new NamedClip[]
        {
            new NamedClip { id = "jump", volume = 0.7f, pitchJitter = 0.08f },
            new NamedClip { id = "land", volume = 0.55f, pitchJitter = 0.05f },
            new NamedClip { id = "collect", volume = 0.8f, pitchJitter = 0.12f },
            new NamedClip { id = "hit", volume = 0.75f, pitchJitter = 0.05f },
            new NamedClip { id = "whoosh", volume = 0.5f, pitchJitter = 0.1f },
        };

        [SerializeField] private int poolSize = 8;
        [SerializeField] private bool proceduralFallback = true;

        private readonly Dictionary<string, NamedClip> _map = new Dictionary<string, NamedClip>();
        private AudioSource[] _pool;
        private int _cursor;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var c in clips)
            {
                if (!string.IsNullOrEmpty(c.id))
                    _map[c.id] = c;
            }

            _pool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var go = new GameObject($"SFX_{i}");
                go.transform.SetParent(transform);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                _pool[i] = src;
            }
        }

        public static void Play(string id)
        {
            if (Instance != null) Instance.PlayInternal(id);
        }

        private void PlayInternal(string id)
        {
            if (!_map.TryGetValue(id, out var def))
            {
                if (proceduralFallback) Beep(id);
                return;
            }

            var src = _pool[_cursor++ % _pool.Length];
            float pitch = 1f + Random.Range(-def.pitchJitter, def.pitchJitter);
            src.pitch = pitch;
            src.volume = def.volume <= 0f ? 0.7f : def.volume;

            if (def.clip != null)
            {
                src.PlayOneShot(def.clip, src.volume);
            }
            else if (proceduralFallback)
            {
                Beep(id);
            }
        }

        /// <summary>
        /// Процедурный «щелчок» без ассетов — чтобы сразу было слышно feedback.
        /// </summary>
        private void Beep(string id)
        {
            float freq = id switch
            {
                "jump" => 520f,
                "land" => 180f,
                "collect" => 880f,
                "hit" => 120f,
                _ => 400f
            };
            int sampleRate = 44100;
            float duration = 0.08f;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var clip = AudioClip.Create("proc_" + id, samples, 1, sampleRate, false);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float env = 1f - t / duration;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * env * 0.35f;
            }
            clip.SetData(data, 0);
            var src = _pool[_cursor++ % _pool.Length];
            src.pitch = 1f;
            src.PlayOneShot(clip, 0.7f);
        }
    }
}
