using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public event Action OnUpdated;

    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
        set 
        {
            pokemons = value;
            OnUpdated?.Invoke();
        }
    }

    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }

    private void Start()
    {
        foreach (Pokemon pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.Hp > 0).FirstOrDefault();
    }

    public bool AddPokemonToParty(Pokemon newPokemon)
    {
        newPokemon.Exp = newPokemon.PokemonBase.GetExpForLevel(newPokemon.Level - 1) +
            UnityEngine.Random.Range(0, newPokemon.GetNextLevelExpLeft() / 2);
        newPokemon.CureElementStatus();
        newPokemon.CureStatus();
        if (pokemons.Count < 6)
        {
            AchievementManager.Instance.Complete(newPokemon.PokemonBase.Achievement, newPokemon.PokemonBase.PokemonName);
            pokemons.Add(newPokemon);
            OnUpdated?.Invoke();
            return true;
        }
        AchievementManager.Instance.Complete(newPokemon.PokemonBase.Achievement, newPokemon.PokemonBase.PokemonName);
        GameManager.Instance.Storage.PushPokemon(newPokemon);
        return false;
    }

    public void PartyUpdated()
    {
        OnUpdated?.Invoke();
    }

    public bool CheckForEvolutions()
    {
        return pokemons.Any(p => p.CheckForEvolution() != null);
    }

    public IEnumerator RunEvolutions()
    {
        foreach (var pokemon in pokemons)
        {
            var evolution = pokemon.CheckForEvolution();
            if (evolution != null)
            {
                yield return EvolutionState.I.Evolve(pokemon, evolution);
            }
        }
    }

    public bool HasPokemon(Pokemon pokemon)
    {
        return pokemons.Any(p => p.PokemonBase.PokemonName == pokemon.PokemonBase.PokemonName);
    }
}
