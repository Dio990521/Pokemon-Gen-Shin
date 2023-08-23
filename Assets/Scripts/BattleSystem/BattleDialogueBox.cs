using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] private int lettersPerSecond;
    [SerializeField] private Text dialogueText;

    [SerializeField] private GameObject actionSeletor;
    [SerializeField] private GameObject moveSelector;
    [SerializeField] private GameObject moveDetails;
    [SerializeField] private GameObject choiceBox;

    [SerializeField] private List<Text> actionTexts;
    [SerializeField] private Image actionSelectorImage;
    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private Image moveSelectorImage;

    [SerializeField] private Text ppText;
    [SerializeField] private Text typeText;
    [SerializeField] private Image yesSelector;
    [SerializeField] private Image noSelector;

    public bool IsChoiceBoxEnabled => choiceBox.activeSelf;

    public void SetDialogue(string dialogue)
    {
        dialogueText.text = dialogue;
    }

    // Show dialogue texts by one character after one character
    public IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";
        foreach (var letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogueText(bool isEnable)
    {
        dialogueText.enabled = isEnable;
    }

    public void EnableActionSelector(bool isEnable)
    {
        actionSeletor.SetActive(isEnable);
    }

    public void EnableMoveSelector(bool isEnable)
    {
        moveSelector.SetActive(isEnable);
        moveDetails.SetActive(isEnable);
    }

    public void EnableChoiceBox(bool isEnable)
    {
        choiceBox.SetActive(isEnable);
    }

    // Update the cursor when the player is choosing an action
    public void UpdateActionSelection(int selectedAction)
    {
        switch (selectedAction)
        {
            case 0:
                actionSelectorImage.transform.localPosition = new Vector2(216f, 23f);
                break;
            case 1:
                actionSelectorImage.transform.localPosition = new Vector2(366f, 23f);
                break;
            case 2:
                actionSelectorImage.transform.localPosition = new Vector2(216f, -22f);
                break;
            case 3:
                actionSelectorImage.transform.localPosition = new Vector2(366f, -22f);
                break;
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesSelector.gameObject.SetActive(true);
            noSelector.gameObject.SetActive(false);
        }
        else
        {
            yesSelector.gameObject.SetActive(false);
            noSelector.gameObject.SetActive(true);
        }
    }

    // Update the cursor and the move information when the player is choosing a move
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        switch (selectedMove)
        {
            case 0:
                moveSelectorImage.transform.localPosition = new Vector2(-430f, 18f);
                break;
            case 1:
                moveSelectorImage.transform.localPosition = new Vector2(-155f, 18f);
                break;
            case 2:
                moveSelectorImage.transform.localPosition = new Vector2(-430f, -20f);
                break;
            case 3:
                moveSelectorImage.transform.localPosition = new Vector2(-155f, -20f);
                break;
        }

        ppText.text = $"PP {move.PP} / {move.MoveBase.PP}";
        typeText.text = $" Ù–‘/{move.MoveBase.Type}";

        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else
        {
            ppText.color = Color.black;
        }

    }

    // Set the move name to '-' if moves are not enough to fill the move box
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count;i++)
        {
            moveTexts[i].text = i < moves.Count ? moves[i].MoveBase.MoveName : "-";
        }
    }
}
