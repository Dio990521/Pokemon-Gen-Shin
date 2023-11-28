using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChecker : MonoBehaviour, ISavable
{
    [SerializeField] private List<ItemBase> items;
    [SerializeField] private Dialogue failDialogue;
    [SerializeField] private Dialogue successDialogue;
    [SerializeField] private CutsceneName cutsceneName;
    [SerializeField] private bool _needRemoveItem;

    [HideInInspector]
    public bool Used = false;

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

        if (!Used && count == items.Count)
        {
            Used = true;
            if (_needRemoveItem)
            {
                foreach (var item in items)
                {
                    Inventory.GetInventory().RemoveItem(item);
                }
            }
            GameKeyManager.Instance.SetBoolValue(cutsceneName.ToString(), true);
            yield return DialogueManager.Instance.ShowDialogue(successDialogue);
        }
        else if (!Used && count != items.Count)
        {
            yield return DialogueManager.Instance.ShowDialogue(failDialogue);
        }
    }

    public object CaptureState()
    {
        return Used;
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;
    }
}
