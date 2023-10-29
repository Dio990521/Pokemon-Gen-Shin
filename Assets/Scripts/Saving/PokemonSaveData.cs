using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PokemonSaveData
{
    public string PokemonName;
    public int Hp;
    public int Level;
    public int Exp;
    public ConditionID? StatusId;
    public List<MoveSaveData> Moves;
    public string PassiveMoveName;
    public int[] StatusBias;
    public PokeballType PokeBall;
}

