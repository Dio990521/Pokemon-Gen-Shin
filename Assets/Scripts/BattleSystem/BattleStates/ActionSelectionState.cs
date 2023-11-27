using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] private ActionSelectionUI _selectionUI;

    public static ActionSelectionState I { get; private set; }
    public ActionSelectionUI SelectionUI { get => _selectionUI; set => _selectionUI = value; }

    private BattleSystem _battleSystem;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(BattleSystem owner)
    {
        _battleSystem = owner;
        SelectionUI.gameObject.SetActive(true);
        SelectionUI.ResetSelection();
        SelectionUI.OnSelected += OnActionSelected;
        RunTurnState.I.SkipEnemyTurn = false;
        StartCoroutine(_battleSystem.DialogueBox.TypeDialogue($"想要\n{_battleSystem.PlayerUnit.pokemon.PokemonBase.PokemonName}做什么？"));
    }

    public override void Execute()
    {
        SelectionUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        SelectionUI.gameObject.SetActive(false);
        SelectionUI.OnSelected -= OnActionSelected;
    }

    private void OnActionSelected(int selection)
    {
        //AudioManager.Instance.PlaySE(SFX.CONFIRM);
        if (selection == 0)
        {
            // Fight
            _battleSystem.SelectedAction = BattleAction.Move;
            MoveSelectionState.I.Moves = _battleSystem.PlayerUnit.pokemon.Moves;
            _battleSystem.StateMachine.ChangeState(MoveSelectionState.I);
        }
        else if (selection == 1)
        {
            // Bag
            StartCoroutine(GoToInventoryState());
        }
        else if (selection == 2)
        {
            // Pokemon
            StartCoroutine(GoToPartyState());
        }
        else if (selection == 3)
        {
            // Run
            _battleSystem.SelectedAction = BattleAction.Run;
            _battleSystem.StateMachine.ChangeState(RunTurnState.I);
        }
    }

    private IEnumerator GoToPartyState()
    {
        yield return GameManager.Instance.StateMachine.PushAndWait(PartyState.I);

        var selectedPokemon = PartyState.I.SelectedPokemon;
        if (selectedPokemon != null)
        {
            _battleSystem.SelectedAction = BattleAction.SwitchPokemon;
            _battleSystem.SelectedPokemon = selectedPokemon;
            _battleSystem.StateMachine.ChangeState(RunTurnState.I);
        }
    }

    private IEnumerator GoToInventoryState()
    {
        yield return GameManager.Instance.StateMachine.PushAndWait(InventoryState.I);
        var selectedItem = InventoryState.I.SelectedItem;
        if (selectedItem != null)
        {
            _battleSystem.SelectedAction = BattleAction.UseItem;
            _battleSystem.SelectedItem = selectedItem;
            _battleSystem.StateMachine.ChangeState(RunTurnState.I);
            
        }
    }
}
