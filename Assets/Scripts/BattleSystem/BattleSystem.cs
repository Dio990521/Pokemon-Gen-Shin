using DG.Tweening;
using PokeGenshinUtils.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private BattleUnit _playerUnit;
    [SerializeField] private BattleUnit _enemyUnit;

    [SerializeField] private BattleDialogueBox _dialogueBox;
    [SerializeField] private PartyScreen _partyScreen;

    [SerializeField] private Image _playerImage;
    [SerializeField] private Image _trainerImage;
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
    public BattleDialogueBox DialogueBox1 { get => _dialogueBox; set => _dialogueBox = value; }
    public PartyScreen PartyScreen { get => _partyScreen; set => _partyScreen = value; }
    public Pokemon SelectedPokemon { get; set; }
    public ItemBase SelectedItem { get; set; }

    public int currentAction;
    public int currentMove;
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

    private BattleTrigger battleTrigger;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon, BattleTrigger trigger=BattleTrigger.LongGrass)
    {
        _dialogueBox.SetDialogue("");
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
        _dialogueBox.SetDialogue("");
        AudioManager.Instance.PlayMusic(trainer.StartBGM);
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
            _playerUnit.ResetAnimation();
            _enemyUnit.ResetAnimation();
            _enemyUnit.SetUp(WildPokemon);
            _playerUnit.UnitEnterAnimation();
            _enemyUnit.UnitEnterAnimation();
            yield return new WaitForSeconds(1.5f);
            yield return _dialogueBox.TypeDialogue($"野生的{_enemyUnit.pokemon.PokemonBase.PokemonName}出现了！");
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // Trianer Battle
            
            _trainerImage.sprite = trainer.Sprite;
            _playerUnit.ResetAnimation();
            _enemyUnit.ResetAnimation();
            _playerUnit.UnitEnterAnimation();
            _enemyUnit.UnitEnterAnimation();
            yield return new WaitForSeconds(1.5f);
            yield return _dialogueBox.TypeDialogue($"{trainer.TrainerName}想要进行宝可梦对战！");
            yield return new WaitForSeconds(2f);

            // Send out first pokemon of the trainer
            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            _enemyUnit.ChangeUnit(enemyPokemon);
            AudioManager.Instance.PlaySE(SFX.BALL_OUT);
            yield return _dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{enemyPokemon.PokemonBase.PokemonName}！");
            
            yield return new WaitForSeconds(2f);

            // Send out first pokemon of the player
            _playerImage.gameObject.SetActive(false);
            _playerUnit.gameObject.SetActive(true);
            
        }
        _partyScreen.Init();
        var playerPokemon = PlayerParty.GetHealthyPokemon();
        _playerUnit.ChangeUnit(playerPokemon);
        yield return _dialogueBox.TypeDialogue($"就决定是你了，\n{playerPokemon.PokemonBase.PokemonName}！");
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
        yield return new WaitForSeconds(2f);

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
                AudioManager.Instance.PlayMusic(trainer.WinBGM);
                yield return _dialogueBox.TypeDialogue($"你打败了{trainer.TrainerName}！");
                Wallet.I.AddMoney(trainer.WinMoney, false);
                yield return _dialogueBox.TypeDialogue($"你抢走了对方{trainer.WinMoney}摩拉！");
            }
            else
            {
                AudioManager.Instance.PlayMusic(BGM.VICTORY_WILD_POKEMON);
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
        yield return new WaitForSeconds(2f);
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
            yield return new WaitForSeconds(2f);
        }
        _partyScreen.SwitchPokemonSlot(0, _partyScreen.SelectedItem);
        _playerUnit.ChangeUnit(newPokemon);
        yield return _dialogueBox.TypeDialogue($"轮到你登场了！\n去吧，{newPokemon.PokemonBase.PokemonName}！");
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
        
    }

    public IEnumerator SendNextTrainerPokemon()
    {
        var nextPokemon = TrainerParty.GetHealthyPokemon();
        _enemyUnit.SetUp(nextPokemon);
        yield return _dialogueBox.TypeDialogue($"{trainer.TrainerName}派出了{nextPokemon.PokemonBase.PokemonName}！");
    }

    public IEnumerator ThrowPokeball(PokeballItem pokeballItem)
    {
        _dialogueBox.EnableActionSelector(false);
        if (IsTrainerBattle)
        {
            yield return _dialogueBox.TypeDialogue($"你不能偷对方的宝可梦！");
            yield break;
        }

        if (pokeballItem.BallType == PokeballType.Genshin && !_enemyUnit.pokemon.PokemonBase.IsHuman)
        {
            yield return _dialogueBox.TypeDialogue($"纠缠之缘只能用于捕捉\n人型宝可梦！");
            yield break;
        }

        if (pokeballItem.BallType != PokeballType.Genshin && _enemyUnit.pokemon.PokemonBase.IsHuman)
        {
            yield return _dialogueBox.TypeDialogue($"精灵球只能用于捕捉\n非人型宝可梦！");
            yield break;
        }

        
        yield return _dialogueBox.TypeDialogue($"{player.PlayerName}扔出了{pokeballItem.ItemName}！");
        AudioManager.Instance.PlaySE(SFX.THROW_BALL);

        var pokeballObj = Instantiate(_pokeballSprite, _playerUnit.transform.position - new Vector3(5, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();
        pokeball.sprite = pokeballItem.InBattleIcon;

        // Animations
        yield return pokeball.transform.DOJump(_enemyUnit.transform.position, 2f, 1, 1f).WaitForCompletion();
        AudioManager.Instance.PlaySE(SFX.BALL_OUT);
        yield return _enemyUnit.PlayCaptureAnimation();
        pokeball.transform.DOMoveY(_enemyUnit.transform.position.y - 6f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(_enemyUnit.pokemon, pokeballItem);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokemon is caught
            AudioManager.Instance.PlayMusic(BGM.CATCH_POKEMON);
            yield return _dialogueBox.TypeDialogue($"抓到了{_enemyUnit.pokemon.PokemonBase.PokemonName}！");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            _enemyUnit.pokemon.PokeballSprite = pokeballItem.InBattleIcon;
            _enemyUnit.pokemon.CatchPlace = GameManager.Instance.CurrentScene.MapName;
            PlayerParty.AddPokemon(_enemyUnit.pokemon);
            yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}成为了你的伙伴！");
            if (_partyScreen.Pokemons.Count >= 6)
            {
                yield return _dialogueBox.TypeDialogue($"由于队伍已满，\n{_enemyUnit.pokemon.PokemonBase.PokemonName}被送进了仓库！");
            }

            Destroy(pokeball);
            yield return BattleOver(true, true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return _enemyUnit.PlayBreakOutAnimation();

            AudioManager.Instance.PlaySE(SFX.BALL_OUT);
            yield return _dialogueBox.TypeDialogue($"{_enemyUnit.pokemon.PokemonBase.PokemonName}破球而出了！");

            Destroy(pokeball);
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
