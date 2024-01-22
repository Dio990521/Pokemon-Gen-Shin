using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum Achievement { Forest, Beach, Desert, Moutain, Ship, Secret, Tower, Special, None }

public class AchievementManager : Singleton<AchievementManager>, ISavable, IManager
{
    private Dictionary<Achievement, HashSet<string>> _data;
    private float _totalPokemon;

    private List<int> _pokemonCount;

    public List<int> PokemonCount { get => _pokemonCount; set => _pokemonCount = value; }

    public void ResetData()
    {
        _data = new Dictionary<Achievement, HashSet<string>>()
        {
            { Achievement.Forest, new HashSet<string>() },
            { Achievement.Beach, new HashSet<string>() },
            { Achievement.Desert, new HashSet<string>() },
            { Achievement.Moutain, new HashSet<string>() },
            { Achievement.Ship, new HashSet<string>() },
            { Achievement.Secret, new HashSet<string>() },
            { Achievement.Tower, new HashSet<string>() },
            { Achievement.Special, new HashSet<string>() },
        };
    }

    public void Init()
    {
        PokemonCount = new List<int>(new int[8]);
        _data = new Dictionary<Achievement, HashSet<string>>()
        {
            { Achievement.Forest, new HashSet<string>() },
            { Achievement.Beach, new HashSet<string>() },
            { Achievement.Desert, new HashSet<string>() },
            { Achievement.Moutain, new HashSet<string>() },
            { Achievement.Ship, new HashSet<string>() },
            { Achievement.Secret, new HashSet<string>() },
            { Achievement.Tower, new HashSet<string>() },
            { Achievement.Special, new HashSet<string>() },
        };

        foreach (var key in PokemonDB.GetAllKeys())
        {
            var pokemon = PokemonDB.GetObjectByName(key);
            if (pokemon.Achievement != Achievement.None)
            {
                PokemonCount[(int)pokemon.Achievement] += 1;
            }
        }

        foreach (int value in PokemonCount)
        {
            _totalPokemon += value;
        }
    }

    public void Complete(Achievement key, string name)
    {
        if (key != Achievement.None)
        {
            _data[key].Add(name);
        }
    }

    public bool HasComplete(Achievement key, string name)
    {
        if (!_data.ContainsKey(key)) return false;
        return _data[key].Contains(name);
    }

    public int GetProgress(Achievement key)
    {
        return _data[key].Count;
    }

    public float GetTotalProgress()
    {
        float totalSum = 0f;
        foreach (HashSet<string> set in _data.Values)
        {
            totalSum += set.Count;
        }
        return totalSum / _totalPokemon * 100f;
    }

    public object CaptureState()
    {
        return _data;
    }


    public void RestoreState(object state)
    {
        _data = (Dictionary<Achievement, HashSet<string>>)state;
    }

}
