using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] private ChoiceText choiceTextPrefab;

    private bool choiceSelected = false;
    private bool cancel = false;

    private List<ChoiceText> choiceTexts = new List<ChoiceText>();
    private int currentChoice;

    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        cancel = false;
        currentChoice = 0;
        gameObject.SetActive(true);

        // Delete existing choices
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        choiceTexts = new List<ChoiceText>();
        foreach (var choice in choices)
        {
            var choiceTextObject = Instantiate(choiceTextPrefab, transform);
            choiceTextObject.TextField.text = choice;
            choiceTexts.Add(choiceTextObject);
        }

        yield return new WaitUntil(() => choiceSelected == true || cancel == true);
        currentChoice = cancel ? -1 : currentChoice;
        onChoiceSelected?.Invoke(currentChoice);
        CloseBox();
    }

    private void CloseBox()
    {
        gameObject.SetActive(false);
        DialogueManager.Instance.CloseDialog();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentChoice;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentChoice;
        }

        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
        for (int i = 0; i < choiceTexts.Count; ++i)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            choiceSelected = true;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            cancel = true;
            GameManager.Instance.StartFreeRoamState();
        }
    }
}
