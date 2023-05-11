using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Create new pokeball")]
public class PokeballItem : ItemBase
{
    [SerializeField] private float catchRateModifier = 1;
    [SerializeField] private Sprite inBattleIcon;

    public override bool Use(Pokemon pokemon)
    {
        if (GameManager.Instance.State == GameState.Battle)
        {
            return true;
        }
        return false;
    }

    public float CatchRateModifier => catchRateModifier;
    public Sprite InBattleIcon => inBattleIcon;
}
