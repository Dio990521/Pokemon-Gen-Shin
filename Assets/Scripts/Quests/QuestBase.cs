using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quests/Create a new quest")]
public class QuestBase : ScriptableObject
{
    [SerializeField] private string questName;
    [SerializeField] private string description;

    [SerializeField] private Dialogue startDialogue;
    [SerializeField] private Dialogue inProgressDialogue;
    [SerializeField] private Dialogue completedDialogue;

    [SerializeField] private ItemBase requiredItem;
    [SerializeField] private ItemBase rewardItem;

    public string QuestName => questName;
    public string Description => description; 
    public Dialogue StartDialogue => startDialogue;
    public Dialogue InProgressDialogue => inProgressDialogue?.Lines?.Count > 0 ? inProgressDialogue : startDialogue;
    public ItemBase RewardItem => rewardItem;
    public ItemBase RequiredItem => requiredItem;
    public Dialogue CompletedDialogue => completedDialogue;

}
