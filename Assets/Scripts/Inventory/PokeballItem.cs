using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] private float catchRateModifier = 1;
    [SerializeField] private Sprite inBattleIcon;

    public override bool CanUseOutsideBattle => false;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public float CatchRateModifier => catchRateModifier;
    public Sprite InBattleIcon => inBattleIcon;
}
