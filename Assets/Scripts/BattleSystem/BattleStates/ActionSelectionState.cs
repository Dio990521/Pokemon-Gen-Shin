using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] private ActionSelectionUI _selectionUI;

    public static ActionSelectionState I { get; private set; }

    private BattleSystem _battleSystem;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(BattleSystem owner)
    {
        _battleSystem = owner;
        _selectionUI.gameObject.SetActive(true);
        _selectionUI.OnSelected += OnActionSelected;

        StartCoroutine(_battleSystem.DialogueBox.TypeDialogue($"想要\n{_battleSystem.PlayerUnit.pokemon.PokemonBase.PokemonName}做什么？"));
    }

    public override void Execute()
    {
        _selectionUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        _selectionUI.gameObject.SetActive(false);
        _selectionUI.OnSelected -= OnActionSelected;
    }

    private void OnActionSelected(int selection)
    {
        AudioManager.Instance.PlaySE(SFX.CONFIRM);
        if (selection == 0)
        {
            // Fight
            MoveSelectionState.I.Moves = _battleSystem.PlayerUnit.pokemon.Moves;
            _battleSystem.StateMachine.ChangeState(MoveSelectionState.I);
        }
    }
}
