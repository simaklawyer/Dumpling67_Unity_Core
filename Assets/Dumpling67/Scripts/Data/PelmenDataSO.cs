using UnityEngine;

[CreateAssetMenu(fileName = "PelmenData", menuName = "Dumpling67/Pelmen Data")]
public class PelmenDataSO : ScriptableObject
{
    public string pelmenId;
    public string displayName;
    public Sprite icon;
    public int rarity; // 1-5
    public int baseValue;
    public GameObject prefab;
}
