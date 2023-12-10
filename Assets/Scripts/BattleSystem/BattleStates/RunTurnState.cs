using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RunTurnState : State<BattleSystem>
{
    public static RunTurnState I { get; private set; }

    private BattleSystem _battleSystem;

    private BattleUnit _playerUnit;
    private BattleUnit _enemyUnit;
    private BattleDialogueBox _dialogueBox;
    private PartyScreen _partyScreen;
    private bool _isTrainerBattle;
    private PokemonParty _playerParty;
    private Pokemon _wildPokemon;
    private PokemonParty _trainerParty;

    private bool _isRunSuccessful;
    public bool EnemyContinue;

    public bool SkipEnemyTurn = false;
    public bool SkipPlayerTurn = false;


    private void Awake()
    {
        I = this;
    }

    public override void Enter(BattleSystem owner)
    {
        _battleSystem = owner;
        _playerUnit = owner.PlayerUnit;
        _enemyUnit = owner.EnemyUnit;
        _dialogueBox = owner.DialogueBox;
        _partyScreen = owner.PartyScreen;
        _isTrainerBattle = owner.IsTrainerBattle;
        _playerParty = owner.PlayerParty;
        _wildPokemon = owner.WildPokemon;
        _trainerParty = owner.TrainerParty;
        _isRunSuccessful = false;
        EnemyContinue = false;
        StartCoroutine(RunTurns(_battleSystem.SelectedAction));
}

    public override void Execute()
    {
        base.Execute();
    }

    public override void Exit(bool sfx = true)
    {
        base.Exit(sfx);
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        if (playerAction == BattleAction.Move)
        {
            _playerUnit.pokemon.CurrentMove = _playerUnit.pokemon.Moves[_battleSystem.SelectedMove];

            if (_enemyUnit.IsAccumulating) // 之前使用了蓄力技能，还在蓄力中
            {
                if (_enemyUnit.CurAccumulatePower > 0)
                {
                    _enemyUnit.CurAccumulatePower -= 1;
                    yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}正在积蓄力量！！");
                }
                else
                {
                    _enemyUnit.IsAccumulating = false;
                }
            }
            else // 之前未使用蓄力技能
            {
                _enemyUnit.pokemon.CurrentMove = _enemyUnit.pokemon.GetRandomMove();
                // 如果使用了蓄力技能
                if (_enemyUnit.pokemon.CurrentMove.MoveBase.AccumulatePower > 0)
                {
                    _enemyUnit.CurAccumulatePower = _enemyUnit.pokemon.CurrentMove.MoveBase.AccumulatePower - 1;
                    yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}开始蓄力！！");
                    _enemyUnit.IsAccumulating = true;
                }
                else
                {
                    _enemyUnit.IsAccumulating = false;
                }
            }

            int playerMovePriority = _playerUnit.pokemon.CurrentMove.MoveBase.Priority;
            int enemyMovePriority = _enemyUnit.pokemon.CurrentMove.MoveBase.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (playerMovePriority < enemyMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (playerMovePriority != 10 && playerMovePriority == enemyMovePriority)
            {
                playerGoesFirst = _playerUnit.pokemon.Speed >= _enemyUnit.pokemon.Speed;
            }

            var firstUnit = playerGoesFirst ? _playerUnit : _enemyUnit;
            var secondUnit = playerGoesFirst ? _enemyUnit : _playerUnit;

            var secondPokemon = secondUnit.pokemon;

            // First turn
            if (firstUnit.IsPlayerUnit || !firstUnit.IsAccumulating) // 玩家，或者敌人没有蓄力，否则什么也不做
            {
                yield return RunMove(firstUnit, secondUnit, firstUnit.pokemon.CurrentMove);
                yield return RunAfterTurn(firstUnit);
                if (!firstUnit.IsPlayerUnit && BattleState.I.BossType == BossType.leijun) // 敌人是雷电将军boss
                {
                    var enemyMove2 = firstUnit.pokemon.GetRandomMove();
                    yield return RunMove(firstUnit, secondUnit, enemyMove2);
                }
                if (_battleSystem.IsBattleOver) yield break;
            }

            if (secondPokemon.Hp > 0)
            {
                // Second turn
                if (secondUnit.IsPlayerUnit || !secondUnit.IsAccumulating) //玩家，或者敌人没有蓄力，否则什么也不做
                {
                    yield return RunMove(secondUnit, firstUnit, secondUnit.pokemon.CurrentMove);
                    yield return RunAfterTurn(secondUnit);
                    if (!secondUnit.IsPlayerUnit && BattleState.I.BossType == BossType.leijun)
                    {
                        var enemyMove2 = secondUnit.pokemon.GetRandomMove();
                        yield return RunMove(secondUnit, firstUnit, enemyMove2);
                    }
                    if (_battleSystem.IsBattleOver) yield break;
                }
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                _dialogueBox.EnableActionSelector(false);
                yield return _battleSystem.SwitchPokemon(_battleSystem.SelectedPokemon);
                _battleSystem.StateMachine.ChangeState(ActionSelectionState.I);
                yield break;
            }
            else if (playerAction == BattleAction.UseItem)
            {
                _dialogueBox.EnableActionSelector(false);

                if (_battleSystem.SelectedItem is PokeballItem)
                {
                    yield return _battleSystem.ThrowPokeball(_battleSystem.SelectedItem as PokeballItem);
                    if (!EnemyContinue)
                    {
                        yield break;
                    }
                }

            }
            else if (playerAction == BattleAction.Run)
            {
                _dialogueBox.EnableActionSelector(false);
                yield return TryToEspace();
                if (_isRunSuccessful)
                {
                    yield break;
                }
            }

            // Enemy Turn
            if (!SkipEnemyTurn)
            {
                if (!_enemyUnit.IsAccumulating)
                {
                    var enemyMove = _enemyUnit.pokemon.GetRandomMove();
                    yield return RunMove(_enemyUnit, _playerUnit, enemyMove);

                    if (BattleState.I.BossType == BossType.leijun)
                    {
                        var enemyMove2 = _enemyUnit.pokemon.GetRandomMove();
                        yield return RunMove(_enemyUnit, _playerUnit, enemyMove2);
                    }

                    yield return RunAfterTurn(_enemyUnit);
                    if (_battleSystem.IsBattleOver) yield break;
                }
            }
        }

        // Back to selecting actions
        if (!_battleSystem.IsBattleOver)
        {
            _battleSystem.StateMachine.ChangeState(ActionSelectionState.I);
        }
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        if (sourceUnit.IsPlayerUnit && move.PP <= 0)
        {
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}没有PP使用\n{move.MoveBase.MoveName}！");
            yield break;
        }
        bool canRunMove = sourceUnit.pokemon.CanPerformMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.pokemon);

        if (move.MoveBase.MoveBGM != BGM.NONE)
        {
            AudioManager.Instance.PlayMusicVolume(move.MoveBase.MoveBGM, volume: 0.7f, fade: true);
        }

        if (move.MoveBase.Category != MoveCategory.Status)
        {
            sourceUnit.PlayAttackAnimation();
        }

        move.PP -= 1;
        yield return _dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}使用了\n{move.MoveBase.MoveName}！");

        if (CheckIfMoveHits(move, sourceUnit.pokemon, targetUnit.pokemon))
        {
            if (move.MoveBase.MoveFX != null)
                targetUnit.PlayPerformMoveAnimation(move.MoveBase.MoveFX.MoveEffectSprites);
            if (move.MoveBase.Target == MoveTarget.Foe)
            {
                targetUnit.PlayHitAnimation();
            }

            bool isElementReaction = false;
            if (move.MoveBase.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveBase, move.MoveBase.Effects, sourceUnit, targetUnit, move.MoveBase.Target);
            }
            else
            {
                if (move.MoveBase.Category == MoveCategory.Healing)
                {
                    if (sourceUnit.IsPlayerUnit)
                    {
                        foreach (var pokemon in _playerParty.Pokemons)
                        {
                            pokemon.IncreaseHP(move.MoveBase.Power);
                        }
                        yield return _dialogueBox.TypeDialogue($"己方所有宝可梦恢复了{move.MoveBase.Power}HP！");
                    }
                    else
                    {
                        if (_isTrainerBattle)
                        {
                            foreach (var pokemon in _trainerParty.Pokemons)
                            {
                                pokemon.IncreaseHP(move.MoveBase.Power);
                            }
                            yield return _dialogueBox.TypeDialogue($"对方所有宝可梦恢复了{move.MoveBase.Power}HP！");
                        }
                        else
                        {
                            _enemyUnit.pokemon.IncreaseHP(move.MoveBase.Power);
                            yield return _dialogueBox.TypeDialogue($"对方恢复了{move.MoveBase.Power}HP！");
                        }

                    }

                    yield return new WaitForSeconds(1f);
                }
                else
                {
                    DamageDetails damageDetails = targetUnit.pokemon.TakeDamage(move, sourceUnit.pokemon);
                    isElementReaction = damageDetails.IsElementReaction;
                    if (damageDetails.Effectiveness > 1f)
                    {
                        AudioManager.Instance.PlaySE(SFX.EFFICIENT_ATTACK);
                    }
                    else if (damageDetails.Damage == 0 || damageDetails.Effectiveness < 1f)
                    {
                        AudioManager.Instance.PlaySE(SFX.LOW_ATTACK);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySE(SFX.ATTACK);
                    }
                    yield return targetUnit.Hud.WaitForHPUpdate();
                    yield return ShowDamageDetials(sourceUnit.pokemon, targetUnit.pokemon, damageDetails);
                }

                if (targetUnit.pokemon.ElementStatus == null && move.MoveBase.Effects != null && targetUnit.pokemon.Status?.Id != ConditionID.jiejing && targetUnit.pokemon.Hp > 0)
                {
                    yield return RunMoveEffects(move.MoveBase, move.MoveBase.Effects, sourceUnit, targetUnit, move.MoveBase.Target, isElementReaction);
                }
            }

            if (targetUnit.pokemon.ElementStatus == null && move.MoveBase.SecondaryEffects != null && move.MoveBase.SecondaryEffects.Count > 0
                && targetUnit.pokemon.Hp > 0)
            {
                foreach (var secondary in move.MoveBase.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                    {
                        yield return RunMoveEffects(move.MoveBase, secondary, sourceUnit, targetUnit, secondary.Target);
                    }
                }
            }

            if (targetUnit.pokemon.Hp <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}\n的攻击没有命中！");
        }
    }

    private IEnumerator RunMoveEffects(MoveBase moveBase, MoveEffects effects, BattleUnit source, BattleUnit target, MoveTarget moveTarget, bool isElementReaction=false)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.pokemon.ApplyBoosts(effects.Boosts, source.OnBoostEffect);
            }
            else
            {
                target.pokemon.ApplyBoosts(effects.Boosts, target.OnBoostEffect);
            }
        }

        if (effects.Status != ConditionID.none)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.pokemon.SetStatus(effects.Status);
            }
            else
            {
                target.pokemon.SetStatus(effects.Status);
            }
        }

        if (!isElementReaction && effects.ElementStatus != ConditionID.none)
        {
            if (!(target.pokemon.PokemonBase.IsSlime && moveBase.Type == target.pokemon.PokemonBase.Type1))
            {
                target.pokemon.SetElementStatus(effects.ElementStatus);
            }
        }

        yield return ShowStatusChanges(source.pokemon);
        yield return ShowStatusChanges(target.pokemon);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (_battleSystem.IsBattleOver) yield break;

        sourceUnit.pokemon.AfterTurn();
        yield return ShowStatusChanges(sourceUnit.pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (!sourceUnit.IsPlayerUnit && BattleState.I.BossType == BossType.cao && !sourceUnit.OnSecondPhase)
        {
            sourceUnit.OnSecondPhase = true;
            yield return new WaitForSeconds(0.5f);
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}进入了狂暴状态！！");
            yield return new WaitForSeconds(0.25f);
            sourceUnit.pokemon.IncreaseHP(sourceUnit.pokemon.MaxHp / 2);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            var move = sourceUnit.pokemon.BossSpecialMove;
            yield return RunMove(_enemyUnit, _playerUnit, move);
        }
        if (sourceUnit.pokemon.Hp <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
        }
    }

    public bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.MoveBase.AlwaysHits)
        {
            return true;
        }

        if (BattleState.I.BossType == BossType.langwang)
        {
            return false;
        }

        float moveAccuracy = move.MoveBase.Accuracy;
        int speedDelta = Mathf.Clamp(source.Speed - target.Speed, 0, 100);

        return Random.Range(1, 101) <= moveAccuracy + speedDelta * Random.Range(0f, 1f);
    }

    // Display conditons message to the dialogue box
    private IEnumerator ShowStatusChanges(Pokemon pokemom)
    {
        while (pokemom.StatusChanges.Count > 0)
        {
            string message = pokemom.StatusChanges.Dequeue();
            yield return _dialogueBox.TypeDialogue(message);
        }
    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        AudioManager.Instance.PlaySE(SFX.FAINTED);
        faintedUnit.PlayFaintAnimation();
        yield return _dialogueBox.TypeDialogue($"{faintedUnit.pokemon.PokemonBase.PokemonName}\n倒下了！");

        yield return new WaitForSeconds(1f);

        if (!faintedUnit.IsPlayerUnit)
        {
            // Exp gain
            int expYield = faintedUnit.pokemon.PokemonBase.ExpYield;
            int enemyLevel = faintedUnit.pokemon.Level;
            float trainerBonus = (_isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / Random.Range(5.9f, 6.05f));
            foreach (var pokemon in _playerParty.Pokemons)
            {
                if (pokemon.Level < 100)
                    pokemon.Exp += expGain;
            }

            yield return _dialogueBox.TypeDialogue($"队伍中的所有宝可梦\n各自获得了{expGain}点经验值！");
            yield return _playerUnit.Hud.SetExpSmooth();

            // Check level up
            foreach (var pokemon in _playerParty.Pokemons)
            {
                while (pokemon.CheckForLevelUp())
                {
                    if (pokemon == _playerUnit.pokemon)
                    {
                        _playerUnit.Hud.SetLevel();
                    }
                    AudioManager.Instance.PlaySE(SFX.LEVEL_UP, true);
                    yield return _dialogueBox.TypeDialogue($"{pokemon.PokemonBase.PokemonName}升级了！\n等级提升至{pokemon.Level}！");

                    // Try to learn a new Move
                    var newMove = pokemon.GetLearnableMoveAtCurrentLevel();
                    if (newMove != null)
                    {
                        if (pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                        {
                            pokemon.LearnMove(newMove.MoveBase);
                            yield return _dialogueBox.TypeDialogue($"{pokemon.PokemonBase.PokemonName}习得了新技能\n{newMove.MoveBase.MoveName}！");
                        }
                        else
                        {
                            yield return _dialogueBox.TypeDialogue($"{pokemon.PokemonBase.PokemonName}想要学习{newMove.MoveBase.MoveName}...");
                            yield return _dialogueBox.TypeDialogue($"但是{pokemon.PokemonBase.PokemonName}掌握的技能太多了！");
                            yield return _dialogueBox.TypeDialogue($"想要让{pokemon.PokemonBase.PokemonName}\n遗忘哪个技能？");

                            MoveToForgetState.I.NewMove = newMove.MoveBase;
                            MoveToForgetState.I.CurrentMoves = pokemon.Moves.Select(m => m.MoveBase).ToList();
                            yield return GameManager.Instance.StateMachine.PushAndWait(MoveToForgetState.I);

                            int moveIndex = MoveToForgetState.I.Selection;
                            if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                            {
                                // Don't learn the new move
                                yield return _dialogueBox.TypeDialogue($"{pokemon.PokemonBase.PokemonName}放弃学习{newMove.MoveBase.MoveName}！");
                            }
                            else
                            {
                                // Forget the selected move and learn new move
                                var selevtedMove = pokemon.Moves[moveIndex].MoveBase;
                                yield return _dialogueBox.TypeDialogue($"{pokemon.PokemonBase.PokemonName}忘掉了{selevtedMove.MoveName}！");
                                pokemon.Moves[moveIndex] = new Move(newMove.MoveBase);
                            }
                        }
                    }
                    if (pokemon == _playerUnit.pokemon)
                        yield return _playerUnit.Hud.SetExpSmooth(true);
                }

            }
            
            yield return new WaitForSeconds(1f);
        }

        yield return CheckForBattleOver(faintedUnit);
    }

    private IEnumerator CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            Pokemon nextPokemon = _playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                yield return GameManager.Instance.StateMachine.PushAndWait(PartyState.I);
                yield return _battleSystem.SwitchPokemon(PartyState.I.SelectedPokemon);
            }
            else
            {
                yield return _dialogueBox.TypeDialogue("莹手上已经没有\n可以上场的宝可梦！");
                yield return _dialogueBox.TypeDialogue("莹眼前一黑...");
                yield return Fader.FadeIn(1f);
                yield return TeleportManager.Instance.GameOverTransport();
                yield return _battleSystem.BattleOver(false);
                

            }
        }
        else
        {
            if (!_isTrainerBattle)
            {
                yield return _battleSystem.BattleOver(true);
            }
            else
            {
                var nextPokemon = _trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    // Send out next pokemon
                    AboutToUseState.I.NewPokemon = nextPokemon;
                    yield return _battleSystem.StateMachine.PushAndWait(AboutToUseState.I);
                }
                else
                {
                    yield return _battleSystem.BattleOver(true);
                }
            }

        }
    }

    private IEnumerator ShowDamageDetials(Pokemon sourceUnit, Pokemon targetUnit, DamageDetails damageDetails)
    {
        if (damageDetails.Damage == 0)
        {
            yield return _dialogueBox.TypeDialogue($"{targetUnit.PokemonBase.PokemonName}免疫了此元素攻击！");
            yield break;
        }
        if (damageDetails.IsKuosan)
        {
            yield return _dialogueBox.TypeDialogue("扩散的元素反应发生了！");
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.PokemonBase.PokemonName}的元素附着\n被转移到了{targetUnit.PokemonBase.PokemonName}！");
        }
        else if (damageDetails.IsNoneElement)
        {
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.PokemonBase.PokemonName}的攻击附加了\n自身的元素附着！");
        }

        if (damageDetails.IsPsn || damageDetails.IsPar || damageDetails.IsCfs || damageDetails.IsSlp || damageDetails.IsFrz || damageDetails.IsBrn)
        {
            yield return _dialogueBox.TypeDialogue($"打出了元素反应！");
        }
        else if (damageDetails.IsZhanfang)
        {
            yield return _dialogueBox.TypeDialogue("绽放的元素反应发生了！");
            yield return _dialogueBox.TypeDialogue($"从{targetUnit.PokemonBase.PokemonName}吸取了HP！");
            _playerUnit.pokemon.IncreaseHP((int)(damageDetails.Damage * 0.5f));
            yield return new WaitForSeconds(0.5f);
        }
        else if (damageDetails.IsZhengfa)
        {
            yield return _dialogueBox.TypeDialogue($"蒸发的元素反应发生了！\n对{targetUnit.PokemonBase.PokemonName}造成了大量伤害！");
        }
        else if (damageDetails.IsRonghua)
        {
            yield return _dialogueBox.TypeDialogue($"融化的元素反应发生了！\n对{targetUnit.PokemonBase.PokemonName}造成了大量伤害！");
        }

        if (damageDetails.Effectiveness > 1f)
        {
            yield return _dialogueBox.TypeDialogue("并且，效果拔群！");
        }
        else if (damageDetails.Effectiveness < 1f)
        {
            yield return _dialogueBox.TypeDialogue("不过，效果一般！");
        }

    }

    private IEnumerator TryToEspace()
    {

        if (_isTrainerBattle || BattleState.I.BossPokemon != null)
        {
            _isRunSuccessful = true;
            yield return _dialogueBox.TypeDialogue($"你不能从这场战斗中逃跑！");
            _battleSystem.StateMachine.ChangeState(ActionSelectionState.I);
            yield break;
        }

        ++_battleSystem.EscapeAttempts;

        int playerSpeed = _playerUnit.pokemon.Speed;
        int enemySpeed = _enemyUnit.pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            AudioManager.Instance.PlaySE(SFX.ESCAPE);
            _isRunSuccessful = true;
            yield return _dialogueBox.TypeDialogue($"成功逃跑了！");
            yield return _battleSystem.BattleOver(false);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * _battleSystem.EscapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                AudioManager.Instance.PlaySE(SFX.ESCAPE);
                _isRunSuccessful = true;
                yield return _dialogueBox.TypeDialogue($"成功逃跑了！");
                yield return _battleSystem.BattleOver(false);
            }
            else
            {
                _isRunSuccessful = false;
                yield return _dialogueBox.TypeDialogue($"逃跑失败了！");
            }
        }
    }
}
