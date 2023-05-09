using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovery item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] private int hpAmount;
    [SerializeField] private bool restoreMaxHp;

    [Header("PP")]
    [SerializeField] private int ppAmount;
    [SerializeField] private bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] private bool revive;
    [SerializeField] private bool maxRevive;

    public override bool Use(Pokemon pokemon)
    {
        // Revive
        if (revive || maxRevive)
        {
            if (pokemon.Hp > 0)
            {
                return false;
            }

            if (revive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp / 2);
            }
            else if (maxRevive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }

            pokemon.CureStatus();

            return true;
        }

        // No other items can be used on fainted pokemon
        if (pokemon.Hp == 0)
        {
            return false;
        }

        // Restore HP
        if (restoreMaxHp || hpAmount > 0)
        {
            if (pokemon.Hp == pokemon.MaxHp)
            {
                return false;
            }

            if (restoreMaxHp)
            {
                pokemon.IncreaseHP(pokemon.MaxHp);
            }
            else
            {
                pokemon.IncreaseHP(hpAmount);
            }
            
        }

        // Recover Status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (pokemon.Status == null && pokemon.VolatileStatus == null)
            {
                return false;
            }

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                {
                    pokemon.CureStatus();
                }
                else if (pokemon.VolatileStatus.Id == status)
                {
                    pokemon.CureVolatileStatus();
                }
                else
                {
                    return false;
                }
            }
        }

        // Restore PP
        if (restoreMaxPP)
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(m.MoveBase.PP));
        }
        else if (ppAmount > 0) 
        {
            pokemon.Moves.ForEach(m => m.IncreasePP(ppAmount));
        }

        return true;
    }
}
