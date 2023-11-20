using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new AvoidPokemon item")]
public class AvoidPokemonItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
