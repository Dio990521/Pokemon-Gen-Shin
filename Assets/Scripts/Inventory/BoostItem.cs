using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new boost item")]
public class BoostItem : ItemBase
{
    public bool IsExBoost;

    public List<int> BoostValue;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
