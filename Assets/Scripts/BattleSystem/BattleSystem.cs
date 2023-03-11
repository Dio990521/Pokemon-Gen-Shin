using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver
}

public enum BattleAction 
{ 
    Move, 
    SwitchPokemon, 
    UseItem, 
    Run 
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;

    public BattleState state;
    public BattleState? prevState;
    public int currentAction;
    public int currentMove;
    public int currentMember;

    public event Action<bool> OnBattleOver;

    public PokemonParty playerParty;
    public Pokemon wildPokemon;
    public PokemonParty trainerParty;

    private bool isTrainerBattler = false;
    PlayerController player;
    TrainerController trainer;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.isTrainerBattler = false;
        AudioManager.instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        AudioManager.instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        this.isTrainerBattler = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {

        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattler)
        {
            // Wild Pokemon Battle

            // set up pokemons data
            playerUnit.SetUp(playerParty.GetHealthyPokemon());
            enemyUnit.SetUp(wildPokemon);

            // set up move box
            dialogueBox.SetMoveNames(playerUnit.pokemon.Moves);
            yield return dialogueBox.TypeDialogue($"野生的{enemyUnit.pokemon.PokemonBase.PokemonName}出现了！");
        }
        else
        {
            // Trianer Battle
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}想要进行宝可梦对战！");

            // Send out first pokemon of the trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.SetUp(enemyPokemon);
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{enemyPokemon.PokemonBase.PokemonName}！");

            // Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = trainerParty.GetHealthyPokemon();
            playerUnit.SetUp(enemyPokemon);
            yield return dialogueBox.TypeDialogue($"就决定是你了，{playerPokemon.PokemonBase.PokemonName}！");
            dialogueBox.SetMoveNames(playerUnit.pokemon.Moves);

        }

        yield return new WaitForSeconds(4f);
        
        ActionSelection();
    }

    private IEnumerator BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        if (won)
        {
            AudioManager.instance.PlayMusic(BGM.VICTORY_WILD_POKEMON);
            StartCoroutine(dialogueBox.TypeDialogue($"你打败了{enemyUnit.pokemon.PokemonBase.PokemonName}！"));
        }
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
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
        else if (state == BattleState.PartyScreen)
        {
            HandlePartyScreenSelection();
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

            }
            else if (currentAction == 2)
            {
                // Pokemon party
                prevState = state;
                OpenPartyScreen();

            }
            else if (currentAction == 3)
            {
                // Run

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

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
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
                var selectedPokemon = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
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
            yield return sourceUnit.Hud.UpdateHp();
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
                yield return targetUnit.Hud.UpdateHp();
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
                AudioManager.instance.PlaySE(SFX.FAINTED);
                targetUnit.PlayFaintAnimation();
                yield return dialogueBox.TypeDialogue($"{targetUnit.pokemon.PokemonBase.PokemonName}\n倒下了！");

                yield return new WaitForSeconds(1f);

                CheckForBattleOver(targetUnit);
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
        yield return sourceUnit.Hud.UpdateHp();
        if (sourceUnit.pokemon.Hp <= 0)
        {
            AudioManager.instance.PlaySE(SFX.FAINTED);
            sourceUnit.PlayFaintAnimation();
            yield return dialogueBox.TypeDialogue($"{sourceUnit.pokemon.PokemonBase.PokemonName}\n倒下了！");

            yield return new WaitForSeconds(1f);

            CheckForBattleOver(sourceUnit);
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
            if (!isTrainerBattler)
            {
                StartCoroutine(BattleOver(true));
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    // Send out next pokemon
                    StartCoroutine(SendNextTrainerPokemon(nextPokemon));
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
            AudioManager.instance.PlaySE(SFX.EFFICIENT_ATTACK);
            yield return dialogueBox.TypeDialogue("牛逼！效果拔群！");
        } 
        else if (damageDetails.TypeEffectiveness < 1f)
        {
            AudioManager.instance.PlaySE(SFX.LOW_ATTACK);
            yield return dialogueBox.TypeDialogue("呃，效果一般！");
        }
        else
        {
            AudioManager.instance.PlaySE(SFX.ATTACK);
        }
    }

    private void HandlePartyScreenSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 1;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMember += 1;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMember -= 1;
        }

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            AudioManager.instance.PlaySE(SFX.CONFIRM);
            Pokemon seletedMember = playerParty.Pokemons[currentMember];
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

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(seletedMember));
            }

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private IEnumerator SwitchPokemon(Pokemon newPokemon)
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

        state = BattleState.RunningTurn;
    
    }

    IEnumerator SendNextTrainerPokemon(Pokemon nextPokemon)
    {
        state = BattleState.Busy;

        enemyUnit.SetUp(nextPokemon);
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{nextPokemon.PokemonBase.PokemonName}！");
        state = BattleState.RunningTurn;
    }

}
