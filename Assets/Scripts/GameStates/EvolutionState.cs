using Game.Tool.Singleton;
using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionState : State<GameManager>
{
    [SerializeField] private GameObject evolutionUI;
    [SerializeField] private Image pokemonImage;

    public static EvolutionState I { get; private set; }

    private void Awake()
    {
        I = this;
    }


    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        var gameManager = GameManager.Instance;
        gameManager.StateMachine.Push(this);

        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.PokemonBase.FrontSprite;
        var oldPokemon = pokemon.PokemonBase;
        yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}���ڽ�����");

        pokemon.Evolve(evolution);
        pokemonImage.sprite = pokemon.PokemonBase.FrontSprite;
        yield return DialogueManager.Instance.ShowDialogueText($"{oldPokemon.PokemonName}�ɹ�����Ϊ\n{pokemon.PokemonBase.PokemonName}�ˣ�");

        evolutionUI.SetActive(false);

        gameManager.PartyScreen.SetPartyData();
        AudioManager.Instance.PlayMusic(gameManager.CurrentScene.SceneMusic, fade: true);

        gameManager.StateMachine.Pop();
    }
}