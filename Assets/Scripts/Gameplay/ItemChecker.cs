using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : MonoBehaviour, ISavable
{
    [SerializeField] private List<ItemBase> items;
    [SerializeField] private Dialogue failDialogue;
    [SerializeField] private Dialogue successDialogue;
    [SerializeField] private CutsceneName cutsceneName;


    private bool used = false;

    public IEnumerator CheckItem(PlayerController player)
    {
        var count = 0;
        foreach (var item in items)
        {
            if (player.GetComponent<Inventory>().HasItem(item))
            {
                count++;
            }
        }

        if (!used && count == items.Count)
        {
            used = true;
            GameKeyManager.Instance.SetBoolValue(cutsceneName.ToString(), true);
            yield return DialogueManager.Instance.ShowDialogue(successDialogue);
        }
        else if (!used && count != items.Count)
        {
            yield return DialogueManager.Instance.ShowDialogue(failDialogue);
        }
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
