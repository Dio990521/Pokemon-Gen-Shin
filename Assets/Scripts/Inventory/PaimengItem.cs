using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new Paimeng item")]
public class PaimengItem : ItemBase
{
    private int hpAmount = 1;
    // Start is called before the first frame update

    public override bool Use(Pokemon pokemon)
    {
        pokemon.IncreaseHP(hpAmount);
        return false;
    }
}
