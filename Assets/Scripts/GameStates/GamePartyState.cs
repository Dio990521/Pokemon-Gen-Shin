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

    public override void Exit()
    {
        AudioManager.Instance.PlaySE(SFX.CANCEL);
        partyScreen.ResetSelection();
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    private void OnPokemonSelected(int selection)
    {
        if (selection == partyScreen.Pokemons.Count)
        {
            AudioManager.Instance.PlaySE(SFX.CANCEL);
            OnBack();
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            if (_gameManager.StateMachine.GetPrevState() == InventoryState.I)
            {
                print($"Use Item {selection}");
            }
            else
            {
                // Summary screen
                print($"open summary screen {selection}");
            }

            
            
        }
        
    }

    private void OnBack()
    {
        _gameManager.StateMachine.Pop();
    }
}
