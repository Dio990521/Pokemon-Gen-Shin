using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new Pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private string pokemonName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;

    [SerializeField] private int maxHp;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;

    [SerializeField] private int expYield;
    [SerializeField] private GrowthRate growthRate;

    [SerializeField] private int catchRate = 255;

    [SerializeField] private List<LearnableMove> learnableMoves;

    public static int MaxNumOfMoves { get; set; } = 4;

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
    public string PokemonName { get { return pokemonName; } }
    public string Description { get { return description; } }
    public int CatchRate { get { return catchRate; } }
    public int ExpYield { get { return expYield; } }
    public GrowthRate GrowthRate { get { return growthRate; } }
    public List<LearnableMove> LearnableMoves { get { return learnableMoves; } }

}


public enum Stat
{
    攻击,
    防御,
    特攻,
    特防,
    速度,
    命中率,
    闪避率
}

public enum GrowthRate
{
    Fast, MediumFast
}