using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] private GameObject evolutionUI;
    [SerializeField] private Image pokemonImage;

    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;


    public static EvolutionManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);

        pokemonImage.sprite = pokemon.PokemonBase.FrontSprite;
        var oldPokemon = pokemon.PokemonBase;
        yield return DialogueManager.Instance.ShowDialogueText($"{pokemon.PokemonBase.PokemonName}���ڽ�����");

        pokemon.Evolve(evolution);
        pokemonImage.sprite = pokemon.PokemonBase.FrontSprite;
        yield return DialogueManager.Instance.ShowDialogueText($"{oldPokemon.PokemonName}�ɹ�����Ϊ\n{pokemon.PokemonBase.PokemonName}�ˣ�");

        evolutionUI.SetActive(false);
        OnCompleteEvolution?.Invoke();
    }
}
