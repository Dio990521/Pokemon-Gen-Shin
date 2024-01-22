using PokeGenshinUtils.SelectionUI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleMoveSelectionUI : SelectionUI<TextSlot>
{
    [SerializeField] private List<TextSlot> _moveTexts;
    [SerializeField] private Text _typeText;
    [SerializeField] private Text _ppText;

    private List<Move> _moves;

    private void Start()
    {
        SetSelectionSettings(SelectionType.Grid, 2);
    }

    public void SetMoves(List<Move> moves)
    {
        _moves = moves;
        SetItems(_moveTexts.Take(moves.Count).ToList());
        for (int i = 0; i < _moveTexts.Count; ++i)
        {
            if (i < moves.Count)
            {
                _moveTexts[i].SetText(moves[i].MoveBase.MoveName);
            }
            else
            {
                _moveTexts[i].SetText("-");
            }
        }
    }

    public override void UpdateSelectionUI()
    {
        base.UpdateSelectionUI();
        var move = _moves[selectedItem];
        _ppText.text = $"PP {move.PP} / {move.MoveBase.PP}";
        _typeText.text = $"属性/{move.MoveBase.Type}";
        if (move.MoveBase.AlwaysHits)
        {
            _typeText.text += "、必中";
        }
        if (move.MoveBase.Priority > 0)
        {
            _typeText.text += "、先手";
        }

        if (move.PP == 0)
        {
            _ppText.color = Color.red;
        }
        else
        {
            _ppText.color = Color.black;
        }
    }
}
