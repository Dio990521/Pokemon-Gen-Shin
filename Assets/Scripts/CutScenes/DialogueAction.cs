using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueAction : CutsceneAction
{
    [SerializeField] private Dialogue dialogue;

    public override IEnumerator Play()
    {
        if (dialogue.AfterCutsceneActivate == CutsceneName.None ||
            GameKeyManager.Instance.GetBoolValue(dialogue.AfterCutsceneActivate.ToString()))
            yield return DialogueManager.Instance.ShowDialogue(dialogue);
    }
}
