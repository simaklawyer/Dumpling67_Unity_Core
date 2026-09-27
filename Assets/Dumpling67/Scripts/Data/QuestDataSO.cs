using UnityEngine;

[CreateAssetMenu(fileName = "QuestData", menuName = "Dumpling67/Quest Data")]
public class QuestDataSO : ScriptableObject
{
    public string questId;
    public string title;
    [TextArea] public string description;
    public int targetCount;
    public int rewardSoftCurrency;
    public bool isDaily;
}
