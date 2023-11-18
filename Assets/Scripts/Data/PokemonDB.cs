using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB : ScriptableObjectDB<PokemonBase>
{
    public static PokemonBase GetRandomPokemon()
    {
        var key = GetRandomKey();
        return GetObjectByName(key);
    }
}
