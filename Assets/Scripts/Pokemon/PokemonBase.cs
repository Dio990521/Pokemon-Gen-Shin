using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private string pokemonName;

    [Header("Status")]
    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    [SerializeField] private bool isHuman;
    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    [SerializeField] private GrowthRate growthRate;

    [Header("Moves")]
    [SerializeField] private List<LearnableMove> learnableMoves;

    [Header("Evolution")]
    [SerializeField] private List<Evolution> evolutions;
    [SerializeField] private bool _isEvoState;

    [Header("Sprites")]
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private bool isLargePortrait;

    [Header("Battle Rewards")]
    [SerializeField] private ItemBase reward;
    [SerializeField] private float rewardProb;

    [Header("Others")]
    [SerializeField] private int expYield;
    [SerializeField] private int catchRate = 255;
    [SerializeField] private Achievement _achievement;
    [SerializeField] private PassiveMoveBase passiveMove;
    [SerializeField] private bool isSlime;
    public Dictionary<PassiveMoveType, float> EffectivenessData;

    public static int MaxNumOfMoves { get; set; } = 4;

    public void InitPassiveMove()
    {
        EffectivenessData = new Dictionary<PassiveMoveType, float>();
        if (passiveMove?.PassiveMoveEffects.Count > 0)
        {
            foreach (var effect in passiveMove.PassiveMoveEffects)
            {
                EffectivenessData[effect.PassiveMoveType] = effect.Effectiveness;
            }
        }
    }

    public int GetExpForLevel(int level)
    {
        if (growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level * level) / 5;
        }
        else if (growthRate == GrowthRate.MediumFast)
        {
            return level * level * level;
        }
        return -1;
    }

    public int MaxHp { get { return maxHp; } }
    public int Attack { get { return attack; } }
    public int Defense { get { return defense; } }
    public int SpAttack { get { return spAttack; } }
    public int SpDefense { get { return spDefense; } }
    public int Speed { get { return speed; } }
    public Sprite FrontSprite { get { return frontSprite; } }
    public Sprite BackSprite { get { return backSprite; } }
    public PokemonType Type1 { get { return type1; } }
    public PokemonType Type2 { get { return type2; } }
    public bool IsHuman { get { return isHuman; } }
    public string PokemonName { get { return pokemonName; } }
    public int CatchRate { get { return catchRate; } }
    public int ExpYield { get { return expYield; } }
    public GrowthRate GrowthRate { get { return growthRate; } }
    public List<LearnableMove> LearnableMoves { get { return learnableMoves; } }
    public List<Evolution> Evolutions { get { return evolutions; } }   
    public ItemBase Reward { get { return reward; } }
    public float RewardProb { get { return rewardProb; } }

    public PassiveMoveBase PassiveMove { get => passiveMove; set => passiveMove = value; }


    public bool IsEvoState { get => _isEvoState; set => _isEvoState = value; }
    public Achievement Achievement { get => _achievement; set => _achievement = value; }
    public bool IsLargePortrait { get => isLargePortrait; set => isLargePortrait = value; }
    public bool IsSlime { get => isSlime; set => isSlime = value; }
}


public enum Stat
{
    攻击,
    防御,
    特攻,
    特防,
    速度,
    生命,
    命中率,
    闪避率,
}

public enum GrowthRate
{
    Fast, MediumFast
}

[System.Serializable]
public class Evolution
{
    [SerializeField] private PokemonBase evolvesInto;
    [SerializeField] private int requiredLevel;
    [SerializeField] private EvolutionItem requiredItem;

    public PokemonBase EvolvesInto => evolvesInto;
    public int RequiredLevel => requiredLevel;  
    public EvolutionItem RequiredItem => requiredItem;
}