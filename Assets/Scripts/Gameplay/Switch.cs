using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, InteractableObject, ISavable
{
    public bool Used { get; set; } = false;

    public Action OnPuzzleChange;

    public object CaptureState()
    {
        return Used;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!Used)
        {
            Used = true;
            yield return DialogueManager.Instance.ShowDialogueText("你按下了开关！");
            OnPuzzleChange?.Invoke();
        }
    }

    public void RestoreState(object state)
    {
        Used = (bool)state;
    }


}
