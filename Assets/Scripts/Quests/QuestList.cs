using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestList : MonoBehaviour, ISavable
{
    private List<Quest> quests = new List<Quest>();

    public event Action OnUpdated;

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest))
        {
            quests.Add(quest);
        }

        OnUpdated?.Invoke();
    }

    public bool IsStarted(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.QuestName == questName)?.Status;
        return questStatus == QuestStatus.Started || questStatus == QuestStatus.Completed;
    }

    public bool IsCompleted(string questName)
    {
        var questStatus = quests.FirstOrDefault(q => q.Base.QuestName == questName)?.Status;
        return questStatus == QuestStatus.Completed;
    }

    public static QuestList GetQuestList()
    {
        return FindObjectOfType<PlayerController>().GetComponent<QuestList>();
    }

    public object CaptureState()
    {
        return quests.Select(q => q.GetSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;
        if (saveData != null)
        {
            quests = saveData.Select(q => new Quest(q)).ToList();
            OnUpdated?.Invoke();
        }
    }
}
