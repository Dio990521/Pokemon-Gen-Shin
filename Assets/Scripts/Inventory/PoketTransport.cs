using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new PoketTransport item")]
public class PoketTransport : ItemBase
{
    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
