using PokeGenshinUtils.StateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

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
            _enemyUnit.pokemon.CurrentMove = _enemyUnit.pokemon.GetRandomMove();

            int playerMovePriority = _playerUnit.pokemon.CurrentMove.MoveBase.Priority;
            int enemyMovePriority = _enemyUnit.pokemon.CurrentMove.MoveBase.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (playerMovePriority < enemyMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (playerMovePriority < enemyMovePriority)
            {
                playerGoesFirst = _playerUnit.pokemon.Speed >= _enemyUnit.pokemon.Speed;
            }

            var firstUnit = playerGoesFirst ? _playerUnit : _enemyUnit;
            var secondUnit = playerGoesFirst ? _enemyUnit : _playerUnit;

            var secondPokemon = secondUnit.pokemon;

            // First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (_battleSystem.IsBattleOver) yield break;

            if (secondPokemon.Hp > 0)
            {
                // Second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (_battleSystem.IsBattleOver) yield break;
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
                }

            }
            else if (playerAction == BattleAction.Run)
            {
                _dialogueBox.EnableActionSelector(false);
                yield return TryToEspace();
            }

            // Enemy Turn
            var enemyMove = _enemyUnit.pokemon.GetRandomMove();
            yield return RunMove(_enemyUnit, _playerUnit, enemyMove);
            yield return RunAfterTurn(_enemyUnit);
            if (_battleSystem.IsBattleOver) yield break;
        }

        // Back to selecting actions
        if (!_battleSystem.IsBattleOver)
        {
            _battleSystem.StateMachine.ChangeState(ActionSelectionState.I);
        }
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        bool canRunMove = sourceUnit.pokemon.CanPerformMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.pokemon);
            yield return sourceUnit.Hud.WaitForHPUpdate();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.pokemon);

        if (move.MoveBase.Category != MoveCategory.Status)
        {
            sourceUnit.PlayAttackAnimation();
        }

        move.PP -= 1;
        yield return _dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}ʹ����\n{move.MoveBase.MoveName}��");

        if (CheckIfMoveHits(move, sourceUnit.pokemon, targetUnit.pokemon))
        {
            if (move.MoveBase.Target == MoveTarget.Foe)
            {
                targetUnit.PlayHitAnimation();
            }

            bool isElementReaction = false;
            if (move.MoveBase.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveBase.Effects, sourceUnit.pokemon, targetUnit.pokemon, move.MoveBase.Target);
            }
            else
            {
                DamageDetails damageDetails = targetUnit.pokemon.TakeDamage(move, sourceUnit.pokemon);
                isElementReaction = damageDetails.IsElementReaction;
                if (damageDetails.Critical > 1f || damageDetails.TypeEffectiveness > 1f)
                {
                    AudioManager.Instance.PlaySE(SFX.EFFICIENT_ATTACK);
                }
                else if (damageDetails.TypeEffectiveness < 1f)
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
                yield return RunMoveEffects(move.MoveBase.Effects, sourceUnit.pokemon, targetUnit.pokemon, move.MoveBase.Target, isElementReaction);
            }

            if (targetUnit.pokemon.ElementStatus == null && move.MoveBase.SecondaryEffects != null && move.MoveBase.SecondaryEffects.Count > 0
                && targetUnit.pokemon.Hp > 0)
            {
                foreach (var secondary in move.MoveBase.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary, sourceUnit.pokemon, targetUnit.pokemon, secondary.Target);
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
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}\n�Ĺ���û�����У�");
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget, bool isElementReaction=false)
    {
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }

        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        if (!isElementReaction && effects.ElementStatus != ConditionID.none)
        {
            target.SetElementStatus(effects.ElementStatus);
        }

        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (_battleSystem.IsBattleOver) yield break;

        sourceUnit.pokemon.AfterTurn();
        yield return ShowStatusChanges(sourceUnit.pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
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

        float moveAccuracy = move.MoveBase.Accuracy;
        int accuracy = source.StatBoosts[Stat.������];
        int evasion = target.StatBoosts[Stat.������];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };

        moveAccuracy = accuracy > 0 ? moveAccuracy * boostValues[accuracy]
            : moveAccuracy / boostValues[-accuracy];

        moveAccuracy = evasion > 0 ? moveAccuracy / boostValues[evasion]
            : moveAccuracy * boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
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
        yield return _dialogueBox.TypeDialogue($"{faintedUnit.pokemon.PokemonBase.PokemonName}\n�����ˣ�");

        yield return new WaitForSeconds(1f);

        if (!faintedUnit.IsPlayerUnit)
        {
            // Exp gain
            int expYield = faintedUnit.pokemon.PokemonBase.ExpYield;
            int enemyLevel = faintedUnit.pokemon.Level;
            float trainerBonus = (_isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            _playerUnit.pokemon.Exp += expGain;

            yield return _dialogueBox.TypeDialogue($"{_playerUnit.pokemon.PokemonBase.PokemonName}�����\n{expGain}�㾭��ֵ��");
            yield return _playerUnit.Hud.SetExpSmooth();

            // Check level up
            while (_playerUnit.pokemon.CheckForLevelUp())
            {
                _playerUnit.Hud.SetLevel();
                AudioManager.Instance.PlaySE(SFX.LEVEL_UP, true);
                yield return _dialogueBox.TypeDialogue($"{_playerUnit.pokemon.PokemonBase.PokemonName}�����ˣ�\n�ȼ�������{_playerUnit.pokemon.Level}��");

                // Try to learn a new Move
                var newMove = _playerUnit.pokemon.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if (_playerUnit.pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        _playerUnit.pokemon.LearnMove(newMove.MoveBase);
                        yield return _dialogueBox.TypeDialogue($"{_playerUnit.pokemon.PokemonBase.PokemonName}ϰ�����¼���\n{newMove.MoveBase.MoveName}��");
                    }
                    else
                    {
                        yield return _dialogueBox.TypeDialogue($"{_playerUnit.pokemon.PokemonBase.PokemonName}��Ҫѧϰ{newMove.MoveBase.MoveName}...");
                        yield return _dialogueBox.TypeDialogue($"����{_playerUnit.pokemon.PokemonBase.PokemonName}���յļ���̫���ˣ�");
                        yield return _dialogueBox.TypeDialogue($"��Ҫ��{_playerUnit.pokemon.PokemonBase.PokemonName}\n�����ĸ����ܣ�");

                        MoveToForgetState.I.NewMove = newMove.MoveBase;
                        MoveToForgetState.I.CurrentMoves = _playerUnit.pokemon.Moves.Select(m => m.MoveBase).ToList();
                        yield return GameManager.Instance.StateMachine.PushAndWait(MoveToForgetState.I);

                        int moveIndex = MoveToForgetState.I.Selection;
                        if (moveIndex == PokemonBase.MaxNumOfMoves || moveIndex == -1)
                        {
                            // Don't learn the new move
                            yield return _dialogueBox.TypeDialogue($"{_playerUnit.pokemon.PokemonBase.PokemonName}����ѧϰ{newMove.MoveBase.MoveName}��");
                        }
                        else
                        {
                            // Forget the selected move and learn new move
                            var selevtedMove = _playerUnit.pokemon.Moves[moveIndex].MoveBase;
                            yield return _dialogueBox.TypeDialogue($"{_playerUnit.pokemon.PokemonBase.PokemonName}������{selevtedMove.MoveName}��");
                            _playerUnit.pokemon.Moves[moveIndex] = new Move(newMove.MoveBase);
                        }
                    }
                }

                yield return _playerUnit.Hud.SetExpSmooth(true);
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
        if (damageDetails.IsKuosan)
        {
            yield return _dialogueBox.TypeDialogue("��ɢ��Ԫ�ط�Ӧ�����ˣ�");
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.PokemonBase.PokemonName}��Ԫ�ظ���\n��ת�Ƶ���{targetUnit.PokemonBase.PokemonName}��");
        }
        else if (damageDetails.IsNoneElement)
        {
            yield return _dialogueBox.TypeDialogue($"{sourceUnit.PokemonBase.PokemonName}�Ĺ���������\n�����Ԫ�ظ��ţ�");
        }

        if (damageDetails.IsPsn || damageDetails.IsPar || damageDetails.IsCfs || damageDetails.IsSlp || damageDetails.IsFrz || damageDetails.IsBrn)
        {
            yield return _dialogueBox.TypeDialogue($"�����Ԫ�ط�Ӧ��");
        }
        else if (damageDetails.IsZhanfang)
        {
            yield return _dialogueBox.TypeDialogue("���ŵ�Ԫ�ط�Ӧ�����ˣ�");
            yield return _dialogueBox.TypeDialogue($"��{targetUnit.PokemonBase.PokemonName}��ȡ��HP��");
            _playerUnit.pokemon.IncreaseHP((int)(damageDetails.Damage * 0.3f));
        }
        else if (damageDetails.IsZhengfa)
        {
            yield return _dialogueBox.TypeDialogue($"������Ԫ�ط�Ӧ�����ˣ�\n��{targetUnit.PokemonBase.PokemonName}����˴����˺���");
        }
        else if (damageDetails.IsRonghua)
        {
            yield return _dialogueBox.TypeDialogue($"�ڻ���Ԫ�ط�Ӧ�����ˣ�\n��{targetUnit.PokemonBase.PokemonName}����˴����˺���");
        }

        if (damageDetails.TypeEffectiveness > 1f)
        {
            yield return _dialogueBox.TypeDialogue("ţ�ƣ�Ч����Ⱥ��");
        }
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return _dialogueBox.TypeDialogue("����Ч��һ�㣡");
        }

    }

    private IEnumerator TryToEspace()
    {

        if (_isTrainerBattle)
        {
            yield return _dialogueBox.TypeDialogue($"�㲻�ܴ��ⳡս�������ܣ�");
            yield break;
        }

        ++_battleSystem.EscapeAttempts;

        int playerSpeed = _playerUnit.pokemon.Speed;
        int enemySpeed = _enemyUnit.pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            AudioManager.Instance.PlaySE(SFX.ESCAPE);
            yield return _dialogueBox.TypeDialogue($"�ɹ������ˣ�");
            yield return _battleSystem.BattleOver(false);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * _battleSystem.EscapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                AudioManager.Instance.PlaySE(SFX.ESCAPE);
                yield return _dialogueBox.TypeDialogue($"�ɹ������ˣ�");
                yield return _battleSystem.BattleOver(false);
            }
            else
            {
                yield return _dialogueBox.TypeDialogue($"����ʧ���ˣ�");
            }
        }
    }
}
