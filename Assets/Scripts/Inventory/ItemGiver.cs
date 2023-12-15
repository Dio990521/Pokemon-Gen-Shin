using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count = 1;
    [SerializeField] private Dialogue dialogue;

    [SerializeField] private CutsceneName _afterCutscene;

    private bool used = false;

    public IEnumerator GiveItem(PlayerController player)
    {
        if (_afterCutscene != CutsceneName.None &&
            !GameKeyManager.Instance.GetBoolValue(_afterCutscene.ToString())) yield break;
        yield return DialogueManager.Instance.ShowDialogue(dialogue);

        if (!used)
        {
            player.GetComponent<Inventory>().AddItem(item, count);

            used = true;
            string dialogueText = $"你获得了{item.ItemName}！";
            if (count > 1)
            {
                dialogueText = $"你获得了{item.ItemName}x{count}！";
            }
            yield return DialogueManager.Instance.ShowDialogueText(dialogueText);
        }

    }

    public bool CanBeGiven()
    {
        var cutsceneActivate = _afterCutscene == CutsceneName.None || _afterCutscene != CutsceneName.None &&
            GameKeyManager.Instance.GetBoolValue(_afterCutscene.ToString());
        return item != null && cutsceneActivate && !used && count > 0;
    }

    public object CaptureState()
    {
        return used;
    }

    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}
