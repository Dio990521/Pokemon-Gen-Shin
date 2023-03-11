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

    private int currentLine = 0;
    private Dialogue dialogue;
    private bool isTyping;

    public bool IsShowing { get; private set; }

    public static DialogueManager Instance { get; private set; }

    private Action OnDialogFinished;

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

    public IEnumerator ShowDialogue(Dialogue dialogue, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialogue?.Invoke();
        IsShowing = true;
        this.dialogue = dialogue;
        OnDialogFinished = onFinished;
        dialogueBox.SetActive(true);
        StartCoroutine(TypeDialogue(dialogue.Lines[0]));
    }

    // Show dialogue texts by one character after one character
    public IEnumerator TypeDialogue(string line)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;

    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialogue.Lines.Count)
            {
                StartCoroutine(TypeDialogue(dialogue.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogueBox.SetActive(false);
                OnDialogFinished?.Invoke();
                OnCloseDialogue?.Invoke();
            }
        }
    }
}
