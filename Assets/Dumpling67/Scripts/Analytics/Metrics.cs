using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dumpling67.Analytics
{
    public static class Metrics
    {
        public interface IMetricsSink
        {
            void Track(string eventName, Dictionary<string, object> props);
        }

        private class DebugSink : IMetricsSink
        {
            public void Track(string eventName, Dictionary<string, object> props)
            {
                string extra = props != null && props.Count > 0 ? " " + JsonUtility.ToJson(new PropBag(props)) : "";
                Debug.Log($"[Metrics] {eventName}{extra}");
            }
        }

        [Serializable]
        private class PropBag
        {
            public List<string> keys = new List<string>();
            public List<string> values = new List<string>();
            public PropBag(Dictionary<string, object> props)
            {
                foreach (var kv in props)
                {
                    keys.Add(kv.Key);
                    values.Add(kv.Value != null ? kv.Value.ToString() : "");
                }
            }
        }

        private static IMetricsSink _sink = new DebugSink();

        public static void SetSink(IMetricsSink sink)
        {
            _sink = sink ?? new DebugSink();
        }

        public static void Track(string eventName, Dictionary<string, object> props = null)
        {
            try { _sink.Track(eventName, props); }
            catch (Exception e) { Debug.LogWarning($"[Metrics] {e.Message}"); }
        }

        public static void SessionStart() =>
            Track("session_start", new Dictionary<string, object> {
                { "ts", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
            });

        public static void BoxOpened(string boxId, string rarity, bool isNew) =>
            Track("box_opened", new Dictionary<string, object> {
                { "box_id", boxId }, { "rarity", rarity }, { "is_new", isNew ? 1 : 0 }
            });

        public static void RunFinished(int meters, int coins, int highScore) =>
            Track("run_finished", new Dictionary<string, object> {
                { "meters", meters }, { "coins", coins }, { "high_score", highScore }
            });

        public static void AdRewarded(string placement) =>
            Track("ad_rewarded", new Dictionary<string, object> { { "placement", placement } });

        public static void IapPurchase(string productId, bool success) =>
            Track("iap", new Dictionary<string, object> {
                { "product_id", productId }, { "success", success ? 1 : 0 }
            });

        public static void QuestClaimed(string questId) =>
            Track("quest_claimed", new Dictionary<string, object> { { "quest_id", questId } });
    }
}
