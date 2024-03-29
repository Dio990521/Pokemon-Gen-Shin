using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private GameObject nameBox;
    [SerializeField] private ChoiceBox choiceBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text nameText;
    [SerializeField] private int lettersPerSecond;

    public event Action OnShowDialogue;
    public event Action OnDialogueFinished;

    public bool IsShowing { get; private set; }

    public IEnumerator ShowDialogueText(string text, bool waitForInput=true, bool autoClose=true, string name="")
    {
        OnShowDialogue?.Invoke();
        IsShowing = true;
        dialogueBox.SetActive(true);
        if (name.Length > 0)
        {
            nameText.text = name;
            nameBox.SetActive(true);
        }

        yield return TypeDialogue(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
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
        nameBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialogue?.Invoke();
        IsShowing = true;

        dialogueBox.SetActive(true);
        if (dialogue.SpeakerName.Length > 0)
        {
            nameText.text = dialogue.SpeakerName;
            nameBox.SetActive(true);
        }

        foreach (var line in dialogue.Lines)
        {
            yield return TypeDialogue(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
        }

        dialogueBox.SetActive(false);
        nameBox.SetActive(false);
        IsShowing = false;
        OnDialogueFinished?.Invoke();


    }

    public IEnumerator ShowDialogue(Dialogue dialogue, SFX se, int playAt)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialogue?.Invoke();
        IsShowing = true;

        dialogueBox.SetActive(true);
        if (dialogue.SpeakerName.Length > 0)
        {
            nameText.text = dialogue.SpeakerName;
            nameBox.SetActive(true);
        }

        for (int i = 0; i < dialogue.Lines.Count; i++)
        {
            if (i == playAt)
            {
                AudioManager.Instance.PlaySE(se, true);
            }
            yield return TypeDialogue(dialogue.Lines[i]);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
        }

        dialogueBox.SetActive(false);
        nameBox.SetActive(false);
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

}
