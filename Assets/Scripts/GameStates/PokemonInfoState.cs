using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonInfoState : State<GameManager>
{
    [SerializeField] private PokemonInfoUI _pokemonInfoUI;
    public Pokemon SelectedPokemon { get; set; }

    public static PokemonInfoState I { get; private set; }

    private GameManager _gameManager;

    private void Awake()
    {
        I = this;
    }

    public override void Enter(GameManager owner)
    {
        _gameManager = owner;
        _pokemonInfoUI.Show(SelectedPokemon);
    }

    public override void Execute()
    {
        _pokemonInfoUI.HandleUpdate();
    }

    public override void Exit(bool sfx = true)
    {
        SelectedPokemon = null;
        _pokemonInfoUI.gameObject.SetActive(false);
    }
}
