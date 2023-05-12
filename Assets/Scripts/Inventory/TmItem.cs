using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new TM or HM")]
public class TmItem : ItemBase
{
    [SerializeField] private MoveBase move;

    public MoveBase Move => move;

    public override bool Use(Pokemon pokemon)
    {
        // Learning move is handled from Inventory UI
        // If it was learned then return true
        return pokemon.HasMove(move);
    }

    
}
