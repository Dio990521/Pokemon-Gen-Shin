using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] private List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] private List<PokemonEncounterRecord> wildPokemonsInWater;
    [SerializeField] private List<PokemonEncounterRecord> wildPokemonsInDesert;
    [SerializeField] private List<PokemonEncounterRecord> wildPokemonsInShip;
    [SerializeField] private List<PokemonEncounterRecord> wildPokemonsInCave;


    [HideInInspector]
    [SerializeField] private int totalChance = 0;

    [HideInInspector]
    [SerializeField] private int totalChanceInWater = 0;

    [HideInInspector]
    [SerializeField] private int totalChanceInDesert = 0;

    [HideInInspector]
    [SerializeField] private int totalChanceInCave = 0;

    [HideInInspector]
    [SerializeField] private int totalChanceInShip = 0;

    private void OnValidate()
    {
        CalculateChancePercentage();
    }

    private void Start()
    {
        CalculateChancePercentage();
    }

    private void CalculateChancePercentage()
    {
        totalChance = -1;
        totalChanceInWater = -1;
        totalChanceInDesert = -1;
        totalChanceInCave = -1;
        totalChanceInShip = -1;

        if (wildPokemons.Count > 0)
        {
            totalChance = 0;
            foreach (var record in wildPokemons)
            {
                record.chanceLower = totalChance;
                record.chanceUpper = totalChance + record.chancePercentage;

                totalChance += record.chancePercentage;
            }
        }

        if (wildPokemonsInWater.Count > 0)
        {
            totalChanceInWater = 0;
            foreach (var record in wildPokemonsInWater)
            {
                record.chanceLower = totalChanceInWater;
                record.chanceUpper = totalChanceInWater + record.chancePercentage;

                totalChanceInWater += record.chancePercentage;
            }
        }

        if (wildPokemonsInDesert.Count > 0)
        {
            totalChanceInDesert = 0;
            foreach (var record in wildPokemonsInDesert)
            {
                record.chanceLower = totalChanceInDesert;
                record.chanceUpper = totalChanceInDesert + record.chancePercentage;

                totalChanceInDesert += record.chancePercentage;
            }
        }

        if (wildPokemonsInCave.Count > 0)
        {
            totalChanceInCave = 0;
            foreach (var record in wildPokemonsInCave)
            {
                record.chanceLower = totalChanceInCave;
                record.chanceUpper = totalChanceInCave + record.chancePercentage;

                totalChanceInCave += record.chancePercentage;
            }
        }


        if (wildPokemonsInShip.Count > 0)
        {
            totalChanceInShip = 0;
            foreach (var record in wildPokemonsInShip)
            {
                record.chanceLower = totalChanceInShip;
                record.chanceUpper = totalChanceInShip + record.chancePercentage;

                totalChanceInShip += record.chancePercentage;
            }
        }
    }

    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        var pokemonList = wildPokemons;
        if (trigger == BattleTrigger.Water)
        {
            pokemonList = wildPokemonsInWater;
        }
        else if (trigger == BattleTrigger.Desert)
        {
            pokemonList = wildPokemonsInDesert;
        }
        else if (trigger == BattleTrigger.Cave)
        {
            pokemonList = wildPokemonsInCave;
        }
        else if (trigger == BattleTrigger.Ship)
        {
            pokemonList = wildPokemonsInShip;
        }

        int randVal = Random.Range(1, 101);
        var pokemonRecord = pokemonList.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);
        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);

        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        wildPokemon.Init();
        return wildPokemon;
    }
}

[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chancePercentage;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
