using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();
        var pokemonArray = Resources.LoadAll<PokemonBase>("Prefabs/Pokemons");
        foreach (var pokemon in pokemonArray)
        {
            if (pokemons.ContainsKey(pokemon.PokemonName))
            {
                Debug.LogError($"There are 2 pokemons with the same name: {pokemon.PokemonName}");
                continue;
            }
            pokemons[pokemon.PokemonName] = pokemon;
        }
    }

    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name) )
        {
            Debug.LogError($"Pokemon not found with the name {name}");
            return null;
        }
        return pokemons[name];
    }
}
