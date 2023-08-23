using DG.Tweening;
using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum BattleStates
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

public enum BattleTrigger { LongGrass, Water, Desert }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogueBox dialogueBox;
    [SerializeField] private PartyScreen partyScreen;

    [SerializeField] private Image playerImage;
    [SerializeField] private Image trainerImage;
    [SerializeField] private GameObject pokeballSprite;
    [SerializeField] private ForgetMoveSelectionUI moveSelectionUI;
    [SerializeField] private InventoryUI inventoryUI;

    [SerializeField] List<Sprite> backgroundImages;
    [SerializeField] List<Sprite> enemyGroundSprites;
    [SerializeField] List<Sprite> playerGroundSprites;
    [SerializeField] Image backgroundImage;

    public BattleStates state;

    public StateMachine<BattleSystem> StateMachine { get; private set; }
    public BattleDialogueBox DialogueBox { get => dialogueBox; set => dialogueBox = value; }
    public BattleUnit PlayerUnit { get => playerUnit; set => playerUnit = value; }
    public BattleUnit EnemyUnit { get => enemyUnit; set => enemyUnit = value; }
    public BattleDialogueBox DialogueBox1 { get => dialogueBox; set => dialogueBox = value; }
    public PartyScreen PartyScreen { get => partyScreen; set => partyScreen = value; }
    public Pokemon SelectedPokemon { get; set; }

    public int currentAction;
    public int currentMove;
    private bool aboutToUseChoice = true;
    public int SelectedMove { get; set; }

    public bool IsBattleOver { get; private set; }

    public event Action<bool> OnBattleOver;

    public PokemonParty PlayerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }
    public PokemonParty TrainerParty { get; private set; }

    public BattleAction SelectedAction { get; set; }

    public bool IsTrainerBattle { get; private set; } = false;
    PlayerController player;
    TrainerController trainer;

    public int EscapeAttempts { get; set; }
    public TrainerController Trainer { get => trainer; set => trainer = value; }

    MoveBase moveToLearn;

    private BattleTrigger battleTrigger;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger=BattleTrigger.LongGrass)
    {
        dialogueBox.SetDialogue("");
        this.IsTrainerBattle = false;
        AudioManager.Instance.PlayMusic(BGM.BATTLE_WILD_POKEMON);
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        battleTrigger = trigger;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        dialogueBox.SetDialogue("");
        AudioManager.Instance.PlayMusic(BGM.BATTLE_TRAINER);
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;
        this.IsTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        battleTrigger = trigger;
        StartCoroutine(SetupBattle());
    }

    public void CreateBattleStateMachine()
    {
        StateMachine = new StateMachine<BattleSystem>(this);
    }

    public IEnumerator SetupBattle()
    {

        playerUnit.SetDefaultPlayerSprite();
        playerUnit.HideHud();
        enemyUnit.HideHud();

        backgroundImage.sprite = backgroundImages[((int)battleTrigger)];
        playerUnit.SetGroundImage(playerGroundSprites[((int)battleTrigger)]);
        enemyUnit.SetGroundImage(enemyGroundSprites[((int)battleTrigger)]);

        if (!IsTrainerBattle)
        {
            // Wild Pokemon Battle

            // set up pokemons data
            playerUnit.ResetAnimation();
            enemyUnit.ResetAnimation();
            enemyUnit.SetUp(WildPokemon);
            playerUnit.UnitEnterAnimation();
            enemyUnit.UnitEnterAnimation();
            yield return new WaitForSeconds(1.5f);
            yield return dialogueBox.TypeDialogue($"Ұ����{enemyUnit.pokemon.PokemonBase.PokemonName}�����ˣ�");
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // Trianer Battle
            
            trainerImage.sprite = trainer.Sprite;
            playerUnit.ResetAnimation();
            enemyUnit.ResetAnimation();
            playerUnit.UnitEnterAnimation();
            enemyUnit.UnitEnterAnimation();
            yield return new WaitForSeconds(1.5f);
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}��Ҫ���б����ζ�ս��");
            yield return new WaitForSeconds(2f);

            // Send out first pokemon of the trainer
            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            enemyUnit.ChangeUnit(enemyPokemon);
            AudioManager.Instance.PlaySE(SFX.BALL_OUT);
            yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}�ɳ���{enemyPokemon.PokemonBase.PokemonName}��");
            
            yield return new WaitForSeconds(2f);

            // Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            
        }
        partyScreen.Init();
        var playerPokemon = PlayerParty.GetHealthyPokemon();
        playerUnit.ChangeUnit(playerPokemon);
        yield return dialogueBox.TypeDialogue($"�;��������ˣ�\n{playerPokemon.PokemonBase.PokemonName}��");
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
        //dialogueBox.SetMoveNames(playerUnit.pokemon.Moves);
        yield return new WaitForSeconds(2f);

        playerUnit.ShowHud();
        enemyUnit.ShowHud();
        IsBattleOver = false;
        EscapeAttempts = 0;

        StateMachine.ChangeState(ActionSelectionState.I);
    }

    public IEnumerator BattleOver(bool won, bool isCatch=false)
    {
        IsBattleOver = true;
        if (!isCatch && won)
        {
            if (IsTrainerBattle)
            {
                AudioManager.Instance.PlayMusic(trainer.WinBGM);
                yield return dialogueBox.TypeDialogue($"������{trainer.TrainerName}��");
                Wallet.i.AddMoney(trainer.WinMoney, false);
                yield return dialogueBox.TypeDialogue($"�������˶Է�{trainer.WinMoney}Ħ����");
            }
            else
            {
                AudioManager.Instance.PlayMusic(BGM.VICTORY_WILD_POKEMON);
                yield return dialogueBox.TypeDialogue($"������{enemyUnit.pokemon.PokemonBase.PokemonName}��");
                if (enemyUnit.pokemon.PokemonBase.RewardProb != 0 && enemyUnit.pokemon.PokemonBase.Reward != null)
                {
                    float tmp = UnityEngine.Random.Range(0f, 1f);
                    if (tmp <= enemyUnit.pokemon.PokemonBase.RewardProb)
                    {
                        Inventory.GetInventory().AddItem(enemyUnit.pokemon.PokemonBase.Reward, playSE: false);
                        yield return dialogueBox.TypeDialogue($"{enemyUnit.pokemon.PokemonBase.PokemonName}������{enemyUnit.pokemon.PokemonBase.Reward.ItemName}��");
                    }
                }
            }
            
        }
        PlayerParty.Pokemons.ForEach(p => p.OnBattleOver());
        playerUnit.Hud.ClearData();
        enemyUnit.Hud.ClearData();
        yield return new WaitForSeconds(2f);
        OnBattleOver(won);
    }

    // Player turn
    private void ActionSelection()
    {
        SelectedAction = 0;
        state = BattleStates.ActionSelection;
        playerUnit.Hud.gameObject.SetActive(true);
        enemyUnit.Hud.gameObject.SetActive(true);
        StartCoroutine(dialogueBox.TypeDialogue($"��Ҫ\n{playerUnit.pokemon.PokemonBase.PokemonName}��ʲô��"));
        dialogueBox.EnableActionSelector(true);
    }

    // Check action cursor and move cursor
    public void HandleUpdate()
    {
        StateMachine.Execute();

        if (state == BattleStates.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = BattleStates.ActionSelection;
            };

            Action<ItemBase> onItemUsed = (ItemBase usedItem) =>
            {
                StartCoroutine(OnItemUsed(usedItem));
            };

            //inventoryUI.HandleUpdate(onBack, onItemUsed);
        }
        else if (state == BattleStates.PartyScreen)
        {
            HandlePartyScreenSelection();
        }
        else if (state == BattleStates.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleStates.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                moveSelectionUI.gameObject.SetActive(false);
                if (moveIndex == PokemonBase.MaxNumOfMoves)
                {
                    // Don't learn new move
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}����ѧϰ{moveToLearn.MoveName}��"));
                }
                else
                {
                    // Forget the selected move and learn new move
                    var selevtedMove = playerUnit.pokemon.Moves[moveIndex].MoveBase;
                    StartCoroutine(dialogueBox.TypeDialogue($"{playerUnit.pokemon.PokemonBase.PokemonName}������{selevtedMove.MoveName}��"));
                    playerUnit.pokemon.Moves[moveIndex] = new Move(moveToLearn);
                }

                moveToLearn = null;
                state = BattleStates.RunningTurn;
            };

            

            //moveSelectionUI.HandleMoveSelection(onMoveSelected);
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
            //AudioManager.Instance.PlaySE(SFX.CONFIRM);
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
               // StartCoroutine(RunTurns(BattleAction.Run));
            }
        }

    }

    // Open move box
    private void MoveSelection()
    {
        state = BattleStates.MoveSelection;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogueText(false);
        dialogueBox.EnableMoveSelector(true);
    }

    private IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleStates.Busy;
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}��Ҫ��{newPokemon.PokemonBase.PokemonName}�ϳ���\n�Ƿ�Ҫ������ǰ��ս�����Σ�");
        state = BattleStates.AboutToUse;
        dialogueBox.EnableChoiceBox(true);
    }

    private IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = BattleStates.Busy;
        yield return dialogueBox.TypeDialogue($"��Ҫ��{pokemon.PokemonBase.PokemonName}\n�����ĸ����ܣ�");
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.MoveBase).ToList(), newMove);
        moveToLearn = newMove;
        state = BattleStates.MoveToForget;
    }

    private void OpenBag()
    {
        state = BattleStates.Bag;
        inventoryUI.Show();
    }

    private void OpenPartyScreen()
    {
        //partyScreen.CalledFrom = state;
        state = BattleStates.PartyScreen;
        partyScreen.Show();
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
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            //StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogueText(true);
            ActionSelection();
        }
    }

 

    private void HandlePartyScreenSelection()
    {
        Action onSelected = () =>
        {
            //AudioManager.Instance.PlaySE(SFX.CONFIRM);
            Pokemon seletedMember = partyScreen.SelectedMember;
            if (seletedMember.Hp <= 0)
            {
                partyScreen.SetMessageText("�����ˣ���һ���ɣ�");
                return;
            }
            if (seletedMember == playerUnit.pokemon)
            {
                partyScreen.SetMessageText("���Ѿ��ϳ��ˣ���һ���ɣ�");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            //if (partyScreen.CalledFrom == BattleState.ActionSelection)
            //{
            //    StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            //}
            //else
            //{
            //    state = BattleState.Busy;
            //    bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
            //    StartCoroutine(SwitchPokemon(seletedMember, isTrainerAboutToUse));
            //}

            //partyScreen.CalledFrom = null;
        };

        Action onBack = () =>
        {
            if (playerUnit.pokemon.Hp <= 0)
            {
                partyScreen.SetMessageText("����Ҫѡ��һ�������Σ�");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            //if (partyScreen.CalledFrom == BattleState.AboutToUse)
            //{
            //    StartCoroutine(SendNextTrainerPokemon());
            //}
            //else
            //{
            //    ActionSelection();
            //}

            //partyScreen.CalledFrom = null;
        };

        //partyScreen.HandleUpdate(onSelected, onBack);
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
            //AudioManager.Instance.PlaySE(SFX.CONFIRM);
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
            //AudioManager.Instance.PlaySE(SFX.CANCEL);
            dialogueBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)//, bool isTrainerAboutToUse=false)
    {
        
        if (playerUnit.pokemon.Hp > 0)
        {
            yield return dialogueBox.TypeDialogue($"���úã�{playerUnit.pokemon.PokemonBase.PokemonName}��");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        partyScreen.SwitchPokemonSlot(0, partyScreen.SelectedItem);
        playerUnit.ChangeUnit(newPokemon);
        //dialogueBox.SetMoveNames(newPokemon.Moves);
        yield return dialogueBox.TypeDialogue($"�ֵ���ǳ��ˣ�\nȥ�ɣ�{newPokemon.PokemonBase.PokemonName}��");
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);

        //if (isTrainerAboutToUse)
        //{
        //    StartCoroutine(SendNextTrainerPokemon());
        //}

        
    }

    public IEnumerator SendNextTrainerPokemon()
    {
        state = BattleStates.Busy;

        var nextPokemon = TrainerParty.GetHealthyPokemon();
        enemyUnit.SetUp(nextPokemon);
        yield return dialogueBox.TypeDialogue($"{trainer.TrainerName}�ɳ���{nextPokemon.PokemonBase.PokemonName}��");
        state = BattleStates.RunningTurn;
    }

    private IEnumerator OnItemUsed(ItemBase usedItem)
    {
        state = BattleStates.Busy;
        inventoryUI.gameObject.SetActive(false);

        if (usedItem is PokeballItem)
        {
            yield return ThrowPokeball((PokeballItem)usedItem);
        }

        //StartCoroutine(RunTurns(BattleAction.UseItem));
    }

    private IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        state = BattleStates.Busy;
        dialogueBox.EnableActionSelector(false);
        if (IsTrainerBattle)
        {
            yield return dialogueBox.TypeDialogue($"�㲻��͵�Է��ı����Σ�");
            state = BattleStates.RunningTurn;
            yield break;
        }

        if (pokeballItem.BallType == PokeballType.Genshin && !enemyUnit.pokemon.PokemonBase.IsHuman)
        {
            yield return dialogueBox.TypeDialogue($"����֮Եֻ�����ڲ�׽\n���ͱ����Σ�");
            state = BattleStates.RunningTurn;
            yield break;
        }

        if (pokeballItem.BallType != PokeballType.Genshin && enemyUnit.pokemon.PokemonBase.IsHuman)
        {
            yield return dialogueBox.TypeDialogue($"������ֻ�����ڲ�׽\n�����ͱ����Σ�");
            state = BattleStates.RunningTurn;
            yield break;
        }

        
        yield return dialogueBox.TypeDialogue($"{player.PlayerName}�ӳ���{pokeballItem.ItemName}��");
        AudioManager.Instance.PlaySE(SFX.THROW_BALL);

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(5, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.InBattleIcon;

        // Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position, 2f, 1, 1f).WaitForCompletion();
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
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
            AudioManager.Instance.PlayMusic(BGM.CATCH_POKEMON);
            yield return dialogueBox.TypeDialogue($"ץ����{enemyUnit.pokemon.PokemonBase.PokemonName}��");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            enemyUnit.pokemon.PokeballSprite = pokeballItem.InBattleIcon;
            enemyUnit.pokemon.CatchPlace = GameManager.Instance.CurrentScene.MapName;
            PlayerParty.AddPokemon(enemyUnit.pokemon);
            yield return dialogueBox.TypeDialogue($"{enemyUnit.pokemon.PokemonBase.PokemonName}��Ϊ����Ļ�飡");

            Destroy(pokeball);
            yield return BattleOver(true, true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            AudioManager.Instance.PlaySE(SFX.BALL_OUT);
            yield return dialogueBox.TypeDialogue($"{enemyUnit.pokemon.PokemonBase.PokemonName}��������ˣ�");

            Destroy(pokeball);
            state = BattleStates.RunningTurn;
        }
    }

    private int TryToCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {

        switch (pokeballItem.BallType)
        {
            case PokeballType.Master: 
                return 4;
            case PokeballType.Beast:
                AudioManager.Instance.PlaySE(SFX.ATTACK);
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



}
