using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceBox : SelectionUI<TextSlot>
{
    [SerializeField] private TextSlot choiceTextPrefab;

    private List<TextSlot> choiceTexts;

    public void ShowChoices(List<string> choices, Action<int> onChoiceSelected=null, bool cancelX=true)
    {

        gameObject.SetActive(true);

        // Delete existing choices
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        choiceTexts = new List<TextSlot>();
        foreach (var choice in choices)
        {
            TextSlot choiceTextObject = Instantiate(choiceTextPrefab, transform);
            choiceTextObject.SetText(choice);
            choiceTextObject.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(choiceTextObject.GetTextWidth(), 60f);
            choiceTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(choiceTextObject.GetTextWidth() + 100f, 60f);
            choiceTextObject.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
            choiceTexts.Add(choiceTextObject);
        }

        SetItems(choiceTexts);

    }


    public void CloseBox()
    {
        gameObject.SetActive(false);
        DialogueManager.Instance.CloseDialog();
    }

}
