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
                pokemon.IncreaseHP(pokemon.MaxHp / 2, true);
            }
            else if (maxRevive)
            {
                pokemon.IncreaseHP(pokemon.MaxHp, true);
            }

            pokemon.CureStatus();

            return true;
        }

        // No other items can be used on fainted pokemon
        if (pokemon.Hp == 0 || (pokemon.Hp == pokemon.MaxHp && hpAmount > 0)
            || (pokemon.Hp == pokemon.MaxHp && restoreMaxHp))
        {
            return false;
        }

        var used = false;

        // Restore HP
        if (restoreMaxHp)
        {
            if (pokemon.Hp == pokemon.MaxHp)
            {
                return false;
            }
            pokemon.IncreaseHP(pokemon.MaxHp);
            used = true;
        }

        if (hpAmount > 0)
        {
            pokemon.IncreaseHP(hpAmount);
            used = true;
        }

        // Recover Status
        if (recoverAllStatus || status != ConditionID.none)
        {
            if (pokemon.Status == null)
            {
                return used;
            }

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                used = true;
            }
            else
            {
                if (pokemon.Status.Id == status)
                {
                    pokemon.CureStatus();
                    used = true;
                }
                else
                {
                    return used;
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
