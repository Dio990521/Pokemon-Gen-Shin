using PokeGenshinUtils.SelectionUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ForgetMoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] private List<Text> moveTexts;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].MoveName;
        }

        moveTexts[currentMoves.Count].text = newMove.MoveName;

        SetItems(GetComponentsInChildren<TextSlot>().ToList());
    }

}
