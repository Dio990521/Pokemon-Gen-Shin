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
                    Name = "ˮ",
                    StartMessage = "������ˮԪ�أ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}���ŵ�ˮԪ����ʧ�ˣ�");
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
                    Name = "��",
                    StartMessage = "�����˻�Ԫ�أ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}���ŵĻ�Ԫ����ʧ�ˣ�");
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
                    Name = "��",
                    StartMessage = "�����˱�Ԫ�أ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}���ŵı�Ԫ����ʧ�ˣ�");
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
                    Name = "��",
                    StartMessage = "�����˲�Ԫ�أ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}���ŵĲ�Ԫ����ʧ�ˣ�");
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
                    Name = "��",
                    StartMessage = "��������Ԫ�أ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.ElementStatusTime = 2;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.ElementStatusTime <= 0)
                        {
                            pokemon.CureElementStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}���ŵ���Ԫ����ʧ�ˣ�");
                            return true;
                        }
                        pokemon.ElementStatusTime--;
                        return true;
                    },
                }

            },

            {
                ConditionID.psn, new Condition()
                {
                    Name = "����",
                    StartMessage = "�����ˣ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(2, 4);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�ĳ���Ч����ʧ�ˣ�");
                            return true;
                        }
                        pokemon.StatusTime--;
                        return false;
                    },
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 16);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�����ܳ��ص���ĥ��");
                    }
                }

            },
            {
                ConditionID.brn, new Condition()
                {
                    Name = "ȼ��",
                    StartMessage = "�������ˣ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(1, 3);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�����ˣ�");
                            return true;
                        }
                        pokemon.StatusTime--;
                        return false;
                    },
                    OnAfterTurn = (Pokemon pokemon) =>
                    {
                        pokemon.DecreaseHP(pokemon.MaxHp / 8);
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}����ȼ�գ�");
                    }
                }
            },
            {
                ConditionID.par, new Condition()
                {
                    Name = "���",
                    StartMessage = "���˸����ˣ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = Random.Range(2, 4);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�����ˣ�");
                            return true;
                        }
                        if (Random.Range(1, 5) <= 2)
                        {
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�鵽�����ˣ�");
                            return false;
                        }
                        pokemon.StatusTime--;
                        return false;
                    },
                }
            },
            {
                ConditionID.frz, new Condition()
                {
                    Name = "����",
                    StartMessage = "��ס�ˣ�",
                    OnStart= (Pokemon pokemon) =>
                    {
                        pokemon.StatusTime = 1;
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�ⶳ�ˣ�");
                            return true;
                        }
                        pokemon.StatusTime--;
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
                        pokemon.StatusTime = Random.Range(2, 4);
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
                        pokemon.StatusTime = Random.Range(1, 5);
                    },
                    OnBeforeMove = (Pokemon pokemon) =>
                    {
                        if (pokemon.StatusTime <= 0)
                        {
                            pokemon.CureStatus();
                            pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}�ӻ������һ����ң�");
                            return true;
                        }
                        pokemon.StatusTime--;

                        if (Random.Range(1, 3) == 1)
                        {
                            return true;
                        }
                        pokemon.StatusChanges.Enqueue($"{pokemon.PokemonBase.PokemonName}��֪���룡");
                        pokemon.DecreaseHP(pokemon.MaxHp / 10);
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
    none, psn, brn, slp, par, frz, confusion, hydro, pyro, dendro, cryo, electro
}
