using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new evolution item")]
public class EvolutionItem : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
