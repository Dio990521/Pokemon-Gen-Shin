using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUi : MonoBehaviour
{
    [SerializeField] private List<Text> moveTexts;
    [SerializeField] private Image moveForgetCursor;
    [SerializeField] private float cursorOffset = 61.536f;
    [SerializeField] private float initYPos = -65.66f;

    private int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = newMove.MoveName;
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ++currentSelection;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            --currentSelection;
        }

        currentSelection = Mathf.Clamp(currentSelection, 0, PokemonBase.MaxNumOfMoves);
        moveForgetCursor.transform.localPosition = new Vector3(moveForgetCursor.transform.localPosition.x, initYPos - currentSelection * cursorOffset, 0f);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.instance.PlaySE(SFX.CONFIRM);
            onSelected?.Invoke(currentSelection);
        }
    }

}
