using UnityEngine;

[CreateAssetMenu(fileName = "EconomyBalance", menuName = "Dumpling67/Economy Balance")]
public class EconomyBalanceSO : ScriptableObject
{
    public int startingSoftCurrency = 100;
    public int startingHardCurrency = 10;
    public int pelmenCollectReward = 5;
    public int levelCompleteBonus = 50;
    public float softToHardRate = 100f;
}
