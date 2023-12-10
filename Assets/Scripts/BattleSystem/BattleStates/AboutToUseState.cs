using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutToUseState : State<BattleSystem>
{
    public Pokemon NewPokemon { get; set; }

    private bool aboutToUseChoice;

    public static AboutToUseState I { get; private set; }

    private BattleSystem _battleSystem;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(BattleSystem owner)
    {
        _battleSystem = owner;
        
        StartCoroutine(StartState());
    }

    public override void Execute()
    {
        if (!_battleSystem.DialogueBox.IsChoiceBoxEnabled)
        {
            return;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            AudioManager.Instance.PlaySE(SFX.CURSOR);
            aboutToUseChoice = !aboutToUseChoice;
        }

        _battleSystem.DialogueBox.UpdateChoiceBox(aboutToUseChoice);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            _battleSystem.DialogueBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                // Yes
                StartCoroutine(SwitchAndContinueBattle());
            }
            else
            {
                // No
                StartCoroutine(ContinueBattle());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            _battleSystem.DialogueBox.EnableChoiceBox(false);
            StartCoroutine(ContinueBattle());
        }
    }

    private IEnumerator StartState()
    {
        yield return _battleSystem.DialogueBox.TypeDialogue($"{_battleSystem.Trainer.TrainerName}想要让{NewPokemon.PokemonBase.PokemonName}上场！\n是否要更换当前出战宝可梦？");
        _battleSystem.DialogueBox.EnableChoiceBox(true);
    }

    private IEnumerator ContinueBattle()
    {
        yield return _battleSystem.SendNextTrainerPokemon();
        _battleSystem.StateMachine.Pop();
    }

    private IEnumerator SwitchAndContinueBattle()
    {
        yield return GameManager.Instance.StateMachine.PushAndWait(PartyState.I);
        var selectedPokemon = PartyState.I.SelectedPokemon;
        if (selectedPokemon != null)
        {
            yield return _battleSystem.SwitchPokemon(selectedPokemon);
        }

        yield return ContinueBattle();
    }
}
