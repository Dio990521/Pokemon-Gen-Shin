using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private ChoiceBox choiceBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private int lettersPerSecond;

    public event Action OnShowDialogue;
    public event Action OnDialogueFinished;

    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialogueText(string text, bool waitForInput=true, bool autoClose=true,
        List<string> choices=null, Action<int> onChoiceSelected=null)
    {
        OnShowDialogue?.Invoke();
        IsShowing = true;
        dialogueBox.SetActive(true);

        yield return TypeDialogue(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }

        OnDialogueFinished?.Invoke();

    }

    public void CloseDialog()
    {
        dialogueBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialogue(Dialogue dialogue, List<string> choices=null, Action<int> onChoiceSelected=null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialogue?.Invoke();
        IsShowing = true;

        dialogueBox.SetActive(true);

        foreach (var line in dialogue.Lines)
        {
            yield return TypeDialogue(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            if (line != dialogue.Lines[dialogue.Lines.Count - 1])
            {
                AudioManager.Instance.PlaySE(SFX.CONFIRM);
            }
            
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogueBox.SetActive(false);
        IsShowing = false;
        OnDialogueFinished?.Invoke();


    }

    // Show dialogue texts by one character after one character
    public IEnumerator TypeDialogue(string line)
    {
        dialogueText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void HandleUpdate()
    {

    }
}
