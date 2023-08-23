using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State<BattleSystem>
{
    [SerializeField] private BattleMoveSelectionUI _selectionUI;
    [SerializeField] private GameObject _moveDetailsUI;

    public List<Move> Moves { get; set; }

    public static MoveSelectionState I { get; private set; }

    private BattleSystem _battleSystem;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(BattleSystem owner)
    {
        _battleSystem = owner;
        _selectionUI.SetMoves(Moves);
        _selectionUI.ResetSelection();
        _selectionUI.gameObject.SetActive(true);
        _selectionUI.OnSelected += OnMoveSelection;
        _selectionUI.OnBack += OnBack;

        _moveDetailsUI.SetActive(true);
        _battleSystem.DialogueBox.EnableDialogueText(false);
    }

    public override void Execute()
    {
        _selectionUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        _selectionUI.gameObject.SetActive(false);
        _selectionUI.OnSelected -= OnMoveSelection;
        _selectionUI.OnBack -= OnBack;

        _moveDetailsUI.SetActive(false);
        _battleSystem.DialogueBox.EnableDialogueText(true);
    }

    private void OnMoveSelection(int selection)
    {
        AudioManager.Instance.PlaySE(SFX.CONFIRM);
        _battleSystem.SelectedMove = selection;
        _battleSystem.StateMachine.ChangeState(RunTurnState.I);
    }

    private void OnBack()
    {
        _battleSystem.StateMachine.ChangeState(ActionSelectionState.I);
    }
}
