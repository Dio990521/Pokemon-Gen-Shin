using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase pokemonBase;
    [SerializeField] private int level;
    private Sprite _pokeballSprite;
    private string _catchPlace;

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
    public Condition ElementStatus { get; private set; }
    public int ElementStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public Sprite PokeballSprite { get => _pokeballSprite; set => _pokeballSprite = value; }
    public string CatchPlace { get => _catchPlace; set => _catchPlace = value; }

    public event System.Action OnStatusChanged;
    public event System.Action OnHpChanged;

    public Pokemon(PokemonSaveData saveData)
    {
        pokemonBase = PokemonDB.GetObjectByName(saveData.pokemonName);
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
        ElementStatus = null;
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
        ElementStatus = null;
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            pokemonName = pokemonBase.name,
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

        int oldMaxHp = MaxHp;
        MaxHp = Mathf.FloorToInt(pokemonBase.MaxHp * Level / 100f) + Level + 10;

        if (oldMaxHp != 0)
        {
            Hp += MaxHp - oldMaxHp;
        }
        
    }

    public int GetNextLevelExpLeft()
    {
        return pokemonBase.GetExpForLevel(level + 1) - Exp;
    }


    public float GetNormalizedExp()
    {
        int currLevelExp = pokemonBase.GetExpForLevel(Level);
        int nextLevelExp = pokemonBase.GetExpForLevel(Level + 1);

        float normalizedExp = (float)(Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
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

    public Evolution CheckForEvolution()
    {
        return pokemonBase.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level);
    }

    public Evolution CheckForEvolution(ItemBase item)
    {
        return pokemonBase.Evolutions.FirstOrDefault(e => e.RequiredItem == item);
    }

    public void Evolve(Evolution evolution)
    {
        pokemonBase = evolution.EvolvesInto;
        CalculateStats();
    }

    public void Heal()
    {
        Hp = MaxHp;
        OnHpChanged?.Invoke();
        CureStatus();
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return PokemonBase.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public List<LearnableMove> GetLearnableMovesAtCurrentLevel()
    {
        return PokemonBase.LearnableMoves.Where(x => x.Level <= level).ToList();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > PokemonBase.MaxNumOfMoves) return;
        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.MoveBase == moveToCheck) > 0;
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
        if (ElementStatus != null)
        {
            ConditionID elementReactionRes = ConditionsDB.GetElementReaction(ElementStatus.Id, move.MoveBase.Effects.ElementStatus);
            if (ConditionsDB.IsContinuousElementReaction(elementReactionRes))
            {
                CureElementStatus();
                SetStatus(elementReactionRes);
            }
        }

        DamageDetails damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical= critical,
            Fainted = false,
            StatusName = Status?.Name,
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

        if (ElementStatus?.OnBeforeMove != null)
        {
            if (!ElementStatus.OnBeforeMove(this))
            {
                canPerformMove = false;
            }
        }

        return canPerformMove;
    }

    public void AfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        ElementStatus?.OnAfterTurn?.Invoke(this);
    }

    public void DecreaseHP(int damage)
    {
        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
        OnHpChanged?.Invoke();
    }

    public void IncreaseHP(int amount)
    {
        Hp = Mathf.Clamp(Hp + amount, 0, MaxHp);
        OnHpChanged?.Invoke();
    }

    public void SetStatus(ConditionID conditionId)
    {
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

    public void SetElementStatus(ConditionID conditionId)
    {
        if (ElementStatus != null) return;
        ElementStatus = ConditionsDB.Conditions[conditionId];
        ElementStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{pokemonBase.PokemonName}{ElementStatus.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureElementStatus()
    {
        ElementStatus = null;
        OnStatusChanged?.Invoke();
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
        ElementStatus = null;
        ResetStatBoost();
    }

}