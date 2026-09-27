#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Dumpling67.Data;

namespace Dumpling67.EditorTools
{
    /// <summary>
    /// Меню: Dumpling67 → Gacha Balance Simulator
    /// Гоняет BlindBoxSO.GetRandomDrop() N раз и показывает статистику.
    /// </summary>
    public class GachaBalanceSimulator : EditorWindow
    {
        private BlindBoxSO _box;
        private int _iterations = 20000;
        private string _report = "";
        private Vector2 _scroll;

        [MenuItem("Dumpling67/Gacha Balance Simulator")]
        public static void Open()
        {
            GetWindow<GachaBalanceSimulator>("Gacha Balance");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox(
                "Симулирует BlindBoxSO.GetRandomDrop() N раз. Нужен настроенный BlindBoxSO.",
                MessageType.Info);

            _box = (BlindBoxSO)EditorGUILayout.ObjectField("Blind Box", _box, typeof(BlindBoxSO), false);
            _iterations = EditorGUILayout.IntSlider("Iterations", _iterations, 1000, 100000);

            if (GUILayout.Button("Run Simulation") && _box != null)
            {
                Run();
            }

            if (!string.IsNullOrEmpty(_report))
            {
                _scroll = EditorGUILayout.BeginScrollView(_scroll);
                EditorGUILayout.TextArea(_report, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
        }

        private void Run()
        {
            var counts = new Dictionary<string, int>();
            int nulls = 0;
            for (int i = 0; i < _iterations; i++)
            {
                var r = _box.GetRandomDrop();
                if (r == null) { nulls++; continue; }
                string key = r.pelmenId ?? r.name;
                if (!counts.ContainsKey(key)) counts[key] = 0;
                counts[key]++;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"Iterations: {_iterations}");
            sb.AppendLine($"Null drops: {nulls} ({100f * nulls / _iterations:F2}%)");
            sb.AppendLine("---");
            foreach (var kv in counts.OrderByDescending(x => x.Value))
            {
                float pct = 100f * kv.Value / _iterations;
                sb.AppendLine($"{kv.Key}: {kv.Value} ({pct:F2}%)");
            }
            _report = sb.ToString();
        }
    }
}
#endif
