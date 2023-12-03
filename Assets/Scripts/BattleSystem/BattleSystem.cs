using DG.Tweening;
using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction 
{ 
    Move, 
    SwitchPokemon, 
    UseItem, 
    Run 
}

public enum BattleTrigger { LongGrass, Water, Desert, Cave, Land, Arena1, Arena2, Arena3, Ship }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleUnit _enemyUnit;

    [SerializeField] private BattleDialogueBox _dialogueBox;
    [SerializeField] private PartyScreen _partyScreen;

    [SerializeField] private Image _playerImage;
    [SerializeField] private GameObject _pokeballSprite;
    [SerializeField] private ForgetMoveSelectionUI _moveSelectionUI;
    [SerializeField] private InventoryUI _inventoryUI;

    [SerializeField] private List<Sprite> _backgroundImages;
    [SerializeField] private List<Sprite> _enemyGroundSprites;
    [SerializeField] private List<Sprite> _playerGroundSprites;
    [SerializeField] private Image _backgroundImage;

    public StateMachine<BattleSystem> StateMachine { get; private set; }
    public BattleDialogueBox DialogueBox { get => _dialogueBox; set => _dialogueBox = value; }
    public BattleUnit PlayerUnit { get => _playerUnit; set => _playerUnit = value; }
    public BattleUnit EnemyUnit { get => _enemyUnit; set => _enemyUnit = value; }
    public PartyScreen PartyScreen { get => _partyScreen; set => _partyScreen = value; }
    public Pokemon SelectedPokemon { get; set; }
    public ItemBase SelectedItem { get; set; }

    public int currentAction;
    public int currentMove;
    public int SelectedMove { get; set; }

    public bool IsBattleOver { get; private set; }

    public static event Action<bool> OnBattleOver;

    public PokemonParty PlayerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }
    public PokemonParty TrainerParty { get; private set; }

    public BattleAction SelectedAction { get; set; }

    public bool IsTrainerBattle { get; private set; } = false;
    PlayerController player;
    TrainerController trainer;

    public int EscapeAttempts { get; set; }
    public TrainerController Trainer { get => trainer; set => trainer = value; }
    public GameObject PokeballSprite { get => _pokeballSprite; set => _pokeballSprite = value; }

    private BattleTrigger battleTrigger;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger=BattleTrigger.LongGrass)
    {
        _dialogueBox.SetDialogue("");
        this.IsTrainerBattle = false;
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();
        battleTrigger = trigger;
        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty, BattleTrigger trigger = BattleTrigger.LongGrass)
    {
        _dialogueBox.SetDialogue("");
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;
        this.IsTrainerBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();
        AudioManager.Instance.PlayMusicVolume(trainer.StartBGM);
        battleTrigger = trigger;
        StartCoroutine(SetupBattle());
    }

    public void CreateBattleStateMachine()
    {
        StateMachine = new StateMachine<BattleSystem>(this);
    }

    public IEnumerator SetupBattle()
    {
        _playerUnit.SetDefaultPlayerSprite();
        _playerUnit.HideHud();
        _enemyUnit.HideHud();

        _backgroundImage.sprite = _backgroundImages[((int)battleTrigger)];
        _playerUnit.SetGroundImage(_playerGroundSprites[((int)battleTrigger)]);
        _enemyUnit.SetGroundImage(_enemyGroundSprites[((int)battleTrigger)]);

        if (!IsTrainerBattle)
        {
            // Wild Pokemon Battle

            // set up pokemons data
            _enemyUnit.TrainerSprite.enabled = false;
            _playerUnit.ResetAnimation();
            _enemyUnit.ResetAnimation();
            _enemyUnit.SetUp(WildPokemon);
            _playerUnit.UnitEnterAnimation();
            _enemyUnit.UnitEnterAnimation();
            yield return new WaitForSeconds(1f);
            if (!BattleState.I.IsSuperBoss)
            {
                yield return _dialogueBox.TypeDialogue($"野生的{_enemyUnit.pokemon.PokemonBase.PokemonName}出现了！");
            }
            else
            {
                yield return _dialogueBox.TypeDialogue($"异世界幻影\n{_enemyUnit.pokemon.PokemonBase.PokemonName}出现了！");
            }
            yield return new WaitForSeconds(1f);
        }
        else
        {
            _enemyUnit.TrainerSprite.enabled = true;
            _enemyUnit.PokemonSprite.enabled = false;
            // Trianer Battle
            _enemyUnit.TrainerSprite.sprite = trainer.Sprite;
            _playerUnit.ResetAnimation();
            _enemyUnit.ResetAnimation();
            _playerUnit.UnitEnterAnimation();
            _enemyUnit.UnitEnterAnimation();
            yield return new WaitForSeconds(1.5f);
            yield return _dialogueBox.TypeDialogue($"{trainer.TrainerName}\n想要进行宝可梦对战！");
            yield return new WaitForSeconds(0.5f);

            // Send out first pokemon of the trainer
            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            _enemyUnit.Pokeball.DOFade(1f, 0.01f);
            _enemyUnit.Pokeball.sprite = _enemyUnit.PokeballCloseSprite;
            yield return _enemyUnit.MoveTrainerImage(380f, true);
            yield return new WaitForSeconds(0.5f);
            _enemyUnit.Pokeball.sprite = _enemyUnit.PokeballOpenSprite;
            _enemyUnit.Pokeball.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.2f);
            _enemyUnit.PokemonSprite.enabled = true;
            _enemyUnit.ChangeUnit(enemyPokemon, true);
            yield return _dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了\n{enemyPokemon.PokemonBase.PokemonName}！");
           
            // Send out first pokemon of the player
            _playerImage.gameObject.SetActive(false);
            _playerUnit.gameObject.SetActive(true);

            if (BattleState.I.Guide)
            {
                yield return _dialogueBox.TypeDialogue($"{enemyPokemon.PokemonBase.PokemonName}被突如其来的战\n吓出了一身冷汗！！");
                yield return _dialogueBox.TypeDialogue($"汗水使{enemyPokemon.PokemonBase.PokemonName}附加了水元素状态！");
                BattleState.I.Guide = false;
            }

        }
        _partyScreen.Init();
        var playerPokemon = PlayerParty.GetHealthyPokemon();
        yield return _playerUnit.PlayerThrowBallAnimation(this, playerPokemon);
        yield return _dialogueBox.TypeDialogue($"就决定是你了，\n{playerPokemon.PokemonBase.PokemonName}！", 0.9f);
        _playerUnit.ChangeUnit(playerPokemon);
        yield return new WaitForSeconds(1f);

        _playerUnit.ShowHud();
        _enemyUnit.ShowHud();
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
                AudioManager.Instance.PlayMusicVolume(trainer.WinBGM);
                yield return _dialogueBox.TypeDialogue($"你打败了{trainer.TrainerName}！");
                yield return _enemyUnit.MoveTrainerImage(300f, false, 1f);
                yield return _dialogueBox.TypeDialogue($"{trainer.DialogueAfterBattle.Lines[0]}");
                if (trainer.WinMoney > 0)
                {
                    Wallet.I.AddMoney(trainer.WinMoney, false);
                    yield return _dialogueBox.TypeDialogue($"你抢走了对方{trainer.WinMoney}摩拉！");
                }
                if (trainer.IsGymLeader)
                {
                    Wallet.I.IncreaseVisaLimit(100000);
                    yield return _dialogueBox.TypeDialogue($"作为奖励，你的信用卡额度\n提升了100000摩拉！");
                }
            }
            else
            {
                AudioManager.Instance.PlayMusicVolume(BGM.VICTORY_WILD_POKEMON);
                yield return _dialogueBox.TypeDialogue($"你打败了{_enemyUnit.pokemon.PokemonBase.PokemonName}！");
                if (_enemyUnit.pokemon.PokemonBase.RewardProb != 0 && _enemyUnit.pokemon.PokemonBase.Reward != null)
                {
                    float tmp = UnityEngine.Random.Range(0f, 1f);
                    if (tmp <= _enemyUnit.pokemon.PokemonBase.RewardProb)
                    {
                        Inventory.GetInventory().AddItem(_enemyUnit.pokemon.PokemonBase.Reward, playSE: false);
                        yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}掉落了{_enemyUnit.pokemon.PokemonBase.Reward.ItemName}！");
                    }
                }
            }
            
        }
        PlayerParty.Pokemons.ForEach(p => p.OnBattleOver());
        _playerUnit.Hud.ClearData();
        _enemyUnit.Hud.ClearData();
        yield return new WaitForSeconds(1f);
        OnBattleOver(won);
    }

    public void HandleUpdate()
    {
        StateMachine.Execute();
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        
        if (_playerUnit.pokemon.Hp > 0)
        {
            yield return _dialogueBox.TypeDialogue($"做得好，{_playerUnit.pokemon.PokemonBase.PokemonName}！");
            _playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1f);
        }
        _partyScreen.SwitchPokemonSlot(0, PartyState.I.Selection);
        yield return _playerUnit.ThrowBallAnimation(this);
        yield return _dialogueBox.TypeDialogue($"轮到你登场了！\n去吧，{newPokemon.PokemonBase.PokemonName}！", 0.7f);
        _playerUnit.ChangeUnit(newPokemon);
        yield return new WaitForSeconds(1.5f);

    }

    public IEnumerator SendNextTrainerPokemon()
    {
        var nextPokemon = TrainerParty.GetHealthyPokemon();
        _enemyUnit.PokemonSprite.enabled = false;
        _enemyUnit.SetNewTrainerPokemon(nextPokemon);
        _enemyUnit.Pokeball.DOFade(1f, 0.01f);
        _enemyUnit.Pokeball.sprite = _enemyUnit.PokeballCloseSprite;
        yield return new WaitForSeconds(0.5f);
        _enemyUnit.Pokeball.sprite = _enemyUnit.PokeballOpenSprite;
        _enemyUnit.Pokeball.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        _enemyUnit.PokemonSprite.enabled = true;
        _enemyUnit.UnitChangeAnimation();
        _enemyUnit.Hud.SetData(nextPokemon);
        yield return _dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{nextPokemon.PokemonBase.PokemonName}！");
    }

    public IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        _dialogueBox.EnableActionSelector(false);

        if (IsTrainerBattle)
        {
            RunTurnState.I.EnemyContinue = false;
            yield return _dialogueBox.TypeDialogue($"你不能偷对方的宝可梦！");
            StateMachine.ChangeState(ActionSelectionState.I);
            yield break;
        }
        else if (BattleState.I.IsSuperBoss)
        {
            RunTurnState.I.EnemyContinue = false;
            yield return _dialogueBox.TypeDialogue($"你不能捕捉这个宝可梦！");
            StateMachine.ChangeState(ActionSelectionState.I);
            yield break;
        }

        if ((pokeballItem.BallType == PokeballType.EX_Genshin || pokeballItem.BallType == PokeballType.Genshin) 
            && !_enemyUnit.pokemon.PokemonBase.IsHuman)
        {
            RunTurnState.I.EnemyContinue = false;
            yield return _dialogueBox.TypeDialogue($"纠缠之缘只能用于捕捉\n人型宝可梦！");
            StateMachine.ChangeState(ActionSelectionState.I);
            yield break;
        }

        if (pokeballItem.BallType != PokeballType.EX_Genshin && pokeballItem.BallType != PokeballType.Genshin
            && _enemyUnit.pokemon.PokemonBase.IsHuman)
        {
            RunTurnState.I.EnemyContinue = false;
            yield return _dialogueBox.TypeDialogue($"这种球只能用于捕捉\n非人型宝可梦！");
            StateMachine.ChangeState(ActionSelectionState.I);
            yield break;
        }


        yield return _dialogueBox.TypeDialogue($"{player.PlayerName}扔出了{pokeballItem.ItemName}！");
        if (pokeballItem.BallType == PokeballType.Beast)
        {
            AudioManager.Instance.PlaySE(SFX.BEAST_YIGE);
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.THROW_BALL);
        }

        var pokeballObj = Instantiate(PokeballSprite, _playerUnit.transform.position - new Vector3(5, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.InBattleIcon;

        // Animations
        var ballDest = _enemyUnit.transform.position + new Vector3(0f, 5f, 0);
        var originPos = _enemyUnit.transform.position;
        yield return pokeball.transform.DOJump(ballDest, 2f, 1, 1f).WaitForCompletion();

        if (pokeballItem.BallType == PokeballType.Iron) 
        {
            yield return pokeball.transform.DOJump(ballDest + new Vector3(10f, 3f, 0), 3f, 1, 1.5f);
            AudioManager.Instance.PlaySE(SFX.ATTACK);
            if (UnityEngine.Random.Range(0, 100) < 2)
            {
                AudioManager.Instance.PlaySE(SFX.EFFICIENT_ATTACK);
                _enemyUnit.pokemon.DecreaseHP(99999);
                yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}被砸得眼冒金星！");
                RunTurnState.I.EnemyContinue = false;
                yield return BattleOver(true, false);
            }
            else
            {
                AudioManager.Instance.PlaySE(SFX.LOW_ATTACK);
                _enemyUnit.pokemon.DecreaseHP(1);
                yield return _dialogueBox.TypeDialogue($"对{_enemyUnit.pokemon.PokemonBase.PokemonName}造成了1点伤害！");
                RunTurnState.I.EnemyContinue = true;
            }
            yield break;
        }
        pokeball.sprite = pokeballItem.OpenIcon;
        yield return _enemyUnit.PlayCaptureAnimation(ballDest);
        pokeball.sprite = pokeballItem.InBattleIcon;
        yield return pokeball.transform.DOMoveY(5f, 1f)
            .SetEase(Ease.OutBounce)
            .SetLoops(1, LoopType.Yoyo);
        if (pokeballItem.BallType == PokeballType.Beast)
        {
            AudioManager.Instance.PlaySE(SFX.BEAST_HE);
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.BALL_BOUNCE);
        }
        yield return new WaitForSeconds(0.65f);
        if (pokeballItem.BallType == PokeballType.Beast)
        {
            AudioManager.Instance.PlaySE(SFX.BEAST_HE);
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.BALL_BOUNCE);
        }
        yield return new WaitForSeconds(0.3f);
        if (pokeballItem.BallType == PokeballType.Beast)
        {
            AudioManager.Instance.PlaySE(SFX.BEAST_HE);
        }
        else
        {
            AudioManager.Instance.PlaySE(SFX.BALL_BOUNCE);
        }
        yield return new WaitForSeconds(0.15f);
        int shakeCount = TryToCatchPokemon(_enemyUnit.pokemon, pokeballItem);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            if (pokeballItem.BallType == PokeballType.Beast)
            {
                AudioManager.Instance.PlaySE(SFX.BEAST_A);
            }
            else
            {
                AudioManager.Instance.PlaySE(SFX.BALL_SHAKE);
            }
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 20f), 1f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon is caught
            if (pokeballItem.BallType == PokeballType.Beast)
            {
                AudioManager.Instance.PlaySE(SFX.BEAST_YARIMASU);
            }
            AudioManager.Instance.PlayMusicVolume(BGM.CATCH_POKEMON);
            yield return _dialogueBox.TypeDialogue($"抓到了{_enemyUnit.pokemon.PokemonBase.PokemonName}！");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            _enemyUnit.pokemon.PokeballSpriteType = pokeballItem.BallType;
            _enemyUnit.pokemon.CatchPlace = GameManager.Instance.CurrentScene.MapName;
            if (pokeballItem.BallType == PokeballType.EX_Genshin || pokeballItem.BallType == PokeballType.EX_Guaishou)
            {
                _enemyUnit.pokemon.SetBestBias();
            }
            PlayerParty.AddPokemonToParty(_enemyUnit.pokemon);
            yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}成为了你的伙伴！");
            if (_partyScreen.Pokemons.Count >= 6)
            {
                yield return _dialogueBox.TypeDialogue($"由于队伍已满，\n{_enemyUnit.pokemon.PokemonBase.PokemonName}被送进了仓库！");
            }
            RunTurnState.I.EnemyContinue = false;
            Destroy(pokeballObj);
            yield return BattleOver(true, true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            AudioManager.Instance.PlaySE(SFX.BALL_OUT);
            yield return _enemyUnit.PlayBreakOutAnimation(originPos);
            yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}破球而出了！");
            RunTurnState.I.EnemyContinue = true;
            Destroy(pokeballObj);
        }
    }

    private int TryToCatchPokemon(Pokemon pokemon, PokeballItem pokeballItem)
    {

        switch (pokeballItem.BallType)
        {
            case PokeballType.Master: 
                return 4;
            case PokeballType.FiveFive:
                int prob = UnityEngine.Random.Range(0, 100);
                return prob >= 50 ? 4 : 1;
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

    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 25;
        GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, 200));
        GUILayout.Label("BATTLE STATE STACK", style);
        foreach (var state in StateMachine.StateStack)
        {
            GUILayout.Label(state.GetType().ToString(), style);
        }
        GUILayout.EndArea();
    }


}
