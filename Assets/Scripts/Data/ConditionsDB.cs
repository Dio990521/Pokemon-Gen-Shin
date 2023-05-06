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
                    Name = "�ж�",
                    StartMessage = "�ж��ˣ�",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�����ܶ�����ĥ��");
                    }
                }

            },
            {
                ConditionID.brn, new Condition()
                {
                    Name = "ȼ��",
                    StartMessage = "�������ˣ�",
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}����ȼ�գ�");
                    }
                }
            },
            {
                ConditionID.par, new Condition()
                {
                    Name = "���",
                    StartMessage = "���˸����ˣ�",
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (Random.Range(1, 5) == 1)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�鵽�����ˣ�");
                            return false;
                        }
                        return true;
                    }
                }
            },
            {
                ConditionID.frz, new Condition()
                {
                    Name = "����",
                    StartMessage = "��ס�ˣ�",
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (Random.Range(1, 5) == 1)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�ⶳ�ˣ�");
                            return true;
                        }
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}һ��������");
                        return false;
                    }
                }
            },
            {
                ConditionID.slp, new Condition()
                {
                    Name = "˯��",
                    StartMessage = "�����˯��",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(1, 4);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}���ˣ�");
                            return true;
                        }
                        pokemon.StatusTime--;
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}˯�ĺ��㣡");
                        return false;
                    }
                }
            },
            {
                ConditionID.confusion, new Condition()
                {
                    Name = "����",
                    StartMessage = "�����ˣ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.VolatileStatusTime = Random.Range(1, 5);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.VolatileStatusTime <= 0)
                        {
                            pokemon.CureVolatileStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�ӻ������һ����ң�");
                            return true;
                        }
                        pokemon.VolatileStatusTime--;

                        if (Random.Range(1, 3) == 1)
                        {
                            return true;
                        }
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}��֪���룡");
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�����Լ�һȭ��");
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
