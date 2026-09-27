using UnityEngine;

namespace Dumpling67.Data
{
    [CreateAssetMenu(fileName = "BlindBox", menuName = "Dumpling67/Blind Box")]
    public class BlindBoxSO : ScriptableObject
    {
        public string boxId;
        public string displayName;
        public Sprite icon;
        public int priceSoft;
        public int priceHard;
        [Range(0f, 1f)] public float newChance = 0.3f;
        public PelmenDataSO[] possibleRewards;
        public int[] weights; // same length as possibleRewards
    }
}
