using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceBox : SelectionUI<TextSlot>
{
    [SerializeField] private TextSlot choiceTextPrefab;

    //private bool choiceSelected = false;
    //private bool cancel = false;

    private List<TextSlot> choiceTexts;
    //private int currentChoice;
    //private bool _cancelX;

    public void ShowChoices(List<string> choices, Action<int> onChoiceSelected=null, bool cancelX=true)
    {
        //choiceSelected = false;
        //cancel = false;
        //currentChoice = 0;
        //_cancelX = cancelX;
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

        //yield return new WaitUntil(() => choiceSelected == true || cancel == true);
        //currentChoice = cancel ? -1 : currentChoice;
        //onChoiceSelected?.Invoke(currentChoice);
    }


    public void CloseBox()
    {
        gameObject.SetActive(false);
        DialogueManager.Instance.CloseDialog();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    ++currentChoice;
        //}
        //else if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    --currentChoice;
        //}

        //currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
        //for (int i = 0; i < choiceTexts.Count; ++i)
        //{
        //    choiceTexts[i].SetSelected(i == currentChoice);
        //}

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    AudioManager.Instance.PlaySE(SFX.CONFIRM);
        //    choiceSelected = true;
        //}

        //if (_cancelX)
        //{
        //    if (Input.GetKeyDown(KeyCode.X))
        //    {
        //        AudioManager.Instance.PlaySE(SFX.CANCEL);
        //        cancel = true;
        //        //GameManager.Instance.StartFreeRoamState();
        //    }
        //}
    }
}
