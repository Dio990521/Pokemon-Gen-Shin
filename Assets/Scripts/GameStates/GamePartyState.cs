using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePartyState : State<GameManager>
{
    [SerializeField] private PartyScreen partyScreen;
    public static GamePartyState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        AudioManager.Instance.PlaySE(SFX.CONFIRM);
        _gameManager = owner;
        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;
    }

    public override void Execute()
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit(bool sfx)
    {
        if (sfx)
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
        }
        partyScreen.ResetSelection();
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    private void OnPokemonSelected(int selection)
    {
        if (selection == partyScreen.Pokemons.Count)
        {
            OnBack();
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            if (_gameManager.StateMachine.GetPrevState() == InventoryState.I)
            {
                StartCoroutine(GoToUseItemState());
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
        _gameManager.StateMachine.Pop();
    }
}
