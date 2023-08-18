using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToForgetState : State<GameManager>
{
    [SerializeField] private MoveSelectionUI _moveSelectionUI;

    public List<MoveBase> CurrentMoves { get; set; }
    public MoveBase NewMove { get; set; }
    public int Selection { get; set; }

    public static MoveToForgetState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        Selection = 0;
        _moveSelectionUI.gameObject.SetActive(true);
        _moveSelectionUI.SetMoveData(CurrentMoves, NewMove);
        _moveSelectionUI.OnSelected += OnMoveSelected;
        _moveSelectionUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        _moveSelectionUI.HandleUpdate();
    }

    public override void Exit(bool sfx)
    {
        _moveSelectionUI.gameObject.SetActive(false);
        _moveSelectionUI.OnSelected -= OnMoveSelected;
        _moveSelectionUI.OnBack -= OnBack;
    }

    private void OnMoveSelected(int selection)
    {
        Selection = selection;
        _gameManager.StateMachine.Pop();
    }

    private void OnBack()
    {
        Selection = -1;
        _gameManager.StateMachine.Pop();
    }
}
