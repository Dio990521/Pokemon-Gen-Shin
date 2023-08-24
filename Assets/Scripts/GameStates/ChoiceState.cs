using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceState : State<GameManager>
{

    [SerializeField] private ChoiceBox _choiceBox;
    public List<string> Choices { get; set; }
    public int Selection { get; set; }
    public bool AllowCancel { get; set; } = true;

    public static ChoiceState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public void Clear()
    {
        Choices.Clear();
        Selection = -1;
        AllowCancel = true;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _choiceBox.ShowChoices(Choices);
        _choiceBox.ResetSelection();
        _choiceBox.OnSelected += OnChoiceSelected;
        _choiceBox.OnBack += OnBack;
    }

    public override void Execute()
    {
        _choiceBox.HandleUpdate(AllowCancel);
    }

    public override void Exit(bool sfx = true)
    {
        _choiceBox.OnSelected -= OnChoiceSelected;
        _choiceBox.OnBack -= OnBack;
        _choiceBox.CloseBox();
        Clear();
    }

    private void OnChoiceSelected(int selection)
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
