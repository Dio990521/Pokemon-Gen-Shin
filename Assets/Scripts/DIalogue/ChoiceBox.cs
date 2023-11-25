using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceBox : SelectionUI<TextSlot>
{
    [SerializeField] private TextSlot choiceTextPrefab;

    private List<TextSlot> _choiceTexts;
    private List<string> _choices;
    public readonly static int MaxChoice = 7;
    public bool IsFirstPage = true;

    public void ShowChoices(List<string> choices, Action<int> onChoiceSelected=null, bool cancelX=true)
    {
        _choices = choices;
        _choiceTexts = new List<TextSlot>();
        gameObject.SetActive(true);

        // Delete existing choices
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        IsFirstPage = true;
        ChangeChoices(IsFirstPage);

    }

    public void ChangeChoices(bool isFirstPage)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        _choiceTexts.Clear();
        IsFirstPage = isFirstPage;
        if (isFirstPage)
        {
            var firstPageCount = Mathf.Clamp(_choices.Count, 0, MaxChoice);
            for (int i = 0; i < firstPageCount; i++)
            {
                _choiceTexts.Add(CreateTextSlot(_choices[i]));
            }
            if (_choices.Count > MaxChoice)
            {
                _choiceTexts.Add(CreateTextSlot("下一页"));
            }
        }
        else
        {
            for (int i = MaxChoice; i < _choices.Count; i++)
            {
                _choiceTexts.Add(CreateTextSlot(_choices[i]));
            }
            _choiceTexts.Add(CreateTextSlot("上一页"));
        }
        SetItems(_choiceTexts);
    }

    private TextSlot CreateTextSlot(string text)
    {
        TextSlot choiceTextObject = Instantiate(choiceTextPrefab, transform);
        choiceTextObject.SetText(text);
        choiceTextObject.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(choiceTextObject.GetTextWidth(), 60f);
        choiceTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(choiceTextObject.GetTextWidth() + 100f, 60f);
        choiceTextObject.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleLeft;
        return choiceTextObject;
    }


    public void CloseBox()
    {
        gameObject.SetActive(false);
        DialogueManager.Instance.CloseDialog();
    }

}
