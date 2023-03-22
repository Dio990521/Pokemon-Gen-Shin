using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase pokemonBase;
    [SerializeField] private int level;

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        this.pokemonBase = pBase;
        this.level = pLevel;

        Init();
    }

    public int Attack { get { return GetStat(Stat.����); } }
    public int Defense { get { return GetStat(Stat.����); } }
    public int SpAttack { get { return GetStat(Stat.�ع�); } }
    public int SpDefense { get { return GetStat(Stat.�ط�); } }
    public int Speed { get { return GetStat(Stat.�ٶ�); } }
    public int MaxHp { get; private set; }

    public PokemonBase PokemonBase { get { return pokemonBase; } }
    public int Level { get { return level; } }
    public int Hp { get; set; }
    public int Exp { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public bool HpChanged { get; set; }
    public event System.Action OnStatusChanged;

    // Initialize the pokemon
    public void Init()
    {
        Moves = new List<Move>();
        foreach (var move in pokemonBase.LearnableMoves)
        {
            if (move.Level <= level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
            if (Moves.Count >= 4)
            {
                break;
            }
        }

        CalculateStats();

        Hp = MaxHp;
        Exp = pokemonBase.GetExpForLevel(Level);
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    // Calculate the pokemon's status by based status
    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.����, Mathf.FloorToInt(pokemonBase.Attack * Level / 100f) + 5 },
            { Stat.����, Mathf.FloorToInt(pokemonBase.Defense * Level / 100f) + 5 },
            { Stat.�ع�, Mathf.FloorToInt(pokemonBase.SpAttack * Level / 100f) + 5 },
            { Stat.�ط�, Mathf.FloorToInt(pokemonBase.SpDefense * Level / 100f) + 5 },
            { Stat.�ٶ�, Mathf.FloorToInt(pokemonBase.Speed * Level / 100f) + 5 }
        };

        MaxHp = Mathf.FloorToInt(pokemonBase.MaxHp * Level / 100f) + Level + 10;
    }

    public bool CheckForLevelUp()
    {
        if (Exp > pokemonBase.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    // Reset all status to the default state
    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.����, 0 },
            { Stat.����, 0 },
            { Stat.�ع�, 0},
            { Stat.�ط�, 0},
            { Stat.�ٶ�, 0},
            { Stat.������, 0 },
            { Stat.������, 0 },
        };
    }

    // Get current status (boosted or not boosted)
    private int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
        statVal = (boost >= 0) ? Mathf.FloorToInt(statVal * boostValues[boost]) 
            : Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    // Apply boosted satus and show the dialogues
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
            {
                StatusChanges.Enqueue($"{pokemonBase.PokemonName}��{stat}�����ˣ�");
            }
            else
            {
                StatusChanges.Enqueue($"{pokemonBase.PokemonName}��{stat}�½��ˣ�");
            }
        }
    }

    // Calculate the damage when the pokemon get hurt
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
        {
            critical = 2f;
        }

        float type = TypeChart.GetEffectiveness(move.MoveBase.Type, this.pokemonBase.Type1)
            * TypeChart.GetEffectiveness(move.MoveBase.Type, this.pokemonBase.Type2);

        DamageDetails damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical= critical,
            Fainted = false,
        };

        float attack = move.MoveBase.Category == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        float defense = move.MoveBase.Category == MoveCategory.Special ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.MoveBase.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        UpdateHP(damage);

        return damageDetails;
    }

    // Check if the pokemon can perform a move before the action
    public bool CanPerformMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        return canPerformMove;
    }

    public void AfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void UpdateHP(int damage)
    {
        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
        HpChanged = true;
    }

    public void SetStatus(ConditionID conditionId)
    {
        if (Status != null) return;
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{pokemonBase.PokemonName}{Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionId)
    {
        if (VolatileStatus != null) return;
        VolatileStatus = ConditionsDB.Conditions[conditionId];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{pokemonBase.PokemonName}{VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    // used when it's enemy pokemon's turn 
    public Move GetRandomMove()
    {
        var movesWithPP = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

}