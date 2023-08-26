using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PartyState : State<GameManager>
{
    [SerializeField] private PartyScreen _partyScreen;

    public Pokemon SelectedPokemon { get; private set; }
    public static PartyState I { get; private set; }

    private GameManager _gameManager;
    private bool _swaping;
    private int _swapIndex;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        //AudioManager.Instance.PlaySE(SFX.CONFIRM);
        _swaping = false;
        _gameManager = owner;
        SelectedPokemon = null;
        _partyScreen.gameObject.SetActive(true);
        GameManager.Instance.PartyScreen.SetMessageText("选择一个宝可梦。");
        _partyScreen.OnSelected += OnPokemonSelected;
        _partyScreen.OnBack += OnBack;
    }

    public override void Execute()
    {
        _partyScreen.HandleUpdate();
    }

    public override void Exit(bool sfx)
    {
        if (sfx)
        {
            //AudioManager.Instance.PlaySE(SFX.CANCEL);
        }
        _partyScreen.ResetSelection();
        _partyScreen.gameObject.SetActive(false);
        _partyScreen.OnSelected -= OnPokemonSelected;
        _partyScreen.OnBack -= OnBack;
    }

    private void OnPokemonSelected(int selection)
    {
        if (selection == _partyScreen.Pokemons.Count)
        {
            OnBack();
        }
        else
        {
            if (_swaping)
            {
                _partyScreen.SwitchPokemonSlot(_swapIndex, selection);
                _swaping = false;
                return;
            }
            //AudioManager.Instance.PlaySE(SFX.CONFIRM);
            SelectedPokemon = _partyScreen.SelectedMember;
            var prevState = _gameManager.StateMachine.GetPrevState();
            if (prevState == InventoryState.I)
            {
                StartCoroutine(GoToUseItemState());
            }
            else if (prevState == BattleState.I)
            {
                var battleState = prevState as BattleState;
                if (SelectedPokemon.Hp <= 0)
                {
                    _partyScreen.SetMessageText("它摸了，换一个吧！");
                    return;
                }
                if (SelectedPokemon == battleState.BattleSystem.PlayerUnit.pokemon)
                {
                    _partyScreen.SetMessageText("它已经上场了，换一个吧！");
                    return;
                }

                _gameManager.StateMachine.Pop();
            }
            else
            {
                StartCoroutine(GoToPartyMenuState(selection));
            }

        }
        
    }
    
    private IEnumerator GoToPartyMenuState(int selection)
    {
        PartyMenuState.I.SelectedPokemon = SelectedPokemon;
        PartyMenuState.I.Selection = selection;
        yield return _gameManager.StateMachine.PushAndWait(PartyMenuState.I);

        int choice = PartyMenuState.I.Selection;
        if (choice == 0)
        {
            // Pokemon Info
            PokemonInfoState.I.SelectedPokemon = SelectedPokemon;
            _gameManager.StateMachine.Push(PokemonInfoState.I);
        }
        else if (choice == 1)
        {
            // Swap Pokemon
            _swapIndex = selection;
            GameManager.Instance.PartyScreen.SetMessageText("选择要交换的宝可梦。");
            _swaping = true;
        }
    }


    private IEnumerator GoToUseItemState()
    {
        yield return _gameManager.StateMachine.PushAndWait(UseItemState.I);
        _gameManager.StateMachine.Pop(false);
    }

    private void OnBack()
    {
        SelectedPokemon = null;
        var prevState = _gameManager.StateMachine.GetPrevState();
        if (prevState == BattleState.I)
        {
            var battleState = prevState as BattleState;

            if (battleState.BattleSystem.PlayerUnit.pokemon.Hp <= 0)
            {
                _partyScreen.SetMessageText("必须要选择一个宝可梦！");
                return;
            }
        }
        _gameManager.StateMachine.Pop();
    }
}
