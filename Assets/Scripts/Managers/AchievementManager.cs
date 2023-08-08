using Game.Tool.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public enum Achievement { Forest1, Forest2, Beach1, Beach2, Desert1, Desert2, Moutain1, Moutain2, Ship1, Ship2, Secret1, Secret2, Tower1, Tower2, Special1, Special2 }

public class AchievementManager : Singleton<AchievementManager>, ISavable
{
    private Dictionary<Achievement, HashSet<string>> _data;
    [SerializeField] private List<int> _pokemonCount;
    private float _totalPokemon;

    public List<int> PokemonCount { get => _pokemonCount; set => _pokemonCount = value; }

    override protected void Awake()
    {
        base.Awake();
        _data = new Dictionary<Achievement, HashSet<string>>()
        {
            { Achievement.Forest1, new HashSet<string>() },
            { Achievement.Forest2, new HashSet<string>() },
            { Achievement.Beach1, new HashSet<string>() },
            { Achievement.Beach2, new HashSet<string>() },
            { Achievement.Desert1, new HashSet<string>() },
            { Achievement.Desert2, new HashSet<string>() },
            { Achievement.Moutain1, new HashSet<string>() },
            { Achievement.Moutain2, new HashSet<string>() },
            { Achievement.Ship1, new HashSet<string>() },
            { Achievement.Ship2, new HashSet<string>() },
            { Achievement.Secret1, new HashSet<string>() },
            { Achievement.Secret2, new HashSet<string>() },
            { Achievement.Tower1, new HashSet<string>() },
            { Achievement.Tower2, new HashSet<string>() },
            { Achievement.Special1, new HashSet<string>() },
            { Achievement.Special2, new HashSet<string>() },
        };
        foreach (int value in _pokemonCount)
        {
            _totalPokemon += value;
        }
    }

    public void Complete(Achievement key, string name)
    {
        _data[key].Add(name);
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
