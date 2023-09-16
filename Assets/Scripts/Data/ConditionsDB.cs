using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } =
        new Dictionary<ConditionID, Condition>()
        {
            {
                ConditionID.hydro, new Condition()
                {
                    Name = "水",
                    StartMessage = "附着了水元素！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}附着的水元素消失了！");
                            return true;
                        }
                        pokemon.ElementStatusTime--;
                        return true;
                    },
                }

            },
            {
                ConditionID.pyro, new Condition()
                {
                    Name = "火",
                    StartMessage = "附着了火元素！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}附着的火元素消失了！");
                            return true;
                        }
                        pokemon.ElementStatusTime--;
                        return true;
                    },
                }

            },
            {
                ConditionID.cryo, new Condition()
                {
                    Name = "冰",
                    StartMessage = "附着了冰元素！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}附着的冰元素消失了！");
                            return true;
                        }
                        pokemon.ElementStatusTime--;
                        return true;
                    },
                }

            },
            {
                ConditionID.dendro, new Condition()
                {
                    Name = "草",
                    StartMessage = "附着了草元素！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}附着的草元素消失了！");
                            return true;
                        }
                        pokemon.ElementStatusTime--;
                        return true;
                    },
                }

            },
            {
                ConditionID.electro, new Condition()
                {
                    Name = "雷",
                    StartMessage = "附着了雷元素！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}附着的雷元素消失了！");
                            return true;
                        }
                        pokemon.ElementStatusTime--;
                        return true;
                    },
                }

            },
            {
                ConditionID.jiejing, new Condition()
                {
                    Name = "结晶",
                    StartMessage = "结晶化！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}结晶化解除了！");
                            return true;
                        }
                        pokemon.StatusTime--;
                        return true;
                    },
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}正在免疫一切元素附着！");
                    }
                }

            },
            {
                ConditionID.psn, new Condition()
                {
                    Name = "超载",
                    StartMessage = "超载了！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(2, 4);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}的超载效果消失了！");
                            return true;
                        }
                        pokemon.StatusTime--;
                        return true;
                    },
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}正遭受超载的折磨！");
                    }
                }

            },
            {
                ConditionID.brn, new Condition()
                {
                    Name = "燃烧",
                    StartMessage = "烧起来了！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(1, 3);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}不烧了！");
                            return true;
                        }
                        pokemon.StatusTime--;
                        return true;
                    },
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}正在燃烧！");
                    }
                }
            },
            {
                ConditionID.par, new Condition()
                {
                    Name = "麻痹",
                    StartMessage = "麻了个痹了！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(2, 4);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}不麻了！");
                            return true;
                        }
                        if (Random.Range(1, 5) <= 2)
                        {
                            pokemon.StatusTime--;
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}麻到动不了！");
                            return false;
                        }
                        pokemon.StatusTime--;
                        return true;
                    },
                }
            },
            {
                ConditionID.frz, new Condition()
                {
                    Name = "冻结",
                    StartMessage = "冻住了！",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = 1;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}解冻了！");
                            return true;
                        }
                        pokemon.StatusTime--;
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
                        pokemon.StatusTime = Random.Range(2, 4);
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
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}的HP恢复了一点！");
                        pokemon.IncreaseHP((int)(pokemon.MaxHp * 0.1f));
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
                        pokemon.StatusTime = Random.Range(1, 5);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}从混乱中找回自我！");
                            return true;
                        }
                        pokemon.StatusTime--;

                        if (Random.Range(1, 3) == 1)
                        {
                            return true;
                        }
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}不知所措！");
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}给了自己一拳！");
                        pokemon.DecreaseHP(pokemon.MaxHp / 10);
                        return false;
                    }
                }
            },

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

    public static ConditionID GetElementReaction(ConditionID elementA, ConditionID elementB)
    {
        if (elementA == ConditionID.hydro && elementB == ConditionID.dendro || elementA == ConditionID.dendro && elementB == ConditionID.hydro)
        {
            return ConditionID.zhanfang;
        }
        else if (elementA == ConditionID.electro && elementB == ConditionID.dendro || elementA == ConditionID.dendro && elementB == ConditionID.electro)
        {
            return ConditionID.slp;
        }
        else if (elementA == ConditionID.pyro && elementB == ConditionID.dendro || elementA == ConditionID.dendro && elementB == ConditionID.pyro)
        {
            return ConditionID.brn;
        }
        else if (elementA == ConditionID.hydro && elementB == ConditionID.pyro || elementA == ConditionID.pyro && elementB == ConditionID.hydro)
        {
            return ConditionID.zhengfa;
        }
        else if (elementA == ConditionID.hydro && elementB == ConditionID.cryo || elementA == ConditionID.cryo && elementB == ConditionID.hydro)
        {
            return ConditionID.frz;
        }
        else if (elementA == ConditionID.hydro && elementB == ConditionID.electro || elementA == ConditionID.electro && elementB == ConditionID.hydro)
        {
            return ConditionID.par;
        }
        else if (elementA == ConditionID.pyro && elementB == ConditionID.cryo || elementA == ConditionID.cryo && elementB == ConditionID.pyro)
        {
            return ConditionID.ronghua;
        }
        else if (elementA == ConditionID.pyro && elementB == ConditionID.electro || elementA == ConditionID.electro && elementB == ConditionID.pyro)
        {
            return ConditionID.psn;
        }
        else if (elementA == ConditionID.cryo && elementB == ConditionID.electro || elementA == ConditionID.electro && elementB == ConditionID.cryo)
        {
            return ConditionID.confusion;
        }
        else if (IsElementCondition(elementA) && elementB == ConditionID.anemo || IsElementCondition(elementB) && elementA == ConditionID.anemo)
        {
            return ConditionID.kuosan;
        }
        else if (IsElementCondition(elementA) && elementB == ConditionID.geo || IsElementCondition(elementB) && elementA == ConditionID.geo)
        {
            return ConditionID.jiejing;
        }
        return ConditionID.none;
    }

    public static bool IsElementCondition(ConditionID conditionID)
    {
        return conditionID == ConditionID.hydro || conditionID == ConditionID.pyro 
            || conditionID == ConditionID.dendro || conditionID == ConditionID.cryo
            || conditionID == ConditionID.electro || conditionID == ConditionID.geo
            || conditionID == ConditionID.anemo;
    }

    public static bool IsContinuousElementReaction(ConditionID conditionID)
    {
        return conditionID == ConditionID.psn || conditionID == ConditionID.brn
            || conditionID == ConditionID.slp || conditionID == ConditionID.par
            || conditionID == ConditionID.frz || conditionID == ConditionID.confusion
            || conditionID == ConditionID.anemo;
    }
}

public enum ConditionID
{
    psn, brn, slp, par, frz, confusion, jiejing, zhanfang, zhengfa, ronghua, kuosan, hydro, pyro, dendro, cryo, electro, geo, anemo, none
}
