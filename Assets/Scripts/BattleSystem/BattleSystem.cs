using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    Bag,
    PartyScreen,
    AboutToUse,
    MoveToForget,
    BattleOver
}

public enum BattleAction 
{ 
    Move, 
    SwitchPokemon, 
    UseItem, 
    Run 
}

public enum BattleTrigger { LongGrass, Water }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    [SerializeField] private GameObject pokeballSprite;
    [SerializeField] private MoveSelectionUI moveSelectionUI;
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField] List<Sprite> backgroundImages;
    [SerializeField] List<Sprite> enemyGroundSprites;
    [SerializeField] List<Sprite> playerGroundSprites;
    [SerializeField] Image backgroundImage;

    public BattleState state;
    
    public int currentAction;
    public int currentMove;
    private bool aboutToUseChoice = true;

    public event Action<bool> OnBattleOver;

    public PokemonParty playerParty;
    public Pokemon wildPokemon;
    public PokemonParty trainerParty;

    private bool isTrainerBattle = false;
    PlayerController player;
    TrainerController trainer;

    int escapeAttempts;
    MoveBase moveToLearn;

    private BattleTrigger battleTrigger;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger=BattleTrigger.LongGrass)
    {
        this.isTrainerBattle = false;
        AudioManager.instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        battleTrigger = trigger;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        AudioManager.instance.PlayMusic(BGM.BATTLE_TRAINER);
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        this.isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        battleTrigger = trigger;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.SetDefaultPlayerSprite();
        playerUnit.HideHud();
        enemyUnit.HideHud();

        backgroundImage.sprite = backgroundImages[((int)battleTrigger)];
        playerUnit.SetGroundImage(playerGroundSprites[((int)battleTrigger)]);
        enemyUnit.SetGroundImage(enemyGroundSprites[((int)battleTrigger)]);

        if (!isTrainerBattle)
        {
            // Wild Pokemon Battle

            // set up pokemons data
            playerUnit.ResetAnimation();
            enemyUnit.ResetAnimation();
            enemyUnit.SetUp(wildPokemon);
            playerUnit.UnitEnterAnimation();
            enemyUnit.UnitEnterAnimation();

            yield return dialogueBox.TypeDialogue($"野生的{enemyUnit.pokemon.PokemonBase.PokemonName}出现了！");
            yield return new WaitForSeconds(4f);
        }
        else
        {
            // Trianer Battle
            
            trainerImage.sprite = trainer.Sprite;
            playerUnit.ResetAnimation();
            enemyUnit.ResetAnimation();
            playerUnit.UnitEnterAnimation();
            enemyUnit.UnitEnterAnimation();

            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}想要进行宝可梦对战！");
            yield return new WaitForSeconds(4f);

            // Send out first pokemon of the trainer
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.ChangeUnit(enemyPokemon);
            AudioManager.instance.PlaySE(SFX.BALL_OUT);
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{enemyPokemon.PokemonBase.PokemonName}！");
            
            yield return new WaitForSeconds(2f);

            // Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            
        }
        partyScreen.Init();
        var playerPokemon = playerParty.GetHealthyPokemon();
        playerUnit.ChangeUnit(playerPokemon);
        yield return dialogueBox.TypeDialogue($"就决定是你了，\n{playerPokemon.PokemonBase.PokemonName}！");
        AudioManager.instance.PlaySE(SFX.BALL_OUT);
        dialogueBox.SetMoveNames(playerUnit.pokemon.Moves);
        yield return new WaitForSeconds(2f);

        playerUnit.ShowHud();
        enemyUnit.ShowHud();
        escapeAttempts = 0;
        ActionSelection();
    }

    private IEnumerator BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        if (won)
        {
            if (isTrainerBattle)
            {
                AudioManager.instance.PlayMusic(BGM.VICTORY_TRAINER);
                StartCoroutine(dialogueBox.TypeDialogue($"你打败了{trainer.TrainerName}！"));
            }
            else
            {
                AudioManager.instance.PlayMusic(BGM.VICTORY_WILD_POKEMON);
                StartCoroutine(dialogueBox.TypeDialogue($"你打败了{enemyUnit.pokemon.PokemonBase.PokemonName}！"));
            }
            
        }
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        yield return new WaitForSeconds(3f);
        OnBattleOver(won);
    }

    // Player turn
    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        playerUnit.Hud.gameObject.SetActive(true);
        enemyUnit.Hud.gameObject.SetActive(true);
        StartCoroutine(dialogueBox.TypeDialogue($"想要\n{playerUnit.pokemon.PokemonBase.PokemonName}做什么？"));
        dialogueBox.EnableActionSelector(true);
    }

    // Check action cursor and move cursor
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleState.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };

            inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxNumOfMoves)
                {
                    // Don't learn new move
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}放弃学习{moveToLearn.MoveName}！"));
                }
                else
                {
                    // Forget the selected move and learn new move
                    var selevtedMove = playerUnit.pokemon.Moves[moveIndex].MoveBase;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}忘掉了{selevtedMove.MoveName}！"));
                    playerUnit.pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleState.RunningTurn;
            };

            

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    // Press Z to open the move box
    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentAction += 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentAction -= 1;
        }

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.instance.PlaySE(SFX.CONFIRM);
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                OpenBag();
            }
            else if (currentAction == 2)
            {
                // Pokemon party
                OpenPartyScreen();

            }
            else if (currentAction == 3)
            {
                // Run
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }

    }

    // Open move box
    private void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    private IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}想要让{newPokemon.PokemonBase.PokemonName}上场！\n是否要更换当前出战宝可梦？");
        state = BattleState.AboutToUse;
        dialogueBox.EnableChoiceBox(true);
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleState.Busy;
        yield return dialogueBox.TypeDialogue($"想要让{pokemon.PokemonBase.PokemonName}\n遗忘哪个技能？");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.MoveBase).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleState.MoveToForget;
    }

    private void OpenBag()
    {
        state = BattleState.Bag;
        inventoryUI.gameObject.SetActive(true);
    }

    private void OpenPartyScreen()
    {
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
    }

    // Press Z to use the selected move
    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMove += 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMove -= 1;
        }

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.pokemon.Moves.Count - 1);

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = playerUnit.pokemon.Moves[currentMove];
            if (move.PP == 0) return;
            AudioManager.instance.PlaySE(SFX.CONFIRM);
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {
            playerUnit.pokemon.CurrentMove = playerUnit.pokemon.Moves[currentMove];
            enemyUnit.pokemon.CurrentMove = enemyUnit.pokemon.GetRandomMove();

            int playerMovePriority = playerUnit.pokemon.CurrentMove.MoveBase.Priority;
            int enemyMovePriority = enemyUnit.pokemon.CurrentMove.MoveBase.Priority;

            // Check who goes first
            bool playerGoesFirst = true;
            if (playerMovePriority < enemyMovePriority)
            {
                playerGoesFirst = false;
            } 
            else if (playerMovePriority < enemyMovePriority)
            {
                playerGoesFirst = playerUnit.pokemon.Speed >= enemyUnit.pokemon.Speed;
            }
            
            var firstUnit = playerGoesFirst ? playerUnit : enemyUnit;
            var secondUnit = playerGoesFirst ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.pokemon;

            // First turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.Hp > 0)
            {
                // Second turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
            
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                dialogueBox.EnableActionSelector(false);
                var selectedPokemon = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogueBox.EnableActionSelector(false);
            }
            else if (playerAction == BattleAction.Run)
            {
                dialogueBox.EnableActionSelector(false);
                yield return TryToEspace();
            }

            // Enemy Turn
            var enemyMove = enemyUnit.pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        // Back to selecting actions
        if (state != BattleState.BattleOver)
        {
            ActionSelection();
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
        yield return dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}使用了\n{move.MoveBase.MoveName}！");

        if (CheckIfMoveHits(move, sourceUnit.pokemon, targetUnit.pokemon))
        {
            if (move.MoveBase.Target == MoveTarget.Foe)
            {
                targetUnit.PlayHitAnimation();
            }


            if (move.MoveBase.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.MoveBase.Effects, sourceUnit.pokemon, targetUnit.pokemon, move.MoveBase.Target);
            }
            else
            {
                DamageDetails damageDetails = targetUnit.pokemon.TakeDamage(move, sourceUnit.pokemon);
                if (damageDetails.Critical > 1f)
                {
                    AudioManager.instance.PlaySE(SFX.EFFICIENT_ATTACK);
                }

                if (damageDetails.TypeEffectiveness > 1f)
                {
                    AudioManager.instance.PlaySE(SFX.EFFICIENT_ATTACK);
                }
                else if (damageDetails.TypeEffectiveness < 1f)
                {
                    AudioManager.instance.PlaySE(SFX.LOW_ATTACK);
                }
                else
                {
                    AudioManager.instance.PlaySE(SFX.ATTACK);
                }
                yield return targetUnit.Hud.WaitForHPUpdate();
                yield return ShowDamageDetials(damageDetails);
            }

            if (move.MoveBase.SecondaryEffects != null && move.MoveBase.SecondaryEffects.Count > 0 
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
            yield return dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}\n的攻击没有命中！");
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
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

            if (effects.Status != ConditionID.none)
            {
                target.SetStatus(effects.Status);
            }

            if (effects.VolatileStatus != ConditionID.none)
            {
                target.SetVolatileStatus(effects.VolatileStatus);
            }

            yield return ShowStatusChanges(source);
            yield return ShowStatusChanges(target);
        }
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        sourceUnit.pokemon.AfterTurn();
        yield return ShowStatusChanges(sourceUnit.pokemon);
        yield return sourceUnit.Hud.WaitForHPUpdate();
        if (sourceUnit.pokemon.Hp <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    public bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.MoveBase.AlwaysHits)
        {
            return true;
        }

        float moveAccuracy = move.MoveBase.Accuracy;
        int accuracy = source.StatBoosts[Stat.命中率];
        int evasion = target.StatBoosts[Stat.闪避率];

        var boostValues = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f};

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
            yield return dialogueBox.TypeDialogue(message);
        }
    }

    private IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        AudioManager.instance.PlaySE(SFX.FAINTED);
        faintedUnit.PlayFaintAnimation();
        yield return dialogueBox.TypeDialogue($"{faintedUnit.pokemon.PokemonBase.PokemonName}\n倒下了！");

        yield return new WaitForSeconds(1f);

        if (!faintedUnit.IsPlayerUnit)
        {
            // Exp gain
            int expYield = faintedUnit.pokemon.PokemonBase.ExpYield;
            int enemyLevel = faintedUnit.pokemon.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.pokemon.Exp += expGain;

            yield return dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}获得了\n{expGain}点经验值！");
            yield return playerUnit.Hud.SetExpSmooth();

            // Check level up
            while (playerUnit.pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}升级了！\n等级提升至{playerUnit.pokemon.Level}！");

                // Try to learn a new Move
                var newMove = playerUnit.pokemon.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if (playerUnit.pokemon.Moves.Count < PokemonBase.MaxNumOfMoves)
                    {
                        playerUnit.pokemon.LearnMove(newMove.MoveBase);
                        yield return dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}习得了新技能\n{newMove.MoveBase.MoveName}！");
                        dialogueBox.SetMoveNames(playerUnit.pokemon.Moves);
                    }
                    else
                    {
                        yield return dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}想要学习{newMove.MoveBase.MoveName}...");
                        yield return dialogueBox.TypeDialogue($"但是{playerUnit.pokemon.PokemonBase.PokemonName}掌握的技能太多了！");
                        yield return ChooseMoveToForget(playerUnit.pokemon, newMove.MoveBase);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }
                
                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);
        }

        CheckForBattleOver(faintedUnit);
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            Pokemon nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(BattleOver(false));
            }
        }
        else
        {
            if (!isTrainerBattle)
            {
                StartCoroutine(BattleOver(true));
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    // Send out next pokemon
                    StartCoroutine(AboutToUse(nextPokemon));
                }
                else
                {
                    StartCoroutine(BattleOver(true));
                }
            }
            
        }
    }

    private IEnumerator ShowDamageDetials(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogueBox.TypeDialogue("震惊！是会心一击！");
        }

        if (damageDetails.TypeEffectiveness> 1f)
        {
            yield return dialogueBox.TypeDialogue("牛逼！效果拔群！");
        } 
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogueBox.TypeDialogue("呃，效果一般！");
        }
    }

    private void HandlePartyScreenSelection()
    {
        Action onSelected = () =>
        {
            AudioManager.instance.PlaySE(SFX.CONFIRM);
            Pokemon seletedMember = partyScreen.SelectedMember;
            if (seletedMember.Hp <= 0)
            {
                partyScreen.SetMessageText("它摸了，换一个吧！");
                return;
            }
            if (seletedMember == playerUnit.pokemon)
            {
                partyScreen.SetMessageText("它已经上场了，换一个吧！");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.ActionSelection)
            {
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
                StartCoroutine(SwitchPokemon(seletedMember, isTrainerAboutToUse));
            }

            partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.pokemon.Hp <= 0)
            {
                partyScreen.SetMessageText("必须要选择一个宝可梦！");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (partyScreen.CalledFrom == BattleState.AboutToUse)
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
            {
                ActionSelection();
            }

            partyScreen.CalledFrom = null;
        };

        partyScreen.HandleUpdate(onSelected, onBack);
    }

    private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            aboutToUseChoice = !aboutToUseChoice;
        }

        dialogueBox.UpdateChoiceBox(aboutToUseChoice);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.instance.PlaySE(SFX.CONFIRM);
            dialogueBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                OpenPartyScreen();
            }
            else
            {
                StartCoroutine(SendNextTrainerPokemon());
            }
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon, bool isTrainerAboutToUse=false)
    {
        
        if (playerUnit.pokemon.Hp > 0)
        {
            yield return dialogueBox.TypeDialogue($"做得好，{playerUnit.pokemon.PokemonBase.PokemonName}！");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        
        playerUnit.ChangeUnit(newPokemon);
        dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"轮到你登场了！\n去吧，{newPokemon.PokemonBase.PokemonName}！");
        AudioManager.instance.PlaySE(SFX.BALL_OUT);

        if (isTrainerAboutToUse)
        {
            StartCoroutine(SendNextTrainerPokemon());
        }
        else
        {
            state = BattleState.RunningTurn;
        }
        
    }

    private IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.SetUp(nextPokemon);
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{nextPokemon.PokemonBase.PokemonName}！");
        state = BattleState.RunningTurn;
    }

    private IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleState.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is PokeballItem)
        {
            yield return ThrowPokeball((PokeballItem)usedItem);
        }

        StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    private IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"你不能偷对方的宝可梦！");
            state = BattleState.RunningTurn;
            yield break;
        }

        
        dialogueBox.EnableActionSelector(false);
        yield return dialogueBox.TypeDialogue($"{player.PlayerName}扔出了{pokeballItem.ItemName}！");
        AudioManager.instance.PlaySE(SFX.THROW_BALL);

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(5, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.InBattleIcon;

        // Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position, 2f, 1, 1f).WaitForCompletion();
        AudioManager.instance.PlaySE(SFX.BALL_OUT);
        yield return enemyUnit.PlayCaptureAnimation();
        pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 6f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.pokemon, pokeballItem);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon is caught
            yield return dialogueBox.TypeDialogue($"抓到了{enemyUnit.pokemon.PokemonBase.PokemonName}！");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.pokemon);
            yield return dialogueBox.TypeDialogue($"{enemyUnit.pokemon.PokemonBase.PokemonName}成为了你的伙伴！");

            Destroy(pokeball);
            yield return BattleOver(true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            AudioManager.instance.PlaySE(SFX.BALL_OUT);
            yield return dialogueBox.TypeDialogue($"{enemyUnit.pokemon.PokemonBase.PokemonName}破球而出了！");

            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }
    }

    private int TryToCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {

        switch (pokeballItem.BallType)
        {
            case PokeballType.Master: 
                return 4;
            case PokeballType.Beast:
                AudioManager.instance.PlaySE(SFX.ATTACK);
                break;
            case PokeballType.FiveFive:
                int prob = UnityEngine.Random.Range(0, 100);
                return prob >= 50 ? 4 : 1;
            case PokeballType.Random:
                // TODO
                return 2;
            case PokeballType.Iron:
                // TODO
                return 2;

        }

        float a = (3 * pokemon.MaxHp - 2 * pokemon.Hp) * pokeballItem.CatchRateModifier * pokemon.PokemonBase.CatchRate * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHp);
        
        if (a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
            {
                break;
            }

            ++shakeCount;
        }

        return shakeCount;
    }

    private IEnumerator TryToEspace()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"你不能从这场战斗中逃跑！");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.pokemon.Speed;
        int enemySpeed = enemyUnit.pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            AudioManager.instance.PlaySE(SFX.ESCAPE);
            yield return dialogueBox.TypeDialogue($"成功逃跑了！");
            yield return BattleOver(false);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                AudioManager.instance.PlaySE(SFX.ESCAPE);
                yield return dialogueBox.TypeDialogue($"成功逃跑了！");
                yield return BattleOver(false);
            }
            else
            {
                yield return dialogueBox.TypeDialogue($"逃跑失败了！");
                state = BattleState.RunningTurn;
            }
        }
    }

}
