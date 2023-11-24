using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public int Selection;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _swaping = false;
        _gameManager = owner;
        SelectedPokemon = null;
        _partyScreen.gameObject.SetActive(true);
        _partyScreen.Party.PartyUpdated();
        GameManager.Instance.PartyScreen.SetMessageText("ѡ��һ�������Ρ�");
        _partyScreen.OnSelected += OnPokemonSelected;
        _partyScreen.OnBack += OnBack;
    }

    public override void Execute()
    {
        _partyScreen.HandleUpdate();
    }

    public override void Exit(bool sfx)
    {
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
            Selection = selection;
            SelectedPokemon = _partyScreen.SelectedMember;
            var prevState = _gameManager.StateMachine.GetPrevState();
            if (prevState == InventoryState.I)
            {
                StartCoroutine(GoToUseItemState());
            }
            else if (prevState == BattleState.I)
            {
                StartCoroutine(GoToPartyMenuState(selection, true));
            }
            else if (prevState == PCMenuState.I)
            {
                _gameManager.StateMachine.Pop();
            }
            else
            {
                StartCoroutine(GoToPartyMenuState(selection));
            }

        }
        
    }
    
    private IEnumerator GoToPartyMenuState(int selection, bool inBattle=false)
    {
        PartyMenuState.I.SelectedPokemon = SelectedPokemon;
        PartyMenuState.I.Selection = selection;
        PartyMenuState.I.InBattle = inBattle;
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
            if (!inBattle)
            {
                // Swap Pokemon
                _swapIndex = selection;
                GameManager.Instance.PartyScreen.SetMessageText("ѡ��Ҫ��������һ�������Ρ�");
                _swaping = true;
            }
            else
            {
                if (SelectedPokemon.Hp <= 0)
                {
                    _partyScreen.SetMessageText("�����ˣ���һ���ɣ�");
                    yield break;
                }
                if (SelectedPokemon == BattleState.I.BattleSystem.PlayerUnit.pokemon)
                {
                    _partyScreen.SetMessageText("���Ѿ��ϳ��ˣ���һ���ɣ�");
                    yield break;
                }
                _gameManager.StateMachine.Pop();

            }
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
                _partyScreen.SetMessageText("����Ҫѡ��һ�������Σ�");
                return;
            }
        }
        _gameManager.StateMachine.Pop();
    }
}
