using Game.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase pokemonBase;
    [SerializeField] private int level;
    private string _catchPlace;
    private int[] _statusBias;
    private int[] _exStatusBias;

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

    public Move BossSpecialMove;
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition ElementStatus { get; private set; }
    public int ElementStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public PokeballType PokeballSpriteType;
    public string CatchPlace { get => _catchPlace; set => _catchPlace = value; }

    public event System.Action OnStatusChanged;
    public event System.Action OnHpChanged;
    public event System.Action<Stat, int> OnBuffChanged;

    public Pokemon(PokemonSaveData saveData)
    {
        pokemonBase = PokemonDB.GetObjectByName(saveData.PokemonName);
        Hp = saveData.Hp;
        Level = saveData.Level;
        Exp = saveData.Exp;
        _statusBias = saveData.StatusBias;
        _exStatusBias = saveData.EXStatusBias;
        PokeballSpriteType = saveData.PokeBall;

        if (saveData.StatusId != null)
        {
            Status = ConditionsDB.Conditions[saveData.StatusId.Value];
        }
        else
        {
            Status = null;
        }

        Moves = saveData.Moves.Select(s => new Move(s)).ToList();
        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        ElementStatus = null;
    }

    // Initialize the pokemon
    public void Init()
    {
        _statusBias = new int[6];
        _exStatusBias = new int[6];
        System.Random ran = new System.Random();
        for (int i = 0; i < _statusBias.Length; ++i)
        {
            _statusBias[i] = ran.Next(0, 11);

        }
        _statusBias[5] = ran.Next(10, 21);
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
        if (pokemonBase.BossSpecialMove.MoveBase != null)
        {
            BossSpecialMove = new Move(pokemonBase.BossSpecialMove.MoveBase);
        }

        CalculateStats();

        Hp = MaxHp;
        Exp = pokemonBase.GetExpForLevel(Level);
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        ElementStatus = null;
        pokemonBase.InitPassiveMove();
    }

    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            PokemonName = pokemonBase.name,
            Hp = Hp,
            Level = Level,
            Exp = Exp,
            StatusId = Status?.Id,
            Moves = Moves.Select(m => m.GetSaveData()).ToList(),
            StatusBias = _statusBias,
            EXStatusBias = _exStatusBias,
            PokeBall = PokeballSpriteType
        };

        if (pokemonBase.PassiveMove != null)
        {
            saveData.PassiveMoveName = pokemonBase.PassiveMove.MoveName;
        }
        return saveData;
    }

    // Calculate the pokemon's status by based status
    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>
            {
                { Stat.攻击, Mathf.FloorToInt(pokemonBase.Attack * Level / 100f) + _statusBias[0] + _exStatusBias[0] },
                { Stat.防御, Mathf.FloorToInt(pokemonBase.Defense * Level / 100f) + _statusBias[1] + _exStatusBias[1]},
                { Stat.特攻, Mathf.FloorToInt(pokemonBase.SpAttack * Level / 100f) + _statusBias[2] + _exStatusBias[2]},
                { Stat.特防, Mathf.FloorToInt(pokemonBase.SpDefense * Level / 100f) + _statusBias[3] + _exStatusBias[3]},
                { Stat.速度, Mathf.FloorToInt(pokemonBase.Speed * Level / 100f) + _statusBias[4] + _exStatusBias[4]}
            };


        int oldMaxHp = MaxHp;
        MaxHp = Mathf.FloorToInt(pokemonBase.MaxHp * Level / 100f) + Level + _statusBias[5] + _exStatusBias[5];

        if (oldMaxHp != 0)
        {
            Hp += MaxHp - oldMaxHp;
        }
        
    }

    public bool IsBestStatus(Stat status)
    {
        if (status == Stat.生命)
        {
            return _statusBias[(int)status] == 20;
        }
        return _statusBias[(int)status] == 10;
    }

    public bool TryAddBias(List<int> boostValue)
    {
        bool isBoost = false;
        for (int i = 0; i < boostValue.Count - 1; ++i)
        {
            if (_statusBias[i] == 10 || boostValue[i] == 0)
            {
                continue;
            }
            if (_statusBias[i] + boostValue[i] >= 10)
            {
                _statusBias[i] = 10;
                isBoost = true;
            }
            else
            {
                _statusBias[i] += boostValue[i];
                isBoost = true;
            }

        }
        if (!(_statusBias[5] == 20 || boostValue[5] == 0))
        {
            if (_statusBias[5] + boostValue[5] >= 20)
            {
                _statusBias[5] = 20;
                isBoost = true;
            }
            else
            {
                _statusBias[5] += boostValue[5];
                isBoost = true;
            }
        }

        CalculateStats();
        return isBoost;
    }

    public void SetBestBias()
    {
        for (int i = 0; i < _statusBias.Length; ++i)
        {
            _statusBias[i] = 10;

        }
        _statusBias[5] = 20;
        CalculateStats();
    }

    public void AddExStatusBias(List<int> boostValue)
    {
        for (int i = 0; i < boostValue.Count; ++i)
        {
            _exStatusBias[i] += boostValue[i];
        }
        CalculateStats();
    }

    public int GetNextLevelExpLeft()
    {
        if (level == 100)
        {
            return 0;
        }
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
        if (level == 100)
        {
            return false;
        }
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
        return pokemonBase.Evolutions.FirstOrDefault(e => e.RequiredLevel <= level && e.RequiredItem == null);
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

    public void GameOverHeal()
    {
        Hp = 1;
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
        var boostValues = new float[] { 1f, 1.25f, 1.5f, 1.75f, 2f, 2.25f, 2.5f }; // 小幅 1 中幅2 大幅3 最大幅 6
        statVal = (boost >= 0) ? Mathf.FloorToInt(statVal * boostValues[boost]) 
            : Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    // Apply boosted satus and show the dialogues
    public void ApplyBoosts(List<StatBoost> statBoosts, Action<int> OnBoostEffect)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            if (StatBoosts[stat] + boost > 6 || StatBoosts[stat] + boost < -6)
            {
                if (boost > 0)
                {
                    StatusChanges.Enqueue($"{pokemonBase.PokemonName}的{stat}无法再上升了！");
                }
                else
                {
                    StatusChanges.Enqueue($"{pokemonBase.PokemonName}的{stat}无法再下降了！");
                }
            }
            else
            {
                StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

                if (boost > 0)
                {
                    AudioManager.Instance.PlaySE(SFX.BOOST);
                    StatusChanges.Enqueue($"{pokemonBase.PokemonName}的{stat}上升了！");
                }
                else
                {
                    AudioManager.Instance.PlaySE(SFX.BOOST_DOWN);
                    StatusChanges.Enqueue($"{pokemonBase.PokemonName}的{stat}下降了！");
                }

                OnBoostEffect?.Invoke(boost);
                OnBuffChanged?.Invoke(stat, StatBoosts[stat]);
            }


        }
    }

    // Calculate the damage when the pokemon get hurt
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {

        DamageDetails damageDetails = new DamageDetails()
        {
            Fainted = false,
            Effectiveness = 1f
        };

        float attack = move.MoveBase.Category == MoveCategory.Special ? attacker.SpAttack : attacker.Attack;
        float defense = move.MoveBase.Category == MoveCategory.Special ? SpDefense : Defense;

        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.MoveBase.Power * ((float)attack / defense) + 2;

        float elementReactionRate = 1f;
        float effectiveness = 1f;
        if (!(pokemonBase.IsSlime && move.MoveBase.Type == pokemonBase.Type1))
        {
            if (move.MoveBase.Effects.ElementStatus == ConditionID.none)
            {
                if (attacker.ElementStatus != null)
                {
                    damageDetails.IsNoneElement = true;
                    if (ElementStatus == null)
                    {
                        SetElementStatus(attacker.ElementStatus.Id, true);
                    }
                    else
                    {
                        move.MoveBase.Effects.ElementStatus = attacker.ElementStatus.Id;
                        elementReactionRate = CheckElementReaction(ElementStatus, move, attacker, damageDetails, out effectiveness);
                        move.MoveBase.Effects.ElementStatus = ConditionID.none;
                    }
                }
            }
            else
            {
                elementReactionRate = CheckElementReaction(ElementStatus, move, attacker, damageDetails, out effectiveness);
            }
        }
        damageDetails.Effectiveness = effectiveness;
        damageDetails.StatusName = Status?.Name;
        float modifiers = UnityEngine.Random.Range(0.85f, 1f);
        int damage = Mathf.FloorToInt(d * modifiers * elementReactionRate);
        if (pokemonBase.IsSlime && move.MoveBase.Type == pokemonBase.Type1)
        {
            damage = 0;
        }
        DecreaseHP(damage);
        damageDetails.Damage = damage;
        return damageDetails;
    }

    private float CheckElementReaction(Condition elementStatus, Move attackerMove, Pokemon attacker, DamageDetails damageDetails, out float effectiveness)
    {
        effectiveness = 1f;
        if (Status != null && Status.Id == ConditionID.jiejing)
        {
            return 1f;
        }
        
        if (attackerMove.MoveBase.Effects.ElementStatus == ConditionID.anemo)
        {
            if (attacker.ElementStatus != null)
            {
                damageDetails.IsKuosan = true;
                if (ElementStatus != null)
                {
                    float elementReactionRate = PerformElementReaction(ElementStatus.Id, attacker.ElementStatus.Id, damageDetails, out effectiveness);
                    attacker.CureElementStatus();
                    return elementReactionRate;
                }
                else
                {
                    SetElementStatus(attacker.ElementStatus.Id);
                }
            }
            attacker.CureElementStatus();
        }
        else if (ElementStatus != null && attackerMove.MoveBase.Effects.ElementStatus == ConditionID.geo)
        {
            if (ElementStatus != null)
            {
                CureElementStatus();
            }
            attacker.SetStatus(ConditionID.jiejing);
            damageDetails.IsJiejing = true;
        }
        else if (elementStatus != null)
        {
            return PerformElementReaction(elementStatus.Id, attackerMove.MoveBase.Effects.ElementStatus, damageDetails, out effectiveness);
        }

        return 1f;
    }

    private float PerformElementReaction(ConditionID element, ConditionID attackerElement, DamageDetails damageDetails, out float effectiveness)
    {
        effectiveness = 1f;
        if (element == attackerElement)
        {
            ElementStatusTime += 2;
            return effectiveness;
        }
        ConditionID elementReactionRes = ConditionsDB.GetElementReaction(element, attackerElement);
        damageDetails.IsElementReaction = true;
        if (pokemonBase.PassiveMove != null)
        {
            var passiveType = ElementReactionUtil.ConditionIDToPassiveType(elementReactionRes);
            if (pokemonBase.EffectivenessData.TryGetValue(passiveType, out float res))
            {
                effectiveness *= res;
            }
        }
        damageDetails.Effectiveness = effectiveness;
        if (elementReactionRes == ConditionID.psn)
        {
            CureElementStatus();
            SetStatus(elementReactionRes);
            damageDetails.IsPsn = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.brn)
        {
            CureElementStatus();
            SetStatus(elementReactionRes);
            damageDetails.IsBrn = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.par)
        {
            CureElementStatus();
            SetStatus(elementReactionRes);
            damageDetails.IsPar = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.slp)
        {
            CureElementStatus();
            SetStatus(elementReactionRes);
            damageDetails.IsSlp = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.confusion)
        {
            CureElementStatus();
            SetStatus(elementReactionRes);
            damageDetails.IsCfs = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.frz)
        {
            CureElementStatus();
            SetStatus(elementReactionRes);
            damageDetails.IsFrz = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.zhanfang)
        {
            CureElementStatus();
            damageDetails.IsZhanfang = true;
            return 1.25f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.zhengfa)
        {
            CureElementStatus();
            damageDetails.IsZhengfa = true;
            return 1.75f * effectiveness;
        }
        else if (elementReactionRes == ConditionID.ronghua)
        {
            CureElementStatus();
            damageDetails.IsRonghua = true;
            return 1.75f * effectiveness;
        }
        SetElementStatus(attackerElement);
        return effectiveness;
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

    public void SetElementStatus(ConditionID conditionId, bool putongMove=false)
    {
        var prevElementStatus = ElementStatus;
        if (conditionId == ConditionID.geo || conditionId == ConditionID.anemo) return;
        ElementStatus = ConditionsDB.Conditions[conditionId];
        ElementStatus?.OnStart?.Invoke(this);
        if (prevElementStatus == null || prevElementStatus?.Id != ElementStatus.Id)
        {
            if (!putongMove)
            {
                StatusChanges.Enqueue($"{pokemonBase.PokemonName}{ElementStatus.StartMessage}");
            }
        }

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

        int r = UnityEngine.Random.Range(0, movesWithPP.Count);
        return movesWithPP[r];
    }

    public void OnBattleOver()
    {
        ElementStatus = null;
        ResetStatBoost();
    }

}