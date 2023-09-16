using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PokemonSaveData
{
    public string pokemonName;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
    public string passiveMoveName;
}
