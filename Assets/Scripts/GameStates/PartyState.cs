using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyState : State<GameManager>
{
    [SerializeField] private PartyScreen _partyScreen;

    public Pokemon SelectedPokemon { get; private set; }
    public static PartyState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        //AudioManager.Instance.PlaySE(SFX.CONFIRM);
        _gameManager = owner;
        SelectedPokemon = null;
        _partyScreen.gameObject.SetActive(true);
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
                    _partyScreen.SetMessageText("�����ˣ���һ���ɣ�");
                    return;
                }
                if (SelectedPokemon == battleState.BattleSystem.PlayerUnit.pokemon)
                {
                    _partyScreen.SetMessageText("���Ѿ��ϳ��ˣ���һ���ɣ�");
                    return;
                }

                _gameManager.StateMachine.Pop();
            }
            else
            {
                // Summary screen
                print($"open summary screen {selection}");
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
