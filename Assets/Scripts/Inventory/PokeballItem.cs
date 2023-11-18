using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] private float catchRateModifier = 1;
    [SerializeField] private Sprite inBattleIcon;
    [SerializeField] private Sprite openIcon;
    [SerializeField] private PokeballType ballType;

    public override bool CanUseOutsideBattle => false;
    public PokeballType BallType => ballType;

    public override bool Use(Pokemon pokemon)
    {
        return true;
    }

    public float CatchRateModifier => catchRateModifier;
    public Sprite InBattleIcon => inBattleIcon;

    public Sprite OpenIcon { get => openIcon; set => openIcon = value; }
}

public enum PokeballType
{
    Guaishou,
    Gaoji,
    Chaoji,
    Iron,
    FiveFive,
    Beast,
    Master,
    Genshin,
    None,
}
