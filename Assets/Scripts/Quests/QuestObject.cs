using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{

    [SerializeField] private QuestBase questToCheck;
    [SerializeField] private ObjectActions onStart;
    [SerializeField] private ObjectActions onComplete;

    private QuestList questList;

    private void Awake()
    {
        questList = QuestList.GetQuestList();
        questList.OnUpdated += UpdateObjectStatus;
    }

    private void Start()
    {
        UpdateObjectStatus();
    }

    private void OnDestroy()
    {
        questList.OnUpdated -= UpdateObjectStatus;
    }

    public void UpdateObjectStatus()
    {
        if (onStart != ObjectActions.DoNothing && questList.IsStarted(questToCheck.QuestName))
        {
            foreach(Transform child in transform)
            {
                if (onStart == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);
                }
                else if (onStart == ObjectActions.Disable)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        if (onComplete != ObjectActions.DoNothing && questList.IsCompleted(questToCheck.QuestName))
        {
            foreach (Transform child in transform)
            {
                if (onComplete == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);
                }
                else if (onComplete == ObjectActions.Disable)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

}

public enum ObjectActions { DoNothing, Enable, Disable }
