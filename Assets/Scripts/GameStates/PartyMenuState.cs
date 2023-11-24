using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMenuState : State<GameManager>
{

    [SerializeField] private PartyMenu _partyMenu;
    public Pokemon SelectedPokemon { get; set; }
    public int Selection { get; set; }

    public static PartyMenuState I { get; private set; }

    private GameManager _gameManager;
    public bool InBattle;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _partyMenu.OnSelected += OnChoiceSelected;
        _partyMenu.OnBack += OnBack;
        _partyMenu.gameObject.SetActive(true);
        _partyMenu.ResetSelection();
    }

    public override void Execute()
    {
        _partyMenu.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        SelectedPokemon = null;
        _partyMenu.OnSelected -= OnChoiceSelected;
        _partyMenu.OnBack -= OnBack;
        _partyMenu.gameObject.SetActive(false);
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
