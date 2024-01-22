using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new EscapeItem item")]
public class EscapeItem : ItemBase
{
    public override bool CanUseOutsideBattle => false;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }
}
