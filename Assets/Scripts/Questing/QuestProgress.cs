namespace Questing
{
    public class QuestProgress
    {
        public QuestStatusEnum QuestStatus { get; set; }
        public QuestObjective QuestObjective { get; set; }
    }

    
    public class QuestObjective
    {
        private int _requiredAmount;
        public int CurrentAmount { get; set; }

        public QuestObjective(int currAmount, int reqAmount)
        {
            CurrentAmount = currAmount;
            _requiredAmount = reqAmount;
        }

        public void UpdateCurrentAmount(int amount)
        {
            CurrentAmount += amount;
        }
        
        
    }
}