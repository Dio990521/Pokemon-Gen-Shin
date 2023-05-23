using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }

    public Quest(QuestBase questBase)
    {
        Base = questBase;
    }

    public IEnumerator StartQuest()
    {
        Status = QuestStatus.None;
        yield return DialogueManager.Instance.ShowDialogue(Base.StartDialogue);

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);
    
    }

    public IEnumerator CompleteQuest()
    {
        Status = QuestStatus.Completed;
        yield return DialogueManager.Instance.ShowDialogue(Base.CompletedDialogue);

        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            inventory.RemoveItem(Base.RequiredItem);
        }

        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);
            yield return DialogueManager.Instance.ShowDialogueText($"ƒ„ªÒµ√¡À{Base.RewardItem.ItemName}£°");
        }

        var questList = QuestList.GetQuestList();
        questList.AddQuest(this);

    }

    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            if (!inventory.HasItem(Base.RequiredItem))
            {
                return false;
            }
        }

        return true;
    }

 

}

public enum QuestStatus { None, Started, Completed}