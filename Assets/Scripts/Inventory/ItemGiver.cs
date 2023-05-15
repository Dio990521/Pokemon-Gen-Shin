using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count = 1;
    [SerializeField] private Dialogue dialogue;

    private bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        yield return DialogueManager.Instance.ShowDialogue(dialogue);
        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;

        string dialogueText = $"你获得了{item.ItemName}！";
        if (count > 1)
        {
            dialogueText = $"你获得了{item.ItemName}x{count}！";
        }
        yield return DialogueManager.Instance.ShowDialogueText(dialogueText);
    }

    public bool CanBeGiven()
    {
        return item != null && !used && count > 0;
    }
}
