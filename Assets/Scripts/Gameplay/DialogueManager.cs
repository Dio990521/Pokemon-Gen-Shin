using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Text dialogueText;
    [SerializeField] private int lettersPerSecond;

    public event Action OnShowDialogue;
    public event Action OnCloseDialogue;

    public bool IsShowing { get; private set; }

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        
    }

    public IEnumerator ShowDialogueText(string text, bool waitForInput=true, bool autoClose=true)
    {
        OnShowDialogue?.Invoke();
        IsShowing = true;
        dialogueBox.SetActive(true);

        yield return TypeDialogue(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (autoClose)
        {
            CloseDialog();
        }

    }

    public void CloseDialog()
    {
        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();
    }

    public IEnumerator ShowDialogue(Dialogue dialogue)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialogue?.Invoke();
        IsShowing = true;

        dialogueBox.SetActive(true);

        foreach (var line in dialogue.Lines)
        {
            yield return TypeDialogue(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        dialogueBox.SetActive(false);
        IsShowing = false;
        OnCloseDialogue?.Invoke();


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
