using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string moveName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    public bool IsPercentage;
    [SerializeField] private int accuracy;
    [SerializeField] private bool alwaysHits;
    [SerializeField] private int pp;
    [SerializeField] private int priority;
    public bool CureAllStatus;
    public bool CureAllElementStatus;

    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private MoveTarget target;


    [SerializeField] private List<SecondaryEffects> secondaryEffects;

    [SerializeField] private MoveFX moveFX;
    [SerializeField] private bool allOutAttack;
    public int AccumulatePower;

    [Header("攻击后恢复百分比生命")]
    [SerializeField] private bool goldExperience;
    [SerializeField] private int goldExperienceValue;


    public string MoveName
    {
        get { return moveName; }
    }

    public string Description
    {
        get { return description; }
    }

    public PokemonType Type 
    { 
        get { return type; } 
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }

    public int PP
    {
        get { return pp; }
    }

    public int Priority
    {
        get { return priority; }
    }

    public MoveCategory Category { get { return category; } }

    public MoveEffects Effects { get { return effects; } }

    public List<SecondaryEffects> SecondaryEffects { get { return secondaryEffects; } }

    public MoveTarget Target { get { return target; } }

    public bool IsSpecial
    {
        get
        {
            if (type == PokemonType.火 || type == PokemonType.水 || type == PokemonType.草
                || type == PokemonType.冰 || type == PokemonType.雷)
            {
                return true;
            }
            return false;
        }
    }

    public MoveFX MoveFX { get => moveFX; set => moveFX = value; }
    public bool AllOutAttack { get => allOutAttack; set => allOutAttack = value; }
    public bool GoldExperience { get => goldExperience; set => goldExperience = value; }
    public int GoldExperienceValue { get => goldExperienceValue; set => goldExperienceValue = value; }
}

public enum MoveCategory
{
    Physical,
    Special,
    Status,
    Healing,
}

public enum MoveTarget
{
    Foe, 
    Self
}

public enum PassiveMoveType
{
    Slp,
    Frz,
    Psn,
    Brn,
    Par,
    Cfs,
    Kuosan,
    Jiejing,
    Zhengfa,
    Ronghua,
    Zhanfang,
    None
}
