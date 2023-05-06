using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    public int Attack { get { return GetStat(Stat.攻击); } }
    public int Defense { get { return GetStat(Stat.防御); } }
    public int SpAttack { get { return GetStat(Stat.特攻); } }
    public int SpDefense { get { return GetStat(Stat.特防); } }
    public int Speed { get { return GetStat(Stat.速度); } }
    public int MaxHp { get; private set; }

    public PokemonBase PokemonBase { get { return pokemonBase; } }
    public int Level { get { return level; } set { level = value; } }
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
    public event System.Action OnHpChanged;

    public Pokemon(PokemonSaveData saveData)
    {
        pokemonBase = PokemonDB.GetPokemonByName(saveData.pokemonName);
        Hp = saveData.hp;
        Level = saveData.level;
        Exp = saveData.exp;

        if (saveData.statusId != null)
        {
            Status = ConditionsDB.Conditions[saveData.statusId.Value];
        }
        else
        {
            Status = null;
        }

        Moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

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
            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
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

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            pokemonName = pokemonBase.PokemonName,
            hp = Hp,
            level = Level,
            exp = Exp,
            statusId = Status?.Id,
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

    // Calculate the pokemon's status by based status
    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.攻击, Mathf.FloorToInt(pokemonBase.Attack * Level / 100f) + 5 },
            { Stat.防御, Mathf.FloorToInt(pokemonBase.Defense * Level / 100f) + 5 },
            { Stat.特攻, Mathf.FloorToInt(pokemonBase.SpAttack * Level / 100f) + 5 },
            { Stat.特防, Mathf.FloorToInt(pokemonBase.SpDefense * Level / 100f) + 5 },
            { Stat.速度, Mathf.FloorToInt(pokemonBase.Speed * Level / 100f) + 5 }
        };

        MaxHp = Mathf.FloorToInt(pokemonBase.MaxHp * Level / 100f) + Level + 10;
    }

    public bool CheckForLevelUp()
    {
        if (Exp > pokemonBase.GetExpForLevel(level + 1))
        {
            ++level;
            CalculateStats();
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return PokemonBase.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(LearnableMove moveToLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves) return;
        Moves.Add(new Move(moveToLearn.MoveBase));
    }

    // Reset all status to the default state
    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            { Stat.攻击, 0 },
            { Stat.防御, 0 },
            { Stat.特攻, 0},
            { Stat.特防, 0},
            { Stat.速度, 0},
            { Stat.命中率, 0 },
            { Stat.闪避率, 0 },
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
                StatusChanges.Enqueue($"{pokemonBase.PokemonName}的{stat}上升了！");
            }
            else
            {
                StatusChanges.Enqueue($"{pokemonBase.PokemonName}的{stat}下降了！");
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
        DecreaseHP(damage);

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

    public void DecreaseHP(int damage)
    {
        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
        OnHpChanged?.Invoke();
        HpChanged = true;
    }

    public void IncreaseHP(int amount)
    {
        Hp = Mathf.Clamp(Hp + amount, 0, MaxHp);
        OnHpChanged?.Invoke();
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