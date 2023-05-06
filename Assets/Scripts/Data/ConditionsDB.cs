using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
        new Dictionary<ConditionID, Condition>()
        {
            {
                ConditionID.psn, new Condition()
                {
                    Name = "中毒",
                    StartMessage = "中毒了！",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}正遭受毒素折磨！");
                    }
                }

            },
            {
                ConditionID.brn, new Condition()
                {
                    Name = "燃烧",
                    StartMessage = "烧起来了！",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}正在燃烧！");
                    }
                }
            },
            {
                ConditionID.par, new Condition()
                {
                    Name = "麻痹",
                    StartMessage = "麻了个痹了！",
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (Random.Range(1, 5) == 1)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}麻到动不了！");
                            return false;
                        }
                        return true;
                    }
                }
            },
            {
                ConditionID.frz, new Condition()
                {
                    Name = "冻结",
                    StartMessage = "冻住了！",
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (Random.Range(1, 5) == 1)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}解冻了！");
                            return true;
                        }
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}一冻不动！");
                        return false;
                    }
                }
            },
            {
                ConditionID.slp, new Condition()
                {
                    Name = "睡眠",
                    StartMessage = "昏昏欲睡！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(1, 4);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}醒了！");
                            return true;
                        }
                        pokemon.StatusTime--;
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}睡的很香！");
                        return false;
                    }
                }
            },
            {
                ConditionID.confusion, new Condition()
                {
                    Name = "混乱",
                    StartMessage = "混乱了！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.VolatileStatusTime = Random.Range(1, 5);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.VolatileStatusTime <= 0)
                        {
                            pokemon.CureVolatileStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}从混乱中找回自我！");
                            return true;
                        }
                        pokemon.VolatileStatusTime--;

                        if (Random.Range(1, 3) == 1)
                        {
                            return true;
                        }
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}不知所措！");
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}给了自己一拳！");
                        return false;
                    }
                }
            }

        };

    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
        {
            return 1f;
        }
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
        {
            return 2f;
        }
        else if (condition.Id == ConditionID.slp || condition.Id == ConditionID.frz)
        {
            return 1.5f;
        }

        return 1f;
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz, confusion
}
