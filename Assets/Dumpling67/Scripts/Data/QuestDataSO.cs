using UnityEngine;

namespace Dumpling67.Data
{
    public enum QuestType
    {
        OpenBoxes,
        RunDistance,
        CollectCoins,
        SquishCount,
        UnlockPelmen
    }

    [CreateAssetMenu(fileName = "NewQuest", menuName = "Dumpling67/Quest Data", order = 3)]
    public class QuestDataSO : ScriptableObject
    {
        public string questId = "q1";
        public string title = "Открой 3 коробки";
        [TextArea(1, 3)]
        public string description = "Испытай удачу в гаче!";
        public QuestType questType = QuestType.OpenBoxes;
        public int targetValue = 3;
        public int rewardCoins = 150;
        public bool isDaily = true;
    }
}
